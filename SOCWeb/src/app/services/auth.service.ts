// auth.service.ts
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = 'https://localhost:5001/api/auth';

  constructor(private readonly http: HttpClient) { }

  login(username: string, password: string) {
    return this.http.post(`${this.apiUrl}/login`, { username, password });
  }

  logout() {
    localStorage.removeItem('token');
  }

  isAuthenticated() {
    const token = localStorage.getItem('token');
    return !!token;
  }

  googleLogin() {
    window.location.href = `${this.apiUrl}/google-login`;
  }

  handleCallback(token: string) {
    localStorage.setItem('token', token);
  }
}
