// storage.js - Gestion du localStorage via JavaScript interop

window.storageInterop = {
    setItem: function (key, value) {
        try {
            localStorage.setItem(key, value);
            console.log(`[LocalStorage] Stored key: ${key}`);
            return true;
        } catch (e) {
            console.error(`[LocalStorage] Error storing ${key}:`, e);
            return false;
        }
    },
    getItem: function (key) {
        try {
            const value = localStorage.getItem(key);
            if (value) {
                console.log(`[LocalStorage] Retrieved key: ${key}`);
            }
            return value;
        } catch (e) {
            console.error(`[LocalStorage] Error retrieving ${key}:`, e);
            return null;
        }
    },
    removeItem: function (key) {
        try {
            localStorage.removeItem(key);
            console.log(`[LocalStorage] Removed key: ${key}`);
            return true;
        } catch (e) {
            console.error(`[LocalStorage] Error removing ${key}:`, e);
            return false;
        }
    },
    clear: function () {
        try {
            localStorage.clear();
            console.log('[LocalStorage] Cleared all items');
            return true;
        } catch (e) {
            console.error('[LocalStorage] Error clearing:', e);
            return false;
        }
    }
};
