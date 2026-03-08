import { Component, effect, ElementRef, inject, input, OnDestroy, signal, ViewChild } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DatePipe } from '@angular/common';
import { Lote, TipoHistorico, StockMovement, HistoricoPage } from '@models/lote';

const TIPO_LABEL: Record<TipoHistorico, string> = {
  Entrada:            'Entrada',
  SaidaConsignacao:   'Saída por consignação',
  RetornoConsignacao: 'Retorno de consignação',
  Ajuste:             'Ajuste',
  Perda:              'Perda',
};

const PAGE_SIZE = 10;
const ROW_HEIGHT_PX = 42; // padding×2 + border + content + gap — must match SCSS

@Component({
  selector: 'app-lote-history',
  imports: [DatePipe],
  templateUrl: './lote-history.html',
  styleUrl: './lote-history.scss',
})
export class LoteHistory implements OnDestroy {
  private http = inject(HttpClient);

  lote = input.required<Lote>();

  historico = signal<StockMovement[]>([]);
  loadingInitial = signal(false);
  loadingMore = signal(false);
  hasMore = signal(false);
  readonly tipoLabel = TIPO_LABEL;
  readonly listHeight = `${PAGE_SIZE * ROW_HEIGHT_PX}px`;

  showTopFog = signal(false);
  showBottomFog = signal(false);

  private page = 1;
  private observer?: IntersectionObserver;
  private scrollEl?: HTMLElement;
  private sentinelEl?: Element;

  @ViewChild('scrollContainer') set scrollContainer(el: ElementRef<HTMLElement> | undefined) {
    this.scrollEl = el?.nativeElement;
    if (el) this.updateFogs(el.nativeElement);
    this.setupObserver();
  }

  @ViewChild('sentinel') set sentinel(el: ElementRef | undefined) {
    this.sentinelEl = el?.nativeElement;
    this.setupObserver();
  }

  private setupObserver() {
    this.observer?.disconnect();
    if (this.sentinelEl && this.scrollEl) {
      this.observer = new IntersectionObserver(
        entries => { if (entries[0].isIntersecting) this.loadMore(); },
        { root: this.scrollEl },
      );
      this.observer.observe(this.sentinelEl);
    }
  }

  onScroll(el: HTMLElement) {
    this.updateFogs(el);
  }

  private refreshFogs() {
    // defer one tick so Angular renders the new items before measuring
    setTimeout(() => {
      if (this.scrollEl) this.updateFogs(this.scrollEl);
    });
  }

  private updateFogs(el: HTMLElement) {
    this.showTopFog.set(el.scrollTop > 0);
    this.showBottomFog.set(el.scrollTop + el.clientHeight < el.scrollHeight - 1);
  }

  constructor() {
    effect(() => this.loadInitial(this.lote().id));
  }

  ngOnDestroy() {
    this.observer?.disconnect();
  }

  loadMore() {
    if (!this.hasMore() || this.loadingMore()) return;
    const nextPage = this.page + 1;
    this.loadingMore.set(true);
    this.fetchPage(this.lote().id, nextPage).subscribe({
      next: (res) => {
        this.historico.update(h => [...h, ...res.items]);
        this.hasMore.set(res.hasMore);
        this.page = nextPage;
        this.loadingMore.set(false);
        this.refreshFogs();
      },
      error: () => this.loadingMore.set(false),
    });
  }

  private loadInitial(id: number) {
    this.page = 1;
    this.historico.set([]);
    this.hasMore.set(false);
    this.loadingInitial.set(true);

    this.fetchPage(id, 1).subscribe({
      next: (page) => {
        this.historico.set(page.items);
        this.hasMore.set(page.hasMore);
        this.loadingInitial.set(false);
        this.refreshFogs();
      },
      error: () => this.loadingInitial.set(false),
    });
  }

  private fetchPage(id: number, page: number) {
    return this.http.get<HistoricoPage>(`/api/lotes/${id}/historico`, {
      params: { pagina: page, tamanhoPagina: PAGE_SIZE },
    });
  }
}
