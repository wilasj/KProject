import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';
import { signal } from '@angular/core';
import { of } from 'rxjs';
import { vi } from 'vitest';
import { SecureLayout } from './secure-layout';
import { Auth } from '@core/auth';

describe('SecureLayout', () => {
  let component: SecureLayout;
  let fixture: ComponentFixture<SecureLayout>;
  let navigateSpy: ReturnType<typeof vi.spyOn>;
  let mockAuth: {
    isLoggedIn: ReturnType<typeof signal<boolean>>;
    email: ReturnType<typeof signal<string | null>>;
    logout: ReturnType<typeof vi.fn>;
  };

  beforeEach(async () => {
    mockAuth = {
      isLoggedIn: signal(true),
      email: signal('test@test.com'),
      logout: vi.fn(() => of(undefined)),
    };

    await TestBed.configureTestingModule({
      imports: [SecureLayout],
      providers: [
        provideRouter([]),
        { provide: Auth, useValue: mockAuth },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(SecureLayout);
    component = fixture.componentInstance;
    navigateSpy = vi.spyOn(TestBed.inject(Router), 'navigate').mockResolvedValue(true);
    await fixture.whenStable();
  });

  it('deve criar o componente', () => {
    expect(component).toBeTruthy();
  });

  it('deve chamar auth.logout ao fazer logout', () => {
    component.onLogout();

    expect(mockAuth.logout).toHaveBeenCalledOnce();
  });

  it('deve navegar para /login apos logout', () => {
    component.onLogout();

    expect(navigateSpy).toHaveBeenCalledWith(['/login']);
  });
});
