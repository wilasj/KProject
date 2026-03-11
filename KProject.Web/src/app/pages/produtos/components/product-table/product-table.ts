import { Component, input, output, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Product } from '@models/produto';
import { ProductAccordion } from '../product-accordion/product-accordion';
import { Pagination } from '@components/pagination/pagination';

@Component({
  selector: 'app-product-table',
  imports: [DatePipe, ProductAccordion, Pagination],
  templateUrl: './product-table.html',
  styleUrl: './product-table.scss',
})
export class ProductTable {
  products   = input.required<Product[]>();
  total      = input.required<number>();
  currentPage = input.required<number>();
  pageSize   = input<number>(10);

  pageChange = output<number>();

  expandedProductId = signal<number | null>(null);

  onToggle(productId: number) {
    this.expandedProductId.update(id => id === productId ? null : productId);
  }
}
