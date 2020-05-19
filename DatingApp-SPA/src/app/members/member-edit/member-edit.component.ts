import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { User } from 'src/app/_models/user';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { NgForm } from '@angular/forms';
import { UserService } from 'src/app/_services/user.service';
import { AuthService } from 'src/app/_services/auth.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {

  @ViewChild('editForm') editForm: NgForm;
  user: User;
  photoUrl: string;
  @HostListener('window:beforeunload', ['$event']) // this protects if form isdirty and we close browser window/tab.
  unloadNotification($event: any) {
    if (this.editForm.dirty) {
      $event.returnValue = true;
    }
  }

  constructor(private _route: ActivatedRoute,
              private _alert: AlertifyService, private _userSer: UserService, private _auth: AuthService) { }

  ngOnInit() {
    this._route.data.subscribe(data => {
      this.user = data['user'];
    });
    this._auth.currentPhotoUrl.subscribe(photoUrl => this.photoUrl = photoUrl);
  }

  updateUser() {
    this._userSer.updateUser(this._auth.decodedToken.nameid, this.user).subscribe(next => {
      this._alert.success('Success updating profile');
      this.editForm.reset(this.user); // resets teh dirty and makes the form pristine to what they are as they are saved passing user
    }, error => {
      this._alert.error(error);
    });
  }

  updateMainPhoto(photoUrl) {
    this.user.photoUrl = photoUrl;
  }
}
