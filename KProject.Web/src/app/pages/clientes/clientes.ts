import { Component, inject, signal, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { Cliente, ClientesResponse } from '@models/cliente';
import { SearchBar } from '@components/search-bar/search-bar';
import { ClientDrawer } from './components/client-drawer/client-drawer';
import { ClientTable } from './components/client-table/client-table';

@Component({
  selector: 'app-clientes',
  imports: [SearchBar, ClientDrawer, ClientTable],
  templateUrl: './clientes.html',
  styleUrl: './clientes.scss',
})
export class Clientes implements OnInit {
  private http   = inject(HttpClient);
  private router = inject(Router);
  private route  = inject(ActivatedRoute);

  readonly pageSize = 10;

  clients       = signal<Cliente[]>([]);
  total         = signal(0);
  currentPage   = signal(1);
  loading       = signal(false);
  drawerOpen    = signal(false);
  initialSearch = signal('');

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      const search = params['busca'] ?? '';
      const page   = Number(params['pagina'] ?? 1);
      this.initialSearch.set(search);
      this.currentPage.set(page);
      this.fetchClients(search, page);
    });
  }

  onSearch(term: string) {
    this.router.navigate([], {
      queryParams: { busca: term || null, pagina: 1 },
      queryParamsHandling: 'merge',
    });
  }

  onPageChange(page: number) {
    this.router.navigate([], {
      queryParams: { pagina: page },
      queryParamsHandling: 'merge',
    });
  }

  onClientCreated() {
    this.drawerOpen.set(false);
    this.fetchClients(this.initialSearch(), this.currentPage());
  }

  private fetchClients(search: string, page: number) {
    this.loading.set(true);
    const params: Record<string, string | number> = { pagina: page, tamanhoPagina: this.pageSize };
    if (search) params['busca'] = search;

    this.http.get<ClientesResponse>('/api/clientes', { params }).subscribe({
      next: (res) => {
        this.clients.set(res.items);
        this.total.set(res.total);
        this.loading.set(false);
      },
      error: () => {
        this.clients.set([]);
        this.loading.set(false);
      },
    });
  }
}
