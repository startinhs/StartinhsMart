from flask import Flask, request, jsonify, send_from_directory
import numpy as np
from keras.src.saving import load_model
from keras.src.legacy.preprocessing import image
from keras.src.applications.inception_v3 import preprocess_input
import io
import pandas as pd
from pymongo import MongoClient
import datetime

app = Flask(__name__)
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

@app.route('/')
def index():
    return send_from_directory('PawsomePets', 'index.html')

@app.route('/gioi-thieu.html')
def gioi_thieu():
    return send_from_directory('PawsomePets', 'gioi-thieu.html')

@app.route('/lien-he.html')
def lien_he():
    return send_from_directory('PawsomePets', 'lien-he.html')

@app.route('/thu-ngo.html')
def thu_ngo():
    return send_from_directory('PawsomePets', 'thu-ngo.html')

@app.route('/chinh-sach.html')
@app.route('/chinh-sach/<path:filename>.html')
def chinh_sach(filename=None):
    if filename is None:
        return send_from_directory('PawsomePets', 'chinh-sach.html')
    return send_from_directory('PawsomePets/chinh-sach', f'{filename}.html')

@app.route('/tin-tuc.html')
@app.route('/tin-tuc/<path:filename>.html')
def tin_tuc(filename=None):
    if filename is None:
        return send_from_directory('PawsomePets', 'tin-tuc.html')
    return send_from_directory('PawsomePets/tin-tuc', f'{filename}.html')

@app.route('/cho-corgi/')
@app.route('/cho-corgi/<path:filename>.html')
def cho_corgi(filename=None):
    if filename is None:
        return send_from_directory('PawsomePets/cho-corgi', 'index.html')
    return send_from_directory('PawsomePets/cho-corgi', f'{filename}.html')

@app.route('/cho-lap-xuong/')
@app.route('/cho-lap-xuong/<path:filename>.html')
def cho_lap_xuong(filename=None):
    if filename is None:
        return send_from_directory('PawsomePets/cho-lap-xuong', 'index.html')
    return send_from_directory('PawsomePets/cho-lap-xuong', f'{filename}.html')

@app.route('/cho-phoc-soc/')
@app.route('/cho-phoc-soc/<path:filename>.html')
def cho_phoc_soc(filename=None):
    if filename is None:
        return send_from_directory('PawsomePets/cho-phoc-soc', 'index.html')
    return send_from_directory('PawsomePets/cho-phoc-soc', f'{filename}.html')

@app.route('/cho-poodle/')
@app.route('/cho-poodle/<path:filename>.html')
def cho_poodle(filename=None):
    if filename is None:
        return send_from_directory('PawsomePets/cho-poodle', 'index.html')
    return send_from_directory('PawsomePets/cho-poodle', f'{filename}.html')

@app.route('/cho-shiba/')
@app.route('/cho-shiba/<path:filename>.html')
def cho_shiba(filename=None):
    if filename is None:
        return send_from_directory('PawsomePets/cho-shiba', 'index.html')
    return send_from_directory('PawsomePets/cho-shiba', f'{filename}.html')

@app.route('/prediction')
def prediction():
    return send_from_directory('PawsomePets', 'prediction.html')

# Kết nối MongoDB
client = MongoClient("mongodb+srv://tinh:tinh@pawsomepetscluster.hvkej.mongodb.net/?retryWrites=true&w=majority&appName=PawsomePetsCluster")
mongo_db = client["DogBreedPrediction"]
history_collection = mongo_db["PredictionHistories"]

@app.route('/predict', methods=['POST'])
def predict():
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

if __name__ == '__main__':
    app.run(port=8080, debug=True)
