import { Component, input, output } from '@angular/core';
import { Cliente } from '@models/cliente';
import { Pagination } from '@components/pagination/pagination';

@Component({
  selector: 'app-client-table',
  imports: [Pagination],
  templateUrl: './client-table.html',
  styleUrl: './client-table.scss',
})
export class ClientTable {
  clients     = input.required<Cliente[]>();
  total       = input.required<number>();
  currentPage = input.required<number>();
  pageSize    = input<number>(10);

  pageChange = output<number>();
}
