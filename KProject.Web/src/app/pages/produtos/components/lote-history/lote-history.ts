import { Component, effect, inject, input, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DatePipe } from '@angular/common';
import { LoteDetail, TipoHistorico } from '@models/lote';

const TIPO_LABEL: Record<TipoHistorico, string> = {
  Entrada:            'Entrada',
  SaidaConsignacao:   'Saída por consignação',
  RetornoConsignacao: 'Retorno de consignação',
  Ajuste:             'Ajuste',
  Perda:              'Perda',
};

@Component({
  selector: 'app-lote-history',
  imports: [DatePipe],
  templateUrl: './lote-history.html',
  styleUrl: './lote-history.scss',
})
export class LoteHistory {
  private http = inject(HttpClient);

  loteId = input.required<number>();

  detail = signal<LoteDetail | null>(null);
  loading = signal(false);
  readonly tipoLabel = TIPO_LABEL;

  constructor() {
    effect(() => {
      this.loadDetail(this.loteId());
    });
  }

  private loadDetail(id: number) {
    this.loading.set(true);
    this.detail.set(null);
    this.http.get<LoteDetail>(`/api/lotes/${id}`).subscribe({
      next: (d) => {
        this.detail.set(d);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }
}
