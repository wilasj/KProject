import { Component, viewChild } from '@angular/core';
import { DatePipe } from '@angular/common';
import { PageLayout, PageLayoutConfig, PageLayoutContentDef, PageLayoutDrawerDef } from '@components/page-layout/page-layout';
import { DataTable, DataTableRowDef } from '@components/data-table/data-table';
import { ProductDrawer } from './components/product-drawer/product-drawer';
import { ProductAccordion } from './components/product-accordion/product-accordion';

@Component({
  selector: 'app-produtos',
  imports: [DatePipe, PageLayout, PageLayoutContentDef, PageLayoutDrawerDef, DataTable, DataTableRowDef, ProductDrawer, ProductAccordion],
  templateUrl: './produtos.html',
  styleUrl: './produtos.scss',
})
export class Produtos {
  layout = viewChild.required(PageLayout);

  config: PageLayoutConfig = {
    title: 'Produtos',
    buttonLabel: 'Novo Produto',
    searchPlaceholder: 'Buscar produtos...',
    endpoint: '/api/produtos',
    pageSize: 10,
  };
}
