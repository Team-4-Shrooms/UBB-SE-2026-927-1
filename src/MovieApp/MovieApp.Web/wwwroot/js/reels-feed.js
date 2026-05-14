
window.toggleLike = function (id, url) {
    if (typeof $ === 'undefined') {
        console.error("jQuery is not loaded!");
        return;
    }

    $.post(url, { reelId: id }, function (data) {
        const countElem = document.getElementById('like-count-' + id);
        const btnElem = document.getElementById('like-btn-' + id);
        const iconElem = btnElem ? btnElem.querySelector('i') : null;

        if (countElem) countElem.innerText = data.likeCount;
        if (btnElem) btnElem.classList.toggle('btn-danger-active', data.isLiked);
        if (iconElem) iconElem.className = data.isLiked ? 'bi bi-heart-fill' : 'bi bi-heart';
    }).fail(function () {
        console.error("Failed to update like for reel:", id);
    });
};

window.handleWatch = function (id, videoElement, url) {
    const endTime = Date.now();
    const start = videoElement.startTime || endTime;
    const duration = (endTime - start) / 1000;
    const percentage = videoElement.duration ? (videoElement.currentTime / videoElement.duration) * 100 : 0;

    recordWatch(id, url, Math.round(duration), Math.round(percentage));
    videoElement.startTime = Date.now();
};

window.updateProgress = function (id, video) {
    if (!video.duration) return;
    const fill = document.getElementById('progress-' + id);
    if (fill) fill.style.width = ((video.currentTime / video.duration) * 100) + '%';
};

function recordWatch(id, url, duration, percentage) {
    if (typeof $ === 'undefined') return;

    $.post(url, {
        reelId: id,
        watchDurationSec: duration,
        watchPercentage: percentage
    });
}
