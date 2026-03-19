// 브라우저 알림 헬퍼
window.notificationHelper = {
    requestPermission: async function () {
        if (!("Notification" in window)) return "unsupported";
        return await Notification.requestPermission();
    },
    getPermission: function () {
        if (!("Notification" in window)) return "unsupported";
        return Notification.permission;
    },
    showNotification: function (title, body, url) {
        if (Notification.permission !== "granted") return false;
        const n = new Notification(title, {
            body: body,
            icon: "/favicon.png",
            tag: "devtodo-" + Date.now()
        });
        if (url) {
            n.onclick = function () {
                window.focus();
                window.location.href = url;
                n.close();
            };
        }
        return true;
    }
};
