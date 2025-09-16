const API_BASE_URL = 'http://localhost:5253/api';

export const getLearningMode = async () => {
    const response = await fetch(`${API_BASE_URL}/learning-mode`);
    return response.json();
};

export const setLearningMode = async (enabled) => {
    const response = await fetch(`${API_BASE_URL}/learning-mode`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ enabled }),
    });
    return response.json();
};
