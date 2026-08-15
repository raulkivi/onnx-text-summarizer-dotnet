# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/).

## [Unreleased]

### Changed
- Retargeted from `net9.0` (out of support) to `net10.0` (current LTS).
- Upgraded `Microsoft.ML.OnnxRuntime` 1.22.1 → 1.29.0 and `Microsoft.ML.Tokenizers` 1.0.2 → 2.0.0.
- `ONNXTextSummarizer.SummarizeText()` now labels its output as
  `[Extractive fallback — ONNX neural generation not yet implemented]` instead of the previous,
  misleading `[ONNX Model Loaded]` prefix. Behavior is unchanged (it still runs the extractive
  summarizer); only the labeling is corrected to not imply neural generation occurred.
- README rewritten to accurately describe current capabilities and link a new
  "Current Limitations" section instead of documenting the target/aspirational pipeline as done.

### Added
- `tests/TextSummarizer.Tests`, an xUnit project covering the extractive summarizer, wired into
  `onnx.sln` and CI.
- `.github/workflows/ci.yml` — builds and runs tests on push/PR to `main`.
- `LICENSE` (MIT, matching the license already declared in the README).
- `.gitattributes` to normalize line endings and stop whole-file diffs from line-ending churn.

## [0.1.0] - 2025-08-22

### Added
- Initial release: extractive (word-frequency) text summarizer with position bias and keyword
  boosting, plus scaffolding for ONNX/T5-based neural summarization.
- `onnx_model_finder.py` utility for discovering ONNX summarization models on Hugging Face.
