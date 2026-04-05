import { Component } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { Drawer } from './drawer';

describe('Drawer', () => {
  let fixture: ComponentFixture<Drawer>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [Drawer] }).compileComponents();
    fixture = TestBed.createComponent(Drawer);
    fixture.componentRef.setInput('title', 'Test Title');
    fixture.componentRef.setInput('subtitle', '');
    fixture.componentRef.setInput('errors', []);
    fixture.detectChanges();
  });

  it('renders the title', () => {
    const title = fixture.debugElement.query(By.css('.drawer__title'));
    expect(title.nativeElement.textContent.trim()).toBe('Test Title');
  });

  it('does not render subtitle when empty', () => {
    const subtitle = fixture.debugElement.query(By.css('.drawer__subtitle'));
    expect(subtitle).toBeNull();
  });

  it('renders subtitle when provided', () => {
    fixture.componentRef.setInput('subtitle', 'A subtitle');
    fixture.detectChanges();
    const subtitle = fixture.debugElement.query(By.css('.drawer__subtitle'));
    expect(subtitle.nativeElement.textContent.trim()).toBe('A subtitle');
  });

  it('does not render error block when errors is empty', () => {
    const errors = fixture.debugElement.query(By.css('.drawer__errors'));
    expect(errors).toBeNull();
  });

  it('renders errors when provided', () => {
    fixture.componentRef.setInput('errors', [
      { code: 'E1', description: 'Error one' },
      { code: 'E2', description: 'Error two' },
    ]);
    fixture.detectChanges();
    const spans = fixture.debugElement.queryAll(By.css('.drawer__errors span'));
    expect(spans.length).toBe(2);
    expect(spans[0].nativeElement.textContent.trim()).toBe('Error one');
    expect(spans[1].nativeElement.textContent.trim()).toBe('Error two');
  });

  it('emits close when close button is clicked', () => {
    let closed = false;
    fixture.componentInstance.close.subscribe(() => (closed = true));
    fixture.debugElement.query(By.css('.drawer__close')).nativeElement.click();
    expect(closed).toBe(true);
  });
});

@Component({
  template: `<app-drawer title="T"><span class="projected">content</span></app-drawer>`,
  imports: [Drawer],
})
class ProjectionHost {}

describe('Drawer content projection', () => {
  it('projects content into ng-content', async () => {
    await TestBed.configureTestingModule({ imports: [ProjectionHost] }).compileComponents();
    const pFixture = TestBed.createComponent(ProjectionHost);
    pFixture.detectChanges();
    const found = pFixture.debugElement.query(By.css('.projected'));
    expect(found).not.toBeNull();
    expect(found.nativeElement.textContent.trim()).toBe('content');
  });
});
