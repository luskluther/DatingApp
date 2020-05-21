import { Component, OnInit, Input } from '@angular/core';
import { Message } from 'src/app/_models/message';
import { UserService } from 'src/app/_services/user.service';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { tap } from 'rxjs/operators';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {

  @Input() recipientId: number;
  messages: Message[];
  newMessage: any = {};

  constructor(private _user: UserService, private _auth: AuthService, private _alert: AlertifyService) { }

  ngOnInit() {
    this.loadMessages();
  }

  loadMessages() {
    const currentUserId = +this._auth.decodedToken.nameid;
    this._user.getMessageThread(this._auth.decodedToken.nameid, this.recipientId)
    .pipe( // tap allows to do osmething before we subscribe
      // pipe typcally takes some type of input data that we get from subscription
      // and then transforms it into a desired one that we can use later. here we are using tap
      // for a similar purpose i believe

      // tap was called do before. then they changed it
      tap(messages => {
        for (let i = 0; i < messages.length; i++) {
          if (messages[i].isRead === false && messages[i].recipientId === currentUserId) {
            this._user.markAsRead(currentUserId, messages[i].id);
          }
        }
      })
    )
    .subscribe(messages => {
      this.messages = messages;
    }, error => {
      this._alert.error(error);
    });
  }

  sendMessage() {
    this.newMessage.recipientId = this.recipientId;
    this._user.sendMessage(this._auth.decodedToken.nameid, this.newMessage).subscribe((message: Message) => {
      this.messages.unshift(message);
      this.newMessage.content = '';
    }, error => {
      this._alert.error(error);
    });
  }

}
