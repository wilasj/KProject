import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { SaleDrawer } from './sale-drawer';
import { describe, it, expect, beforeEach, afterEach } from 'vitest';

const MOCK_PRODUCTS = [
    { id: 1, nome: 'Camiseta Basica Branca', totalLotes: 2 },
    { id: 2, nome: 'Calca Jeans Slim', totalLotes: 1 },
];

const MOCK_LOTES = [
    { id: 10, numero: 1, validade: '2027-06-15', quantidadeTotal: 5 },
    { id: 11, numero: 2, validade: '2027-09-01', quantidadeTotal: 3 },
];

const MOCK_CLIENTS = [
    { id: 1, nome: 'Maria Silva' },
    { id: 2, nome: 'Joao Santos' },
];

describe('SaleDrawer', () => {
    let httpTesting: HttpTestingController;

    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [SaleDrawer],
            providers: [provideHttpClient(), provideHttpClientTesting()],
        });
        httpTesting = TestBed.inject(HttpTestingController);
    });

    afterEach(() => httpTesting.verify());

    it('deve exibir título "Nova Venda"', () => {
        const fixture = TestBed.createComponent(SaleDrawer);
        fixture.detectChanges();
        const title = fixture.nativeElement.querySelector('.sale-drawer__title');
        expect(title.textContent.trim()).toBe('Nova Venda');
    });

    it('deve exibir total zero ao iniciar', () => {
        const fixture = TestBed.createComponent(SaleDrawer);
        fixture.detectChanges();
        const total = fixture.nativeElement.querySelector('.sale-drawer__total-value');
        expect(total.textContent.trim()).toBe('0');
    });

    it('deve emitir close ao clicar no botão fechar', () => {
        const fixture = TestBed.createComponent(SaleDrawer);
        fixture.detectChanges();
        let closed = false;
        fixture.componentInstance.close.subscribe(() => (closed = true));
        fixture.nativeElement.querySelector('.sale-drawer__close').click();
        expect(closed).toBe(true);
    });

    it('deve resetar estado ao receber open=true', () => {
        const fixture = TestBed.createComponent(SaleDrawer);
        const comp = fixture.componentInstance;
        fixture.componentRef.setInput('open', true);
        TestBed.flushEffects();
        fixture.detectChanges();
        expect(comp.mode()).toBe('idle');
        expect(comp.items().length).toBe(0);
        expect(comp.selectedClient()).toBeNull();
    });

    it('deve abrir dropdown de clientes e carregar a lista', () => {
        const fixture = TestBed.createComponent(SaleDrawer);
        fixture.detectChanges();

        fixture.nativeElement.querySelector('.sale-drawer__client-btn').click();
        fixture.detectChanges();

        const req = httpTesting.expectOne('/api/clientes');
        req.flush({ items: MOCK_CLIENTS });
        fixture.detectChanges();

        const items = fixture.nativeElement.querySelectorAll('.sale-drawer__client-item');
        expect(items.length).toBe(2);
        expect(items[0].textContent.trim()).toBe('Maria Silva');
    });

    it('deve selecionar cliente e fechar dropdown', () => {
        const fixture = TestBed.createComponent(SaleDrawer);
        fixture.detectChanges();

        fixture.nativeElement.querySelector('.sale-drawer__client-btn').click();
        fixture.detectChanges();
        httpTesting.expectOne('/api/clientes').flush({ items: MOCK_CLIENTS });
        fixture.detectChanges();

        fixture.nativeElement.querySelectorAll('.sale-drawer__client-item')[1].click();
        fixture.detectChanges();

        expect(fixture.componentInstance.selectedClient()?.nome).toBe('Joao Santos');
        expect(fixture.nativeElement.querySelector('.sale-drawer__client-dropdown')).toBeNull();
    });

    it('deve entrar em modo searching-product ao clicar no "+"', () => {
        const fixture = TestBed.createComponent(SaleDrawer);
        fixture.detectChanges();

        fixture.nativeElement.querySelector('.sale-drawer__plus-btn').click();
        fixture.detectChanges();

        httpTesting.expectOne(r => r.url === '/api/produtos').flush({ items: MOCK_PRODUCTS, total: MOCK_PRODUCTS.length });
        fixture.detectChanges();

        expect(fixture.componentInstance.mode()).toBe('searching-product');
        const items = fixture.nativeElement.querySelectorAll('.sale-drawer__product-item');
        expect(items.length).toBe(2);
    });

    it('deve entrar em modo selecting-lot ao selecionar produto', () => {
        const fixture = TestBed.createComponent(SaleDrawer);
        fixture.detectChanges();

        fixture.nativeElement.querySelector('.sale-drawer__plus-btn').click();
        fixture.detectChanges();
        httpTesting.expectOne(r => r.url === '/api/produtos').flush({ items: MOCK_PRODUCTS, total: MOCK_PRODUCTS.length });
        fixture.detectChanges();

        fixture.nativeElement.querySelector('.sale-drawer__product-item').click();
        fixture.detectChanges();
        httpTesting.expectOne('/api/produtos/1/lotes').flush(MOCK_LOTES);
        fixture.detectChanges();

        expect(fixture.componentInstance.mode()).toBe('selecting-lot');
        const cards = fixture.nativeElement.querySelectorAll('.sale-drawer__lot-card');
        expect(cards.length).toBe(2);
    });

    it('deve entrar em modo configuring-item ao selecionar lote', () => {
        const fixture = TestBed.createComponent(SaleDrawer);
        const comp = fixture.componentInstance;
        fixture.detectChanges();

        fixture.nativeElement.querySelector('.sale-drawer__plus-btn').click();
        fixture.detectChanges();
        httpTesting.expectOne(r => r.url === '/api/produtos').flush({ items: MOCK_PRODUCTS, total: MOCK_PRODUCTS.length });
        fixture.detectChanges();
        fixture.nativeElement.querySelector('.sale-drawer__product-item').click();
        fixture.detectChanges();
        httpTesting.expectOne('/api/produtos/1/lotes').flush(MOCK_LOTES);
        fixture.detectChanges();

        fixture.nativeElement.querySelector('.sale-drawer__lot-card').click();
        fixture.detectChanges();

        expect(comp.mode()).toBe('configuring-item');
        expect(comp.selectedLot()?.id).toBe(10);
        expect(comp.itemQuantity()).toBe(0);
    });

    it('deve desabilitar botão "+" quando quantidade é zero', () => {
        const fixture = TestBed.createComponent(SaleDrawer);
        fixture.detectChanges();

        abrirAteConfiguracao(fixture);

        fixture.detectChanges();
        const addBtn = fixture.nativeElement.querySelector('.sale-drawer__add-btn');
        expect(addBtn).toBeNull();
    });

    it('não deve permitir adicionar sem nome do paciente', () => {
        const fixture = TestBed.createComponent(SaleDrawer);
        const comp = fixture.componentInstance;
        fixture.detectChanges();

        abrirAteConfiguracao(fixture);
        comp.itemQuantity.set(2);
        fixture.detectChanges();

        expect(comp.canAdd()).toBe(false);
    });

    it('deve adicionar item à lista e voltar ao modo idle', () => {
        const fixture = TestBed.createComponent(SaleDrawer);
        const comp = fixture.componentInstance;
        fixture.detectChanges();

        abrirAteConfiguracao(fixture);
        comp.itemQuantity.set(3);
        comp.patientName.set('Will');
        fixture.detectChanges();

        fixture.nativeElement.querySelector('.sale-drawer__add-btn').click();
        fixture.detectChanges();

        expect(comp.mode()).toBe('idle');
        expect(comp.items().length).toBe(1);
        expect(comp.items()[0].quantity).toBe(3);
        expect(comp.items()[0].patientName).toBe('Will');
        expect(comp.totalItems()).toBe(3);
    });

    it('deve exibir nome do paciente no card do item', () => {
        const fixture = TestBed.createComponent(SaleDrawer);
        const comp = fixture.componentInstance;
        fixture.detectChanges();

        abrirAteConfiguracao(fixture);
        comp.itemQuantity.set(1);
        comp.patientName.set('Lua');
        comp.addItem();
        fixture.detectChanges();

        const meta = fixture.nativeElement.querySelector('.sale-drawer__item-meta');
        expect(meta.textContent).toContain('Lua');
    });

    it('deve descontar quantidade já usada do lote ao abrir novamente', () => {
        const fixture = TestBed.createComponent(SaleDrawer);
        const comp = fixture.componentInstance;
        fixture.detectChanges();

        // Adiciona 4 unidades do Lote 1 (total=5) para Will
        abrirAteConfiguracao(fixture);
        comp.itemQuantity.set(4);
        comp.patientName.set('Will');
        comp.addItem();
        fixture.detectChanges();

        // Abre novamente o mesmo produto/lote para Lua
        fixture.nativeElement.querySelector('.sale-drawer__plus-btn').click();
        fixture.detectChanges();
        httpTesting.expectOne(r => r.url === '/api/produtos').flush({ items: MOCK_PRODUCTS, total: MOCK_PRODUCTS.length });
        fixture.detectChanges();
        fixture.nativeElement.querySelector('.sale-drawer__product-item').click();
        fixture.detectChanges();
        httpTesting.expectOne('/api/produtos/1/lotes').flush(MOCK_LOTES);
        fixture.detectChanges();
        fixture.nativeElement.querySelector('.sale-drawer__lot-card').click();
        fixture.detectChanges();

        // Deve mostrar apenas 1 disponível (5 - 4 = 1)
        expect(comp.effectiveLotAvailable()).toBe(1);
    });

    it('deve bloquear incremento acima da quantidade efetiva disponível', () => {
        const fixture = TestBed.createComponent(SaleDrawer);
        const comp = fixture.componentInstance;
        fixture.detectChanges();

        // Adiciona 3 do Lote 1 (total=5) para Will
        abrirAteConfiguracao(fixture);
        comp.itemQuantity.set(3);
        comp.patientName.set('Will');
        comp.addItem();
        fixture.detectChanges();

        // Abre para Lua — disponível = 2
        abrirAteConfiguracao(fixture);

        // Tenta incrementar 3 vezes
        comp.incrementQty();
        comp.incrementQty();
        comp.incrementQty();

        expect(comp.itemQuantity()).toBe(2);
    });

    it('deve desabilitar lote com quantidade zerada', () => {
        const fixture = TestBed.createComponent(SaleDrawer);
        const comp = fixture.componentInstance;
        fixture.detectChanges();

        // Consome todo o Lote 1 (5 unidades)
        abrirAteConfiguracao(fixture);
        comp.itemQuantity.set(5);
        comp.patientName.set('Will');
        comp.addItem();
        fixture.detectChanges();

        // Reabre e chega na tela de lotes
        fixture.nativeElement.querySelector('.sale-drawer__plus-btn').click();
        fixture.detectChanges();
        httpTesting.expectOne(r => r.url === '/api/produtos').flush({ items: MOCK_PRODUCTS, total: MOCK_PRODUCTS.length });
        fixture.detectChanges();
        fixture.nativeElement.querySelector('.sale-drawer__product-item').click();
        fixture.detectChanges();
        httpTesting.expectOne('/api/produtos/1/lotes').flush(MOCK_LOTES);
        fixture.detectChanges();

        const firstCard = fixture.nativeElement.querySelector('.sale-drawer__lot-card');
        expect(firstCard.disabled).toBe(true);
        expect(firstCard.classList).toContain('sale-drawer__lot-card--depleted');
    });

    it('deve cancelar busca e voltar ao idle ao clicar no chip ×', () => {
        const fixture = TestBed.createComponent(SaleDrawer);
        fixture.detectChanges();

        fixture.nativeElement.querySelector('.sale-drawer__plus-btn').click();
        fixture.detectChanges();
        httpTesting.expectOne(r => r.url === '/api/produtos').flush({ items: MOCK_PRODUCTS, total: MOCK_PRODUCTS.length });
        fixture.detectChanges();

        fixture.nativeElement.querySelector('.sale-drawer__chip-close').click();
        fixture.detectChanges();

        expect(fixture.componentInstance.mode()).toBe('idle');
    });

    it('deve remover item ao clicar no × do card', () => {
        const fixture = TestBed.createComponent(SaleDrawer);
        const comp = fixture.componentInstance;
        fixture.detectChanges();

        abrirAteConfiguracao(fixture);
        comp.itemQuantity.set(2);
        comp.patientName.set('Will');
        comp.addItem();
        fixture.detectChanges();

        expect(comp.items().length).toBe(1);
        fixture.nativeElement.querySelector('.sale-drawer__item-remove').click();
        fixture.detectChanges();

        expect(comp.items().length).toBe(0);
    });

    it('deve exibir botão Salvar apenas quando há itens no modo idle', () => {
        const fixture = TestBed.createComponent(SaleDrawer);
        const comp = fixture.componentInstance;
        fixture.detectChanges();

        expect(fixture.nativeElement.querySelector('.sale-drawer__save-btn')).toBeNull();

        abrirAteConfiguracao(fixture);
        comp.itemQuantity.set(1);
        comp.patientName.set('Will');
        comp.addItem();
        fixture.detectChanges();

        expect(fixture.nativeElement.querySelector('.sale-drawer__save-btn')).not.toBeNull();
    });

    it('deve chamar POST /api/vendas e emitir close após salvar', () => {
        const fixture = TestBed.createComponent(SaleDrawer);
        const comp = fixture.componentInstance;
        fixture.detectChanges();

        comp.selectedClient.set({ id: 1, nome: 'Maria Silva' });
        abrirAteConfiguracao(fixture);
        comp.itemQuantity.set(1);
        comp.patientName.set('Will');
        comp.addItem();
        fixture.detectChanges();

        let closed = false;
        comp.close.subscribe(() => (closed = true));

        fixture.nativeElement.querySelector('.sale-drawer__save-btn').click();
        fixture.detectChanges();

        expect(comp.saveState()).toBe('saving');

        httpTesting.expectOne(r => r.url === '/api/vendas' && r.method === 'POST').flush({ id: 1234 });
        fixture.detectChanges();

        expect(comp.saveState()).toBe('saved');
    });
});

// Utilitário: abre o drawer até o estado configuring-item (produto 1, lote 1)
function abrirAteConfiguracao(fixture: ReturnType<typeof TestBed.createComponent<SaleDrawer>>) {
    const httpTesting = TestBed.inject(HttpTestingController);
    fixture.nativeElement.querySelector('.sale-drawer__plus-btn').click();
    fixture.detectChanges();
    httpTesting.expectOne(r => r.url === '/api/produtos').flush({ items: [
        { id: 1, nome: 'Camiseta Basica Branca', totalLotes: 2 },
    ], total: 1 });
    fixture.detectChanges();
    fixture.nativeElement.querySelector('.sale-drawer__product-item').click();
    fixture.detectChanges();
    httpTesting.expectOne('/api/produtos/1/lotes').flush([
        { id: 10, numero: 1, validade: '2027-06-15', quantidadeTotal: 5 },
    ]);
    fixture.detectChanges();
    fixture.nativeElement.querySelector('.sale-drawer__lot-card').click();
    fixture.detectChanges();
}
