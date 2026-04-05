import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { SaleAccordion } from './sale-accordion';
import { Sale, SaleDetail, EditableItem } from '@models/venda';
import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';

const mockSale: Sale = {
  id: 1042,
  clienteNome: 'Maria Silva',
  criadaEm: '2026-02-27T10:00:00Z',
  status: 'Aberta',
  totalItens: 2,
};

const mockDetail: SaleDetail = {
  id: 1042,
  status: 'Aberta',
  criadaEm: '2026-02-27T10:00:00Z',
  modificadaEm: '2026-02-27T10:00:00Z',
  criadaPor: 'Admin',
  clienteNome: 'Maria Silva',
  itens: [
    { id: 1, produtoNome: 'Produto A', loteNumero: 1, pacienteNome: 'Paciente A', quantidadeConsignada: 10, vendido: 3, devolvido: 2 },
    { id: 2, produtoNome: 'Produto B', loteNumero: 2, pacienteNome: 'Paciente B', quantidadeConsignada: 5,  vendido: 1, devolvido: 0 },
  ],
};

describe('SaleAccordion', () => {
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.resetTestingModule();
    TestBed.configureTestingModule({
      imports: [SaleAccordion],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpTesting.verify());

  function setup(sale = mockSale) {
    const fixture = TestBed.createComponent(SaleAccordion);
    fixture.componentRef.setInput('sale', sale);
    fixture.detectChanges();
    return fixture;
  }

  function expand(fixture: ReturnType<typeof setup>) {
    fixture.componentInstance.toggle();
    fixture.detectChanges();
    httpTesting.expectOne('/api/vendas/1042').flush(mockDetail);
    fixture.detectChanges();
  }

  it('não deve fazer requisição quando recolhido', () => {
    setup();
    httpTesting.expectNone('/api/vendas/1042');
  });

  it('deve fazer GET ao expandir', () => {
    const fixture = setup();
    fixture.componentInstance.toggle();
    fixture.detectChanges();
    httpTesting.expectOne('/api/vendas/1042');
  });

  it('deve calcular total corretamente', () => {
    const fixture = setup();
    expand(fixture);
    expect(fixture.componentInstance.total()).toBe(15);
  });

  it('deve calcular vendidos corretamente', () => {
    const fixture = setup();
    expand(fixture);
    expect(fixture.componentInstance.vendidos()).toBe(4);
  });

  it('deve calcular devolvidos corretamente', () => {
    const fixture = setup();
    expand(fixture);
    expect(fixture.componentInstance.devolvidos()).toBe(2);
  });

  it('deve calcular emAberto corretamente', () => {
    const fixture = setup();
    expand(fixture);
    expect(fixture.componentInstance.emAberto()).toBe(9);
  });

  it('isDirty deve ser false logo após carregar', () => {
    const fixture = setup();
    expand(fixture);
    expect(fixture.componentInstance.isDirty()).toBe(false);
  });

  it('isDirty deve ser true após alterar um item', () => {
    const fixture = setup();
    expand(fixture);

    const updated: EditableItem = { ...mockDetail.itens[0], vendido: 5 };
    fixture.componentInstance.onItemChange(updated);
    fixture.detectChanges();

    expect(fixture.componentInstance.isDirty()).toBe(true);
  });

  it('deve iniciar animação de fechar e recolher após o timer', () => {
    vi.useFakeTimers();
    const fixture = setup();
    expand(fixture);

    fixture.componentInstance.toggle();
    fixture.detectChanges();

    expect(fixture.componentInstance.isClosing()).toBe(true);
    expect(fixture.componentInstance.expanded()).toBe(true);

    vi.advanceTimersByTime(200);
    fixture.detectChanges();

    expect(fixture.componentInstance.expanded()).toBe(false);
    expect(fixture.componentInstance.isClosing()).toBe(false);
    vi.useRealTimers();
  });

  it('onFechar deve chamar POST close e mudar actionState', () => {
    vi.useFakeTimers();
    const fixture = setup();
    expand(fixture);

    fixture.componentInstance.onFechar();
    fixture.detectChanges();

    expect(fixture.componentInstance.actionState()).toBe('closing');

    httpTesting.expectOne('/api/vendas/1042/close').flush(null, { status: 204, statusText: 'No Content' });
    fixture.detectChanges();

    expect(fixture.componentInstance.actionState()).toBe('closed');

    vi.advanceTimersByTime(2000);
    fixture.detectChanges();
    expect(fixture.componentInstance.actionState()).toBe('idle');
    expect(fixture.componentInstance.isClosing()).toBe(true);

    vi.advanceTimersByTime(200);
    fixture.detectChanges();
    expect(fixture.componentInstance.expanded()).toBe(false);
    vi.useRealTimers();
  });

  it('onCancelar deve chamar POST cancel e mudar actionState', () => {
    vi.useFakeTimers();
    const fixture = setup();
    expand(fixture);

    fixture.componentInstance.onCancelar();
    fixture.detectChanges();

    expect(fixture.componentInstance.actionState()).toBe('cancelling');

    httpTesting.expectOne('/api/vendas/1042/cancel').flush(null, { status: 204, statusText: 'No Content' });
    fixture.detectChanges();

    expect(fixture.componentInstance.actionState()).toBe('cancelled');

    vi.advanceTimersByTime(2000);
    fixture.detectChanges();
    expect(fixture.componentInstance.actionState()).toBe('idle');
    expect(fixture.componentInstance.isClosing()).toBe(true);

    vi.advanceTimersByTime(200);
    fixture.detectChanges();
    expect(fixture.componentInstance.expanded()).toBe(false);
    vi.useRealTimers();
  });

  it('não deve exibir botões fechar/cancelar quando status não é Aberta', () => {
    const closedSale: Sale = { ...mockSale, status: 'Fechada' };
    const fixture = setup(closedSale);
    fixture.componentInstance.toggle();
    fixture.detectChanges();

    const closedDetail: SaleDetail = { ...mockDetail, status: 'Fechada' };
    httpTesting.expectOne('/api/vendas/1042').flush(closedDetail);
    fixture.detectChanges();

    const closeBtn = fixture.nativeElement.querySelector('.sale-accordion__btn--close');
    const cancelBtn = fixture.nativeElement.querySelector('.sale-accordion__btn--cancel');
    expect(closeBtn).toBeNull();
    expect(cancelBtn).toBeNull();
  });

  it('botão salvar deve aparecer quando isDirty e sumir quando não', () => {
    const fixture = setup();
    expand(fixture);

    const saveBtn = () => fixture.nativeElement.querySelector('.sale-accordion__btn--save');
    expect(saveBtn().classList.contains('sale-accordion__btn--visible')).toBe(false);

    const updated: EditableItem = { ...mockDetail.itens[0], vendido: 5 };
    fixture.componentInstance.onItemChange(updated);
    fixture.detectChanges();

    expect(saveBtn().classList.contains('sale-accordion__btn--visible')).toBe(true);
  });

  it('onSave deve chamar PATCH e resetar isDirty sem fechar o accordion', () => {
    const fixture = setup();
    expand(fixture);

    const updated: EditableItem = { ...mockDetail.itens[0], vendido: 5 };
    fixture.componentInstance.onItemChange(updated);
    fixture.detectChanges();
    expect(fixture.componentInstance.isDirty()).toBe(true);

    fixture.componentInstance.onSave();
    fixture.detectChanges();

    httpTesting.expectOne('/api/vendas/1042').flush(null, { status: 204, statusText: 'No Content' });
    fixture.detectChanges();

    expect(fixture.componentInstance.isDirty()).toBe(false);
    expect(fixture.componentInstance.expanded()).toBe(true);
  });
});
