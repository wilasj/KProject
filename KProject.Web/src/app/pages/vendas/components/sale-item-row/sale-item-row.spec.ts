import { TestBed } from '@angular/core/testing';
import { SaleItemRow } from './sale-item-row';
import { EditableItem } from '@models/venda';
import { describe, it, expect, beforeEach } from 'vitest';

const mockItem: EditableItem = {
  id: 1,
  produtoNome: 'Camiseta Branca',
  loteNumero: 3,
  pacienteNome: 'Paciente A',
  quantidadeConsignada: 10,
  vendido: 4,
  devolvido: 2,
};

describe('SaleItemRow', () => {
  beforeEach(() => {
    TestBed.resetTestingModule();
    TestBed.configureTestingModule({ imports: [SaleItemRow] });
  });

  function setup(item = mockItem, readOnly = false) {
    const fixture = TestBed.createComponent(SaleItemRow);
    fixture.componentRef.setInput('item', item);
    fixture.componentRef.setInput('readOnly', readOnly);
    fixture.detectChanges();
    return fixture;
  }

  it('deve exibir produto, lote, paciente e ratio no header', () => {
    const fixture = setup();
    const header = fixture.nativeElement.querySelector('.sale-item-row__header');
    expect(header.textContent).toContain('Camiseta Branca');
    expect(header.textContent).toContain('3');
    expect(header.textContent).toContain('Paciente A');
    expect(fixture.nativeElement.querySelector('.sale-item-row__ratio').textContent.trim()).toBe('6/10');
  });

  it('deve expandir ao clicar no header', () => {
    const fixture = setup();
    fixture.nativeElement.querySelector('.sale-item-row__header').click();
    fixture.detectChanges();
    expect(fixture.componentInstance.expanded()).toBe(true);
    expect(fixture.nativeElement.querySelector('.sale-item-row__body')).not.toBeNull();
  });

  it('deve emitir itemChange ao aumentar vendidos', () => {
    const fixture = setup();
    fixture.componentInstance.expanded.set(true);
    fixture.detectChanges();

    let emitted: EditableItem | undefined;
    fixture.componentInstance.itemChange.subscribe((v: EditableItem) => (emitted = v));

    const [, vendidosGroup] = fixture.nativeElement.querySelectorAll('.sale-item-row__control-group');
    vendidosGroup.querySelector('[aria-label="Aumentar vendidos"]').click();

    expect(emitted?.vendido).toBe(5);
  });

  it('deve emitir itemChange ao diminuir devolvidos', () => {
    const fixture = setup();
    fixture.componentInstance.expanded.set(true);
    fixture.detectChanges();

    let emitted: EditableItem | undefined;
    fixture.componentInstance.itemChange.subscribe((v: EditableItem) => (emitted = v));

    const [devolvidosGroup] = fixture.nativeElement.querySelectorAll('.sale-item-row__control-group');
    devolvidosGroup.querySelector('[aria-label="Diminuir devolvidos"]').click();

    expect(emitted?.devolvido).toBe(1);
  });

  it('não deve permitir vendido + devolvido > quantidadeConsignada', () => {
    const fixture = setup({ ...mockItem, vendido: 8, devolvido: 2 });
    fixture.componentInstance.expanded.set(true);
    fixture.detectChanges();

    let emitted: EditableItem | undefined;
    fixture.componentInstance.itemChange.subscribe((v: EditableItem) => (emitted = v));

    const [, vendidosGroup] = fixture.nativeElement.querySelectorAll('.sale-item-row__control-group');
    vendidosGroup.querySelector('[aria-label="Aumentar vendidos"]').click();

    expect(emitted).toBeUndefined();
  });

  it('não deve permitir devolvido negativo', () => {
    const fixture = setup({ ...mockItem, devolvido: 0 });
    fixture.componentInstance.expanded.set(true);
    fixture.detectChanges();

    let emitted: EditableItem | undefined;
    fixture.componentInstance.itemChange.subscribe((v: EditableItem) => (emitted = v));

    const [devolvidosGroup] = fixture.nativeElement.querySelectorAll('.sale-item-row__control-group');
    devolvidosGroup.querySelector('[aria-label="Diminuir devolvidos"]').click();

    expect(emitted).toBeUndefined();
  });

  it('deve desabilitar os controles quando readOnly', () => {
    const fixture = setup(mockItem, true);
    fixture.componentInstance.expanded.set(true);
    fixture.detectChanges();

    const buttons = fixture.nativeElement.querySelectorAll('.sale-item-row__counter-btn');
    buttons.forEach((btn: HTMLButtonElement) => {
      expect(btn.disabled).toBe(true);
    });
  });
});
