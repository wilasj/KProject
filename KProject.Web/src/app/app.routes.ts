import {Routes} from '@angular/router';
import {publicGuard} from '@core/public-guard';
import {authGuard} from '@core/auth-guard';

export const routes: Routes = [
  { path: '', redirectTo: 'vendas', pathMatch: 'full' },
  {
    path: '',
    loadComponent: () => import('@layouts/public-layout/public-layout').then(m => m.PublicLayout),
    canActivate: [publicGuard],
    children: [
      { path: 'login',    loadComponent: () => import('@pages/login/login').then(m => m.Login) },
      { path: 'register', loadComponent: () => import('@pages/register/register').then(m => m.Register) },
    ]
  },
  {
    path: '',
    loadComponent: () => import('@layouts/secure-layout/secure-layout').then(m => m.SecureLayout),
    canActivate: [authGuard],
    children: [
      { path: 'vendas',    loadComponent: () => import('@pages/vendas/vendas').then(m => m.Vendas) },
      { path: 'clientes',  loadComponent: () => import('@pages/clientes/clientes').then(m => m.Clientes) },
      { path: 'estoque',   loadComponent: () => import('@pages/estoque/estoque').then(m => m.Estoque) },
      { path: 'produtos',  loadComponent: () => import('@pages/produtos/produtos').then(m => m.Produtos) },
      { path: 'relatorios',loadComponent: () => import('@pages/relatorios/relatorios').then(m => m.Relatorios) },
    ]
  },
];
