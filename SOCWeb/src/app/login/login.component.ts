import { Component } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
})
export class LoginComponent {
  constructor(
    private readonly authService: AuthService,
    private readonly route: ActivatedRoute
  ) {
    this.route.queryParams.subscribe(params => {
      if (params['token']) {
        this.authService.handleCallback(params['token']);
      }
    });
  }

  loginWithGoogle() {
    this.authService.googleLogin();
  }
}
