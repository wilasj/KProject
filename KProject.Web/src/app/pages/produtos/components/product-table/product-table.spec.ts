import { TestBed } from '@angular/core/testing';
import { ProductTable } from './product-table';
import { Product } from '@models/produto';
import { describe, it, expect, beforeEach } from 'vitest';

const mockProducts: Product[] = [
  { id: 1, nome: 'Produto A', referencia: 'REF-001', descricao: 'Desc A', codigoAnvisa: 'ANV-001', criadoEm: '2026-03-01T00:00:00Z' },
  { id: 2, nome: 'Produto B', referencia: 'REF-002', descricao: 'Desc B', codigoAnvisa: 'ANV-002', criadoEm: '2026-03-02T00:00:00Z' },
];

describe('ProductTable', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [ProductTable] });
  });

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

    const nextBtn = fixture.nativeElement.querySelector('.product-table__page-btn--next');
    nextBtn.click();
    expect(emittedPage).toBe(2);
  });
});
