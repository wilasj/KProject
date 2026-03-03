import { Component, computed, input, output } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Product } from '@models/produto';

@Component({
  selector: 'app-product-table',
  imports: [DatePipe],
  templateUrl: './product-table.html',
  styleUrl: './product-table.scss',
})
export class ProductTable {
  products = input.required<Product[]>();
  total = input.required<number>();
  currentPage = input.required<number>();
  pageSize = input<number>(10);

  pageChange = output<number>();

  totalPages = computed(() => Math.ceil(this.total() / this.pageSize()));
  pages = computed(() => Array.from({ length: this.totalPages() }, (_, i) => i + 1));

  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages()) {
      this.pageChange.emit(page);
    }
  }
}
