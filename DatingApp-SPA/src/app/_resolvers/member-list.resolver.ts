import { Injectable } from '@angular/core';
import { User } from '../_models/user';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

// what this resolver ios doing is , the moment we open a route ( check route.ts file) it will identify what route we are in
// then it will try to check the routes file this route is resolved how ?
// then it will see what is the complmemnt that is userd to resolve and what object/data is used as a resolver data , example user.users
// then that particular component is initialized , the system understands what route it is
// and from the resolver like below returns the data everytime that route is hit in format required.
// in the component just we subscribe to the active route and since that data is already provided we dont need to make call to get
    // user or users everytime we are going to that router with separate get calls , route takes care of it
// by doing all this we would ahve already gotten the data before we activeate or hit a routle to which we can just subscribe
@Injectable()
export class MemberListResolver implements Resolve<User[]> {

    constructor(private _userService: UserService, private _router: Router, private _alertify: AlertifyService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<User[]> {
        return this._userService.getUsers().pipe(
            catchError(error => {
                this._alertify.error('Problem retrieving data');
                this._router.navigate(['/home']);
                return of(null); // just return null since error ,of is a tyle of observable
            })
        );
    }
}
