// wwwroot/notifications.js

let wakeLock = null;
let videoElement = null;

export async function enableWakeLock() {
    if ('wakeLock' in navigator) {
        try {
            wakeLock = await navigator.wakeLock.request('screen');
        } catch (err) { console.log("WakeLock hatası:", err); }
    }

    if (!videoElement) {
        videoElement = document.createElement('video');
        videoElement.muted = true;
        videoElement.loop = true;
        videoElement.setAttribute('playsinline', '');
        videoElement.style.display = 'none';

        const canvas = document.createElement('canvas');
        canvas.width = canvas.height = 1;
        const ctx = canvas.getContext('2d');
        ctx.fillStyle = 'black';
        ctx.fillRect(0, 0, 1, 1);

        videoElement.srcObject = canvas.captureStream(1);
        document.body.appendChild(videoElement);
    }
    videoElement.play().catch(e => console.log("Video başlatılamadı:", e));
}

export function disableWakeLock() {
    if (wakeLock !== null) {
        wakeLock.release();
        wakeLock = null;
    }
    if (videoElement) {
        videoElement.pause();
    }
}

export function playBell() {
    const audio = new Audio('sounds/bell.mp3');
    audio.play().catch(e => console.log("Ses hatası:", e));
}

export function initAudio() {
    const audio = new Audio('sounds/bell.mp3');
    audio.muted = true; // İlk başta sessiz
    audio.play().then(() => {
        audio.pause();
        audio.muted = false;
        console.log("Ses kilidi açıldı.");
    }).catch(e => console.log("Kilit açma başarısız:", e));
}