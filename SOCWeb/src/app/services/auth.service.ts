// auth.service.ts
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = 'https://localhost:5001/api/auth';

  constructor(private readonly http: HttpClient) {}

  googleLogin() {
    window.location.href = `${this.apiUrl}/google-login`;
  }

  handleCallback(token: string) {
    localStorage.setItem('token', token);
  }

  isAuthenticated(): boolean {
    return !!localStorage.getItem('token');
  }

  logout() {
    localStorage.removeItem('token');
  }


}