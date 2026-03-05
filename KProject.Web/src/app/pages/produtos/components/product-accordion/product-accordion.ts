import { Component, effect, inject, input, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Product } from '@models/produto';
import { Lote } from '@models/lote';
import { LoteCard } from '../lote-card/lote-card';
import { LoteForm } from '../lote-form/lote-form';
import { LoteHistory } from '../lote-history/lote-history';

@Component({
  selector: 'app-product-accordion',
  imports: [LoteCard, LoteForm, LoteHistory],
  templateUrl: './product-accordion.html',
  styleUrl: './product-accordion.scss',
})
export class ProductAccordion {
  private http = inject(HttpClient);

  product = input.required<Product>();
  expanded = input(false);

  mode = signal<'grid' | 'form' | 'history'>('grid');
  lotes = signal<Lote[]>([]);
  loading = signal(false);
  selectedLote = signal<Lote | null>(null);

  constructor() {
    effect(() => {
      if (this.expanded()) {
        this.loadLotes();
      } else {
        this.mode.set('grid');
        this.selectedLote.set(null);
      }
    });
  }

  onAddLote() {
    this.mode.set('form');
    this.selectedLote.set(null);
  }

  onLoteSelect(lote: Lote) {
    if (this.selectedLote()?.id === lote.id) {
      this.mode.set('grid');
      this.selectedLote.set(null);
    } else {
      this.mode.set('history');
      this.selectedLote.set(lote);
    }
  }

  onLoteCriado() {
    this.mode.set('grid');
    this.loadLotes();
  }

  onCancelar() {
    this.mode.set('grid');
  }

  private loadLotes() {
    this.loading.set(true);
    this.http.get<Lote[]>(`/api/produtos/${this.product().id}/lotes`).subscribe({
      next: (lotes) => {
        this.lotes.set(lotes);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }
}
