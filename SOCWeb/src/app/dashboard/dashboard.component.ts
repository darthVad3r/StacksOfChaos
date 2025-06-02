import { Component, OnInit } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';

@Component({
  selector: 'app-dashboard',
  styleUrls: ['./dashboard.component.css'],
  standalone: false,
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit {
  userName: string | null = null;
  userEmail: string | null = null;

  ngOnInit() {
    const jwtHelper = new JwtHelperService();
    const storedToken = localStorage.getItem('jwtToken');

    if (storedToken) {
      const decodedToken = jwtHelper.decodeToken(storedToken);
      console.log('Decoded JWT Token:', decodedToken);
      this.userName = decodedToken['name'];
      this.userEmail = decodedToken['email'];
    }
    else {
      console.error('No JWT token found in localStorage');
    }
  }
}