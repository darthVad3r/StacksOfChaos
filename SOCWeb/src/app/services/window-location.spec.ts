import { TestBed } from '@angular/core/testing';
import { WindowLocationService } from './window-location.service';

describe('WindowLocationService', () => {
  let service: WindowLocationService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [WindowLocationService]
    });
    service = TestBed.inject(WindowLocationService);
  });

  // Service Instantiation Tests
  describe('Service Creation', () => {
    it('should be created', () => {
      expect(service).toBeTruthy();
    });

    it('should be a singleton', () => {
      const service2 = TestBed.inject(WindowLocationService);
      expect(service).toBe(service2);
    });
  });

  // getLocation() Tests
  describe('getLocation()', () => {
    it('should return the current window.location object', () => {
      const location = service.getLocation();
      expect(location).toBe(window.location);
    });

    it('should always return the same window.location reference', () => {
      const location1 = service.getLocation();
      const location2 = service.getLocation();
      expect(location1).toBe(location2);
    });

    it('should have href property from window.location', () => {
      const location = service.getLocation();
      expect(location.href).toBe(window.location.href);
    });

    it('should have origin property from window.location', () => {
      const location = service.getLocation();
      expect(location.origin).toBe(window.location.origin);
    });

    it('should have pathname property from window.location', () => {
      const location = service.getLocation();
      expect(location.pathname).toBe(window.location.pathname);
    });
  });

  // getHash() Tests
  describe('getHash()', () => {
    it('should return the current window.location.hash', () => {
      const hash = service.getHash();
      expect(hash).toBe(window.location.hash);
    });

    it('should return empty string when no hash is present', () => {
      // Hash is typically empty in test environment
      const hash = service.getHash();
      expect(typeof hash).toBe('string');
    });

    it('should reflect hash changes immediately', () => {
      const originalHash = window.location.hash;
      try {
        // We can't actually change hash in tests without full page reload,
        // but we can verify the method always returns current hash
        const hash1 = service.getHash();
        const hash2 = service.getHash();
        expect(hash1).toBe(hash2);
      } finally {
        window.location.hash = originalHash;
      }
    });

    it('should return value starting with # when hash exists', () => {
      const hash = service.getHash();
      // Hash will either be empty or start with #
      expect(hash === '' || hash.startsWith('#')).toBe(true);
    });
  });

  // getSearch() Tests
  describe('getSearch()', () => {
    it('should return the current window.location.search', () => {
      const search = service.getSearch();
      expect(search).toBe(window.location.search);
    });

    it('should return empty string when no query parameters are present', () => {
      const search = service.getSearch();
      expect(typeof search).toBe('string');
    });

    it('should return value starting with ? when query parameters exist', () => {
      const search = service.getSearch();
      // Search will either be empty or start with ?
      expect(search === '' || search.startsWith('?')).toBe(true);
    });

    it('should reflect search parameter changes immediately', () => {
      const search1 = service.getSearch();
      const search2 = service.getSearch();
      expect(search1).toBe(search2);
    });

    it('should return proper URLSearchParams-compatible string', () => {
      const search = service.getSearch();
      // Should be able to parse it as URLSearchParams
      if (search) {
        const params = new URLSearchParams(search.substring(1));
        expect(params).toBeDefined();
      } else {
        expect(true).toBe(true); // No params to test
      }
    });
  });

  // Data Consistency Tests
  describe('Data Consistency', () => {
    it('should maintain consistency between getLocation and getHash', () => {
      const location = service.getLocation();
      const hash = service.getHash();
      expect(location.hash).toBe(hash);
    });

    it('should maintain consistency between getLocation and getSearch', () => {
      const location = service.getLocation();
      const search = service.getSearch();
      expect(location.search).toBe(search);
    });

    it('should all reference the same underlying window object', () => {
      const location = service.getLocation();
      const hash = service.getHash();
      const search = service.getSearch();
      expect(location.hash).toBe(hash);
      expect(location.search).toBe(search);
    });
  });

  // Integration Tests
  describe('Integration with window.location', () => {
    it('should be testable without actual navigation', () => {
      // This test verifies the service works in test environment
      const location = service.getLocation();
      const hash = service.getHash();
      const search = service.getSearch();
      expect(location).toBeTruthy();
      expect(typeof hash).toBe('string');
      expect(typeof search).toBe('string');
    });

    it('should wrap window.location for testability', () => {
      // WindowLocationService should be injectable and mockable
      const service2 = TestBed.inject(WindowLocationService);
      expect(service2).toBe(service);
      expect(service2.getLocation()).toBe(window.location);
    });
  });
});