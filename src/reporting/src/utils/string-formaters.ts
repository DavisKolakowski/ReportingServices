export const formatDateTime = (dateString: string) => {
    const date = new Date(dateString);
    return `${date.toLocaleDateString()}, ${date.toLocaleTimeString()}`;
};