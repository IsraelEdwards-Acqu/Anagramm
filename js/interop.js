window.interop = {
    pickFile: async function () {
        return new Promise((resolve) => {
            const input = document.createElement('input');
            input.type = 'file';
            input.onchange = async e => {
                const file = e.target.files[0];
                const arrayBuffer = await file.arrayBuffer();
                resolve(new Uint8Array(arrayBuffer));
            };
            input.click();
        });
    },

    capturePhoto: async function () {
        return new Promise((resolve) => {
            navigator.mediaDevices.getUserMedia({ video: true })
                .then(stream => {
                    const video = document.createElement('video');
                    video.srcObject = stream;
                    video.play();
                    const canvas = document.createElement('canvas');
                    setTimeout(() => {
                        canvas.width = video.videoWidth;
                        canvas.height = video.videoHeight;
                        canvas.getContext('2d').drawImage(video, 0, 0);
                        stream.getTracks().forEach(track => track.stop());
                        canvas.toBlob(blob => {
                            blob.arrayBuffer().then(buf => resolve(new Uint8Array(buf)));
                        }, 'image/png');
                    }, 2000); // capture after 2s
                });
        });
    },

    recordVoiceNote: async function () {
        return new Promise((resolve) => {
            navigator.mediaDevices.getUserMedia({ audio: true })
                .then(stream => {
                    const mediaRecorder = new MediaRecorder(stream);
                    const chunks = [];
                    mediaRecorder.ondataavailable = e => chunks.push(e.data);
                    mediaRecorder.onstop = () => {
                        const blob = new Blob(chunks, { type: 'audio/webm' });
                        blob.arrayBuffer().then(buf => resolve(new Uint8Array(buf)));
                        stream.getTracks().forEach(track => track.stop());
                    };
                    mediaRecorder.start();
                    setTimeout(() => mediaRecorder.stop(), 5000); // record 5s
                });
        });
    }
};
