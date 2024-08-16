export function toFormData (obj, form = new FormData(), namespace = '') {
    Object.keys(obj).forEach(key => {
        const value = obj[ key ];
        const formKey = namespace ? `${namespace}.${key}` : key;

        if (value === null || value === undefined) {
            // Handle null or undefined values explicitly
            form.append(formKey, '');
        } else if (typeof value === 'object' && !(value instanceof Date) && !(value instanceof File)) {
            // Convert objects and arrays to JSON string
            form.append(formKey, JSON.stringify(value));
        } else {
            form.append(formKey, value);
        }
    });

    return form;
}