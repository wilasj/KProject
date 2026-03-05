import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { signal } from '@angular/core';
import { of } from 'rxjs';
import { Sidebar } from './sidebar';
import { Auth } from '@core/auth';
import { InvitePopup } from '../invite-popup/invite-popup';

describe('Sidebar', () => {
  let component: Sidebar;
  let fixture: ComponentFixture<Sidebar>;

  const createComponent = async (email: string | null) => {
    await TestBed.configureTestingModule({
      imports: [Sidebar],
      providers: [
        provideRouter([]),
        {
          provide: Auth,
          useValue: { email: signal(email), criaInvite: vi.fn().mockReturnValue(of('token')) },
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(Sidebar);
    component = fixture.componentInstance;
    await fixture.whenStable();
  };

  it('deve criar o componente', async () => {
    await createComponent('test@test.com');

    expect(component).toBeTruthy();
  });

  it('deve exibir a inicial do email no avatar', async () => {
    await createComponent('joao@test.com');

    expect(component.initial).toBe('J');
  });

  it('deve exibir "?" quando nao ha email', async () => {
    await createComponent(null);

    expect(component.initial).toBe('?');
  });

  it('deve ter 5 itens de navegacao', async () => {
    await createComponent('test@test.com');

    expect(component.navItems.length).toBe(5);
  });

  it('deve emitir logoutClick ao clicar no botao de logout', async () => {
    await createComponent('test@test.com');
    fixture.detectChanges();

    let emitted = false;
    component.logoutClick.subscribe(() => (emitted = true));

    const button = fixture.nativeElement.querySelector('.sidebar__logout');
    button.click();

    expect(emitted).toBe(true);
  });

  it('deve iniciar com inviteOpen como false', async () => {
    await createComponent('test@test.com');
    expect(component.inviteOpen()).toBe(false);
  });

  it('deve abrir o popup ao clicar no botao de convite', async () => {
    await createComponent('test@test.com');
    fixture.detectChanges();

    fixture.nativeElement.querySelector('.sidebar__invite').click();

    expect(component.inviteOpen()).toBe(true);
  });

  it('deve fechar o popup quando InvitePopup emitir close', async () => {
    await createComponent('test@test.com');
    component.inviteOpen.set(true);
    fixture.detectChanges();

    const popupInstance = fixture.debugElement.query(By.directive(InvitePopup)).componentInstance as InvitePopup;
    popupInstance.close.emit();

    expect(component.inviteOpen()).toBe(false);
  });
});
