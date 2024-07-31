export const getInputTypeFromSqlTypeString = (type: string): string => {
    switch (type) {
    case 'int':
    case 'float':
    case 'decimal':
        return 'number';
    case 'varchar':
    case 'text':
        return 'text';
    case 'date':
        return 'date';
    case 'datetime':
        return 'datetime-local';
    default:
        return 'text';
    }
};