import { Component, OnInit } from '@angular/core';
import { AuthService } from './_services/auth.service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { User } from './_models/user';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  private _jwtHelper = new JwtHelperService();

  constructor(private _authService: AuthService) {

  }

  ngOnInit(): void {
    // getting token and current user for the app local use
    const token = localStorage.getItem('token');
    const user: User = JSON.parse(localStorage.getItem('user'));
    if (token) {
      // so if this is done we can protect the token from escaiping from the local storage so after user logs in and regreshes
       // the token still perists in the component itself
      this._authService.decodedToken = this._jwtHelper.decodeToken(token);
    }
    if (user) {
      this._authService.currentUser = user;
      this._authService.changeMemberPhoto(user.photoUrl); // will update current photo in auth service with cureent user dp
    }
  }

}
