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