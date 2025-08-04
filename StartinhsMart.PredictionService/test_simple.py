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

    # Test predict endpoint với một ảnh mẫu
    # Tạo một ảnh test đơn giản (1x1 pixel)
    test_image_data = b'\x89PNG\r\n\x1a\n\x00\x00\x00\rIHDR\x00\x00\x00\x01\x00\x00\x00\x01\x08\x02\x00\x00\x00\x90wS\xde\x00\x00\x00\x0cIDATx\x9cc\xf8\xff\xff?\x00\x05\xfe\x02\xfe\xdc\xccY\xe7\x00\x00\x00\x00IEND\xaeB`\x82'
    
    try:
        files = {'image': ('test.png', test_image_data, 'image/png')}
        response = requests.post('http://127.0.0.1:8080/predict', files=files)
        
        print(f"Predict API status: {response.status_code}")
        if response.status_code == 200:
            print(f"Response: {response.json()}")
        else:
            print(f"Error response: {response.text}")
    except Exception as e:
        print(f"Predict API test failed: {e}")

if __name__ == "__main__":
    test_predict_api() 