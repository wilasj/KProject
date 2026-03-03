import { Component, inject, signal, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { Product, ProductsResponse } from '@models/produto';
import { SearchBar } from '@components/search-bar/search-bar';
import { ProductDrawer } from './components/product-drawer/product-drawer';
import { ProductTable } from './components/product-table/product-table';

@Component({
  selector: 'app-produtos',
  imports: [SearchBar, ProductDrawer, ProductTable],
  templateUrl: './produtos.html',
  styleUrl: './produtos.scss',
})
export class Produtos implements OnInit {
  private http = inject(HttpClient);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  readonly pageSize = 10;

  products = signal<Product[]>([]);
  total = signal(0);
  currentPage = signal(1);
  loading = signal(false);
  drawerOpen = signal(false);
  initialSearch = signal('');

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      const search = params['busca'] ?? '';
      const page = Number(params['pagina'] ?? 1);
      this.initialSearch.set(search);
      this.currentPage.set(page);
      this.fetchProducts(search, page);
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

  onProductCreated() {
    this.drawerOpen.set(false);
    this.fetchProducts(this.initialSearch(), this.currentPage());
  }

  private fetchProducts(search: string, page: number) {
    this.loading.set(true);
    const params: Record<string, string | number> = { pagina: page, tamanhoPagina: this.pageSize };
    if (search) params['busca'] = search;

    this.http.get<ProductsResponse>('/api/produtos', { params }).subscribe({
      next: (res) => {
        this.products.set(res.items);
        this.total.set(res.total);
        this.loading.set(false);
      },
      error: () => {
        this.products.set([]);
        this.loading.set(false);
      },
    });
  }
}
