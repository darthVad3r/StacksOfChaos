import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { Router } from '@angular/router';
import { AuthService } from './auth.service';

/**
 * Comprehensive unit tests for AuthService.
 * Tests authentication, token management, and HTTP communication.
 * Follows SOLID, DRY, and clean code principles.
 */
describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;
  let routerMock: jasmine.SpyObj<Router>;
  const apiUrl = 'https://localhost:5001/api/auth';

  beforeEach(() => {
    const routerSpy = jasmine.createSpyObj('Router', ['navigate']);

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        AuthService,
        { provide: Router, useValue: routerSpy }
      ]
    });

    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
    routerMock = TestBed.inject(Router) as jasmine.SpyObj<Router>;
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  describe('signIn', () => {
    it('should send POST request with correct credentials', () => {
      const email = 'test@example.com';
      const password = 'Password123!';
      const mockResponse = { token: 'jwt-token-12345' };

      service.signIn(email, password).subscribe(response => {
        expect(response.token).toBe('jwt-token-12345');
      });

      const req = httpMock.expectOne(`${apiUrl}/login`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual({ email, password });
      req.flush(mockResponse);
    });

    it('should handle login errors with 401 status', () => {
      service.signIn('invalid@example.com', 'wrong').subscribe({
        error: (error) => {
          expect(error.status).toBe(401);
        }
      });

      const req = httpMock.expectOne(`${apiUrl}/login`);
      req.flush('Unauthorized', { status: 401, statusText: 'Unauthorized' });
    });

    it('should accept special characters in email', () => {
      const email = 'user+tag@example.co.uk';
      const password = 'Pass123!';

      service.signIn(email, password).subscribe();

      const req = httpMock.expectOne(`${apiUrl}/login`);
      expect(req.request.body.email).toBe(email);
      req.flush({ token: 'token' });
    });
  });

  describe('Token Management', () => {
    it('should retrieve token from localStorage', () => {
      const token = 'jwt-token-12345';
      localStorage.setItem('jwtToken', token);

      expect(service.getToken()).toBe(token);
    });

    it('should return null when no token exists', () => {
      localStorage.removeItem('jwtToken');
      expect(service.getToken()).toBeNull();
    });

    it('should get auth headers with valid token', () => {
      const token = 'jwt-token-12345';
      localStorage.setItem('jwtToken', token);

      const headers = service.getAuthHeaders();
      expect(headers['Authorization']).toBe(`Bearer ${token}`);
    });

    it('should return empty headers when no token exists', () => {
      localStorage.removeItem('jwtToken');

      const headers = service.getAuthHeaders();
      expect(headers).toEqual({});
    });

    it('should format auth headers with Bearer scheme', () => {
      localStorage.setItem('jwtToken', 'token123');

      const headers = service.getAuthHeaders();
      expect(headers['Authorization']).toContain('Bearer ');
    });
  });

  describe('Authentication Status', () => {
    it('should return true when authenticated', () => {
      localStorage.setItem('jwtToken', 'valid-token');
      expect(service.isAuthenticated()).toBe(true);
    });

    it('should return false when not authenticated', () => {
      localStorage.removeItem('jwtToken');
      expect(service.isAuthenticated()).toBe(false);
    });

    it('isLoggedIn should match isAuthenticated', () => {
      localStorage.setItem('jwtToken', 'token');
      expect(service.isLoggedIn()).toBe(service.isAuthenticated());
    });
  });

  describe('Return URL Management', () => {
    it('should store return URL in localStorage', () => {
      const returnUrl = '/dashboard';
      service.setReturnUrl(returnUrl);

      expect(localStorage.getItem('returnUrl')).toBe(returnUrl);
    });

    it('should overwrite existing return URL', () => {
      service.setReturnUrl('/first');
      service.setReturnUrl('/second');

      expect(localStorage.getItem('returnUrl')).toBe('/second');
    });
  });

  describe('Google Authentication', () => {
    it('should redirect to Google login endpoint', () => {
      const originalLocation = window.location.href;
      
      // Create a setter spy to track window.location.href assignments
      Object.defineProperty(window, 'location', {
        writable: true,
        value: { href: '' }
      });
      
      service.loginWithGoogle();
      
      expect(window.location.href).toBe(`${apiUrl}/google-login`);
      
      // Restore original location
      Object.defineProperty(window, 'location', {
        writable: true,
        value: { href: originalLocation }
      });
    });

    it('should redirect to Google logout endpoint', () => {
      const originalLocation = window.location.href;
      
      // Create a setter spy to track window.location.href assignments
      Object.defineProperty(window, 'location', {
        writable: true,
        value: { href: '' }
      });
      
      service.logout();
      
      expect(window.location.href).toBe(`${apiUrl}/google-logout`);
      
      // Restore original location
      Object.defineProperty(window, 'location', {
        writable: true,
        value: { href: originalLocation }
      });
    });
  });
});

