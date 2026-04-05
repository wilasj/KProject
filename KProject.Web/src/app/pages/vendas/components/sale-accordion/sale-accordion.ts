import {
  Component,
  computed,
  effect,
  ElementRef,
  inject,
  input,
  OnDestroy,
  output,
  signal,
  ViewChild,
} from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DatePipe } from '@angular/common';
import { Sale, SaleDetail, EditableItem } from '@models/venda';
import { SaleItemRow } from '../sale-item-row/sale-item-row';

type ActionState = 'idle' | 'closing' | 'cancelling' | 'closed' | 'cancelled';

const CLOSE_TRANSITION_MS = 200;

@Component({
  selector: 'app-sale-accordion',
  imports: [DatePipe, SaleItemRow],
  templateUrl: './sale-accordion.html',
  styleUrl: './sale-accordion.scss',
})
export class SaleAccordion implements OnDestroy {
  private http = inject(HttpClient);

  sale = input.required<Sale>();
  saleUpdated = output<void>();

  expanded    = signal(false);
  isClosing   = signal(false);
  loading     = signal(false);
  saving      = signal(false);
  actionState = signal<ActionState>('idle');

  private detail = signal<SaleDetail | null>(null);
  editableItems  = signal<EditableItem[]>([]);

  showTopFog    = signal(false);
  showBottomFog = signal(false);

  private scrollEl?: HTMLElement;
  private actionTimer?: ReturnType<typeof setTimeout>;
  private closeTimer?: ReturnType<typeof setTimeout>;

  @ViewChild('scrollContainer') set scrollContainer(el: ElementRef<HTMLElement> | undefined) {
    this.scrollEl = el?.nativeElement;
    if (el) this.updateFogs(el.nativeElement);
  }

  total      = computed(() => this.editableItems().reduce((s, i) => s + i.quantidadeConsignada, 0));
  vendidos   = computed(() => this.editableItems().reduce((s, i) => s + i.vendido, 0));
  devolvidos = computed(() => this.editableItems().reduce((s, i) => s + i.devolvido, 0));
  emAberto   = computed(() => this.total() - this.vendidos() - this.devolvidos());

  isReadOnly = computed(() => this.detail()?.status !== 'Aberta');

  isDirty = computed(() => {
    const original = this.detail()?.itens ?? [];
    const editable = this.editableItems();
    return editable.some((e, idx) => {
      const o = original[idx];
      return !o || e.vendido !== o.vendido || e.devolvido !== o.devolvido;
    });
  });

  modificadaEm  = computed(() => this.detail()?.modificadaEm ?? null);
  modificadaPor = computed(() => this.detail()?.criadaPor ?? null);
  clienteNome   = computed(() => this.detail()?.clienteNome ?? this.sale().clienteNome);

  constructor() {
    effect(() => {
      if (this.expanded() && !this.detail()) {
        this.loadDetail();
      }
      if (!this.expanded()) {
        this.actionState.set('idle');
      }
    });
  }

  ngOnDestroy() {
    clearTimeout(this.actionTimer);
    clearTimeout(this.closeTimer);
  }

  toggle() {
    if (this.expanded()) {
      this.isClosing.set(true);
      this.closeTimer = setTimeout(() => {
        this.isClosing.set(false);
        this.expanded.set(false);
      }, CLOSE_TRANSITION_MS);
    } else {
      this.expanded.set(true);
    }
  }

  onItemChange(updated: EditableItem) {
    this.editableItems.update(items =>
      items.map(i => (i.id === updated.id ? updated : i)),
    );
  }

  onSave() {
    if (!this.isDirty() || this.saving()) return;
    this.saving.set(true);
    const saved = this.editableItems();
    const body = {
      itens: saved.map(i => ({ id: i.id, vendido: i.vendido, devolvido: i.devolvido })),
    };
    this.http.patch(`/api/vendas/${this.sale().id}`, body).subscribe({
      next: () => {
        this.saving.set(false);
        // Sync baseline so isDirty resets — no list reload needed
        this.detail.update(d => d ? { ...d, itens: saved.map(i => ({ ...i })) } : d);
      },
      error: () => this.saving.set(false),
    });
  }

  onFechar() {
    if (this.actionState() !== 'idle') return;
    this.actionState.set('closing');
    this.http.post(`/api/vendas/${this.sale().id}/close`, {}).subscribe({
      next: () => this.triggerActionFeedback('closed'),
      error: () => this.actionState.set('idle'),
    });
  }

  onCancelar() {
    if (this.actionState() !== 'idle') return;
    this.actionState.set('cancelling');
    this.http.post(`/api/vendas/${this.sale().id}/cancel`, {}).subscribe({
      next: () => this.triggerActionFeedback('cancelled'),
      error: () => this.actionState.set('idle'),
    });
  }

  onScroll(el: HTMLElement) {
    this.updateFogs(el);
  }

  private triggerActionFeedback(state: 'closed' | 'cancelled') {
    this.actionState.set(state);
    this.actionTimer = setTimeout(() => {
      this.actionState.set('idle');
      this.isClosing.set(true);
      this.closeTimer = setTimeout(() => {
        this.isClosing.set(false);
        this.expanded.set(false);
        this.saleUpdated.emit();
      }, CLOSE_TRANSITION_MS);
    }, 2000);
  }

  private loadDetail() {
    this.loading.set(true);
    this.http.get<SaleDetail>(`/api/vendas/${this.sale().id}`).subscribe({
      next: (d) => {
        this.detail.set(d);
        this.editableItems.set(d.itens.map(i => ({ ...i })));
        this.loading.set(false);
        setTimeout(() => {
          if (this.scrollEl) this.updateFogs(this.scrollEl);
        });
      },
      error: () => this.loading.set(false),
    });
  }

  private updateFogs(el: HTMLElement) {
    this.showTopFog.set(el.scrollTop > 0);
    this.showBottomFog.set(el.scrollTop + el.clientHeight < el.scrollHeight - 1);
  }
}
