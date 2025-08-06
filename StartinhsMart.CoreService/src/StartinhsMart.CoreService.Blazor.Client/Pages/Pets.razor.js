// File cleanup functionality
export function clearInputFiles() {
    const fileInputs = document.querySelectorAll('input[type="file"]');
    fileInputs.forEach(input => {
        input.value = '';
    });
}

// Webcam functionality
export function startWebcam() {
    navigator.mediaDevices.getUserMedia({ video: true })
        .then((stream) => {
            let video = document.getElementById("video");
            video.srcObject = stream;
        })
        .catch((err) => {
            console.error("Webcam access error:", err);
        });
}

export function stopWebcam() {
    let video = document.getElementById("video");
    let stream = video.srcObject;
    if (stream) {
        let tracks = stream.getTracks();
        tracks.forEach(track => track.stop());
    }
    video.srcObject = null;
}

export function captureImage() {
    let canvas = document.getElementById("canvas");
    let video = document.getElementById("video");
    let context = canvas.getContext("2d");
    context.drawImage(video, 0, 0, canvas.width, canvas.height);
    return canvas.toDataURL("image/jpeg");
}