// auth.service.ts
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly apiUrl = 'https://localhost:5001/api/auth';

  constructor(private http: HttpClient, private router: Router) {}

  setReturnUrl(returnUrl: any) {
    localStorage.setItem('returnUrl', returnUrl);
  }

  loginWithGoogle() {
    // Redirect to SOCApi Google login endpoint
    window.location.href = `${this.apiUrl}/google-login`;
  }

  logout() {
    // Redirect to SOCApi Google logout endpoint
    window.location.href = `${this.apiUrl}/google-logout`;
  }

  isAuthenticated(): boolean {
    // Check for a JWT token in localStorage (or sessionStorage)
    const token = localStorage.getItem('jwtToken');
    // Optionally, add more checks (e.g., token expiration)
    return !!token;
  }

  isLoggedIn(): boolean {
    return this.isAuthenticated();
  }

  getToken(): string | null {
    return localStorage.getItem('jwtToken');
  }

  getAuthHeaders(): { [header: string]: string } {
    const token = this.getToken();
    return token ? { Authorization: `Bearer ${token}` } : {};
  }
}
