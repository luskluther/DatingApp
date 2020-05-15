import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpErrorResponse, HTTP_INTERCEPTORS } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    // intercepting amd catching an error and what needs to be done.
    intercept(req: import('@angular/common/http').HttpRequest<any>,
              next: import('@angular/common/http').HttpHandler): import('rxjs').Observable<import('@angular/common/http').HttpEvent<any>> {
        return next.handle(req).pipe(
            catchError(error => { // this error is the httperrorreposne that we get from the server that we are trying to handle
                if (error.status === 401) {
                    return throwError(error.statusText);
                }
                if (error instanceof HttpErrorResponse) { // takes care of 500 type errors
                    const applicationError = error.headers.get('Application-Error');
                    if (applicationError) {
                        return throwError(applicationError);
                    }
                    const serverError = error.error;
                    let modelStateErrors = ''; // these erorrs are typically password is required type which is set on the model
                    // condition inside API
                    // Example of such an error
                    //   error:
                    //     errors:
                    //         Password: ["You must specify password between 4-8 characters"]
                    //         UserName: ["The UserName field is required."]
                    //         __proto__: Object
                    //     status: 400
                    if (serverError.erorrs && typeof serverError.error === 'object') {
                        for (const key in serverError.errors) {
                            if (serverError.error[key]) {
                                modelStateErrors += serverError.errors[key] + '\n'; // apprending all modalstateerrors;
                            }
                        }
                    }
                    return throwError(modelStateErrors || serverError || 'Server Error'); // worst case we are sending back server error
                }
            })
        );
    }
}

// exporting the above class and including in module import in this way
// interceptor is of type http_...
// interceptor class is errorinter..
// anything can have multiple interceptors so multi:true
export const ErrorInterceptorProvider = {
    provide: HTTP_INTERCEPTORS,
    useClass : ErrorInterceptor,
    multi: true
};
