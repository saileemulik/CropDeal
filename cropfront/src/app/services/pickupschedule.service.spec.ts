import { TestBed } from '@angular/core/testing';

import { PickupscheduleService } from './pickupschedule.service';

describe('PickupscheduleService', () => {
  let service: PickupscheduleService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PickupscheduleService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
