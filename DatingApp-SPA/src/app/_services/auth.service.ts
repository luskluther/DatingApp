import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map} from 'rxjs/operators';

@Injectable({ // this allows us to inject thing into our service , services are not auto injectable
  providedIn: 'root' // this tells our service and any components that is using thisservice what is providing this serivce.
})
export class AuthService {

    // hardcoding base url can avoid this in real cases.
  baseUrl = 'http://localhost:5000/api/auth/';

  constructor(private _http: HttpClient) { }

  login(model: any) {
    return this._http.post(this.baseUrl + 'login', model) // this gets us the token from the server
                                                            // which will come down from the server as response
      .pipe(
        map((response: any) => {
          const user = response;
          if (user) {
            localStorage.setItem('token', user.token);
          }
        })
      ); // this allows us to chain rxjs operators to our request ( for example we are getting the token which we have to something )
              // these rxjs operators they do similar js funcitonlaity but with ovservables
  }

  register(model: any){
    return this._http.post(this.baseUrl + 'register', model);
  }

}
