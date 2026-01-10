import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { HttpClient, HttpRequest } from '@angular/common/http';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './auth.interceptor';
import { AuthService } from './auth.service';

describe('AuthInterceptor', () => {
  let httpClient: HttpClient;
  let httpTestingController: HttpTestingController;
  let authServiceMock: jasmine.SpyObj<AuthService>;
  const apiUrl = 'https://api.example.com/data';

  beforeEach(() => {
    // Create a spy object for AuthService
    authServiceMock = jasmine.createSpyObj('AuthService', ['getToken']);

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        AuthInterceptor,
        { provide: AuthService, useValue: authServiceMock },
        { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
      ]
    });

    httpClient = TestBed.inject(HttpClient);
    httpTestingController = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    // Verify that there are no unmatched requests
    httpTestingController.verify();
  });

  // Interceptor Instantiation Tests
  describe('Interceptor Creation', () => {
    it('should be created', () => {
      const interceptor = TestBed.inject(AuthInterceptor);
      expect(interceptor).toBeTruthy();
    });

    it('should implement HttpInterceptor interface', () => {
      const interceptor = TestBed.inject(AuthInterceptor);
      expect(interceptor.intercept).toBeDefined();
    });

    it('should have AuthService injected', () => {
      const interceptor = TestBed.inject(AuthInterceptor);
      expect((interceptor as any).authService).toBeDefined();
    });
  });

  // Token Presence Tests
  describe('Token Handling', () => {
    it('should retrieve token from AuthService', () => {
      authServiceMock.getToken.and.returnValue('test-token');

      httpClient.get(apiUrl).subscribe();

      const req = httpTestingController.expectOne(apiUrl);
      expect(authServiceMock.getToken).toHaveBeenCalled();
      req.flush({});
    });

    it('should add Authorization header when token exists', () => {
      authServiceMock.getToken.and.returnValue('test-token-123');

      httpClient.get(apiUrl).subscribe();

      const req = httpTestingController.expectOne(apiUrl);
      expect(req.request.headers.has('Authorization')).toBe(true);
      req.flush({});
    });

    it('should not add Authorization header when token is null', () => {
      authServiceMock.getToken.and.returnValue(null);

      httpClient.get(apiUrl).subscribe();

      const req = httpTestingController.expectOne(apiUrl);
      expect(req.request.headers.has('Authorization')).toBe(false);
      req.flush({});
    });

    it('should not add Authorization header when token is empty string', () => {
      authServiceMock.getToken.and.returnValue('');

      httpClient.get(apiUrl).subscribe();

      const req = httpTestingController.expectOne(apiUrl);
      expect(req.request.headers.has('Authorization')).toBe(false);
      req.flush({});
    });
  });

  // Bearer Token Format Tests
  describe('Authorization Header Format', () => {
    it('should use Bearer scheme for Authorization header', () => {
      authServiceMock.getToken.and.returnValue('token-value');

      httpClient.get(apiUrl).subscribe();

      const req = httpTestingController.expectOne(apiUrl);
      const authHeader = req.request.headers.get('Authorization');
      expect(authHeader).toContain('Bearer');
      req.flush({});
    });

    it('should format Authorization header as "Bearer <token>"', () => {
      const token = 'jwt-token-abc123';
      authServiceMock.getToken.and.returnValue(token);

      httpClient.get(apiUrl).subscribe();

      const req = httpTestingController.expectOne(apiUrl);
      const authHeader = req.request.headers.get('Authorization');
      expect(authHeader).toBe(`Bearer ${token}`);
      req.flush({});
    });

    it('should include full token in Authorization header', () => {
      const fullToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIn0.dozjgNryP4J3jVmNHl0w5N_XgL0n3I9PlFUP0THsR8U';
      authServiceMock.getToken.and.returnValue(fullToken);

      httpClient.get(apiUrl).subscribe();

      const req = httpTestingController.expectOne(apiUrl);
      const authHeader = req.request.headers.get('Authorization');
      expect(authHeader).toBe(`Bearer ${fullToken}`);
      req.flush({});
    });

    it('should handle tokens with special characters', () => {
      const specialToken = 'token-with-special!@#$%^&*()_+-=[]{}|;:,.<>?characters';
      authServiceMock.getToken.and.returnValue(specialToken);

      httpClient.get(apiUrl).subscribe();

      const req = httpTestingController.expectOne(apiUrl);
      const authHeader = req.request.headers.get('Authorization');
      expect(authHeader).toBe(`Bearer ${specialToken}`);
      req.flush({});
    });
  });

  // Request Cloning Tests
  describe('Request Cloning', () => {
    it('should clone request when adding Authorization header', () => {
      authServiceMock.getToken.and.returnValue('token');

      httpClient.get(apiUrl).subscribe();

      const req = httpTestingController.expectOne(apiUrl);
      // If the request was cloned, the original URL should still be the same
      expect(req.request.url).toBe(apiUrl);
      req.flush({});
    });

    it('should preserve original request properties', () => {
      authServiceMock.getToken.and.returnValue('token');
      const testData = { test: 'value' };

      httpClient.post(apiUrl, testData).subscribe();

      const req = httpTestingController.expectOne(apiUrl);
      expect(req.request.body).toEqual(testData);
      expect(req.request.method).toBe('POST');
      req.flush({});
    });

    it('should not modify request when no token', () => {
      authServiceMock.getToken.and.returnValue(null);

      httpClient.get(apiUrl).subscribe();

      const req = httpTestingController.expectOne(apiUrl);
      expect(req.request.headers.has('Authorization')).toBe(false);
      req.flush({});
    });

    it('should maintain existing headers when adding Authorization', () => {
      authServiceMock.getToken.and.returnValue('token');

      httpClient.get(apiUrl, {
        headers: { 'X-Custom-Header': 'custom-value' }
      }).subscribe();

      const req = httpTestingController.expectOne(apiUrl);
      expect(req.request.headers.get('X-Custom-Header')).toBe('custom-value');
      expect(req.request.headers.has('Authorization')).toBe(true);
      req.flush({});
    });
  });

  // Different HTTP Methods Tests
  describe('HTTP Method Support', () => {
    it('should intercept GET requests', () => {
      authServiceMock.getToken.and.returnValue('token');

      httpClient.get(apiUrl).subscribe();

      const req = httpTestingController.expectOne(apiUrl);
      expect(req.request.method).toBe('GET');
      expect(req.request.headers.has('Authorization')).toBe(true);
      req.flush({});
    });

    it('should intercept POST requests', () => {
      authServiceMock.getToken.and.returnValue('token');

      httpClient.post(apiUrl, {}).subscribe();

      const req = httpTestingController.expectOne(apiUrl);
      expect(req.request.method).toBe('POST');
      expect(req.request.headers.has('Authorization')).toBe(true);
      req.flush({});
    });

    it('should intercept PUT requests', () => {
      authServiceMock.getToken.and.returnValue('token');

      httpClient.put(apiUrl, {}).subscribe();

      const req = httpTestingController.expectOne(apiUrl);
      expect(req.request.method).toBe('PUT');
      expect(req.request.headers.has('Authorization')).toBe(true);
      req.flush({});
    });

    it('should intercept DELETE requests', () => {
      authServiceMock.getToken.and.returnValue('token');

      httpClient.delete(apiUrl).subscribe();

      const req = httpTestingController.expectOne(apiUrl);
      expect(req.request.method).toBe('DELETE');
      expect(req.request.headers.has('Authorization')).toBe(true);
      req.flush({});
    });

    it('should intercept PATCH requests', () => {
      authServiceMock.getToken.and.returnValue('token');

      httpClient.patch(apiUrl, {}).subscribe();

      const req = httpTestingController.expectOne(apiUrl);
      expect(req.request.method).toBe('PATCH');
      expect(req.request.headers.has('Authorization')).toBe(true);
      req.flush({});
    });
  });

  // Multiple Requests Tests
  describe('Multiple Requests', () => {
    it('should handle multiple sequential requests with tokens', () => {
      authServiceMock.getToken.and.returnValue('token-1');

      httpClient.get(apiUrl).subscribe();
      let req = httpTestingController.expectOne(apiUrl);
      expect(req.request.headers.get('Authorization')).toBe('Bearer token-1');
      req.flush({});

      authServiceMock.getToken.and.returnValue('token-2');
      httpClient.get(apiUrl).subscribe();
      req = httpTestingController.expectOne(apiUrl);
      expect(req.request.headers.get('Authorization')).toBe('Bearer token-2');
      req.flush({});
    });

    it('should handle token changes between requests', () => {
      authServiceMock.getToken.and.returnValue('old-token');

      httpClient.get(apiUrl).subscribe();
      let req = httpTestingController.expectOne(apiUrl);
      expect(req.request.headers.get('Authorization')).toBe('Bearer old-token');
      req.flush({});

      authServiceMock.getToken.and.returnValue(null);
      httpClient.get(apiUrl).subscribe();
      req = httpTestingController.expectOne(apiUrl);
      expect(req.request.headers.has('Authorization')).toBe(false);
      req.flush({});
    });

    it('should apply interceptor to all parallel requests', () => {
      authServiceMock.getToken.and.returnValue('shared-token');

      httpClient.get(apiUrl).subscribe();
      httpClient.get(apiUrl + '?id=1').subscribe();
      httpClient.get(apiUrl + '?id=2').subscribe();

      const requests = httpTestingController.match(req => req.url.includes(apiUrl));
      expect(requests.length).toBe(3);
      requests.forEach(req => {
        expect(req.request.headers.get('Authorization')).toBe('Bearer shared-token');
        req.flush({});
      });
    });
  });

  // Request Passthrough Tests
  describe('Request Passthrough', () => {
    it('should pass request to next handler', () => {
      authServiceMock.getToken.and.returnValue('token');

      httpClient.get(apiUrl).subscribe();

      const req = httpTestingController.expectOne(apiUrl);
      expect(req).toBeTruthy();
      req.flush({});
    });

    it('should allow response to flow through interceptor', (done) => {
      authServiceMock.getToken.and.returnValue('token');
      const testResponse = { data: 'test' };

      httpClient.get(apiUrl).subscribe(response => {
        expect(response).toEqual(testResponse);
        done();
      });

      const req = httpTestingController.expectOne(apiUrl);
      req.flush(testResponse);
    });

    it('should handle request errors after interception', (done) => {
      authServiceMock.getToken.and.returnValue('token');

      httpClient.get(apiUrl).subscribe(
        () => fail('should have failed'),
        (error) => {
          expect(error.status).toBe(401);
          done();
        }
      );

      const req = httpTestingController.expectOne(apiUrl);
      req.flush('Unauthorized', { status: 401, statusText: 'Unauthorized' });
    });
  });

  // Edge Cases Tests
  describe('Edge Cases', () => {
    it('should handle very long tokens', () => {
      const longToken = 'x'.repeat(10000);
      authServiceMock.getToken.and.returnValue(longToken);

      httpClient.get(apiUrl).subscribe();

      const req = httpTestingController.expectOne(apiUrl);
      const authHeader = req.request.headers.get('Authorization');
      expect(authHeader).toBe(`Bearer ${longToken}`);
      req.flush({});
    });

    it('should handle unicode characters in token', () => {
      const unicodeToken = 'token-日本語-español';
      authServiceMock.getToken.and.returnValue(unicodeToken);

      httpClient.get(apiUrl).subscribe();

      const req = httpTestingController.expectOne(apiUrl);
      const authHeader = req.request.headers.get('Authorization');
      expect(authHeader).toBe(`Bearer ${unicodeToken}`);
      req.flush({});
    });

    it('should handle whitespace in token', () => {
      // Tokens with whitespace are unusual but should be passed through
      const tokenWithSpace = 'token value';
      authServiceMock.getToken.and.returnValue(tokenWithSpace);

      httpClient.get(apiUrl).subscribe();

      const req = httpTestingController.expectOne(apiUrl);
      const authHeader = req.request.headers.get('Authorization');
      expect(authHeader).toBe(`Bearer ${tokenWithSpace}`);
      req.flush({});
    });

    it('should handle empty request body with token', () => {
      authServiceMock.getToken.and.returnValue('token');

      httpClient.post(apiUrl, null).subscribe();

      const req = httpTestingController.expectOne(apiUrl);
      expect(req.request.headers.has('Authorization')).toBe(true);
      expect(req.request.body).toBeNull();
      req.flush({});
    });

    it('should handle complex request bodies', () => {
      authServiceMock.getToken.and.returnValue('token');
      const complexBody = {
        nested: {
          data: [1, 2, 3],
          object: { key: 'value' }
        }
      };

      httpClient.post(apiUrl, complexBody).subscribe();

      const req = httpTestingController.expectOne(apiUrl);
      expect(req.request.body).toEqual(complexBody);
      expect(req.request.headers.has('Authorization')).toBe(true);
      req.flush({});
    });
  });

  // Integration Tests
  describe('Integration with HttpClient', () => {
    it('should work with HttpClient as registered interceptor', () => {
      authServiceMock.getToken.and.returnValue('token');

      httpClient.get(apiUrl).subscribe();

      const req = httpTestingController.expectOne(apiUrl);
      expect(req.request.headers.has('Authorization')).toBe(true);
      req.flush({});
    });

    it('should not interfere with successful responses', (done) => {
      authServiceMock.getToken.and.returnValue('token');
      const successResponse = { success: true };

      httpClient.get(apiUrl).subscribe(response => {
        expect(response).toEqual(successResponse);
        done();
      });

      const req = httpTestingController.expectOne(apiUrl);
      req.flush(successResponse);
    });
  });
});