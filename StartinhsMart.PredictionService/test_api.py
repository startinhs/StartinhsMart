import requests
import json

def test_predict_api():
    # Test health endpoint
    try:
        response = requests.get('http://127.0.0.1:8080/health')
        print(f"Health check: {response.status_code}")
        print(f"Response: {response.json()}")
    except Exception as e:
        print(f"Health check failed: {e}")
        return

    # Test predict endpoint with a sample image
    # You can replace this with an actual image file
    test_image_path = "test_image.jpg"  # You need to provide a test image
    
    try:
        with open(test_image_path, 'rb') as f:
            files = {'image': f}
            response = requests.post('http://127.0.0.1:8080/predict', files=files)
            
        print(f"Predict API status: {response.status_code}")
        print(f"Response: {response.json()}")
    except FileNotFoundError:
        print(f"Test image not found: {test_image_path}")
        print("Please provide a test image to test the predict API")
    except Exception as e:
        print(f"Predict API test failed: {e}")

if __name__ == "__main__":
    test_predict_api() 