import { Injectable } from '@angular/core';
import { User } from '../_models/user';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
export class MemberDetailResolver implements Resolve<User> {

    constructor(private _userService: UserService, private _router: Router, private _alertify: AlertifyService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<User> {
        return this._userService.getUser(route.params['id']).pipe(
            catchError(error => {
                this._alertify.error('Problem retrieving data');
                this._router.navigate(['/members']);
                return of(null); // just return null since error ,of is a tyle of observable
            })
        );
    }
}
