import { TestBed } from '@angular/core/testing';
import { SaleDrawer } from './sale-drawer';
import { describe, it, expect, beforeEach } from 'vitest';

describe('SaleDrawer', () => {
    beforeEach(() => {
        TestBed.configureTestingModule({ imports: [SaleDrawer] });
    });

    it('deve exibir "Venda" quando não há saleId', () => {
        const fixture = TestBed.createComponent(SaleDrawer);
        fixture.detectChanges();
        const title = fixture.nativeElement.querySelector('.sale-drawer__title');
        expect(title.textContent.trim()).toBe('Venda');
    });

    it('deve exibir "Venda #X" quando saleId é X', () => {
        const fixture = TestBed.createComponent(SaleDrawer);
        fixture.componentRef.setInput('saleId', 1042);
        fixture.detectChanges();
        const title = fixture.nativeElement.querySelector('.sale-drawer__title');
        expect(title.textContent.trim()).toContain('Venda #1042');
    });

    it('deve emitir close ao clicar no botão fechar', () => {
        const fixture = TestBed.createComponent(SaleDrawer);
        fixture.detectChanges();
        let closed = false;
        fixture.componentInstance.close.subscribe(() => (closed = true));
        fixture.nativeElement.querySelector('.sale-drawer__close').click();
        expect(closed).toBe(true);
    });
});
