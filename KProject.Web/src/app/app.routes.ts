import {Routes} from '@angular/router';
import {PublicLayout} from '@layouts/public-layout/public-layout';
import {publicGuard} from '@core/public-guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  {
    path: '',
    component: PublicLayout,
    canActivate: [publicGuard],
    children: [
      {
        path: 'login', loadComponent: () => import('@pages/login/login').then(m => m.Login),
      },
      {
        path: 'register', loadComponent: () => import('@pages/register/register').then(m => m.Register),
      }
    ]
  }
];
