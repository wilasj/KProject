import { TestBed } from '@angular/core/testing';
import { LoteCard } from './lote-card';
import { Lote } from '@models/lote';
import { describe, it, expect, beforeEach } from 'vitest';

const mockLote: Lote = {
  id: 1,
  numero: 42,
  validade: '2027-06-30',
  quantidadeTotal: 150,
};

describe('LoteCard', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [LoteCard] });
  });

  it('deve renderizar o número do lote', () => {
    const fixture = TestBed.createComponent(LoteCard);
    fixture.componentRef.setInput('lote', mockLote);
    fixture.detectChanges();
    const el = fixture.nativeElement.querySelector('.lote-card__number');
    expect(el.textContent).toContain('42');
  });

  it('deve renderizar a quantidade em destaque', () => {
    const fixture = TestBed.createComponent(LoteCard);
    fixture.componentRef.setInput('lote', mockLote);
    fixture.detectChanges();
    const el = fixture.nativeElement.querySelector('.lote-card__quantity');
    expect(el.textContent).toContain('150');
  });

  it('deve renderizar a validade formatada', () => {
    const fixture = TestBed.createComponent(LoteCard);
    fixture.componentRef.setInput('lote', mockLote);
    fixture.detectChanges();
    const el = fixture.nativeElement.querySelector('.lote-card__validade');
    expect(el.textContent).toContain('30/06/2027');
  });

  it('deve emitir evento select ao clicar', () => {
    const fixture = TestBed.createComponent(LoteCard);
    fixture.componentRef.setInput('lote', mockLote);
    fixture.detectChanges();

    let emitted: Lote | undefined;
    fixture.componentInstance.select.subscribe((l: Lote) => emitted = l);

    fixture.nativeElement.click();
    expect(emitted).toEqual(mockLote);
  });

  it('deve aplicar classe selected quando selecionado', () => {
    const fixture = TestBed.createComponent(LoteCard);
    fixture.componentRef.setInput('lote', mockLote);
    fixture.componentRef.setInput('selected', true);
    fixture.detectChanges();
    expect(fixture.nativeElement.classList).toContain('lote-card--selected');
  });
});
