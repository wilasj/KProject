import { TestBed } from '@angular/core/testing';
import { SaleTable } from './sale-table';
import { Sale } from '@models/venda';
import { describe, it, expect, beforeEach } from 'vitest';

const mockSales: Sale[] = [
    { id: 1042, clienteNome: 'Maria Silva',  criadaEm: '2026-02-27T10:00:00Z', status: 'Aberta',  totalItens: 8 },
    { id: 1041, clienteNome: 'João Santos',  criadaEm: '2026-02-26T09:00:00Z', status: 'Fechada', totalItens: 5 },
];

describe('SaleTable', () => {
    beforeEach(() => {
        TestBed.configureTestingModule({ imports: [SaleTable] });
    });

    it('deve renderizar as vendas recebidas', () => {
        const fixture = TestBed.createComponent(SaleTable);
        fixture.componentRef.setInput('sales', mockSales);
        fixture.componentRef.setInput('total', 2);
        fixture.componentRef.setInput('currentPage', 1);
        fixture.detectChanges();
        const rows = fixture.nativeElement.querySelectorAll('.sale-table__row');
        expect(rows.length).toBe(2);
    });

    it('deve exibir estado vazio quando não há vendas', () => {
        const fixture = TestBed.createComponent(SaleTable);
        fixture.componentRef.setInput('sales', []);
        fixture.componentRef.setInput('total', 0);
        fixture.componentRef.setInput('currentPage', 1);
        fixture.detectChanges();
        const empty = fixture.nativeElement.querySelector('.sale-table__empty');
        expect(empty).not.toBeNull();
    });

    it('deve exibir o nome do cliente e a data em cada row', () => {
        const fixture = TestBed.createComponent(SaleTable);
        fixture.componentRef.setInput('sales', mockSales);
        fixture.componentRef.setInput('total', 2);
        fixture.componentRef.setInput('currentPage', 1);
        fixture.detectChanges();
        const row = fixture.nativeElement.querySelector('.sale-table__row');
        expect(row.textContent).toContain('Maria Silva');
        expect(row.textContent).toContain('Venda #1042');
    });

    it('deve emitir saleSelect com o id ao clicar numa row', () => {
        const fixture = TestBed.createComponent(SaleTable);
        fixture.componentRef.setInput('sales', mockSales);
        fixture.componentRef.setInput('total', 2);
        fixture.componentRef.setInput('currentPage', 1);
        fixture.detectChanges();

        let emittedId: number | undefined;
        fixture.componentInstance.saleSelect.subscribe((id: number) => emittedId = id);

        fixture.nativeElement.querySelector('.sale-table__row').click();
        expect(emittedId).toBe(1042);
    });

    it('deve emitir pageChange ao clicar em próxima página', () => {
        const fixture = TestBed.createComponent(SaleTable);
        fixture.componentRef.setInput('sales', mockSales);
        fixture.componentRef.setInput('total', 25);
        fixture.componentRef.setInput('currentPage', 1);
        fixture.detectChanges();

        let emittedPage: number | undefined;
        fixture.componentInstance.pageChange.subscribe((p: number) => emittedPage = p);

        fixture.nativeElement.querySelector('.pagination__page-btn--next').click();
        expect(emittedPage).toBe(2);
    });
});
