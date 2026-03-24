import { Component, inject, signal, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { Sale, SalesResponse } from '@models/venda';
import { SearchBar } from '@components/search-bar/search-bar';
import { SaleTable } from './components/sale-table/sale-table';
import { SaleDrawer } from './components/sale-drawer/sale-drawer';

@Component({
    selector: 'app-vendas',
    imports: [SearchBar, SaleTable, SaleDrawer],
    templateUrl: './vendas.html',
    styleUrl: './vendas.scss',
})
export class Vendas implements OnInit {
    private http = inject(HttpClient);
    private router = inject(Router);
    private route = inject(ActivatedRoute);

    readonly pageSize = 10;

    sales = signal<Sale[]>([]);
    total = signal(0);
    currentPage = signal(1);
    loading = signal(false);
    drawerOpen = signal(false);
    selectedSaleId = signal<number | null>(null);
    initialSearch = signal('');

    ngOnInit() {
        this.route.queryParams.subscribe(params => {
            const search = params['busca'] ?? '';
            const page = Number(params['pagina'] ?? 1);
            this.initialSearch.set(search);
            this.currentPage.set(page);
            this.fetchSales(search, page);
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

    openNewSale() {
        this.selectedSaleId.set(null);
        this.drawerOpen.set(true);
    }

    onSaleSelect(id: number) {
        this.selectedSaleId.set(id);
        this.drawerOpen.set(true);
    }

    onSaleCreated() {
        this.fetchSales(this.initialSearch(), this.currentPage());
    }

    onSaleUpdated() {
        this.fetchSales(this.initialSearch(), this.currentPage());
    }

    closeDrawer() {
        this.drawerOpen.set(false);
        this.selectedSaleId.set(null);
    }

    private fetchSales(search: string, page: number) {
        this.loading.set(true);
        const params: Record<string, string | number> = { pagina: page, tamanhoPagina: this.pageSize };
        if (search) params['busca'] = search;

        this.http.get<SalesResponse>('/api/vendas', { params }).subscribe({
            next: (res) => {
                this.sales.set(res.items);
                this.total.set(res.total);
                this.loading.set(false);
            },
            error: () => {
                this.sales.set([]);
                this.loading.set(false);
            },
        });
    }
}
