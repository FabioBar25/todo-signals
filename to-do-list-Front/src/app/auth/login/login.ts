import { FormsModule } from '@angular/forms';
import { Component, DestroyRef, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Router, RouterLink } from '@angular/router';
import { from, timeout, finalize } from 'rxjs';

import { AuthSessionService } from '../../core/auth/auth-session.service';
import { AUTH_TIMEOUT_MS, getAuthErrorMessage } from '../auth.utils';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class LoginComponent {
  private readonly authSession = inject(AuthSessionService);
  private readonly router = inject(Router);
  private readonly destroyRef = inject(DestroyRef);

  readonly email = signal('');
  readonly password = signal('');
  readonly isSubmitting = signal(false);
  readonly error = signal<string | null>(null);

  submit() {
    const email = this.email().trim();
    const password = this.password().trim();

    if (!email || !password) {
      this.error.set('Enter both your email and password to continue.');
      return;
    }

    this.isSubmitting.set(true);
    this.error.set(null);

    from(this.authSession.login({ email, password }))
      .pipe(
        timeout(AUTH_TIMEOUT_MS),
        finalize(() => this.isSubmitting.set(false)),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe({
        next: () => {
          void this.router.navigateByUrl('/tasks');
        },
        error: (error) => {
          this.error.set(getAuthErrorMessage(error, 'login'));
        }
      });
  }
}
