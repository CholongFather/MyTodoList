// TodoDetail 임시 저장 (localStorage)
window.draftStorage = {
    save: function (key, data) {
        var entry = { data: data, ts: Date.now() };
        localStorage.setItem(key, JSON.stringify(entry));
    },
    load: function (key, ttlMs) {
        var raw = localStorage.getItem(key);
        if (!raw) return null;
        try {
            var entry = JSON.parse(raw);
            if (Date.now() - entry.ts > ttlMs) {
                localStorage.removeItem(key);
                return null;
            }
            return entry.data;
        } catch (e) {
            localStorage.removeItem(key);
            return null;
        }
    },
    remove: function (key) {
        localStorage.removeItem(key);
    }
};
