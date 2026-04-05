import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { signal } from '@angular/core';
import { of, Subject } from 'rxjs';
import { Sidebar } from './sidebar';
import { Auth } from '@core/auth';
import { InvitePopup } from '../invite-popup/invite-popup';
import { PdfImporterComponent } from '@components/pdf-importer/pdf-importer.component';
import { ImportService } from '@core/import.service';
import { ImportTask } from '@models/import';

const mockImportService = {
  getTasks: vi.fn().mockReturnValue(of([])),
  uploadFiles: vi.fn().mockReturnValue(of([])),
  connectSse: vi.fn().mockReturnValue(new Subject<Partial<ImportTask>>().asObservable()),
  disconnectSse: vi.fn(),
};

describe('Sidebar', () => {
  let component: Sidebar;
  let fixture: ComponentFixture<Sidebar>;

  beforeEach(() => {
    TestBed.resetTestingModule();
    mockImportService.getTasks.mockReturnValue(of([]));
    mockImportService.connectSse.mockReturnValue(new Subject<Partial<ImportTask>>().asObservable());
    mockImportService.disconnectSse.mockReset();
  });

  const createComponent = async (email: string | null) => {
    await TestBed.configureTestingModule({
      imports: [Sidebar],
      providers: [
        provideRouter([]),
        {
          provide: Auth,
          useValue: { email: signal(email), criaInvite: vi.fn().mockReturnValue(of('token')) },
        },
        { provide: ImportService, useValue: mockImportService },
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

    expect(component.navItems.length).toBe(4);
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

  it('deve iniciar com importOpen como false', async () => {
    await createComponent('test@test.com');
    expect(component.importOpen()).toBe(false);
  });

  it('deve abrir o pdf-importer ao clicar no botao de import', async () => {
    await createComponent('test@test.com');
    fixture.detectChanges();

    fixture.nativeElement.querySelector('.sidebar__import').click();

    expect(component.importOpen()).toBe(true);
  });

  it('deve fechar o pdf-importer quando PdfImporterComponent emitir close', async () => {
    await createComponent('test@test.com');
    component.importOpen.set(true);
    fixture.detectChanges();

    const importerInstance = fixture.debugElement.query(By.directive(PdfImporterComponent)).componentInstance as PdfImporterComponent;
    importerInstance.close.emit();

    expect(component.importOpen()).toBe(false);
  });
});
