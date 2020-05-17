import { Component, OnInit } from '@angular/core';
import { User } from '../../_models/user';
import { UserService } from '../../_services/user.service';
import { AlertifyService } from '../../_services/alertify.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-memberr-list',
  templateUrl: './memberr-list.component.html',
  styleUrls: ['./memberr-list.component.css']
})
export class MemberrListComponent implements OnInit {

  users: User[];

  constructor(private _userService: UserService, private _alertify: AlertifyService, private _route: ActivatedRoute) { }

  ngOnInit() {
    this._route.data.subscribe(data => {
      this.users = data['users']; // getting this data from resovler. no need to call load users again explicitly like we generally do
    });
  }
}
