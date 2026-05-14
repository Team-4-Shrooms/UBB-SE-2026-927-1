
window.toggleLike = function (id, url) {
    if (typeof $ === 'undefined') {
        console.error("jQuery is still not loaded!");
        return;
    }

    $.post(url, { reelId: id }, function (data) {
        const countElem = document.getElementById('like-count-' + id);
        const btnElem = document.getElementById('like-btn-' + id);

        if (countElem) countElem.innerText = data.likeCount;
        if (btnElem) {
            btnElem.classList.toggle('btn-outline-light');
            btnElem.classList.toggle('btn-danger');
        }
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

function recordWatch(id, url, duration, percentage) {
    if (typeof $ === 'undefined') return;

    $.post(url, {
        reelId: id,
        watchDurationSec: duration,
        watchPercentage: percentage
    });
}
