import { Component, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { AdminService } from 'src/app/admin/admin.service';
import { UserView } from 'src/app/shared/models/admin/userView';
import { merge } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { InputDataComponent } from 'src/app/shared/components/input-data/input-data.component';
import { ChatService } from '../chat.service';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-search-member',
  templateUrl: './search-member.component.html',
  styleUrls: ['./search-member.component.scss'],
})
export class SearchMemberComponent {
  searchString: string = '';
  data: UserView[] = [];
  length: number = 0; // tổng số lượng bản ghi
  pageSize: number = 6; // số lượng bản ghi sẽ hiển thị
  displayedColumns: string[] = ['Name', 'DateCreated', 'Options']; // tên cột của bảng

  constructor(
    private adminService: AdminService,
    public dialog: MatDialog,
    private chatService: ChatService,
    private sharedService: SharedService
  ) {}

  /* lưu page index của trang
    paginator.pageIndex
  */
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngAfterViewInit(): void {
    // khi thay đổi this.paginator.pageIndex thì sự kiện này xảy ra
    merge(this.paginator.page).subscribe({
      next: (_) => {
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

  searchName() {
    this.paginator.pageIndex = 0;
    if (this.searchString !== '') {
      this.getUserByname();
    }
  }

  addUserToGroup(user: UserView) {
    const dialogRef = this.dialog.open(InputDataComponent, {
      data: {
        title: 'Enter group name',
        data: 'hello',
      },
    });

    dialogRef.afterClosed().subscribe((data) => {
      const groupNames = this.chatService.groups.map((g) => g.name);
      if (data.trim() === '' || groupNames.includes(data) === false) {
        this.sharedService.showDialog(
          false,
          'Error',
          'Group name is not existed.'
        );
      } else {
        this.chatService.addMemberToGroup(user.email)
      }
    });
  }
}
