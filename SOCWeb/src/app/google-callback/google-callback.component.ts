import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-google-callback',
  template: '<p>Signing you in with Google...</p>'
})
export class GoogleCallbackComponent implements OnInit {
  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit(): void {
    // Extract access_token from URL fragment or query string
    let token: string | null = null;
    if (window.location.hash && window.location.hash.includes('access_token=')) {
      // Fragment: #access_token=...
      token = window.location.hash.split('access_token=')[1]?.split('&')[0] || null;
    } else if (window.location.search && window.location.search.includes('access_token=')) {
      // Query: ?access_token=...
      token = window.location.search.split('access_token=')[1]?.split('&')[0] || null;
    }
    if (token) {
      this.authService.handleCallback(token);
    } else {
      // Optionally, handle error or redirect
      this.router.navigate(['/login']);
    }
  }
}
