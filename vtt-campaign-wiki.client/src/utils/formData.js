export function toFormData (obj) {
    const formData = new FormData();

    Object.keys(obj).forEach(key => {
        if (obj[ key ] !== null && obj[ key ] !== undefined) {
            formData.append(key, obj[ key ]);
        }
    });

    return formData;
}