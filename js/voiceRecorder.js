window.voiceRecorder = {
    mediaRecorder: null,
    chunks: [],

    async start() {
        const stream =
            await navigator.mediaDevices.getUserMedia(
                { audio: true });

        this.chunks = [];

        this.mediaRecorder =
            new MediaRecorder(stream);

        this.mediaRecorder.ondataavailable =
            e => this.chunks.push(e.data);

        this.mediaRecorder.start();
    },

    async stop() {
        return new Promise(resolve => {

            this.mediaRecorder.onstop = async () => {

                const blob =
                    new Blob(
                        this.chunks,
                        { type: 'audio/webm' });

                const buffer =
                    await blob.arrayBuffer();

                resolve(
                    Array.from(
                        new Uint8Array(buffer)));
            };

            this.mediaRecorder.stop();
        });
    }
    window.voicePlayer = {

        play: function (base64Audio) {

            const bytes =
                Uint8Array.from(
                    atob(base64Audio),
                    c => c.charCodeAt(0));

            const blob =
                new Blob(
                    [bytes],
                    {
                        type: "audio/webm"
                    });

            const url =
                URL.createObjectURL(blob);

            new Audio(url).play();
        }
    };
};