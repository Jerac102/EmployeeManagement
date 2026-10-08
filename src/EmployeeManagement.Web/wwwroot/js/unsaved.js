let dirty = false;

window.addEventListener('beforeunload', e => {
    if (dirty) {
        e.preventDefault();
        e.returnValue = '';
    }
});

export function setDirty(value) {
    dirty = value;
}
