const CACHE_NAME = "anagram-cache-v1";

const ASSETS = [
    "/",
    "/index.html",
    "/css/home.css",
    "/css/chat-layout.css",
    "/css/profile.css",
    "/css/updates.css",
    "/css/auth.css",
    "/css/editprofile.css",
    "/css/calls.css",
    "/icon-32.png",
    "/icon-64.png",
    "/icon-192.png",
    "/icon-512.png"
];

self.addEventListener("install", event => {

    event.waitUntil(
        caches.open(CACHE_NAME)
            .then(cache => cache.addAll(ASSETS))
    );

    self.skipWaiting();
});

self.addEventListener("activate", event => {

    event.waitUntil(
        caches.keys()
            .then(keys =>
                Promise.all(
                    keys
                        .filter(k => k !== CACHE_NAME)
                        .map(k => caches.delete(k))
                )
            )
    );

    self.clients.claim();
});

self.addEventListener("fetch", event => {

    const request = event.request;

    if (
        request.url.includes("/api/")
        || request.url.includes("/hubs/")
        || request.url.includes("signalr")
    ) {
        event.respondWith(fetch(request));
        return;
    }

    event.respondWith(
        caches.match(request)
            .then(cached => {

                if (cached)
                    return cached;

                return fetch(request);
            })
    );

});
