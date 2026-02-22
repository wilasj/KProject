import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of } from 'rxjs';
import { Login } from '@pages/login/login';
import { Auth } from '@core/auth';

const mockAuthService = {
  login: vi.fn(),
};

describe('Login', () => {
  let component: Login;
  let fixture: ComponentFixture<Login>;

  beforeEach(async () => {
    mockAuthService.login.mockReset();

    await TestBed.configureTestingModule({
      imports: [Login],
      providers: [
        provideRouter([]),
        { provide: Auth, useValue: mockAuthService },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(Login);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('deve criar o componente', () => {
    expect(component).toBeTruthy();
  });

  it('formulario deve ser invalido quando vazio', () => {
    expect(component.loginForm.valid).toBe(false);
  });

  it('formulario deve ser valido com dados corretos', () => {
    component.loginForm.setValue({ email: 'test@test.com', password: 'senha123' });
    expect(component.loginForm.valid).toBe(true);
  });

  it('nao deve chamar o service se o formulario for invalido', () => {
    component.onSubmit();
    expect(mockAuthService.login).not.toHaveBeenCalled();
  });

  it('deve chamar authService.login com os valores do formulario', () => {
    mockAuthService.login.mockReturnValue(of({ success: true }));
    component.loginForm.setValue({ email: 'test@test.com', password: 'senha123' });

    component.onSubmit();

    expect(mockAuthService.login).toHaveBeenCalledWith('test@test.com', 'senha123');
  });

  it('deve setar erros quando o service retornar falha', () => {
    const erros = [{ code: 'Usuario.LoginFalhou', description: 'Email ou senha inválidos.' }];
    mockAuthService.login.mockReturnValue(of({ success: false, errors: erros }));
    component.loginForm.setValue({ email: 'test@test.com', password: 'senha123' });

    component.onSubmit();

    expect(component.errors()).toEqual(erros);
  });
});
