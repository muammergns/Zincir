export function setItem(key, value) {
    console.log(`[StorageService] Save key: ${key}, Value: ${value}`);
    try {
        localStorage.setItem(key, value);
        return "ok";
    } catch (e) {
        return e.name;
    }
}

export function getItem(key) {
    const value = localStorage.getItem(key);
    console.log(`[StorageService] Reading key: ${key}, Value: ${value}`);
    return value;
}