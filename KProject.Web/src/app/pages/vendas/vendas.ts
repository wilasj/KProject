import { Component, viewChild } from '@angular/core';
import { PageLayout, PageLayoutConfig, PageLayoutContentDef, PageLayoutDrawerDef } from '@components/page-layout/page-layout';
import { DataTable, DataTableRowDef } from '@components/data-table/data-table';
import { SaleDrawer } from './components/sale-drawer/sale-drawer';
import { SaleAccordion } from './components/sale-accordion/sale-accordion';

@Component({
  selector: 'app-vendas',
  imports: [PageLayout, PageLayoutContentDef, PageLayoutDrawerDef, DataTable, DataTableRowDef, SaleDrawer, SaleAccordion],
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
