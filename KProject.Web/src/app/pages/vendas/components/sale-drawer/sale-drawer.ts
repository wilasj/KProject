import { Component, input, output } from '@angular/core';

@Component({
    selector: 'app-sale-drawer',
    templateUrl: './sale-drawer.html',
    styleUrl: './sale-drawer.scss',
})
export class SaleDrawer {
    saleId = input<number | null>(null);
    close = output<void>();
}
