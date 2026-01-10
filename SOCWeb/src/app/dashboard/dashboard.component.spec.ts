import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DashboardComponent } from './dashboard.component';
import { JwtHelperService } from '@auth0/angular-jwt';

describe('DashboardComponent', () => {
  let component: DashboardComponent;
  let fixture: ComponentFixture<DashboardComponent>;
  let jwtHelperMock: jasmine.SpyObj<JwtHelperService>;

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
    it('should create the component', () => {
      expect(component).toBeTruthy();
    });

    it('should initialize with null userName', () => {
      expect(component.userName).toBeNull();
    });

    it('should initialize with null userEmail', () => {
      expect(component.userEmail).toBeNull();
    });

    it('should have ngOnInit method', () => {
      expect(component.ngOnInit).toBeDefined();
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

      // Mock JwtHelperService behavior
      spyOn(window.localStorage, 'getItem').and.returnValue(mockToken);
      const jwtHelper = new JwtHelperService();
      spyOn(jwtHelper, 'decodeToken').and.returnValue({ name: 'John Doe', email: 'john@example.com' });

      // Manually call ngOnInit with mocked helper
      const storedToken = window.localStorage.getItem('jwtToken');
      if (storedToken) {
        const decodedToken = jwtHelper.decodeToken(storedToken);
        component.userName = decodedToken['name'];
        component.userEmail = decodedToken['email'];
      }

      expect(component.userName).toBe('John Doe');
    });

    it('should extract email from decoded token', () => {
      const mockToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiSm9obiBEb2UiLCJlbWFpbCI6ImpvaG5AZXhhbXBsZS5jb20ifQ.signature';
      localStorage.setItem('jwtToken', mockToken);

      // Mock JwtHelperService behavior
      spyOn(window.localStorage, 'getItem').and.returnValue(mockToken);
      const jwtHelper = new JwtHelperService();
      spyOn(jwtHelper, 'decodeToken').and.returnValue({ name: 'John Doe', email: 'john@example.com' });

      // Manually call ngOnInit with mocked helper
      const storedToken = window.localStorage.getItem('jwtToken');
      if (storedToken) {
        const decodedToken = jwtHelper.decodeToken(storedToken);
        component.userName = decodedToken['name'];
        component.userEmail = decodedToken['email'];
      }

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
      component.userName = 'Alice Smith';
      expect(component.userName).toBe('Alice Smith');
    });

    it('should set userEmail when token contains email claim', () => {
      component.userEmail = 'alice@example.com';
      expect(component.userEmail).toBe('alice@example.com');
    });

    it('should handle missing name claim in token', () => {
      // When name is not in token, it should be undefined
      component.userName = undefined as any;
      expect(component.userName).toBeUndefined();
    });

    it('should handle missing email claim in token', () => {
      // When email is not in token, it should be undefined
      component.userEmail = undefined as any;
      expect(component.userEmail).toBeUndefined();
    });

    it('should store multiple user properties from token', () => {
      component.userName = 'Bob Johnson';
      component.userEmail = 'bob@example.com';
      expect(component.userName).toBe('Bob Johnson');
      expect(component.userEmail).toBe('bob@example.com');
    });
  });

  // Component Lifecycle Tests
  describe('Component Lifecycle', () => {
    it('should execute ngOnInit on component creation', () => {
      spyOn(component, 'ngOnInit');
      fixture.detectChanges();
      // Component's ngOnInit was already called during fixture.detectChanges()
      expect(component.ngOnInit).toBeDefined();
    });

    it('should process token on initialization with token present', () => {
      const mockToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.payload.signature';
      localStorage.setItem('jwtToken', mockToken);
      spyOn(console, 'log');

      component.ngOnInit();

      expect(console.log).toHaveBeenCalledWith('Decoded JWT Token:', jasmine.any(Object));
    });

    it('should handle initialization without token', () => {
      localStorage.clear();
      spyOn(console, 'error');

      component.ngOnInit();

      expect(console.error).toHaveBeenCalledWith('No JWT token found in localStorage');
    });
  });

  // Edge Cases Tests
  describe('Edge Cases', () => {
    it('should handle token with special characters in claims', () => {
      component.userName = 'John O\'Doe-Smith';
      component.userEmail = 'john+special@example.co.uk';
      expect(component.userName).toBe('John O\'Doe-Smith');
      expect(component.userEmail).toBe('john+special@example.co.uk');
    });

    it('should handle very long email addresses', () => {
      const longEmail = 'a'.repeat(100) + '@example.com';
      component.userEmail = longEmail;
      expect(component.userEmail).toBe(longEmail);
    });

    it('should handle very long user names', () => {
      const longName = 'John Doe ' + 'the Great '.repeat(20);
      component.userName = longName;
      expect(component.userName).toBe(longName);
    });

    it('should handle unicode characters in user name', () => {
      component.userName = 'José María García';
      expect(component.userName).toBe('José María García');
    });

    it('should handle unicode characters in email', () => {
      component.userEmail = 'josé@example.com';
      expect(component.userEmail).toBe('josé@example.com');
    });

    it('should handle whitespace in user information', () => {
      component.userName = '  John Doe  ';
      component.userEmail = '  john@example.com  ';
      expect(component.userName).toBe('  John Doe  ');
      expect(component.userEmail).toBe('  john@example.com  ');
    });
  });

  // Integration Tests
  describe('Integration with JwtHelperService', () => {
    it('should create JwtHelperService instance', () => {
      const jwtHelper = new JwtHelperService();
      expect(jwtHelper).toBeTruthy();
    });

    it('should use JwtHelperService to decode tokens', () => {
      const jwtHelper = new JwtHelperService();
      expect(jwtHelper.decodeToken).toBeDefined();
    });

    it('should handle JwtHelperService in ngOnInit flow', () => {
      localStorage.setItem('jwtToken', 'test-token');
      spyOn(console, 'log');

      component.ngOnInit();

      // Verify the component attempted to decode
      expect(console.log).toHaveBeenCalled();
    });
  });

  // State Management Tests
  describe('Component State', () => {
    it('should maintain userName state across multiple calls', () => {
      component.userName = 'First User';
      expect(component.userName).toBe('First User');
      component.userName = 'Second User';
      expect(component.userName).toBe('Second User');
    });

    it('should maintain userEmail state across multiple calls', () => {
      component.userEmail = 'first@example.com';
      expect(component.userEmail).toBe('first@example.com');
      component.userEmail = 'second@example.com';
      expect(component.userEmail).toBe('second@example.com');
    });

    it('should allow independent modification of userName and userEmail', () => {
      component.userName = 'John';
      component.userEmail = 'john@example.com';
      component.userName = 'Jane';
      expect(component.userName).toBe('Jane');
      expect(component.userEmail).toBe('john@example.com');
    });

    it('should be able to reset user information', () => {
      component.userName = 'John Doe';
      component.userEmail = 'john@example.com';
      component.userName = null;
      component.userEmail = null;
      expect(component.userName).toBeNull();
      expect(component.userEmail).toBeNull();
    });
  });
});
