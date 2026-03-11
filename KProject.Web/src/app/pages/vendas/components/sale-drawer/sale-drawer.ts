import { Component, computed, effect, inject, input, OnDestroy, output, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { debounceTime, distinctUntilChanged, Subject, Subscription, switchMap } from 'rxjs';

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
    patientCpf: string;
}

@Component({
    selector: 'app-sale-drawer',
    templateUrl: './sale-drawer.html',
    styleUrl: './sale-drawer.scss',
})
export class SaleDrawer implements OnDestroy {
    private http = inject(HttpClient);

    saleId = input<number | null>(null);
    open = input<boolean>(false);
    close = output<void>();

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
    patientCpf = signal('');

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

    effectiveQtyForLot(lot: LotOption): number {
        return lot.quantidadeTotal - (this.consumedByLot().get(lot.id) ?? 0);
    }

    canAdd = computed(() => {
        const cpfDigits = this.patientCpf().replace(/\D/g, '');
        return (
            this.itemQuantity() > 0 &&
            this.itemQuantity() <= this.effectiveLotAvailable() &&
            this.patientName().trim().length > 0 &&
            cpfDigits.length === 11
        );
    });

    addButtonLabel = computed(() => {
        const qty = this.itemQuantity();
        return qty > 0 ? `+ Adicionar ${qty} ${qty === 1 ? 'unidade' : 'unidades'}` : '+ Adicionar';
    });

    // Items list
    items = signal<SaleItem[]>([]);
    totalItems = computed(() => this.items().reduce((sum, i) => sum + i.quantity, 0));
    canSave = computed(() => this.items().length > 0 && this.selectedClient() !== null && this.saveState() === 'idle');

    constructor() {
        effect(() => {
            if (this.open()) this.reset();
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
        this.patientCpf.set('');
        this.items.set([]);
    }

    openClientDropdown() {
        if (!this.clientDropdownOpen()) {
            this.clientDropdownOpen.set(true);
            if (this.allClients().length === 0) {
                this.http.get<ClientOption[]>('/api/clientes').subscribe({
                    next: clients => this.allClients.set(clients),
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
        this.patientCpf.set('');
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

    onPatientCpfInput(event: Event) {
        const formatted = this.formatCpf((event.target as HTMLInputElement).value);
        this.patientCpf.set(formatted);
        (event.target as HTMLInputElement).value = formatted;
    }

    private formatCpf(value: string): string {
        const d = value.replace(/\D/g, '').slice(0, 11);
        if (d.length <= 3) return d;
        if (d.length <= 6) return `${d.slice(0, 3)}.${d.slice(3)}`;
        if (d.length <= 9) return `${d.slice(0, 3)}.${d.slice(3, 6)}.${d.slice(6)}`;
        return `${d.slice(0, 3)}.${d.slice(3, 6)}.${d.slice(6, 9)}-${d.slice(9)}`;
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
                patientCpf: this.patientCpf().replace(/\D/g, ''),
            },
        ]);
        this.mode.set('idle');
        this.selectedProduct.set(null);
        this.selectedLot.set(null);
        this.productSearchTerm.set('');
        this.products.set([]);
        this.itemQuantity.set(0);
        this.patientName.set('');
        this.patientCpf.set('');
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
                pacienteCpf: i.patientCpf,
            })),
        };
        this.http.post('/api/vendas', body).subscribe({
            next: () => {
                this.saveState.set('saved');
                setTimeout(() => this.close.emit(), 1500);
            },
            error: () => this.saveState.set('idle'),
        });
    }
}
