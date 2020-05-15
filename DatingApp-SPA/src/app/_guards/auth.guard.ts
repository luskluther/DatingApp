import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private _authService: AuthService,
              private _router: Router,
              private _alertify: AlertifyService) {

  }

  // we are just checking here if user is logged in or not
  canActivate(): boolean {
    if (this._authService.loggedIn()) { return true; }

    this._alertify.error('You shall not pass');
    this._router.navigate(['/home']); return false;
  }

}
