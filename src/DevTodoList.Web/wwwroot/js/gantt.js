// 간트차트 스크롤 동기화 (4-quadrant 레이아웃)
window.ganttScrollSync = {
    _disposed: false,
    _handler: null,

    init: function (rightBody, leftBody, rightHeader) {
        this._disposed = false;
        if (!rightBody) return;

        this._handler = function () {
            if (leftBody) leftBody.scrollTop = rightBody.scrollTop;
            if (rightHeader) rightHeader.scrollLeft = rightBody.scrollLeft;
        };

        rightBody.addEventListener('scroll', this._handler, { passive: true });
    },

    dispose: function (rightBody) {
        this._disposed = true;
        if (rightBody && this._handler) {
            rightBody.removeEventListener('scroll', this._handler);
        }
        this._handler = null;
    }
};
