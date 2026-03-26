import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Router, RouterLink } from '@angular/router';
import { finalize, timeout } from 'rxjs';

import { AuthApi } from '../../core/api/auth-api';
import { AUTH_TIMEOUT_MS, getAuthErrorMessage } from '../auth.utils';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.scss'
})
export class RegisterComponent {
  private readonly authApi = inject(AuthApi);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  readonly fullName = signal('');
  readonly email = signal('');
  readonly password = signal('');
  readonly confirmPassword = signal('');
  readonly isSubmitting = signal(false);
  readonly error = signal<string | null>(null);
  readonly success = signal<string | null>(null);

  submit() {
    const fullName = this.fullName().trim();
    const email = this.email().trim();
    const password = this.password().trim();
    const confirmPassword = this.confirmPassword().trim();

    if (!fullName || !email || !password || !confirmPassword) {
      this.error.set('Fill in each field before creating your account.');
      this.success.set(null);
      return;
    }

    if (password.length < 8) {
      this.error.set('Use at least 8 characters for your password.');
      this.success.set(null);
      return;
    }

    if (password !== confirmPassword) {
      this.error.set('Passwords must match before you can continue.');
      this.success.set(null);
      return;
    }

    this.isSubmitting.set(true);
    this.error.set(null);
    this.success.set(null);

    this.authApi
      .register({ fullName, email, password })
      .pipe(
        timeout(AUTH_TIMEOUT_MS),
        finalize(() => this.isSubmitting.set(false)),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe({
        next: () => {
          this.success.set('Account created. Redirecting you to your tasks...');
          void this.router.navigateByUrl('/tasks');
        },
        error: (error) => {
          this.error.set(getAuthErrorMessage(error, 'register'));
        }
      });
  }
}
