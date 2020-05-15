import { Injectable } from '@angular/core';
import * as alertify from 'alertifyjs'; // createad a fiile called typings.d.ts for some fix reagarding this

@Injectable({
  providedIn: 'root'
})
export class AlertifyService { // try to wrap thirp party apps or features inside a serice and then use the service for methods

  constructor() { }

  confirm(message: string, okCallback: () => any) {
    alertify.confirm(message, (e: any) => {
      if (e){
        okCallback();
      } else {}
    });
  }

  success(message: string){
    alertify.success(message);
  }

  error(message: string){
    alertify.error(message);
  }

  warning(message: string){
    alertify.warning(message);
  }

  message(message: string){
    alertify.message(message);
  }
}
