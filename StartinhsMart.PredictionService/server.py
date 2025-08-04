from flask import Flask, request, jsonify
import numpy as np
from keras.src.saving import load_model
from keras.src.legacy.preprocessing import image
from keras.src.applications.inception_v3 import preprocess_input
import io
import pandas as pd
from pymongo import MongoClient
import datetime
from flask_cors import CORS

app = Flask(__name__)
CORS(app)  # Cho phép CORS để CoreService có thể gọi API

model = load_model('modelDogBreeds.h5')

# Load breeds list
breeds_df = pd.read_csv('breeds_list.csv')
breeds = breeds_df['Breed'].tolist()

def preprocess_image(file):
    img = image.image_utils.load_img(io.BytesIO(file.read()), target_size=(299, 299))
    img_array = image.image_utils.img_to_array(img)
    img_array = np.expand_dims(img_array, axis=0)
    img_array = preprocess_input(img_array)
    return img_array

# Kết nối MongoDB
client = MongoClient("mongodb+srv://tinh:tinh@pawsomepetscluster.hvkej.mongodb.net/?retryWrites=true&w=majority&appName=PawsomePetsCluster")
mongo_db = client["DogBreedPrediction"]
history_collection = mongo_db["PredictionHistories"]

@app.route('/predict', methods=['POST'])
def predict():
    print(f"Received {request.method} request to /predict")
    print(f"Request headers: {dict(request.headers)}")
    print(f"Request files: {list(request.files.keys())}")
    
    try:
        if 'image' not in request.files:
            return jsonify({'error': 'No image uploaded'})

        file = request.files['image']
        img_array = preprocess_image(file)

        prediction = model.predict(img_array)
        predicted_class = np.argmax(prediction, axis=1)[0]
        confidence = float(np.max(prediction))
        breed_name = breeds[predicted_class]
        
        # Lưu lịch sử vào MongoDB
        if(confidence>0.1):
            history_data = {
                "predicted_class": int(predicted_class),
                "breed_name": breed_name,
                "confidence": confidence,
                "timestamp": datetime.datetime.now()
            }
        else:
            history_data = {
                "messeger": "Không xác định được, vui lòng chụp đúng ảnh chú chó",
                "timestamp": datetime.datetime.now()
            }
        history_collection.insert_one(history_data)
        
        return jsonify({
            'predicted_class': int(predicted_class),
            'breed_name': breed_name,
            'confidence': confidence
        })
    except Exception as e:
        return jsonify({'error': str(e)})

@app.route('/health', methods=['GET'])
def health():
    return jsonify({'status': 'OK', 'message': 'Prediction service is running'})

if __name__ == '__main__':
    app.run(port=8080, debug=True)
