import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DashboardComponent } from './dashboard.component';
import { JwtHelperService } from '@auth0/angular-jwt';

describe('DashboardComponent', () => {
  let component: DashboardComponent;
  let fixture: ComponentFixture<DashboardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DashboardComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(DashboardComponent);
    component = fixture.componentInstance;
    
    // Clear localStorage before each test
    localStorage.clear();
  });

  afterEach(() => {
    // Clean up localStorage after each test
    localStorage.clear();
  });

  // Component Instantiation Tests
  describe('Component Creation', () => {
    it('should create the component with initial null state', () => {
      expect(component).toBeTruthy();
      expect(component.userName).toBeNull();
      expect(component.userEmail).toBeNull();
    });

    it('should have ngOnInit lifecycle hook', () => {
      expect(typeof component.ngOnInit).toBe('function');
    });
  });

  // JWT Token Decoding Tests
  describe('JWT Token Decoding', () => {
    it('should decode JWT token from localStorage', () => {
      const mockToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiSm9obiBEb2UiLCJlbWFpbCI6ImpvaG5AZXhhbXBsZS5jb20ifQ.signature';
      localStorage.setItem('jwtToken', mockToken);
      spyOn(console, 'log');

      component.ngOnInit();

      expect(console.log).toHaveBeenCalledWith('Decoded JWT Token:', jasmine.any(Object));
    });

    it('should extract name from decoded token', () => {
      const mockToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiSm9obiBEb2UiLCJlbWFpbCI6ImpvaG5AZXhhbXBsZS5jb20ifQ.signature';
      localStorage.setItem('jwtToken', mockToken);

      component.ngOnInit();

      expect(component.userName).toBe('John Doe');
    });

    it('should extract email from decoded token', () => {
      const mockToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiSm9obiBEb2UiLCJlbWFpbCI6ImpvaG5AZXhhbXBsZS5jb20ifQ.signature';
      localStorage.setItem('jwtToken', mockToken);

      component.ngOnInit();

      expect(component.userEmail).toBe('john@example.com');
    });
  });

  // localStorage Tests
  describe('localStorage Token Handling', () => {
    it('should look for jwtToken in localStorage', () => {
      const mockToken = 'test-token-123';
      localStorage.setItem('jwtToken', mockToken);
      spyOn(window.localStorage, 'getItem').and.callThrough();

      component.ngOnInit();

      expect(window.localStorage.getItem).toHaveBeenCalledWith('jwtToken');
    });

    it('should handle null token in localStorage', () => {
      spyOn(console, 'error');
      localStorage.clear();

      component.ngOnInit();

      expect(console.error).toHaveBeenCalledWith('No JWT token found in localStorage');
    });

    it('should handle null token from localStorage', () => {
      spyOn(console, 'error');
      localStorage.removeItem('jwtToken');

      component.ngOnInit();

      expect(console.error).toHaveBeenCalledWith('No JWT token found in localStorage');
    });

    it('should handle empty string token', () => {
      spyOn(console, 'error');
      localStorage.setItem('jwtToken', '');

      component.ngOnInit();

      // Empty string is falsy, should trigger error
      expect(console.error).toHaveBeenCalledWith('No JWT token found in localStorage');
    });
  });

  // User Information Display Tests
  describe('User Information Display', () => {
    it('should set userName when token contains name claim', () => {
      const mockToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiQWxpY2UgU21pdGgiLCJlbWFpbCI6ImFsaWNlQGV4YW1wbGUuY29tIn0.signature';
      localStorage.setItem('jwtToken', mockToken);
      
      component.ngOnInit();
      
      expect(component.userName).toBe('Alice Smith');
    });

    it('should set userEmail when token contains email claim', () => {
      const mockToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiQWxpY2UgU21pdGgiLCJlbWFpbCI6ImFsaWNlQGV4YW1wbGUuY29tIn0.signature';
      localStorage.setItem('jwtToken', mockToken);
      
      component.ngOnInit();
      
      expect(component.userEmail).toBe('alice@example.com');
    });

    it('should handle missing name claim in token', () => {
      const mockToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6ImFsaWNlQGV4YW1wbGUuY29tIn0.signature';
      localStorage.setItem('jwtToken', mockToken);
      
      component.ngOnInit();
      
      expect(component.userName).toBeUndefined();
    });

    it('should handle missing email claim in token', () => {
      const mockToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiQWxpY2UgU21pdGgifQ.signature';
      localStorage.setItem('jwtToken', mockToken);
      
      component.ngOnInit();
      
      expect(component.userEmail).toBeUndefined();
    });

    it('should store multiple user properties from token', () => {
      const mockToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiQm9iIEpvaG5zb24iLCJlbWFpbCI6ImJvYkBleGFtcGxlLmNvbSJ9.signature';
      localStorage.setItem('jwtToken', mockToken);
      
      component.ngOnInit();
      
      expect(component.userName).toBe('Bob Johnson');
      expect(component.userEmail).toBe('bob@example.com');
    });
  });

  // Component Lifecycle Tests
  describe('Component Lifecycle', () => {
    it('should initialize component properties from JWT token on ngOnInit', () => {
      const mockToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiSm9obiBEb2UiLCJlbWFpbCI6ImpvaG5AZXhhbXBsZS5jb20ifQ.signature';
      localStorage.setItem('jwtToken', mockToken);

      component.ngOnInit();

      expect(component.userName).toBe('John Doe');
      expect(component.userEmail).toBe('john@example.com');
    });

    it('should log decoded token when token is present', () => {
      const mockToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiSm9obiBEb2UiLCJlbWFpbCI6ImpvaG5AZXhhbXBsZS5jb20ifQ.signature';
      localStorage.setItem('jwtToken', mockToken);
      spyOn(console, 'log');

      component.ngOnInit();

      expect(console.log).toHaveBeenCalledWith('Decoded JWT Token:', jasmine.any(Object));
    });

    it('should log error when token is missing during initialization', () => {
      localStorage.clear();
      spyOn(console, 'error');

      component.ngOnInit();

      expect(console.error).toHaveBeenCalledWith('No JWT token found in localStorage');
    });
  });

  // Helper function to create a valid JWT token with custom payload
  function createMockJwt(payload: any): string {
    const header = btoa(JSON.stringify({ alg: 'HS256', typ: 'JWT' }));
    const encodedPayload = btoa(JSON.stringify(payload));
    return `${header}.${encodedPayload}.mock-signature`;
  }

  // Edge Cases Tests
  describe('Edge Cases', () => {
    it('should handle token with special characters in claims', () => {
      const mockToken = createMockJwt({
        name: 'John O\'Doe-Smith',
        email: 'john+special@example.co.uk'
      });
      localStorage.setItem('jwtToken', mockToken);
      
      component.ngOnInit();
      
      expect(component.userName).toBe('John O\'Doe-Smith');
      expect(component.userEmail).toBe('john+special@example.co.uk');
    });

    it('should handle very long email addresses', () => {
      const longEmail = 'a'.repeat(100) + '@example.com';
      const mockToken = createMockJwt({
        name: 'John',
        email: longEmail
      });
      localStorage.setItem('jwtToken', mockToken);
      
      component.ngOnInit();
      
      expect(component.userEmail).toBe(longEmail);
    });

    it('should handle very long user names', () => {
      const longName = 'John Doe ' + 'the Great '.repeat(20);
      const mockToken = createMockJwt({
        name: longName,
        email: 'john@example.com'
      });
      localStorage.setItem('jwtToken', mockToken);
      
      component.ngOnInit();
      
      expect(component.userName).toBe(longName);
    });

    it('should handle unicode characters in user name', () => {
      const mockToken = createMockJwt({
        name: 'José María García',
        email: 'jose@example.com'
      });
      localStorage.setItem('jwtToken', mockToken);
      
      component.ngOnInit();
      
      expect(component.userName).toBe('José María García');
    });

    it('should handle unicode characters in email', () => {
      const mockToken = createMockJwt({
        name: 'Jose',
        email: 'josé@example.com'
      });
      localStorage.setItem('jwtToken', mockToken);
      
      component.ngOnInit();
      
      expect(component.userEmail).toBe('josé@example.com');
    });

    it('should handle whitespace in user information', () => {
      const mockToken = createMockJwt({
        name: '  John Doe  ',
        email: '  john@example.com  '
      });
      localStorage.setItem('jwtToken', mockToken);
      
      component.ngOnInit();
      
      expect(component.userName).toBe('  John Doe  ');
      expect(component.userEmail).toBe('  john@example.com  ');
    });
  });

  // Integration Tests
  describe('Integration with JwtHelperService', () => {
    it('should decode and use token from JwtHelperService in ngOnInit', () => {
      const mockToken = createMockJwt({
        name: 'Integration Test User',
        email: 'integration@example.com'
      });
      localStorage.setItem('jwtToken', mockToken);
      spyOn(console, 'log');

      component.ngOnInit();

      // Verify the component decoded and used the token correctly
      expect(console.log).toHaveBeenCalledWith('Decoded JWT Token:', jasmine.any(Object));
      expect(component.userName).toBe('Integration Test User');
      expect(component.userEmail).toBe('integration@example.com');
    });

    it('should handle decoding errors gracefully when token is invalid', () => {
      const invalidToken = 'invalid.token.format';
      localStorage.setItem('jwtToken', invalidToken);
      spyOn(console, 'log');

      component.ngOnInit();

      // Component should still attempt to decode (JwtHelperService handles it)
      expect(console.log).toHaveBeenCalledWith('Decoded JWT Token:', jasmine.any(Object));
    });

    it('should log decoded token details during ngOnInit', () => {
      const mockToken = createMockJwt({
        name: 'Test User',
        email: 'test@example.com',
        role: 'user'
      });
      localStorage.setItem('jwtToken', mockToken);
      spyOn(console, 'log');

      component.ngOnInit();

      // Verify component logs the decoded token for debugging
      expect(console.log).toHaveBeenCalledWith('Decoded JWT Token:', jasmine.any(Object));
    });
  });

  // Re-initialization Scenarios
  describe('Re-initialization Scenarios', () => {
    it('should update properties when ngOnInit is called with new token', () => {
      const mockToken1 = createMockJwt({ name: 'First User', email: 'first@example.com' });
      localStorage.setItem('jwtToken', mockToken1);
      component.ngOnInit();
      
      const mockToken2 = createMockJwt({ name: 'Second User', email: 'second@example.com' });
      localStorage.setItem('jwtToken', mockToken2);
      component.ngOnInit();
      
      expect(component.userName).toBe('Second User');
      expect(component.userEmail).toBe('second@example.com');
    });

    it('should clear properties when token is removed and ngOnInit is called again', () => {
      const mockToken = createMockJwt({ name: 'John Doe', email: 'john@example.com' });
      localStorage.setItem('jwtToken', mockToken);
      component.ngOnInit();
      expect(component.userName).toBe('John Doe');
      
      localStorage.removeItem('jwtToken');
      component.ngOnInit();
      
      expect(component.userName).toBeNull();
      expect(component.userEmail).toBeNull();
    });
  });
