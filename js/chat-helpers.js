window.anagram = window.anagram || {};
window.anagram.scrollToBottom = (el) => {
    if (!el) return;
    try {
        el.scrollTop = el.scrollHeight;
    } catch (e) { /* ignore */ }
};
navigator.share({
    title: "Join me on Anagram",
    text: "Let's communicate on Anagram.",
    url: "https://anagram-peach.vercel.app",
    url: "https://anagram.netlify.app"
});
