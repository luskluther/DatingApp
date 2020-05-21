import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { PaginatedResult } from '../_models/pagination';
import { map } from 'rxjs/operators';
import { Message } from '../_models/message';

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

  getUsers(page?, itemsPerPage?, userParams?, likesParam?): Observable<PaginatedResult<User[]>> {
    // paginationd result
    const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();

    // creating params for the call
    let params = new HttpParams();

    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    if (userParams != null) {
      params = params.append('minAge', userParams.minAge);
      params = params.append('maxAge', userParams.maxAge);
      params = params.append('gender', userParams.gender);
      params = params.append('orderBy', userParams.orderBy);
    }

    if (likesParam === 'Likers') {
      params = params.append('likers', 'true');
    }

    if (likesParam === 'Likees') {
      params = params.append('likees', 'true');
    }

    return this._http.get<User[]>(this.baseUrl + 'users', { observe: 'response', params })
    .pipe(map(response => {
      paginatedResult.result = response.body;
      if (response.headers.get('Pagination') != null) {
        paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
      }
      return paginatedResult;
    })); // no need to send the httpoptions here
  }

  getUser(id: string): Observable<User> {
    return this._http.get<User>(this.baseUrl + 'users/' + id);
  }

  updateUser(id: number, user: User) {
    return this._http.put(this.baseUrl + 'users/' + id, user);
  }

  setMainPhoto(userId: number, id: number) {
    return this._http.post(this.baseUrl + 'users/' + userId + '/photos/' + id + '/setMain', {});
  }

  deletePhoto(userId: number, id: number) {
    return this._http.delete(this.baseUrl + 'users/' + userId + '/photos/' + id);
  }

  sendLike(userId: number, recepientId: number) {
    return this._http.post(this.baseUrl + 'users/' + userId + '/like/' + recepientId, {});
  }

  getMessages(userId: number, page?, itemsPerPage?, messageContainer?) {
    const paginatedResult: PaginatedResult<Message[]> = new PaginatedResult<Message[]>();

    let params = new HttpParams();

    params = params.append('messageContainer', messageContainer);

    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    return this._http.get<Message[]>(this.baseUrl + 'users/' + userId + '/messages', { observe: 'response', params })
      .pipe(map(response => {
          paginatedResult.result = response.body;
          if (response.headers.get('Pagination') != null) {
            paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
          }
          return paginatedResult;
      })
    ); // no need to send the httpoptions here
  }

  getMessageThread(userId: number, recipientId: number) {
    return this._http.get<Message[]>(this.baseUrl + 'users/' + userId + '/messages/' + 'thread/' + recipientId);
  }

  sendMessage(userId: number, message: Message) {
    return this._http.post(this.baseUrl + 'users/' + userId + '/messages', message);
  }
}
