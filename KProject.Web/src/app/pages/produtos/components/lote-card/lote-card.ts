import { Component, input, output } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Lote } from '@models/lote';

@Component({
  selector: 'app-lote-card',
  imports: [DatePipe],
  templateUrl: './lote-card.html',
  styleUrl: './lote-card.scss',
  host: {
    '[class.lote-card--selected]': 'selected()',
    '(click)': 'onClick()',
  },
})
export class LoteCard {
  lote = input.required<Lote>();
  selected = input(false);

  select = output<Lote>();

  onClick() {
    this.select.emit(this.lote());
  }
}
