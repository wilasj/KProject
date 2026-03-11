import { Component, input, output } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Sale } from '@models/venda';
import { Pagination } from '@components/pagination/pagination';

@Component({
    selector: 'app-sale-table',
    imports: [DatePipe, Pagination],
    templateUrl: './sale-table.html',
    styleUrl: './sale-table.scss',
})
export class SaleTable {
    sales       = input.required<Sale[]>();
    total       = input.required<number>();
    currentPage = input.required<number>();
    pageSize    = input<number>(10);

    pageChange = output<number>();
    saleSelect = output<number>();
}
