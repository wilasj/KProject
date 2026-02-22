import {CanActivateFn, Router} from '@angular/router';
import {Auth} from '@core/auth';
import {inject} from '@angular/core';

export const publicGuard: CanActivateFn = (route, state) => {
  const authService = inject(Auth);
  // const router = inject(Router);

  return !authService.isLoggedIn();

  // return router.createUrlTree(['/login']);
};
