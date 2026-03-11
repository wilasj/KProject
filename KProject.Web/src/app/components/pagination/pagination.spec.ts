import { TestBed } from '@angular/core/testing';
import { Pagination } from './pagination';
import { describe, it, expect, beforeEach } from 'vitest';

describe('Pagination', () => {
    beforeEach(() => {
        TestBed.configureTestingModule({ imports: [Pagination] });
    });

    it('não renderiza nada quando o total cabe em uma página', () => {
        const fixture = TestBed.createComponent(Pagination);
        fixture.componentRef.setInput('total', 8);
        fixture.componentRef.setInput('current', 1);
        fixture.componentRef.setInput('pageSize', 10);
        fixture.detectChanges();

        const btn = fixture.nativeElement.querySelector('.pagination__page-btn');
        expect(btn).toBeNull();
    });

    it('renderiza os botões de página corretos', () => {
        const fixture = TestBed.createComponent(Pagination);
        fixture.componentRef.setInput('total', 25);
        fixture.componentRef.setInput('current', 1);
        fixture.componentRef.setInput('pageSize', 10);
        fixture.detectChanges();

        // 3 páginas + prev + next = 5 botões
        const btns = fixture.nativeElement.querySelectorAll('.pagination__page-btn');
        expect(btns.length).toBe(5);
    });

    it('marca a página atual como ativa', () => {
        const fixture = TestBed.createComponent(Pagination);
        fixture.componentRef.setInput('total', 25);
        fixture.componentRef.setInput('current', 2);
        fixture.componentRef.setInput('pageSize', 10);
        fixture.detectChanges();

        const active = fixture.nativeElement.querySelector('.pagination__page-btn--active');
        expect(active.textContent.trim()).toBe('2');
    });

    it('desabilita o botão anterior na primeira página', () => {
        const fixture = TestBed.createComponent(Pagination);
        fixture.componentRef.setInput('total', 25);
        fixture.componentRef.setInput('current', 1);
        fixture.componentRef.setInput('pageSize', 10);
        fixture.detectChanges();

        const prev = fixture.nativeElement.querySelector('.pagination__page-btn--prev');
        expect(prev.disabled).toBe(true);
    });

    it('desabilita o botão próximo na última página', () => {
        const fixture = TestBed.createComponent(Pagination);
        fixture.componentRef.setInput('total', 25);
        fixture.componentRef.setInput('current', 3);
        fixture.componentRef.setInput('pageSize', 10);
        fixture.detectChanges();

        const next = fixture.nativeElement.querySelector('.pagination__page-btn--next');
        expect(next.disabled).toBe(true);
    });

    it('emite pageChange ao clicar num botão de página', () => {
        const fixture = TestBed.createComponent(Pagination);
        fixture.componentRef.setInput('total', 25);
        fixture.componentRef.setInput('current', 1);
        fixture.componentRef.setInput('pageSize', 10);
        fixture.detectChanges();

        let emitted: number | undefined;
        fixture.componentInstance.pageChange.subscribe((p: number) => (emitted = p));

        // botões de página numérica: prev(0), pag1(1), pag2(2), pag3(3), next(4)
        const btns = fixture.nativeElement.querySelectorAll('.pagination__page-btn');
        btns[2].click(); // clica em "2"
        expect(emitted).toBe(2);
    });

    it('emite pageChange ao clicar em próxima página', () => {
        const fixture = TestBed.createComponent(Pagination);
        fixture.componentRef.setInput('total', 25);
        fixture.componentRef.setInput('current', 1);
        fixture.componentRef.setInput('pageSize', 10);
        fixture.detectChanges();

        let emitted: number | undefined;
        fixture.componentInstance.pageChange.subscribe((p: number) => (emitted = p));

        fixture.nativeElement.querySelector('.pagination__page-btn--next').click();
        expect(emitted).toBe(2);
    });

    it('emite pageChange ao clicar em página anterior', () => {
        const fixture = TestBed.createComponent(Pagination);
        fixture.componentRef.setInput('total', 25);
        fixture.componentRef.setInput('current', 2);
        fixture.componentRef.setInput('pageSize', 10);
        fixture.detectChanges();

        let emitted: number | undefined;
        fixture.componentInstance.pageChange.subscribe((p: number) => (emitted = p));

        fixture.nativeElement.querySelector('.pagination__page-btn--prev').click();
        expect(emitted).toBe(1);
    });

    it('não emite pageChange ao clicar na página já ativa', () => {
        const fixture = TestBed.createComponent(Pagination);
        fixture.componentRef.setInput('total', 25);
        fixture.componentRef.setInput('current', 1);
        fixture.componentRef.setInput('pageSize', 10);
        fixture.detectChanges();

        let emitCount = 0;
        fixture.componentInstance.pageChange.subscribe(() => emitCount++);

        fixture.nativeElement.querySelector('.pagination__page-btn--active').click();
        expect(emitCount).toBe(0);
    });
});
