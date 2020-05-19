import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { BehaviorSubject } from 'rxjs';

@Injectable({ // this allows us to inject thing into our service , services are not auto injectable
  providedIn: 'root' // this tells our service and any components that is using thisservice what is providing this serivce.
})
export class AuthService {

  // hardcoding base url can avoid this in real cases.
  baseUrl = environment.apiUrl + 'auth/'; // this is to definite some settings depending on env from a file ( dev or prod )
  private _jwtHelper = new JwtHelperService();
  decodedToken: any;
  currentUser: User;
  photoUrl = new BehaviorSubject<string>('../../assets/user.png');
  currentPhotoUrl = this.photoUrl.asObservable();

  constructor(private _http: HttpClient) { }

  login(model: any) {
    return this._http.post(this.baseUrl + 'login', model) // this gets us the token from the server
                                                            // which will come down from the server as response
      .pipe(
        map((response: any) => {
          const user = response;
          if (user) {
            localStorage.setItem('token', user.token);
            localStorage.setItem('user', JSON.stringify(user.user));
            this.decodedToken = this._jwtHelper.decodeToken(user.token);
            this.currentUser = user.user;
            this.changeMemberPhoto(this.currentUser.photoUrl);
          }
        })
      ); // this allows us to chain rxjs operators to our request ( for example we are getting the token which we have to something )
              // these rxjs operators they do similar js funcitonlaity but with ovservables
  }

  register(user: User) {
    return this._http.post(this.baseUrl + 'register', user);
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    return !this._jwtHelper.isTokenExpired(token); // expired it will return false not expired true.
  }

  changeMemberPhoto(photoUrl: string) {
    this.photoUrl.next(photoUrl);
  }
}
