import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {};

  constructor(private _authService: AuthService) { }

  ngOnInit() {
  }

  login() {
    this._authService.login(this.model).subscribe(next => {
      console.log('loggen in successfully');
    }, error => {
      console.log('loggen failed');
    });
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    return !!token; // '!!' this is short hand for : if there is something in token it will return true else if its empty it will return false
  }

  logOut() {
    localStorage.removeItem('token');
    console.log('logged eout');
  }

}
