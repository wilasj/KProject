import { Component, viewChild } from '@angular/core';
import { DatePipe } from '@angular/common';
import { PageLayout, PageLayoutConfig, PageLayoutContentDef, PageLayoutDrawerDef } from '@components/page-layout/page-layout';
import { DataTable, DataTableRowDef } from '@components/data-table/data-table';
import { SaleDrawer } from './components/sale-drawer/sale-drawer';

@Component({
  selector: 'app-vendas',
  imports: [DatePipe, PageLayout, PageLayoutContentDef, PageLayoutDrawerDef, DataTable, DataTableRowDef, SaleDrawer],
  templateUrl: './vendas.html',
  styleUrl: './vendas.scss',
})
export class Vendas {
  layout = viewChild.required(PageLayout);

  config: PageLayoutConfig = {
    title: 'Vendas',
    buttonLabel: 'Nova Venda',
    searchPlaceholder: 'Buscar vendas...',
    endpoint: '/api/vendas',
    pageSize: 10,
  };
}
