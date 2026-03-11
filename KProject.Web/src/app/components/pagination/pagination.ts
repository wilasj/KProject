import { Component, computed, input, output } from '@angular/core';

@Component({
    selector: 'app-pagination',
    templateUrl: './pagination.html',
    styleUrl: './pagination.scss',
})
export class Pagination {
    total    = input.required<number>();
    current  = input.required<number>();
    pageSize = input<number>(10);

    pageChange = output<number>();

    totalPages = computed(() => Math.ceil(this.total() / this.pageSize()));
    pages      = computed(() => Array.from({ length: this.totalPages() }, (_, i) => i + 1));

    goToPage(page: number) {
        if (page >= 1 && page <= this.totalPages() && page !== this.current()) {
            this.pageChange.emit(page);
        }
    }
}
