window.anagram = window.anagram || {};

window.anagram.shareInvite = async () => {

    if (!navigator.share)
        return;

    try {

        await navigator.share({
            title: "Join me on Anagram",
            text: "Let's communicate on Anagram.",
            url: "https://anagram-peach.vercel.app"
        });

    }
    catch {
    }
};

window.anagram.playMessageSound = function (kind) {
    const AudioContext =
        window.AudioContext ||
        window.webkitAudioContext;

    if (!AudioContext)
        return;

    const context = new AudioContext();
    const oscillator = context.createOscillator();
    const gain = context.createGain();

    const isSent = kind === "sent";
    const startFrequency = isSent ? 620 : 420;
    const endFrequency = isSent ? 880 : 620;

    oscillator.type = "sine";
    oscillator.frequency.setValueAtTime(
        startFrequency,
        context.currentTime);

    oscillator.frequency.exponentialRampToValueAtTime(
        endFrequency,
        context.currentTime + 0.12);

    gain.gain.setValueAtTime(
        0.0001,
        context.currentTime);

    gain.gain.exponentialRampToValueAtTime(
        0.08,
        context.currentTime + 0.015);

    gain.gain.exponentialRampToValueAtTime(
        0.0001,
        context.currentTime + 0.16);

    oscillator.connect(gain);
    gain.connect(context.destination);

    oscillator.start();
    oscillator.stop(context.currentTime + 0.18);

    oscillator.addEventListener(
        "ended",
        () => context.close(),
        { once: true });
};