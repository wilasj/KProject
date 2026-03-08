import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { ProductAccordion } from './product-accordion';
import { Product } from '@models/produto';
import { Lote } from '@models/lote';
import { describe, it, expect, beforeEach, afterEach } from 'vitest';

const mockProduct: Product = {
  id: 1,
  nome: 'Produto A',
  referencia: 'REF-001',
  descricao: 'Desc A',
  codigoAnvisa: 'ANV-001',
  criadoEm: '2026-03-01T00:00:00Z',
};

const mockLotes: Lote[] = [
  { id: 1, numero: 10, validade: '2027-06-30', quantidadeTotal: 50 },
  { id: 2, numero: 11, validade: '2027-12-31', quantidadeTotal: 20 },
];

describe('ProductAccordion', () => {
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [ProductAccordion],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpTesting.verify());

  it('não deve fazer requisição quando recolhido', () => {
    const fixture = TestBed.createComponent(ProductAccordion);
    fixture.componentRef.setInput('product', mockProduct);
    fixture.componentRef.setInput('expanded', false);
    fixture.detectChanges();
    httpTesting.expectNone('/api/produtos/1/lotes');
  });

  it('deve exibir skeletons ao expandir', () => {
    const fixture = TestBed.createComponent(ProductAccordion);
    fixture.componentRef.setInput('product', mockProduct);
    fixture.componentRef.setInput('expanded', true);
    fixture.detectChanges();

    httpTesting.expectOne('/api/produtos/1/lotes');

    const skeletons = fixture.nativeElement.querySelectorAll('.lote-skeleton');
    expect(skeletons.length).toBeGreaterThan(0);
  });

  it('deve exibir os lotes após carregar', () => {
    const fixture = TestBed.createComponent(ProductAccordion);
    fixture.componentRef.setInput('product', mockProduct);
    fixture.componentRef.setInput('expanded', true);
    fixture.detectChanges();

    httpTesting.expectOne('/api/produtos/1/lotes').flush(mockLotes);
    fixture.detectChanges();

    const cards = fixture.nativeElement.querySelectorAll('app-lote-card');
    expect(cards.length).toBe(2);
  });

  it('deve mudar para modo formulário ao clicar em adicionar', () => {
    const fixture = TestBed.createComponent(ProductAccordion);
    fixture.componentRef.setInput('product', mockProduct);
    fixture.componentRef.setInput('expanded', true);
    fixture.detectChanges();
    httpTesting.expectOne('/api/produtos/1/lotes').flush(mockLotes);
    fixture.detectChanges();

    fixture.nativeElement.querySelector('.product-accordion__add-btn').click();
    fixture.detectChanges();

    expect(fixture.componentInstance.mode()).toBe('form');
  });

  it('deve mudar para modo histórico ao selecionar um lote', () => {
    const fixture = TestBed.createComponent(ProductAccordion);
    fixture.componentRef.setInput('product', mockProduct);
    fixture.componentRef.setInput('expanded', true);
    fixture.detectChanges();
    httpTesting.expectOne('/api/produtos/1/lotes').flush(mockLotes);
    fixture.detectChanges();

    fixture.nativeElement.querySelector('app-lote-card').click();
    fixture.detectChanges();
    httpTesting.expectOne('/api/lotes/1/historico?pagina=1&tamanhoPagina=10').flush({ items: [], hasMore: false });

    expect(fixture.componentInstance.mode()).toBe('history');
    expect(fixture.componentInstance.selectedLote()?.id).toBe(1);
  });

  it('deve voltar para modo grid ao clicar no mesmo lote selecionado', () => {
    const fixture = TestBed.createComponent(ProductAccordion);
    fixture.componentRef.setInput('product', mockProduct);
    fixture.componentRef.setInput('expanded', true);
    fixture.detectChanges();
    httpTesting.expectOne('/api/produtos/1/lotes').flush(mockLotes);
    fixture.detectChanges();

    const card = fixture.nativeElement.querySelector('app-lote-card');
    card.click();
    fixture.detectChanges();
    httpTesting.expectOne('/api/lotes/1/historico?pagina=1&tamanhoPagina=10').flush({ items: [], hasMore: false });
    fixture.detectChanges();
    card.click();
    fixture.detectChanges();

    expect(fixture.componentInstance.mode()).toBe('grid');
    expect(fixture.componentInstance.selectedLote()).toBeNull();
  });

  it('deve resetar o estado ao recolher', () => {
    const fixture = TestBed.createComponent(ProductAccordion);
    fixture.componentRef.setInput('product', mockProduct);
    fixture.componentRef.setInput('expanded', true);
    fixture.detectChanges();
    httpTesting.expectOne('/api/produtos/1/lotes').flush(mockLotes);
    fixture.detectChanges();

    fixture.nativeElement.querySelector('app-lote-card').click();
    fixture.detectChanges();
    httpTesting.expectOne('/api/lotes/1/historico?pagina=1&tamanhoPagina=10').flush({ items: [], hasMore: false });
    fixture.detectChanges();

    fixture.componentRef.setInput('expanded', false);
    fixture.detectChanges();

    expect(fixture.componentInstance.mode()).toBe('grid');
    expect(fixture.componentInstance.selectedLote()).toBeNull();
  });
});
