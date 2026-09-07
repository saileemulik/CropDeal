import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CroplistingsComponent } from './croplistings.component';

describe('CroplistingsComponent', () => {
  let component: CroplistingsComponent;
  let fixture: ComponentFixture<CroplistingsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CroplistingsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CroplistingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
