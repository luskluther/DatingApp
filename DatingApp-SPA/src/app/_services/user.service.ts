import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/user';

// const httpOptions = {
//    headers: new HttpHeaders( // giving same headers as giving postman post request body headers
//      {
//        'Authorization': 'Bearer ' + localStorage.getItem('token') // we need tto get and send the token while making calls
//      }
//    )
// }; // we do not have to send this because we are alumatically setting jwttoken from interceptor from module tokengetter

@Injectable({
  providedIn: 'root'
})
export class UserService {

  baseUrl = environment.apiUrl; // this is to definite some settings depending on env from a file ( dev or prod )

  constructor(private _http: HttpClient) { }

  getUsers(): Observable<User[]> {
    return this._http.get<User[]>(this.baseUrl + 'users'); // no need to send the httpoptions here
  }

  getUser(id: string): Observable<User> {
    return this._http.get<User>(this.baseUrl + 'users/' + id);
  }

  updateUser(id: number, user: User) {
    return this._http.put(this.baseUrl + 'users/' + id, user);
  }
}
