import { TestBed } from '@angular/core/testing';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { signal } from '@angular/core';
import { publicGuard } from './public-guard';
import { Auth } from './auth';

describe('publicGuard', () => {
  const mockRoute = {} as ActivatedRouteSnapshot;
  const mockState = {} as RouterStateSnapshot;

  beforeEach(() => TestBed.resetTestingModule());

  it('deve retornar true quando nao logado', () => {
    TestBed.configureTestingModule({
      providers: [{ provide: Auth, useValue: { isLoggedIn: signal(false) } }],
    });

    const result = TestBed.runInInjectionContext(() => publicGuard(mockRoute, mockState));

    expect(result).toBe(true);
  });

  it('deve retornar false quando logado', () => {
    TestBed.configureTestingModule({
      providers: [{ provide: Auth, useValue: { isLoggedIn: signal(true) } }],
    });

    const result = TestBed.runInInjectionContext(() => publicGuard(mockRoute, mockState));

    expect(result).toBe(false);
  });
});
