import { Component, OnInit, ViewChild } from '@angular/core';
import { UserView } from 'src/app/shared/models/admin/userView';
import { AdminService } from '../admin.service';
import { MatPaginator } from '@angular/material/paginator';

import { MatSort } from '@angular/material/sort';
import { merge } from 'rxjs';
import { SharedService } from 'src/app/shared/shared.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss'],
})
export class UsersComponent implements OnInit {
  displayedColumns: string[] = [
    'No.',
    'Name',
    'Email',
    'DateCreated',
    'Roles',
    'Options',
  ]; // tên cột của bảng
  data: UserView[] = [];

  length: number = 0; // tổng số lượng bản ghi
  pageSize: number = 6; // số lượng bản ghi sẽ hiển thị

  /* lưu page index của trang
    paginator.pageIndex
  */
  @ViewChild(MatPaginator) paginator!: MatPaginator; //

  /* lưu chỉ thị sort
    sort.direction: asc và desc
  */
  @ViewChild(MatSort) sort!: MatSort;
  searchString: string = '';

  constructor(
    private adminService: AdminService,
    private sharedService: SharedService,
    private router: Router
  ) {}

  ngOnInit(): void {}

  ngAfterViewInit(): void {
    this.InitListUsers();
    // khi thay đổi this.paginator.pageIndex thì sự kiện này xảy ra
    merge(this.paginator.page).subscribe({
      next: (_) => {
        if (this.searchString) {
          this.getUserByname();
        } else {
          this.adminService
            .getUsers(this.paginator.pageIndex, this.pageSize)
            .subscribe({
              next: (res: any) => {
                this.data = res.users;
              },
              error: (err) => {
                console.log(err);
              },
            });
        }
      },
    });
  }

  private InitListUsers() {
    this.adminService
      .getUsers(this.paginator.pageIndex, this.pageSize)
      .subscribe({
        next: (res: any) => {
          this.data = res.users;
          this.length = res.size;
        },
        error: (err) => {
          console.log(err);
        },
      });
  }

  private getUserByname() {
    this.adminService
      .searchUserByName(
        this.searchString,
        this.paginator.pageIndex,
        this.pageSize
      )
      .subscribe({
        next: (res: any) => {
          this.data = res.users;
          this.length = res.size;
        },
        error: (err) => {
          console.log(err);
        },
      });
  }

  deleteUser(user: UserView) {
    this.adminService.deleteUser(user.id).subscribe({
      next: (res: any) => {
        this.sharedService.showDialog(true, res.value.title, res.value.message);
        this.InitListUsers();
        this.length--;
      },
      error: (err) => {
        console.log(err);
      },
    });
  }

  searchName() {
    this.paginator.pageIndex = 0;
    if (this.searchString === '') {
      //this.getLength();
      this.InitListUsers();
    } else {
      this.getUserByname();
    }
  }

  lockUser(user: UserView) {
    this.adminService.lockUser(user.id).subscribe({
      next: (res: any) => {
        user.isLockout = !user.isLockout;
      },
      error: (err) => {
        this.sharedService.showDialog(false, 'Error', err.error);
      },
    });
  }

  unlockUser(user: UserView) {
    this.adminService.unlockUser(user.id).subscribe({
      next: (res: any) => {
        user.isLockout = !user.isLockout;
      },
      error: (err) => {
        this.sharedService.showDialog(false, 'Error', err.error);
      },
    });
  }

  editRole(user: UserView) {
    var name = user.firstName + ' ' + user.lastName;
    this.router.navigateByUrl(`/admin/edit-role/${user.id}?name=${name}`);
  }
}
