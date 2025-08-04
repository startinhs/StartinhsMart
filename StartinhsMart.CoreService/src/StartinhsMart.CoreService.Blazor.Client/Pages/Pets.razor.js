// File cleanup functionality
window.FileCleanup = {
    clearInputFiles: function () {
        const fileInputs = document.querySelectorAll('input[type="file"]');
        fileInputs.forEach(input => {
            input.value = '';
        });
    }
};

// Webcam functionality
window.showWebcamPopup = function () {
    document.getElementById("webcamModal").style.display = "flex";
    navigator.mediaDevices.getUserMedia({ video: true })
        .then((stream) => {
            let video = document.getElementById("video");
            video.srcObject = stream;
        })
        .catch((err) => {
            console.error("Webcam access error:", err);
        });
};

window.closeWebcamPopup = function () {
    document.getElementById("webcamModal").style.display = "none";
    let video = document.getElementById("video");
    let stream = video.srcObject;
    if (stream) {
        let tracks = stream.getTracks();
        tracks.forEach(track => track.stop());
    }
    video.srcObject = null;
};

window.captureImage = function () {
    let canvas = document.getElementById("canvas");
    let video = document.getElementById("video");
    let context = canvas.getContext("2d");
    context.drawImage(video, 0, 0, canvas.width, canvas.height);
    return canvas.toDataURL("image/jpeg");
};