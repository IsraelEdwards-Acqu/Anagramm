window.anagram = window.anagram || {};
window.anagram.scrollToBottom = (el) => {
    if (!el) return;
    try {
        el.scrollTop = el.scrollHeight;
    } catch (e) { /* ignore */ }
};
