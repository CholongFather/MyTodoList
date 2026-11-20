// 간트차트 스크롤 헬퍼
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
    },

    // 오늘 날짜 위치로 가로 스크롤
    scrollToToday: function (containerSelector, leftColWidth, todayOffsetPx) {
        var el = document.querySelector(containerSelector);
        if (!el) return;
        // 오늘 기준선이 화면 중앙~좌측 1/3에 오도록
        var viewWidth = el.clientWidth - leftColWidth;
        var scrollTarget = todayOffsetPx - (viewWidth * 0.3);
        el.scrollLeft = Math.max(0, scrollTarget);
    }
};
