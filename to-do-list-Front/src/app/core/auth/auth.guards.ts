import { CanActivateFn, Router, UrlTree } from '@angular/router';
import { inject } from '@angular/core';

import { AuthSessionService } from './auth-session.service';

export const authRequiredGuard: CanActivateFn = async (): Promise<boolean | UrlTree> => {
  const authSession = inject(AuthSessionService);
  const router = inject(Router);
  try {
    const user = await authSession.ensureSession();

    return user ? true : router.createUrlTree(['/login']);
  } catch {
    return router.createUrlTree(['/login']);
  }
};

export const guestOnlyGuard: CanActivateFn = async (): Promise<boolean | UrlTree> => {
  const authSession = inject(AuthSessionService);
  const router = inject(Router);
  try {
    const user = await authSession.ensureSession();

    return user ? router.createUrlTree(['/tasks']) : true;
  } catch {
    return true;
  }
};
