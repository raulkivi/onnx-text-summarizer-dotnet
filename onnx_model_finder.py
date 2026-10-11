"""
ONNX Model Search Utility
This script helps find available ONNX text summarization models on Hugging Face
"""

import requests

def search_onnx_summarization_models():
    """Search for ONNX text summarization models on Hugging Face"""
    
    url = "https://huggingface.co/api/models"
    params = {
        "pipeline_tag": "summarization",
        "library": "onnx",
        "sort": "downloads",
        "direction": -1,
        "limit": 10
    }
    
    try:
        response = requests.get(url, params=params)
        response.raise_for_status()
        models = response.json()
        
        print("🔍 Top ONNX Text Summarization Models:")
        print("=" * 60)
        
        for i, model in enumerate(models, 1):
            model_id = model.get('id', 'Unknown')
            downloads = model.get('downloads', 0)
            likes = model.get('likes', 0)
            
            print(f"{i:2d}. {model_id}")
            print(f"    Downloads: {downloads:,}")
            print(f"    Likes: {likes}")
            print(f"    URL: https://huggingface.co/{model_id}")
            print()
            
    except requests.RequestException as e:
        print(f"❌ Error searching models: {e}")
        return None
    
    return models

def get_model_files(model_id):
    """Get list of files for a specific model"""
    
    url = f"https://huggingface.co/api/models/{model_id}/tree/main"
    
    try:
        response = requests.get(url)
        response.raise_for_status()
        files = response.json()
        
        print(f"📁 Files in {model_id}:")
        print("-" * 40)
        
        onnx_files = []
        for file_info in files:
            filename = file_info.get('path', '')
            size = file_info.get('size', 0)
            
            if filename.endswith('.onnx'):
                onnx_files.append(filename)
                size_mb = size / (1024 * 1024)
                print(f"🧠 {filename} ({size_mb:.1f} MB)")
            elif filename.endswith('.json') or filename.endswith('.model'):
                print(f"⚙️  {filename}")
        
        return onnx_files
        
    except requests.RequestException as e:
        print(f"❌ Error getting model files: {e}")
        return []

if __name__ == "__main__":
    print("🚀 ONNX Text Summarization Model Finder")
    print("=" * 50)
    
    # Search for models
    models = search_onnx_summarization_models()
    
    # Example: Get files for the Falconsai model we used
    print("\n" + "=" * 60)
    get_model_files("Falconsai/text_summarization")
