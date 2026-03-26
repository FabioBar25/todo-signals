import { HttpErrorResponse } from '@angular/common/http';
import { TimeoutError } from 'rxjs';

export const AUTH_TIMEOUT_MS = 10000;

export function getAuthErrorMessage(error: unknown, action: 'login' | 'register'): string {
  if (error instanceof TimeoutError) {
    return 'The request took too long. Make sure the backend is running and try again.';
  }

  if (error instanceof HttpErrorResponse) {
    const apiMessage = extractApiMessage(error.error);
    if (apiMessage) {
      return apiMessage;
    }

    if (error.status === 0) {
      return 'We could not reach the backend. Check that the API and proxy are running.';
    }

    if (error.status === 400) {
      return action === 'login'
        ? 'Your email or password looks incorrect.'
        : 'Please review the form details and try again.';
    }

    if (error.status === 401) {
      return 'Your email or password looks incorrect.';
    }

    if (error.status === 409) {
      return 'An account with that email already exists.';
    }
  }

  return action === 'login'
    ? 'We could not sign you in right now. Please try again.'
    : 'We could not create your account right now. Please try again.';
}

function extractApiMessage(payload: unknown): string | null {
  if (typeof payload === 'string' && payload.trim()) {
    return payload.trim();
  }

  if (payload && typeof payload === 'object') {
    const maybeMessage = 'message' in payload ? payload.message : null;
    const maybeTitle = 'title' in payload ? payload.title : null;

    if (typeof maybeMessage === 'string' && maybeMessage.trim()) {
      return maybeMessage.trim();
    }

    if (typeof maybeTitle === 'string' && maybeTitle.trim()) {
      return maybeTitle.trim();
    }
  }

  return null;
}
