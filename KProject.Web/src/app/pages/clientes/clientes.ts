import { Component, viewChild } from '@angular/core';
import { PageLayout, PageLayoutConfig, PageLayoutContentDef, PageLayoutDrawerDef } from '@components/page-layout/page-layout';
import { DataTable, DataTableRowDef } from '@components/data-table/data-table';
import { ClientDrawer } from './components/client-drawer/client-drawer';

@Component({
  selector: 'app-clientes',
  imports: [PageLayout, PageLayoutContentDef, PageLayoutDrawerDef, DataTable, DataTableRowDef, ClientDrawer],
  templateUrl: './clientes.html',
  styleUrl: './clientes.scss',
})
export class Clientes {
  layout = viewChild.required(PageLayout);

  config: PageLayoutConfig = {
    title: 'Clientes',
    buttonLabel: 'Novo Cliente',
    searchPlaceholder: 'Buscar clientes...',
    endpoint: '/api/clientes',
    pageSize: 10,
  };
}
