import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VsMapComponent } from './vs-map.component';

describe('VsMapComponent', () => {
  let component: VsMapComponent;
  let fixture: ComponentFixture<VsMapComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ VsMapComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(VsMapComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
