import { TestBed } from '@angular/core/testing';

import { TitleSearchServiceService } from './title-search-service.service';

describe('TitleSearchServiceService', () => {
  let service: TitleSearchServiceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(TitleSearchServiceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
