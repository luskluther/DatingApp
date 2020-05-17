import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {};

  constructor(public _authService: AuthService,
              private _alertify: AlertifyService,
              private _router: Router) { }

  ngOnInit() {
  }

  login() {
    this._authService.login(this.model).subscribe(next => { // next option here takes a function what needs to happen next ?
      this._alertify.success('Loggen successfully');
    }, error => {
      this._alertify.error('Loggen failed');
    }, () => { // complete method which is third parameter to subscribe
      this._router.navigate(['/members']);
    });
  }

  loggedIn() {
    return this._authService.loggedIn();
  }

  logOut() {
    localStorage.removeItem('token');
    this._alertify.success('Successfully logged out');
    this._router.navigate(['/home']);
  }
}
