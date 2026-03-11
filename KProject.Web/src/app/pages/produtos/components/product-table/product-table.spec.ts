import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { ProductTable } from './product-table';
import { Product } from '@models/produto';
import { describe, it, expect, beforeEach, afterEach } from 'vitest';

const mockProducts: Product[] = [
  { id: 1, nome: 'Produto A', referencia: 'REF-001', descricao: 'Desc A', codigoAnvisa: 'ANV-001', criadoEm: '2026-03-01T00:00:00Z' },
  { id: 2, nome: 'Produto B', referencia: 'REF-002', descricao: 'Desc B', codigoAnvisa: 'ANV-002', criadoEm: '2026-03-02T00:00:00Z' },
];

describe('ProductTable', () => {
  let httpTesting: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [ProductTable],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    httpTesting = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpTesting.verify());

  it('deve renderizar os produtos recebidos', () => {
    const fixture = TestBed.createComponent(ProductTable);
    fixture.componentRef.setInput('products', mockProducts);
    fixture.componentRef.setInput('total', 2);
    fixture.componentRef.setInput('currentPage', 1);
    fixture.detectChanges();
    const rows = fixture.nativeElement.querySelectorAll('.product-table__row');
    expect(rows.length).toBe(2);
  });

  it('deve exibir estado vazio quando não há produtos', () => {
    const fixture = TestBed.createComponent(ProductTable);
    fixture.componentRef.setInput('products', []);
    fixture.componentRef.setInput('total', 0);
    fixture.componentRef.setInput('currentPage', 1);
    fixture.detectChanges();
    const empty = fixture.nativeElement.querySelector('.product-table__empty');
    expect(empty).not.toBeNull();
  });

  it('deve emitir pageChange ao clicar em próxima página', () => {
    const fixture = TestBed.createComponent(ProductTable);
    fixture.componentRef.setInput('products', mockProducts);
    fixture.componentRef.setInput('total', 25);
    fixture.componentRef.setInput('currentPage', 1);
    fixture.detectChanges();

    let emittedPage: number | undefined;
    fixture.componentInstance.pageChange.subscribe((p: number) => emittedPage = p);

    const nextBtn = fixture.nativeElement.querySelector('.pagination__page-btn--next');
    nextBtn.click();
    expect(emittedPage).toBe(2);
  });

  it('deve expandir o accordion ao clicar num row', () => {
    const fixture = TestBed.createComponent(ProductTable);
    fixture.componentRef.setInput('products', mockProducts);
    fixture.componentRef.setInput('total', 2);
    fixture.componentRef.setInput('currentPage', 1);
    fixture.detectChanges();

    fixture.nativeElement.querySelector('.product-table__row').click();
    fixture.detectChanges();

    httpTesting.expectOne('/api/produtos/1/lotes');
    const accordion = fixture.nativeElement.querySelector('app-product-accordion .product-accordion');
    expect(accordion).not.toBeNull();
  });

  it('deve recolher o accordion ao clicar no mesmo row', () => {
    const fixture = TestBed.createComponent(ProductTable);
    fixture.componentRef.setInput('products', mockProducts);
    fixture.componentRef.setInput('total', 2);
    fixture.componentRef.setInput('currentPage', 1);
    fixture.detectChanges();

    const row = fixture.nativeElement.querySelector('.product-table__row');
    row.click();
    fixture.detectChanges();
    httpTesting.expectOne('/api/produtos/1/lotes');

    row.click();
    fixture.detectChanges();

    const accordion = fixture.nativeElement.querySelector('app-product-accordion .product-accordion');
    expect(accordion).toBeNull();
  });

  it('deve fechar accordion anterior ao expandir outro row', () => {
    const fixture = TestBed.createComponent(ProductTable);
    fixture.componentRef.setInput('products', mockProducts);
    fixture.componentRef.setInput('total', 2);
    fixture.componentRef.setInput('currentPage', 1);
    fixture.detectChanges();

    const rows = fixture.nativeElement.querySelectorAll('.product-table__row');
    rows[0].click();
    fixture.detectChanges();
    httpTesting.expectOne('/api/produtos/1/lotes');

    rows[1].click();
    fixture.detectChanges();
    httpTesting.expectOne('/api/produtos/2/lotes');

    expect(fixture.componentInstance.expandedProductId()).toBe(2);
  });
});
