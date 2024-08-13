export function toFormData (obj, form = new FormData(), namespace = '') {
    Object.keys(obj).forEach(key => {
        const value = obj[ key ];
        const formKey = namespace ? `${namespace}.${key}` : key;

        if (value === null || value === undefined) {
            // Handle null or undefined values explicitly
            form.append(formKey, '');
        } else if (typeof value === 'object' && !(value instanceof Date) && !(value instanceof File)) {
            // Recursively handle nested objects, converting arrays and objects to JSON
            toFormData(value, form, formKey);
        } else {
            form.append(formKey, value);
        }
    });

    return form;
}