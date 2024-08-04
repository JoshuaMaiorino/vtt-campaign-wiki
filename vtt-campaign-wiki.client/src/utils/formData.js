export function toFormData(obj, form = new FormData(), namespace = '') {
    Object.keys(obj).forEach(key => {
        const value = obj[key];
        const formKey = namespace ? `${namespace}.${key}` : key;

        if (typeof value === 'object' && value !== null && !(value instanceof Date) && !(value instanceof File)) {
            // Directly append JSON string for nested objects except arrays
            form.append(formKey, JSON.stringify(value));
        } else {
            form.append(formKey, value);
        }
    });

    return form;
}
