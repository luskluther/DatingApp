import { Component, OnInit } from '@angular/core';
import { Pagination, PaginatedResult } from '../_models/pagination';
import { Message } from '../_models/message';
import { AuthService } from '../_services/auth.service';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { UserService } from '../_services/user.service';
import { error } from 'protractor';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {

  messages: Message[];
  pagination: Pagination;
  messageContainer = 'Unread';
  constructor(private _user: UserService, private _auth: AuthService, private _route: ActivatedRoute, private _alert: AlertifyService) { }

  ngOnInit() {
    this._route.data.subscribe(data => {
      this.messages = data['messages'].result;
      this.pagination = data['messages'].pagination;
    });
  }

  loadMessages() {
    this._user.getMessages(this._auth.decodedToken.nameid, this.pagination.currentPage,
      this.pagination.itemsPerPage, this.messageContainer).
      subscribe((res: PaginatedResult<Message[]>) => {
        this.messages = res.result;
        this.pagination = res.pagination;
      // tslint:disable-next-line: no-shadowed-variable
      }, error => {
        this._alert.error(error);
      });
  }

  deleteMessage(id: number) {
    this._alert.confirm('Are you sure you want to delete this message ?', () => {
      this._user.deleteMessage(id, this._auth.decodedToken.nameid).subscribe(() => {
        this.messages.splice(this.messages.findIndex(m => m.id === id), 1); // findinfg thiwhc message to delete and then deleting it.
        this._alert.success('Message Deleted');
      // tslint:disable-next-line: no-shadowed-variable
      } , error => {
        this._alert.error('Failed to delete message');
      });
    });
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadMessages();
  }

}
