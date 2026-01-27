using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace TextSummarizer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    Console.WriteLine("Usage: TextSummarizer <input-file.txt>");
                    Console.WriteLine("Example: TextSummarizer input.txt");
                    return;
                }

                string inputFile = args[0];
                
                if (!File.Exists(inputFile))
                {
                    Console.WriteLine($"Error: File '{inputFile}' not found.");
                    return;
                }

                Console.WriteLine($"Processing file: {inputFile}");
                
                // Read input text
                string inputText = File.ReadAllText(inputFile, Encoding.UTF8);
                
                if (string.IsNullOrWhiteSpace(inputText))
                {
                    Console.WriteLine("Error: Input file is empty.");
                    return;
                }

                string summary;
                
                // Try to use ONNX model first, fallback to extractive summarizer
                string modelsPath = "models";
                if (Directory.Exists(modelsPath) && 
                    File.Exists(Path.Combine(modelsPath, "encoder_model.onnx")) &&
                    File.Exists(Path.Combine(modelsPath, "decoder_model_merged.onnx")))
                {
                    Console.WriteLine("Using ONNX neural summarization model...");
                    try
                    {
                        using (var onnxSummarizer = new ONNXTextSummarizer(modelsPath))
                        {
                            summary = onnxSummarizer.SummarizeText(inputText);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"ONNX model failed, falling back to extractive summarizer: {ex.Message}");
                        var fallbackSummarizer = new TextSummarizer();
                        summary = fallbackSummarizer.SummarizeText(inputText);
                    }
                }
                else
                {
                    Console.WriteLine("ONNX models not found, using extractive summarization...");
                    var summarizer = new TextSummarizer();
                    summary = summarizer.SummarizeText(inputText);
                }
                
                // Write summary to file
                string outputFile = "summary.txt";
                File.WriteAllText(outputFile, summary, Encoding.UTF8);
                
                Console.WriteLine($"Summary generated successfully!");
                Console.WriteLine($"Input length: {inputText.Length} characters");
                Console.WriteLine($"Summary length: {summary.Length} characters");
                Console.WriteLine($"Compression ratio: {(double)summary.Length / inputText.Length:P1}");
                Console.WriteLine($"Summary saved to: {outputFile}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
