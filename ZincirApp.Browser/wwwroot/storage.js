export function setItem(key, value) {
    try {
        localStorage.setItem(key, value);
        return "ok";
    } catch (e) {
        return e.name;
    }
}

export function getItem(key) {
    return localStorage.getItem(key);
}