import {Component, inject, output} from '@angular/core';
import {RouterLink, RouterLinkActive} from '@angular/router';
import {Auth} from '@core/auth';

@Component({
  selector: 'app-sidebar',
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss',
})
export class Sidebar {
  private auth = inject(Auth);
  logoutClick = output<void>();

  get initial(): string {
    const email = this.auth.email();
    return email ? email[0].toUpperCase() : '?';
  }

  navItems = [
    { path: '/vendas',    icon: 'point_of_sale', label: 'Vendas'    },
    { path: '/clientes',  icon: 'group',         label: 'Clientes'  },
    { path: '/estoque',   icon: 'inventory_2',   label: 'Estoque'   },
    { path: '/relatorios',icon: 'analytics',     label: 'Relatórios'},
  ];
}
