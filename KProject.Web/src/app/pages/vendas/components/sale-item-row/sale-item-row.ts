import { Component, computed, input, output, signal } from '@angular/core';
import { EditableItem } from '@models/venda';

@Component({
  selector: 'app-sale-item-row',
  templateUrl: './sale-item-row.html',
  styleUrl: './sale-item-row.scss',
})
export class SaleItemRow {
  item     = input.required<EditableItem>();
  readOnly = input<boolean>(false);

  itemChange = output<EditableItem>();

  expanded = signal(false);

  used = computed(() => this.item().vendido + this.item().devolvido);

  toggle() {
    this.expanded.update(v => !v);
  }

  changeVendido(delta: number) {
    const i = this.item();
    const next = i.vendido + delta;
    if (next < 0 || next + i.devolvido > i.quantidadeConsignada) return;
    this.itemChange.emit({ ...i, vendido: next });
  }

  changeDevolvido(delta: number) {
    const i = this.item();
    const next = i.devolvido + delta;
    if (next < 0 || i.vendido + next > i.quantidadeConsignada) return;
    this.itemChange.emit({ ...i, devolvido: next });
  }
}
