import { HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';

import { AuthApi, LoginRequest, RegisterRequest } from '../api/auth-api';
import { AuthUser } from './auth-user';

@Injectable({ providedIn: 'root' })
export class AuthSessionService {
  private readonly authApi = inject(AuthApi);

  readonly currentUser = signal<AuthUser | null>(null);
  readonly initialized = signal(false);

  async refreshSession(): Promise<AuthUser | null> {
    try {
      const user = await firstValueFrom(this.authApi.getCurrentUser());
      this.currentUser.set(user);
      this.initialized.set(true);
      return user;
    } catch (error) {
      if (error instanceof HttpErrorResponse && error.status === 401) {
        this.currentUser.set(null);
        this.initialized.set(true);
        return null;
      }

      throw error;
    }
  }

  async ensureSession(): Promise<AuthUser | null> {
    if (this.initialized()) {
      return this.currentUser();
    }

    return this.refreshSession();
  }

  async login(payload: LoginRequest): Promise<AuthUser> {
    const user = await firstValueFrom(this.authApi.login(payload));
    this.currentUser.set(user);
    this.initialized.set(true);
    return user;
  }

  async register(payload: RegisterRequest): Promise<AuthUser> {
    const user = await firstValueFrom(this.authApi.register(payload));
    this.currentUser.set(user);
    this.initialized.set(true);
    return user;
  }

  async logout(): Promise<void> {
    await firstValueFrom(this.authApi.logout());
    this.currentUser.set(null);
    this.initialized.set(true);
  }
}
