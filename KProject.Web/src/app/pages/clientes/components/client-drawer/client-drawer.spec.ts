import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { ClientDrawer } from './client-drawer';
import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';

describe('ClientDrawer', () => {
  beforeEach(() => {
    TestBed.resetTestingModule();
    TestBed.configureTestingModule({
      imports: [ClientDrawer],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
  });

  afterEach(() => vi.useRealTimers());

  it('deve desabilitar o botão salvar com form inválido', () => {
    const fixture = TestBed.createComponent(ClientDrawer);
    fixture.detectChanges();
    const button = fixture.nativeElement.querySelector('button[type="submit"]');
    expect(button.disabled).toBe(true);
  });

  it('deve habilitar o botão salvar com form válido', () => {
    const fixture = TestBed.createComponent(ClientDrawer);
    fixture.componentInstance.form.setValue({ nome: 'Cliente Teste' });
    fixture.detectChanges();
    const button = fixture.nativeElement.querySelector('button[type="submit"]');
    expect(button.disabled).toBe(false);
  });

  it('deve emitir clientCreated após POST com sucesso', () => {
    vi.useFakeTimers();

    const fixture = TestBed.createComponent(ClientDrawer);
    const http = TestBed.inject(HttpTestingController);
    fixture.detectChanges();

    let emitted = false;
    fixture.componentInstance.clientCreated.subscribe(() => emitted = true);

    fixture.componentInstance.form.setValue({ nome: 'Cliente Teste' });
    fixture.componentInstance.onSubmit();

    const req = http.expectOne('/api/clientes');
    req.flush({ id: 1 });

    expect(fixture.componentInstance.saved()).toBe(true);
    expect(emitted).toBe(false);

    vi.advanceTimersByTime(1500);
    expect(emitted).toBe(true);
    http.verify();
  });
});
