import { TestBed } from '@angular/core/testing';

import { PricenegotiationService } from './pricenegotiation.service';

describe('PricenegotiationService', () => {
  let service: PricenegotiationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PricenegotiationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
