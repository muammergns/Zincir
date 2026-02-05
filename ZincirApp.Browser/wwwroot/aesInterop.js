export async function aesEncrypt(plainText, password, salt, iterations) {
    const encoder = new TextEncoder();
    const data = encoder.encode(plainText);
    const saltBuf = encoder.encode(salt);

    const baseKey = await crypto.subtle.importKey(
        "raw", encoder.encode(password), "PBKDF2", false, ["deriveKey"]
    );

    const key = await crypto.subtle.deriveKey(
        { name: "PBKDF2", salt: saltBuf, iterations: iterations, hash: "SHA-256" },
        baseKey,
        { name: "AES-CBC", length: 256 },
        false,
        ["encrypt"]
    );

    const iv = crypto.getRandomValues(new Uint8Array(16));
    const encrypted = await crypto.subtle.encrypt(
        { name: "AES-CBC", iv: iv },
        key,
        data
    );

    const combined = new Uint8Array(iv.length + encrypted.byteLength);
    combined.set(iv);
    combined.set(new Uint8Array(encrypted), iv.length);

    return btoa(String.fromCharCode(...combined));
}

export async function aesDecrypt(cipherText, password, salt, iterations) {
    const encoder = new TextEncoder();
    const combined = Uint8Array.from(atob(cipherText), c => c.charCodeAt(0));
    const saltBuf = encoder.encode(salt);

    const iv = combined.slice(0, 16);
    const data = combined.slice(16);

    const baseKey = await crypto.subtle.importKey(
        "raw", encoder.encode(password), "PBKDF2", false, ["deriveKey"]
    );

    const key = await crypto.subtle.deriveKey(
        { name: "PBKDF2", salt: saltBuf, iterations: iterations, hash: "SHA-256" },
        baseKey,
        { name: "AES-CBC", length: 256 },
        false,
        ["decrypt"]
    );

    const decrypted = await crypto.subtle.decrypt(
        { name: "AES-CBC", iv: iv },
        key,
        data
    );

    return new TextDecoder().decode(decrypted);
}