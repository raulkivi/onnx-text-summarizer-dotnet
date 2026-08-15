using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.IO;

namespace TextSummarizer
{
    public class ONNXTextSummarizer : IDisposable
    {
        private readonly InferenceSession _encoderSession;
        private readonly InferenceSession _decoderSession;
        private readonly Dictionary<string, int> _vocab;
        private readonly Dictionary<int, string> _reverseVocab;
        private bool _disposed = false;

        // Special tokens for T5 model
        private const int PAD_TOKEN_ID = 0;
        private const int EOS_TOKEN_ID = 1;
        private const int UNK_TOKEN_ID = 2;

        public ONNXTextSummarizer(string modelPath)
        {
            try
            {
                string encoderPath = Path.Combine(modelPath, "encoder_model.onnx");
                string decoderPath = Path.Combine(modelPath, "decoder_model_merged.onnx");
                string tokenizerPath = Path.Combine(modelPath, "tokenizer.json");

                // Load ONNX models
                _encoderSession = new InferenceSession(encoderPath);
                _decoderSession = new InferenceSession(decoderPath);

                // Load tokenizer vocabulary
                _vocab = new Dictionary<string, int>();
                _reverseVocab = new Dictionary<int, string>();
                LoadTokenizer(tokenizerPath);

                Console.WriteLine("ONNX models loaded successfully!");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to initialize ONNX models: {ex.Message}", ex);
            }
        }

        private void LoadTokenizer(string tokenizerPath)
        {
            try
            {
                string json = File.ReadAllText(tokenizerPath);
                using (JsonDocument document = JsonDocument.Parse(json))
                {
                    var root = document.RootElement;
                    if (root.TryGetProperty("model", out var model) && 
                        model.TryGetProperty("vocab", out var vocab))
                    {
                        foreach (var item in vocab.EnumerateObject())
                        {
                            string token = item.Name;
                            int id = item.Value.GetInt32();
                            _vocab[token] = id;
                            _reverseVocab[id] = token;
                        }
                    }
                }
                Console.WriteLine($"Loaded {_vocab.Count} tokens from tokenizer");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not load tokenizer vocabulary: {ex.Message}");
                // Fallback to simple tokenization
                CreateSimpleVocab();
            }
        }

        private void CreateSimpleVocab()
        {
            // Create a minimal vocabulary for demonstration
            _vocab["<pad>"] = PAD_TOKEN_ID;
            _vocab["</s>"] = EOS_TOKEN_ID;
            _vocab["<unk>"] = UNK_TOKEN_ID;
            _reverseVocab[PAD_TOKEN_ID] = "<pad>";
            _reverseVocab[EOS_TOKEN_ID] = "</s>";
            _reverseVocab[UNK_TOKEN_ID] = "<unk>";
        }

        // NOTE: The encoder/decoder sessions are loaded and validated above, but this
        // method does not yet run inference through them. A correct implementation needs
        // SentencePiece tokenization, an encoder forward pass, and autoregressive decoder
        // generation (with cache handling for decoder_model_merged.onnx). Until that lands,
        // this deliberately falls back to the extractive summarizer rather than fabricate
        // output attributed to the neural model. Tracked as a follow-up; see README.
        public string SummarizeText(string text, int maxLength = 150)
        {
            try
            {
                var fallbackSummarizer = new TextSummarizer(3);
                string fallbackSummary = fallbackSummarizer.SummarizeText(text);

                return $"[Extractive fallback — ONNX neural generation not yet implemented] {fallbackSummary}";
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"ONNX summarization failed: {ex.Message}", ex);
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _encoderSession?.Dispose();
                _decoderSession?.Dispose();
                _disposed = true;
            }
        }
    }
}
