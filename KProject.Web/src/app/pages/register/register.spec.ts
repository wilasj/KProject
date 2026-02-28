import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';
import { of } from 'rxjs';
import { Register } from '@pages/register/register';
import { Auth } from '@core/auth';

const mockAuthService = {
  register: vi.fn(),
};

describe('Register', () => {
  let component: Register;
  let fixture: ComponentFixture<Register>;
  let navigateSpy: ReturnType<typeof vi.spyOn>;

  beforeEach(async () => {
    mockAuthService.register.mockReset();

    await TestBed.configureTestingModule({
      imports: [Register],
      providers: [
        provideRouter([]),
        { provide: Auth, useValue: mockAuthService },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(Register);
    component = fixture.componentInstance;
    navigateSpy = vi.spyOn(TestBed.inject(Router), 'navigate').mockResolvedValue(true);
    await fixture.whenStable();
  });

  it('deve criar o componente', () => {
    expect(component).toBeTruthy();
  });

  it('formulario deve ser invalido quando vazio', () => {
    expect(component.registerForm.valid).toBe(false);
  });

  it('formulario deve ser valido com dados corretos', () => {
    component.registerForm.setValue({ email: 'test@test.com', password: 'senha123' });
    expect(component.registerForm.valid).toBe(true);
  });

  it('nao deve chamar o service se o formulario for invalido', () => {
    component.onSubmit();
    expect(mockAuthService.register).not.toHaveBeenCalled();
  });

  it('deve chamar authService.register com os valores do formulario', () => {
    mockAuthService.register.mockReturnValue(of({ success: true }));
    component.registerForm.setValue({ email: 'test@test.com', password: 'senha123' });

    component.onSubmit();

    expect(mockAuthService.register).toHaveBeenCalledWith('test@test.com', 'senha123');
  });

  it('deve navegar para /login apos registro com sucesso', () => {
    mockAuthService.register.mockReturnValue(of({ success: true }));
    component.registerForm.setValue({ email: 'test@test.com', password: 'senha123' });

    component.onSubmit();

    expect(navigateSpy).toHaveBeenCalledWith(['/login']);
  });

  it('deve setar erros quando o service retornar falha', () => {
    const erros = [{ code: 'Usuario.EmailDuplicado', description: 'Email já cadastrado.' }];
    mockAuthService.register.mockReturnValue(of({ success: false, errors: erros }));
    component.registerForm.setValue({ email: 'test@test.com', password: 'senha123' });

    component.onSubmit();

    expect(component.errors()).toEqual(erros);
  });
});
