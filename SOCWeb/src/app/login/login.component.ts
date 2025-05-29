import { Component } from '@angular/core';
import { Router } from '@angular/router'; 
import { AuthService } from '../services/auth.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  constructor(
    private readonly router: Router,
  ) {
  }

  loginWithGoogle() {
    window.location.href = 'https://localhost:52454/api/auth/google-login'; 
  }

}
