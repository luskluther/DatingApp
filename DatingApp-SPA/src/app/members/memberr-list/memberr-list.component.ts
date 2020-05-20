import { Component, OnInit } from '@angular/core';
import { User } from '../../_models/user';
import { UserService } from '../../_services/user.service';
import { AlertifyService } from '../../_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { Pagination, PaginatedResult } from 'src/app/_models/pagination';

@Component({
  selector: 'app-memberr-list',
  templateUrl: './memberr-list.component.html',
  styleUrls: ['./memberr-list.component.css']
})
export class MemberrListComponent implements OnInit {

  users: User[];
  pagination: Pagination;
  user: User = JSON.parse(localStorage.getItem('user'));
  genderList = [{value: 'male', display: 'Male'}, {value: 'female', display: 'Female'}];
  userParams: any = {};

  constructor(private _userService: UserService, private _alertify: AlertifyService, private _route: ActivatedRoute) { }

  ngOnInit() {
    this._route.data.subscribe(data => {
      this.users = data['users'].result; // getting this data from resovler. no need to call load users
                                        // again explicitly like we generally do
      this.pagination = data['users'].pagination;
    });

    this.userParams.gender = this.user.gender === 'female' ? 'male' : 'female';
    this.userParams.minAge = 18;
    this.userParams.maxAge = 99;
    this.userParams.orderBy = 'lastActive';
  }

  pageChanged(event: any) {
    this.pagination.currentPage = event.page;
    this.loadUsers();
  }

  resetFilters() {
    this.userParams.gender = this.user.gender === 'female' ? 'male' : 'female';
    this.userParams.minAge = 18;
    this.userParams.maxAge = 99;
    this.userParams.orderBy = 'lastActive';
    this.loadUsers();
  }

  loadUsers() {
    this._userService
    .getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, this.userParams)
    .subscribe((res: PaginatedResult<User[]>) => {
      this.users = res.result;
      this.pagination = res.pagination;
    });
  }
}
