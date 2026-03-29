import { Component, contentChild, Directive, inject, input, OnInit, signal, TemplateRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { NgTemplateOutlet } from '@angular/common';
import { SearchBar } from '@components/search-bar/search-bar';
import { PaginatedResponse } from '@models/paginated-response';

export interface PageLayoutConfig {
  title: string;
  buttonLabel: string;
  searchPlaceholder: string;
  endpoint: string;
  pageSize?: number;
}

@Directive({ selector: '[pageLayoutContent]' })
export class PageLayoutContentDef {
  constructor(public templateRef: TemplateRef<unknown>) {}
}

@Directive({ selector: '[pageLayoutDrawer]' })
export class PageLayoutDrawerDef {
  constructor(public templateRef: TemplateRef<unknown>) {}
}

@Component({
  selector: 'app-page-layout',
  imports: [NgTemplateOutlet, SearchBar],
  templateUrl: './page-layout.html',
  styleUrl: './page-layout.scss',
})
export class PageLayout<T = unknown> implements OnInit {
  private http = inject(HttpClient);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  config = input.required<PageLayoutConfig>();

  items = signal<T[]>([]);
  total = signal(0);
  currentPage = signal(1);
  loading = signal(false);
  drawerOpen = signal(false);
  initialSearch = signal('');

  contentDef = contentChild.required(PageLayoutContentDef);
  drawerDef = contentChild.required(PageLayoutDrawerDef);

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      const search = params['busca'] ?? '';
      const page = Number(params['pagina'] ?? 1);
      this.initialSearch.set(search);
      this.currentPage.set(page);
      this.fetchItems(search, page);
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

  closeDrawer() {
    this.drawerOpen.set(false);
  }

  onItemSaved() {
    this.drawerOpen.set(false);
    this.fetchItems(this.initialSearch(), this.currentPage());
  }

  private fetchItems(search: string, page: number) {
    const cfg = this.config();
    const pageSize = cfg.pageSize ?? 10;
    this.loading.set(true);
    const params: Record<string, string | number> = { pagina: page, tamanhoPagina: pageSize };
    if (search) {
      params['busca'] = search;
    }

    this.http.get<PaginatedResponse<T>>(cfg.endpoint, { params }).subscribe({
      next: (res) => {
        this.items.set(res.items);
        this.total.set(res.total);
        this.loading.set(false);
      },
      error: () => {
        this.items.set([]);
        this.loading.set(false);
      },
    });
  }
}
