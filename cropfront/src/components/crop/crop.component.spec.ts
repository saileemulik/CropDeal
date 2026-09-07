import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CropComponent } from './crop.component';

describe('CropComponent', () => {
  let component: CropComponent;
  let fixture: ComponentFixture<CropComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CropComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CropComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
