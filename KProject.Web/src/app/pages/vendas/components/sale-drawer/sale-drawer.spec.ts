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
        TestBed.resetTestingModule();
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

    it('deve desabilitar produto com totalLotes === 0', () => {
        const fixture = TestBed.createComponent(SaleDrawer);
        fixture.detectChanges();

        fixture.nativeElement.querySelector('.sale-drawer__plus-btn').click();
        fixture.detectChanges();
        httpTesting.expectOne(r => r.url === '/api/produtos').flush({
            items: [
                { id: 1, nome: 'Produto Com Lotes', totalLotes: 2 },
                { id: 2, nome: 'Produto Sem Lotes', totalLotes: 0 },
            ],
            total: 2,
        });
        fixture.detectChanges();

        const items = fixture.nativeElement.querySelectorAll('.sale-drawer__product-item');
        expect(items.length).toBe(2);
        expect(items[0].disabled).toBe(false);
        expect(items[1].disabled).toBe(true);
        expect(items[1].classList).toContain('sale-drawer__product-item--disabled');
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

    it('deve emitir saleCreated imediatamente ao salvar com sucesso', () => {
        const fixture = TestBed.createComponent(SaleDrawer);
        const comp = fixture.componentInstance;
        fixture.detectChanges();

        comp.selectedClient.set({ id: 1, nome: 'Maria Silva' });
        abrirAteConfiguracao(fixture);
        comp.itemQuantity.set(1);
        comp.patientName.set('Will');
        comp.addItem();
        fixture.detectChanges();

        let created = false;
        comp.saleCreated.subscribe(() => (created = true));

        fixture.nativeElement.querySelector('.sale-drawer__save-btn').click();
        httpTesting.expectOne(r => r.url === '/api/vendas' && r.method === 'POST').flush({ id: 99 });
        fixture.detectChanges();

        expect(created).toBe(true);
    });

    it('não deve emitir saleCreated quando o save falha', () => {
        const fixture = TestBed.createComponent(SaleDrawer);
        const comp = fixture.componentInstance;
        fixture.detectChanges();

        comp.selectedClient.set({ id: 1, nome: 'Maria Silva' });
        abrirAteConfiguracao(fixture);
        comp.itemQuantity.set(1);
        comp.patientName.set('Will');
        comp.addItem();
        fixture.detectChanges();

        let created = false;
        comp.saleCreated.subscribe(() => (created = true));

        fixture.nativeElement.querySelector('.sale-drawer__save-btn').click();
        httpTesting.expectOne(r => r.url === '/api/vendas' && r.method === 'POST').flush(
            { status: 400 }, { status: 400, statusText: 'Bad Request' }
        );
        fixture.detectChanges();

        expect(created).toBe(false);
        expect(comp.saveState()).toBe('idle');
    });

    it('não deve abrir dropdown no primeiro item (sem pacientes recentes)', () => {
        const fixture = TestBed.createComponent(SaleDrawer);
        const comp = fixture.componentInstance;
        fixture.detectChanges();

        abrirAteConfiguracao(fixture);
        comp.onPatientNameFocus();

        expect(comp.patientDropdownOpen()).toBe(false);
    });

    it('deve salvar nome do paciente após addItem', () => {
        const fixture = TestBed.createComponent(SaleDrawer);
        const comp = fixture.componentInstance;
        fixture.detectChanges();

        abrirAteConfiguracao(fixture);
        comp.itemQuantity.set(1);
        comp.patientName.set('Ana');
        comp.addItem();

        expect(comp.recentPatients()).toContain('Ana');
    });

    it('não deve duplicar nomes em recentPatients', () => {
        const fixture = TestBed.createComponent(SaleDrawer);
        const comp = fixture.componentInstance;
        fixture.detectChanges();

        abrirAteConfiguracao(fixture);
        comp.itemQuantity.set(1);
        comp.patientName.set('Ana');
        comp.addItem();
        fixture.detectChanges();

        abrirAteConfiguracao(fixture);
        comp.itemQuantity.set(1);
        comp.patientName.set('Ana');
        comp.addItem();

        expect(comp.recentPatients().filter(n => n === 'Ana').length).toBe(1);
    });

    it('deve abrir dropdown no segundo item', () => {
        const fixture = TestBed.createComponent(SaleDrawer);
        const comp = fixture.componentInstance;
        fixture.detectChanges();

        abrirAteConfiguracao(fixture);
        comp.itemQuantity.set(1);
        comp.patientName.set('Ana');
        comp.addItem();
        fixture.detectChanges(); // necessário para re-renderizar o botão "+" em modo idle

        abrirAteConfiguracao(fixture);
        comp.onPatientNameFocus();

        expect(comp.patientDropdownOpen()).toBe(true);
    });

    it('deve preencher patientName ao selecionar paciente recente', () => {
        const fixture = TestBed.createComponent(SaleDrawer);
        const comp = fixture.componentInstance;
        fixture.detectChanges();

        abrirAteConfiguracao(fixture);
        comp.itemQuantity.set(1);
        comp.patientName.set('Ana');
        comp.addItem();
        fixture.detectChanges(); // necessário para re-renderizar o botão "+" em modo idle

        abrirAteConfiguracao(fixture);
        comp.selectRecentPatient('Ana');

        expect(comp.patientName()).toBe('Ana');
        expect(comp.patientDropdownOpen()).toBe(false);
    });

    it('deve limpar recentPatients no reset', () => {
        const fixture = TestBed.createComponent(SaleDrawer);
        const comp = fixture.componentInstance;
        fixture.detectChanges();

        abrirAteConfiguracao(fixture);
        comp.itemQuantity.set(1);
        comp.patientName.set('Ana');
        comp.addItem();

        fixture.componentRef.setInput('open', false);
        fixture.componentRef.setInput('open', true);
        TestBed.flushEffects();

        expect(comp.recentPatients()).toEqual([]);
    });

    it('deve filtrar pacientes recentes pelo texto digitado', () => {
        const fixture = TestBed.createComponent(SaleDrawer);
        const comp = fixture.componentInstance;
        fixture.detectChanges();

        comp.recentPatients.set(['Ana', 'Bruno']);
        comp.patientName.set('an');

        expect(comp.filteredRecentPatients()).toEqual(['Ana']);
    });

    describe('view/edit mode', () => {
        const SALE_DETAIL = {
        id: 1042,
        status: 'Aberta',
        criadaEm: '2026-02-27T10:00:00Z',
        modificadaEm: '2026-02-27T10:00:00Z',
        criadaPor: 'Admin',
        clienteNome: 'Maria Silva',
        itens: [
            { id: 1, produtoNome: 'Camiseta P', loteNumero: 1, pacienteNome: 'Paciente A', quantidadeConsignada: 10, vendido: 8, devolvido: 1 },
            { id: 2, produtoNome: 'Calca M',    loteNumero: 1, pacienteNome: 'Paciente B', quantidadeConsignada: 5,  vendido: 4, devolvido: 0 },
        ],
    };

    function abrirViewEdit(saleId = 1042) {
        const fixture = TestBed.createComponent(SaleDrawer);
        fixture.componentRef.setInput('saleId', saleId);
        fixture.componentRef.setInput('open', true);
        TestBed.flushEffects();
        fixture.detectChanges();
        return fixture;
    }

    it('deve disparar GET /api/vendas/{id} e ativar saleLoading ao abrir', () => {
        const fixture = abrirViewEdit();
        const comp = fixture.componentInstance;

        expect(comp.saleLoading()).toBe(true);
        httpTesting.expectOne('/api/vendas/1042').flush(SALE_DETAIL);
    });

    it('deve popular editableItems após resposta do GET', () => {
        const fixture = abrirViewEdit();
        const comp = fixture.componentInstance;

        httpTesting.expectOne('/api/vendas/1042').flush(SALE_DETAIL);
        fixture.detectChanges();

        expect(comp.saleLoading()).toBe(false);
        expect(comp.editableItems().length).toBe(2);
        expect(comp.editableItems()[0].vendido).toBe(8);
        expect(comp.editableItems()[0].devolvido).toBe(1);
    });

    it('deve calcular totalVendido e totalDevolvido do estado local', () => {
        const fixture = abrirViewEdit();
        const comp = fixture.componentInstance;

        httpTesting.expectOne('/api/vendas/1042').flush(SALE_DETAIL);
        fixture.detectChanges();

        expect(comp.totalVendido()).toBe(12);   // 8 + 4
        expect(comp.totalDevolvido()).toBe(1);  // 1 + 0
        expect(comp.totalConsignado()).toBe(15); // 10 + 5
    });

    it('deve exibir saleError ao falhar no GET', () => {
        const fixture = abrirViewEdit();
        const comp = fixture.componentInstance;

        httpTesting.expectOne('/api/vendas/1042').flush(
            { status: 500 }, { status: 500, statusText: 'Internal Server Error' }
        );
        fixture.detectChanges();

        expect(comp.saleLoading()).toBe(false);
        expect(comp.saleError()).toBe(true);
    });

    it('isDirty deve ser false antes de qualquer edição', () => {
        const fixture = abrirViewEdit();
        const comp = fixture.componentInstance;

        httpTesting.expectOne('/api/vendas/1042').flush(SALE_DETAIL);
        fixture.detectChanges();

        expect(comp.isDirty()).toBe(false);
    });

    it('isDirty deve ser true após incrementar vendido', () => {
        const fixture = abrirViewEdit();
        const comp = fixture.componentInstance;

        httpTesting.expectOne('/api/vendas/1042').flush(SALE_DETAIL);
        fixture.detectChanges();

        comp.incrementVendido(1); // item[1]: vendido 4 → 5
        expect(comp.isDirty()).toBe(true);
    });

    it('isDirty deve voltar a false se o usuário desfizer a edição', () => {
        const fixture = abrirViewEdit();
        const comp = fixture.componentInstance;

        httpTesting.expectOne('/api/vendas/1042').flush(SALE_DETAIL);
        fixture.detectChanges();

        comp.incrementVendido(1);
        expect(comp.isDirty()).toBe(true);

        comp.decrementVendido(1);
        expect(comp.isDirty()).toBe(false);
    });

    it('incrementVendido não deve ultrapassar quantidadeConsignada - devolvido', () => {
        const fixture = abrirViewEdit();
        const comp = fixture.componentInstance;

        httpTesting.expectOne('/api/vendas/1042').flush(SALE_DETAIL);
        fixture.detectChanges();

        // item[0]: quantidadeConsignada=10, devolvido=1, vendido=8 → max=9
        comp.incrementVendido(0);
        comp.incrementVendido(0); // deve parar em 9
        expect(comp.editableItems()[0].vendido).toBe(9);
    });

    it('decrementVendido não deve ir abaixo de zero', () => {
        const fixture = abrirViewEdit();
        const comp = fixture.componentInstance;

        httpTesting.expectOne('/api/vendas/1042').flush(SALE_DETAIL);
        fixture.detectChanges();

        // item[1]: vendido=4
        comp.decrementVendido(1);
        comp.decrementVendido(1);
        comp.decrementVendido(1);
        comp.decrementVendido(1);
        comp.decrementVendido(1); // deve parar em 0
        expect(comp.editableItems()[1].vendido).toBe(0);
    });

    it('incrementDevolvido não deve ultrapassar quantidadeConsignada - vendido', () => {
        const fixture = abrirViewEdit();
        const comp = fixture.componentInstance;

        httpTesting.expectOne('/api/vendas/1042').flush(SALE_DETAIL);
        fixture.detectChanges();

        // item[1]: quantidadeConsignada=5, vendido=4, devolvido=0 → max=1
        comp.incrementDevolvido(1);
        comp.incrementDevolvido(1); // deve parar em 1
        expect(comp.editableItems()[1].devolvido).toBe(1);
    });

    it('decrementDevolvido não deve ir abaixo de zero', () => {
        const fixture = abrirViewEdit();
        const comp = fixture.componentInstance;

        httpTesting.expectOne('/api/vendas/1042').flush(SALE_DETAIL);
        fixture.detectChanges();

        comp.decrementDevolvido(0); // já está em 1, vai para 0
        comp.decrementDevolvido(0); // deve parar em 0
        expect(comp.editableItems()[0].devolvido).toBe(0);
    });

    it('canSaveEdits deve ser false para status Fechada mesmo com isDirty=true (forçado)', () => {
        const fixture = abrirViewEdit();
        const comp = fixture.componentInstance;

        httpTesting.expectOne('/api/vendas/1042').flush({ ...SALE_DETAIL, status: 'Fechada' });
        fixture.detectChanges();

        // Força dirty diretamente via signal
        comp.editableItems.update(items =>
            items.map((it, i) => i === 0 ? { ...it, vendido: it.vendido + 1 } : it)
        );

        expect(comp.isDirty()).toBe(true);
        expect(comp.canSaveEdits()).toBe(false);
    });

    it('deve disparar PATCH /api/vendas/{id} com todos os itens ao salvar', () => {
        const fixture = abrirViewEdit();
        const comp = fixture.componentInstance;

        httpTesting.expectOne('/api/vendas/1042').flush(SALE_DETAIL);
        fixture.detectChanges();

        comp.incrementVendido(1);
        comp.saveEdits();

        const req = httpTesting.expectOne(r => r.url === '/api/vendas/1042' && r.method === 'PATCH');
        expect(req.request.body.itens.length).toBe(2);
        expect(req.request.body.itens[0]).toEqual({ id: 1, vendido: 8, devolvido: 1 });
        expect(req.request.body.itens[1]).toEqual({ id: 2, vendido: 5, devolvido: 0 });
        req.flush({});
    });

    it('deve emitir saleUpdated e atualizar originals após PATCH bem-sucedido', () => {
        const fixture = abrirViewEdit();
        const comp = fixture.componentInstance;

        httpTesting.expectOne('/api/vendas/1042').flush(SALE_DETAIL);
        fixture.detectChanges();

        let updated = false;
        comp.saleUpdated.subscribe(() => (updated = true));

        comp.incrementVendido(1);
        comp.saveEdits();
        httpTesting.expectOne(r => r.method === 'PATCH').flush({});
        fixture.detectChanges();

        expect(comp.saveState()).toBe('saved');
        expect(updated).toBe(true);
        expect(comp.isDirty()).toBe(false);
        expect(comp.editableItems()[1].original.vendido).toBe(5);
    });

    it('deve resetar saveState para idle após erro no PATCH', () => {
        const fixture = abrirViewEdit();
        const comp = fixture.componentInstance;

        httpTesting.expectOne('/api/vendas/1042').flush(SALE_DETAIL);
        fixture.detectChanges();

        comp.incrementVendido(1);
        comp.saveEdits();
        httpTesting.expectOne(r => r.method === 'PATCH').flush(
            { status: 500 }, { status: 500, statusText: 'Internal Server Error' }
        );
        fixture.detectChanges();

        expect(comp.saveState()).toBe('idle');
    });

    it('deve disparar POST /close, exibir overlay e emitir saleUpdated ao finalizar venda', () => {
        const fixture = abrirViewEdit();
        const comp = fixture.componentInstance;

        httpTesting.expectOne('/api/vendas/1042').flush(SALE_DETAIL);
        fixture.detectChanges();

        let updated = false;
        comp.saleUpdated.subscribe(() => (updated = true));

        comp.finalizeSale();
        expect(comp.actionState()).toBe('closing');

        httpTesting.expectOne(r => r.url === '/api/vendas/1042/close' && r.method === 'POST').flush({});
        fixture.detectChanges();

        expect(comp.actionState()).toBe('closed');
        expect(updated).toBe(true);
    });

    it('deve disparar POST /cancel, exibir overlay e emitir saleUpdated ao cancelar venda', () => {
        const fixture = abrirViewEdit();
        const comp = fixture.componentInstance;

        httpTesting.expectOne('/api/vendas/1042').flush(SALE_DETAIL);
        fixture.detectChanges();

        let updated = false;
        comp.saleUpdated.subscribe(() => (updated = true));

        comp.cancelSale();
        expect(comp.actionState()).toBe('cancelling');

        httpTesting.expectOne(r => r.url === '/api/vendas/1042/cancel' && r.method === 'POST').flush({});
        fixture.detectChanges();

        expect(comp.actionState()).toBe('cancelled');
        expect(updated).toBe(true);
    });

    it('canAction deve ser false quando isDirty', () => {
        const fixture = abrirViewEdit();
        const comp = fixture.componentInstance;

        httpTesting.expectOne('/api/vendas/1042').flush(SALE_DETAIL);
        fixture.detectChanges();

        expect(comp.canAction()).toBe(true);
        comp.incrementVendido(1);
        expect(comp.canAction()).toBe(false);
    });

    it('canAction deve ser false para venda Fechada', () => {
        const fixture = abrirViewEdit();
        const comp = fixture.componentInstance;

        httpTesting.expectOne('/api/vendas/1042').flush({ ...SALE_DETAIL, status: 'Fechada' });
        fixture.detectChanges();

        expect(comp.canAction()).toBe(false);
    });

    it('deve resetar actionState para idle após erro no POST /close', () => {
        const fixture = abrirViewEdit();
        const comp = fixture.componentInstance;

        httpTesting.expectOne('/api/vendas/1042').flush(SALE_DETAIL);
        fixture.detectChanges();

        comp.finalizeSale();
        httpTesting.expectOne(r => r.url === '/api/vendas/1042/close').flush(
            { status: 500 }, { status: 500, statusText: 'Internal Server Error' }
        );
        fixture.detectChanges();

        expect(comp.actionState()).toBe('idle');
    });

    it('deve limpar estado anterior ao reabrir com o mesmo saleId', () => {
        const fixture = abrirViewEdit();
        const comp = fixture.componentInstance;

        httpTesting.expectOne('/api/vendas/1042').flush(SALE_DETAIL);
        fixture.detectChanges();

        // Faz edição
        comp.incrementVendido(0);
        expect(comp.isDirty()).toBe(true);

        // Fecha e reabre
        fixture.componentRef.setInput('open', false);
        fixture.componentRef.setInput('open', true);
        TestBed.flushEffects();
        fixture.detectChanges();

        expect(comp.saleDetail()).toBeNull(); // limpo antes do re-fetch
        expect(comp.isDirty()).toBe(false);

        httpTesting.expectOne('/api/vendas/1042').flush(SALE_DETAIL);
    });
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
