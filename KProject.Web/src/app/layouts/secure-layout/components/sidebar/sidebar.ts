import {Component, inject, output, signal} from '@angular/core';
import {RouterLink, RouterLinkActive} from '@angular/router';
import {Auth} from '@core/auth';
import {InvitePopup} from '../invite-popup/invite-popup';

@Component({
  selector: 'app-sidebar',
  imports: [RouterLink, RouterLinkActive, InvitePopup],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss',
})
export class Sidebar {
  private auth = inject(Auth);
  logoutClick = output<void>();
  inviteOpen = signal(false);

  get initial(): string {
    const email = this.auth.email();
    return email ? email[0].toUpperCase() : '?';
  }

  navItems = [
    { path: '/vendas',    icon: 'point_of_sale', label: 'Vendas'    },
    { path: '/produtos',  icon: 'inventory_2',   label: 'Produtos'  },
    { path: '/clientes',  icon: 'group',         label: 'Clientes'  },
    { path: '/relatorios',icon: 'analytics',     label: 'Relatórios'},
  ];
}
