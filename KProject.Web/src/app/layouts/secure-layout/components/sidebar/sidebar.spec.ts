import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { signal } from '@angular/core';
import { Sidebar } from './sidebar';
import { Auth } from '@core/auth';

describe('Sidebar', () => {
  let component: Sidebar;
  let fixture: ComponentFixture<Sidebar>;

  const createComponent = async (email: string | null) => {
    await TestBed.configureTestingModule({
      imports: [Sidebar],
      providers: [
        provideRouter([]),
        { provide: Auth, useValue: { email: signal(email) } },
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
});
