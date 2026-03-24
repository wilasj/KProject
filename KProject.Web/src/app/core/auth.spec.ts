import { TestBed } from '@angular/core/testing';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { Auth } from './auth';

describe('Auth', () => {
  let service: Auth;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.resetTestingModule();
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(Auth);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  describe('register()', () => {
    it('deve retornar sucesso no 201', () => {
      let result: any;
      service.register('test@test.com', 'password', 'token123').subscribe((r) => (result = r));

      const req = httpMock.expectOne('/api/users/register');
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual({ email: 'test@test.com', password: 'password', conviteToken: 'token123' });
      req.flush(null, { status: 201, statusText: 'Created' });

      expect(result.success).toBe(true);
    });

    it('deve retornar falha com erros no 400', () => {
      let result: any;
      service.register('test@test.com', 'password', 'token123').subscribe((r) => (result = r));

      const req = httpMock.expectOne('/api/users/register');
      req.flush([{ code: 'Register.EmailVazio', description: 'O email não pode estar vazio' }], {
        status: 400,
        statusText: 'Bad Request',
      });

      expect(result.success).toBe(false);
      expect(result.errors).toEqual([
        { code: 'Register.EmailVazio', description: 'O email não pode estar vazio' },
      ]);
    });
  });

  describe('criaInvite()', () => {
    it('deve retornar o token recebido do servidor', () => {
      let result: any;
      service.criaInvite().subscribe((r) => (result = r));

      const req = httpMock.expectOne('/api/convites');
      expect(req.request.method).toBe('POST');
      req.flush({ token: 'abc123xyz' }, { status: 200, statusText: 'OK' });

      expect(result).toBe('abc123xyz');
    });
  });

  describe('login()', () => {
    it('deve setar isLoggedIn e email no sucesso', () => {
      service.login('test@test.com', 'password').subscribe();

      const req = httpMock.expectOne('/api/users/login');
      req.flush(null, { status: 200, statusText: 'OK' });

      expect(service.isLoggedIn()).toBe(true);
      expect(service.email()).toBe('test@test.com');
    });

    it('deve retornar falha e nao setar isLoggedIn no erro', () => {
      let result: any;
      service.login('test@test.com', 'errada').subscribe((r) => (result = r));

      const req = httpMock.expectOne('/api/users/login');
      req.flush([{ code: 'Usuario.LoginFalhou', description: 'Email ou senha inválidos.' }], {
        status: 401,
        statusText: 'Unauthorized',
      });

      expect(result.success).toBe(false);
      expect(service.isLoggedIn()).toBe(false);
    });
  });

  describe('logout()', () => {
    it('deve limpar isLoggedIn e email no sucesso', () => {
      service.login('test@test.com', 'password').subscribe();
      httpMock.expectOne('/api/users/login').flush(null, { status: 200, statusText: 'OK' });

      service.logout().subscribe();
      httpMock.expectOne('/api/users/logout').flush(null, { status: 200, statusText: 'OK' });

      expect(service.isLoggedIn()).toBe(false);
      expect(service.email()).toBeNull();
    });

    it('deve limpar isLoggedIn e email mesmo em caso de erro', () => {
      service.login('test@test.com', 'password').subscribe();
      httpMock.expectOne('/api/users/login').flush(null, { status: 200, statusText: 'OK' });

      service.logout().subscribe();
      httpMock.expectOne('/api/users/logout').flush(null, { status: 500, statusText: 'Error' });

      expect(service.isLoggedIn()).toBe(false);
      expect(service.email()).toBeNull();
    });
  });

  describe('me()', () => {
    it('deve setar isLoggedIn e email no 200', () => {
      service.me().subscribe();

      const req = httpMock.expectOne('/api/users/me');
      req.flush({ email: 'test@test.com' }, { status: 200, statusText: 'OK' });

      expect(service.isLoggedIn()).toBe(true);
      expect(service.email()).toBe('test@test.com');
    });

    it('deve setar isLoggedIn como false no 401', () => {
      service.me().subscribe();

      const req = httpMock.expectOne('/api/users/me');
      req.flush(null, { status: 401, statusText: 'Unauthorized' });

      expect(service.isLoggedIn()).toBe(false);
    });
  });
});
