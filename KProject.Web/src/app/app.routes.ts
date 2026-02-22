import {Routes} from '@angular/router';
import {PublicLayout} from '@layouts/public-layout/public-layout';

export const routes: Routes = [
  {
    path: '',
    component: PublicLayout,
    children: [
      {
        path: 'login', loadComponent: () => import('./pages/login/login').then(m => m.Login),
      },
      {
        path: 'register', loadComponent: () => import('./pages/register/register').then(m => m.Register),
      }
    ]
  }
];
