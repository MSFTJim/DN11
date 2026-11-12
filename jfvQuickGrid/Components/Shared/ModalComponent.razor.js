const handlers = new Map();
let nextHandlerId = 0;

export function registerEscapeHandler(dotNetHelper) {
    const id = ++nextHandlerId;
    const handler = event => {
        if (event.key === "Escape") {
            dotNetHelper.invokeMethodAsync("HandleEscapeKey");
        }
    };

    window.addEventListener("keydown", handler);
    handlers.set(id, handler);
    return id;
}

export function unregisterEscapeHandler(id) {
    const handler = handlers.get(id);
    if (handler) {
        window.removeEventListener("keydown", handler);
        handlers.delete(id);
    }
}
