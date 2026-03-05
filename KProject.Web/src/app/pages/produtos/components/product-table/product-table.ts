import { Component, computed, input, output, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Product } from '@models/produto';
import { ProductAccordion } from '../product-accordion/product-accordion';

@Component({
  selector: 'app-product-table',
  imports: [DatePipe, ProductAccordion],
  templateUrl: './product-table.html',
  styleUrl: './product-table.scss',
})
export class ProductTable {
  products = input.required<Product[]>();
  total = input.required<number>();
  currentPage = input.required<number>();
  pageSize = input<number>(10);

  pageChange = output<number>();

  expandedProductId = signal<number | null>(null);

  totalPages = computed(() => Math.ceil(this.total() / this.pageSize()));
  pages = computed(() => Array.from({ length: this.totalPages() }, (_, i) => i + 1));

  onToggle(productId: number) {
    this.expandedProductId.update(id => id === productId ? null : productId);
  }

  goToPage(page: number) {
    if (page >= 1 && page <= this.totalPages()) {
      this.pageChange.emit(page);
    }
  }
}
