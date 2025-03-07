import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';

@Component({
  selector: 'app-registration-form',
  standalone: false,
  templateUrl: './registration-form.component.html',
  styleUrl: './registration-form.component.css'
})
export class RegistrationFormComponent {

username: string = '';
email: string = '';
password: string = '';
confirmPassword: string = '';

constructor(private http: HttpClient) {}

  register(): void {
    if (this.password !== this.confirmPassword) {
      alert('Passwords do not match');
      return;
    };

  const user = {
    username: this.username,
    email: this.email,
    password: this.password
  }

  this.http.post('http://localhost:3000/register', user)
    .subscribe((response) => {
      alert('User registered successfully');
    }, (error) => {
      console.error('Error registering user', error);
    });
  }
}
