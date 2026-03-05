import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { InvitePopup } from './invite-popup';
import { Auth } from '@core/auth';

describe('InvitePopup', () => {
  let component: InvitePopup;
  let fixture: ComponentFixture<InvitePopup>;
  const mockAuth = { criaInvite: vi.fn() };

  beforeEach(() => {
    mockAuth.criaInvite.mockReset();
  });

  async function setup() {
    await TestBed.configureTestingModule({
      imports: [InvitePopup],
      providers: [{ provide: Auth, useValue: mockAuth }],
    }).compileComponents();

    fixture = TestBed.createComponent(InvitePopup);
    component = fixture.componentInstance;
    await fixture.whenStable();
  }

  describe('ao receber o token com sucesso', () => {
    beforeEach(async () => {
      mockAuth.criaInvite.mockReturnValue(of('abc123'));
      await setup();
    });

    it('deve criar o componente', () => {
      expect(component).toBeTruthy();
    });

    it('deve desativar o loading', () => {
      expect(component.loading()).toBe(false);
    });

    it('deve montar a URL com o token', () => {
      expect(component.inviteUrl()).toBe(`${window.location.origin}/register?token=abc123`);
    });
  });

  describe('ao falhar ao buscar o token', () => {
    beforeEach(async () => {
      mockAuth.criaInvite.mockReturnValue(throwError(() => new Error()));
      await setup();
    });

    it('deve desativar o loading', () => {
      expect(component.loading()).toBe(false);
    });

    it('deve manter inviteUrl como null', () => {
      expect(component.inviteUrl()).toBeNull();
    });
  });

  describe('copy()', () => {
    let writeTextMock: ReturnType<typeof vi.fn>;

    beforeEach(async () => {
      writeTextMock = vi.fn().mockResolvedValue(undefined);
      Object.defineProperty(navigator, 'clipboard', {
        value: { writeText: writeTextMock },
        configurable: true,
        writable: true,
      });
      mockAuth.criaInvite.mockReturnValue(of('abc123'));
      await setup();
    });

    afterEach(() => {
      vi.useRealTimers();
    });

    it('deve escrever a URL no clipboard', () => {
      component.copy();
      expect(writeTextMock).toHaveBeenCalledWith(component.inviteUrl());
    });

    it('deve setar copied como true apos copiar', async () => {
      component.copy();
      await Promise.resolve();
      expect(component.copied()).toBe(true);
    });

    it('deve resetar copied apos 2 segundos', async () => {
      vi.useFakeTimers();
      component.copy();
      await Promise.resolve();
      expect(component.copied()).toBe(true);
      vi.advanceTimersByTime(2000);
      expect(component.copied()).toBe(false);
    });
  });
});
