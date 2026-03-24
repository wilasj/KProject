import { Component, computed, effect, inject, input, OnDestroy, output, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DatePipe } from '@angular/common';
import { debounceTime, distinctUntilChanged, Subject, Subscription, switchMap } from 'rxjs';
import { SaleDetail, SaleItemDetail } from '@models/venda';

type DrawerMode = 'idle' | 'searching-product' | 'selecting-lot' | 'configuring-item';
type SaveState = 'idle' | 'saving' | 'saved';

interface ClientOption { id: number; nome: string; }
interface ProductOption { id: number; nome: string; totalLotes: number; }
interface LotOption { id: number; numero: number; validade: string; quantidadeTotal: number; }

interface SaleItem {
    productId: number;
    productName: string;
    lotId: number;
    lotNumber: number;
    lotAvailable: number;
    quantity: number;
    patientName: string;
}

interface EditableItem {
    original: SaleItemDetail;
    vendido: number;
    devolvido: number;
}

@Component({
    selector: 'app-sale-drawer',
    templateUrl: './sale-drawer.html',
    styleUrl: './sale-drawer.scss',
    imports: [DatePipe],
})
export class SaleDrawer implements OnDestroy {
    private http = inject(HttpClient);

    saleId = input<number | null>(null);
    open = input<boolean>(false);
    close = output<void>();
    saleCreated = output<void>();
    saleUpdated = output<void>();

    mode = signal<DrawerMode>('idle');
    saveState = signal<SaveState>('idle');


    // Client
    clientDropdownOpen = signal(false);
    clientSearchTerm = signal('');
    allClients = signal<ClientOption[]>([]);
    selectedClient = signal<ClientOption | null>(null);
    filteredClients = computed(() => {
        const term = this.clientSearchTerm().toLowerCase();
        return term
            ? this.allClients().filter(c => c.nome.toLowerCase().includes(term))
            : this.allClients();
    });

    // Product search
    productSearchTerm = signal('');
    products = signal<ProductOption[]>([]);
    productsLoading = signal(false);
    private productSearch$ = new Subject<string>();
    private subs = new Subscription();

    // Lot selection
    selectedProduct = signal<ProductOption | null>(null);
    lots = signal<LotOption[]>([]);
    lotsLoading = signal(false);

    // Item configuration
    selectedLot = signal<LotOption | null>(null);
    itemQuantity = signal(0);
    patientName = signal('');
    recentPatients = signal<string[]>([]);
    patientDropdownOpen = signal(false);

    consumedByLot = computed(() => {
        const map = new Map<number, number>();
        for (const item of this.items()) {
            map.set(item.lotId, (map.get(item.lotId) ?? 0) + item.quantity);
        }
        return map;
    });

    effectiveLotAvailable = computed(() => {
        const lot = this.selectedLot();
        if (!lot) return 0;
        return lot.quantidadeTotal - (this.consumedByLot().get(lot.id) ?? 0);
    });

    filteredRecentPatients = computed(() => {
        const term = this.patientName().toLowerCase();
        return term
            ? this.recentPatients().filter(n => n.toLowerCase().includes(term))
            : this.recentPatients();
    });

    effectiveQtyForLot(lot: LotOption): number {
        return lot.quantidadeTotal - (this.consumedByLot().get(lot.id) ?? 0);
    }

    canAdd = computed(() =>
        this.itemQuantity() > 0 &&
        this.itemQuantity() <= this.effectiveLotAvailable() &&
        this.patientName().trim().length > 0
    );

    addButtonLabel = computed(() => {
        const qty = this.itemQuantity();
        return qty > 0 ? `+ Adicionar ${qty} ${qty === 1 ? 'unidade' : 'unidades'}` : '+ Adicionar';
    });

    // Items list
    items = signal<SaleItem[]>([]);
    totalItems = computed(() => this.items().reduce((sum, i) => sum + i.quantity, 0));
    canSave = computed(() => this.items().length > 0 && this.selectedClient() !== null && this.saveState() === 'idle');


    saleDetail     = signal<SaleDetail | null>(null);
    saleLoading    = signal(false);
    saleError      = signal(false);
    editableItems  = signal<EditableItem[]>([]);
    expandedItemId = signal<number | null>(null);

    totalConsignado = computed(() => this.editableItems().reduce((s, i) => s + i.original.quantidadeConsignada, 0));
    totalVendido    = computed(() => this.editableItems().reduce((s, i) => s + i.vendido, 0));
    totalDevolvido  = computed(() => this.editableItems().reduce((s, i) => s + i.devolvido, 0));

    dirtyItems = computed(() =>
        this.editableItems().filter(i => i.vendido !== i.original.vendido || i.devolvido !== i.original.devolvido)
    );
    isDirty = computed(() => this.dirtyItems().length > 0);
    canSaveEdits = computed(() =>
        this.isDirty() &&
        this.saveState() === 'idle' &&
        this.saleDetail()?.status === 'Aberta'
    );

    isItemDirty(item: EditableItem): boolean {
        return item.vendido !== item.original.vendido || item.devolvido !== item.original.devolvido;
    }

    emAberto(item: EditableItem): number {
        return item.original.quantidadeConsignada - item.vendido - item.devolvido;
    }

    isReadOnly = computed(() => this.saleDetail()?.status !== 'Aberta');


    constructor() {
        effect(() => {
            if (!this.open()) return;
            const id = this.saleId();
            if (id === null) {
                this.reset();
            } else {
                this.loadSaleDetail(id);
            }
        });

        this.subs.add(
            this.productSearch$.pipe(
                debounceTime(300),
                distinctUntilChanged(),
                switchMap(term => {
                    this.productsLoading.set(true);
                    return this.http.get<{ items: ProductOption[] }>('/api/produtos', { params: { busca: term } });
                })
            ).subscribe({
                next: res => { this.products.set(res.items); this.productsLoading.set(false); },
                error: () => this.productsLoading.set(false),
            })
        );
    }

    ngOnDestroy() { this.subs.unsubscribe(); }


    private reset() {
        this.mode.set('idle');
        this.saveState.set('idle');
        this.clientDropdownOpen.set(false);
        this.clientSearchTerm.set('');
        this.selectedClient.set(null);
        this.productSearchTerm.set('');
        this.products.set([]);
        this.selectedProduct.set(null);
        this.lots.set([]);
        this.selectedLot.set(null);
        this.itemQuantity.set(0);
        this.patientName.set('');
        this.recentPatients.set([]);
        this.patientDropdownOpen.set(false);
        this.items.set([]);
    }

    openClientDropdown() {
        if (!this.clientDropdownOpen()) {
            this.clientDropdownOpen.set(true);
            if (this.allClients().length === 0) {
                this.http.get<{ items: ClientOption[] }>('/api/clientes').subscribe({
                    next: res => this.allClients.set(res.items),
                });
            }
        } else {
            this.clientDropdownOpen.set(false);
        }
    }

    selectClient(client: ClientOption) {
        this.selectedClient.set(client);
        this.clientDropdownOpen.set(false);
        this.clientSearchTerm.set('');
    }

    onClientSearchInput(event: Event) {
        this.clientSearchTerm.set((event.target as HTMLInputElement).value);
    }

    openProductSearch() {
        this.mode.set('searching-product');
        this.productsLoading.set(true);
        this.http.get<{ items: ProductOption[] }>('/api/produtos', { params: { busca: '' } }).subscribe({
            next: res => {
                this.products.set(res.items);
                this.productsLoading.set(false);
            },
            error: () => this.productsLoading.set(false),
        });
    }

    onProductSearchInput(event: Event) {
        const term = (event.target as HTMLInputElement).value;
        this.productSearchTerm.set(term);
        this.productSearch$.next(term);
    }

    selectProduct(product: ProductOption) {
        this.selectedProduct.set(product);
        this.mode.set('selecting-lot');
        this.lotsLoading.set(true);
        this.http.get<LotOption[]>(`/api/produtos/${product.id}/lotes`).subscribe({
            next: lots => { this.lots.set(lots); this.lotsLoading.set(false); },
            error: () => this.lotsLoading.set(false),
        });
    }

    selectLot(lot: LotOption) {
        this.selectedLot.set(lot);
        this.itemQuantity.set(0);
        this.patientName.set('');
        this.mode.set('configuring-item');
    }

    decrementQty() {
        this.itemQuantity.update(v => Math.max(0, v - 1));
    }

    incrementQty() {
        this.itemQuantity.update(v => Math.min(this.effectiveLotAvailable(), v + 1));
    }

    onPatientNameInput(event: Event) {
        this.patientName.set((event.target as HTMLInputElement).value);
    }

    onPatientNameFocus() {
        if (this.recentPatients().length > 0) this.patientDropdownOpen.set(true);
    }

    onPatientNameBlur() {
        this.patientDropdownOpen.set(false);
    }

    selectRecentPatient(name: string) {
        this.patientName.set(name);
        this.patientDropdownOpen.set(false);
    }

    addItem() {
        const product = this.selectedProduct();
        const lot = this.selectedLot();
        if (!product || !lot || !this.canAdd()) return;

        this.items.update(items => [
            ...items,
            {
                productId: product.id,
                productName: product.nome,
                lotId: lot.id,
                lotNumber: lot.numero,
                lotAvailable: lot.quantidadeTotal,
                quantity: this.itemQuantity(),
                patientName: this.patientName(),
            },
        ]);
        this.mode.set('idle');
        this.selectedProduct.set(null);
        this.selectedLot.set(null);
        this.productSearchTerm.set('');
        this.products.set([]);
        if (!this.recentPatients().includes(this.patientName())) {
            this.recentPatients.update(names => [...names, this.patientName()]);
        }
        this.itemQuantity.set(0);
        this.patientName.set('');
    }

    removeItem(index: number) {
        this.items.update(items => items.filter((_, i) => i !== index));
    }

    cancelSearch() {
        this.mode.set('idle');
        this.selectedProduct.set(null);
        this.selectedLot.set(null);
        this.productSearchTerm.set('');
        this.products.set([]);
    }

    saveSale() {
        if (!this.canSave()) return;
        this.saveState.set('saving');
        const body = {
            clienteId: this.selectedClient()!.id,
            itens: this.items().map(i => ({
                loteId: i.lotId,
                quantidade: i.quantity,
                pacienteNome: i.patientName,
            })),
        };
        this.http.post('/api/vendas', body).subscribe({
            next: () => {
                this.saveState.set('saved');
                this.saleCreated.emit();
                setTimeout(() => this.close.emit(), 1500);
            },
            error: () => this.saveState.set('idle'),
        });
    }

    private loadSaleDetail(id: number) {
        this.saleDetail.set(null);
        this.editableItems.set([]);
        this.expandedItemId.set(null);
        this.saleError.set(false);
        this.saveState.set('idle');
        this.saleLoading.set(true);

        this.http.get<SaleDetail>(`/api/vendas/${id}`).subscribe({
            next: detail => {
                this.saleDetail.set(detail);
                this.editableItems.set(detail.itens.map(item => ({
                    original: item,
                    vendido: item.vendido,
                    devolvido: item.devolvido,
                })));
                this.saleLoading.set(false);
            },
            error: () => {
                this.saleLoading.set(false);
                this.saleError.set(true);
            },
        });
    }

    toggleItem(id: number) {
        this.expandedItemId.update(current => current === id ? null : id);
    }

    incrementVendido(index: number) {
        this.editableItems.update(items => {
            const item = items[index];
            const max = item.original.quantidadeConsignada - item.devolvido;
            if (item.vendido >= max) return items;
            return items.map((it, i) => i === index ? { ...it, vendido: it.vendido + 1 } : it);
        });
    }

    decrementVendido(index: number) {
        this.editableItems.update(items => {
            const item = items[index];
            if (item.vendido <= 0) return items;
            return items.map((it, i) => i === index ? { ...it, vendido: it.vendido - 1 } : it);
        });
    }

    incrementDevolvido(index: number) {
        this.editableItems.update(items => {
            const item = items[index];
            const max = item.original.quantidadeConsignada - item.vendido;
            if (item.devolvido >= max) return items;
            return items.map((it, i) => i === index ? { ...it, devolvido: it.devolvido + 1 } : it);
        });
    }

    decrementDevolvido(index: number) {
        this.editableItems.update(items => {
            const item = items[index];
            if (item.devolvido <= 0) return items;
            return items.map((it, i) => i === index ? { ...it, devolvido: it.devolvido - 1 } : it);
        });
    }

    saveEdits() {
        if (!this.canSaveEdits()) return;
        this.saveState.set('saving');
        const id = this.saleId()!;
        const body = {
            itens: this.editableItems().map(i => ({
                id: i.original.id,
                vendido: i.vendido,
                devolvido: i.devolvido,
            })),
        };
        this.http.patch(`/api/vendas/${id}`, body).subscribe({
            next: () => {
                this.saveState.set('saved');
                this.saleUpdated.emit();
                setTimeout(() => this.close.emit(), 1500);
            },
            error: () => this.saveState.set('idle'),
        });
    }
}
