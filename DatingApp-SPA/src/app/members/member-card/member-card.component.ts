import { Component, OnInit, Input } from '@angular/core';
import { User } from 'src/app/_models/user';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {

  @Input() user: User;

  constructor(private _auth: AuthService, private _userSer: UserService, private _alert: AlertifyService) { }

  ngOnInit() {
  }

  sendLike(recepientId: number) {
    this._userSer.sendLike(this._auth.decodedToken.nameid, recepientId).subscribe(data => {
      this._alert.success('You have liked' + this.user.knownAs);
    }, error => {
      this._alert.error(error);
    });
  }

}
