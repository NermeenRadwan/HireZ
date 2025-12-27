// wwwroot/js/auth.js
import { login } from './api-client.js';

/**
 * handleLoginForm(formElement, onSuccess)
 * - formElement: the <form> DOM node
 * - onSuccess: optional callback called with the login response
 */
export function handleLoginForm(formElement, onSuccess) {
    formElement.addEventListener('submit', async (e) => {
        e.preventDefault();
        const email = (formElement.querySelector('[name="email"]')?.value || '').trim();
        const password = (formElement.querySelector('[name="password"]')?.value || '').trim();

        try {
            const res = await login(email, password);
            console.log('Login success', res);
            if (typeof onSuccess === 'function') onSuccess(res);
        } catch (err) {
            console.error('Login failed', err);
            alert('Login failed: ' + (err.message || err));
        }
    });
}
