import { Component, contentChild, Directive, input, output, TemplateRef } from '@angular/core';
import { NgTemplateOutlet } from '@angular/common';
import { Pagination } from '@components/pagination/pagination';

@Directive({ selector: '[dataTableRow]' })
export class DataTableRowDef<T> {
  constructor(public templateRef: TemplateRef<{ $implicit: T }>) {}
}

@Component({
  selector: 'app-data-table',
  imports: [NgTemplateOutlet, Pagination],
  templateUrl: './data-table.html',
  styleUrl: './data-table.scss',
})
export class DataTable<T extends { id: number }> {
  items       = input.required<T[]>();
  total       = input.required<number>();
  currentPage = input.required<number>();
  pageSize    = input<number>(10);

  emptyIcon    = input<string>('search_off');
  emptyMessage = input<string>('Nenhum item encontrado');

  pageChange = output<number>();

  rowDef = contentChild.required(DataTableRowDef);
}
