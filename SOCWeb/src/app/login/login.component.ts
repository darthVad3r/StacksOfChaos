import { Component } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  standalone: true,
  imports: [CommonModule, FormsModule, MatButtonModule]
})
export class LoginComponent {
  model = { email: '', password: '' };

  constructor(private readonly authService: AuthService) { }

  /**
   * Handles the form submission for user login.
   * Calls the AuthService to authenticate the user with the provided email and password.
   */
  ngOnInit() {
    // Any initialization logic can go here
  }
  /**
   * Submits the login form.
   * Calls the AuthService to sign in with the provided email and password.
   */
  onSubmit() {
    this.authService.signIn(this.model.email, this.model.password).subscribe({
      next: (response) => {
        // Handle successful login, e.g., store token and redirect
        console.log('Login successful', response.token);
      },
      error: (error) => {
        // Handle login error
        console.error('Login failed', error);
      }
    });
  }
}
