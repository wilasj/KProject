import { Component, computed, input, output } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Sale } from '@models/venda';

@Component({
    selector: 'app-sale-table',
    imports: [DatePipe],
    templateUrl: './sale-table.html',
    styleUrl: './sale-table.scss',
})
export class SaleTable {
    sales = input.required<Sale[]>();
    total = input.required<number>();
    currentPage = input.required<number>();
    pageSize = input<number>(10);

    pageChange = output<number>();
    saleSelect = output<number>();

    totalPages = computed(() => Math.ceil(this.total() / this.pageSize()));
    pages = computed(() => Array.from({ length: this.totalPages() }, (_, i) => i + 1));

    goToPage(page: number) {
        if (page >= 1 && page <= this.totalPages()) {
            this.pageChange.emit(page);
        }
    }
}
