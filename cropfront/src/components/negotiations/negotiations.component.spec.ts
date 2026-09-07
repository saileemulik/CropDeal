import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NegotiationsComponent } from './negotiations.component';

describe('NegotiationsComponent', () => {
  let component: NegotiationsComponent;
  let fixture: ComponentFixture<NegotiationsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NegotiationsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NegotiationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
