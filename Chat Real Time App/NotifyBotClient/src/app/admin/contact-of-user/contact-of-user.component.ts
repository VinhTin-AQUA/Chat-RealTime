import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { Router } from '@angular/router';
import { merge } from 'rxjs';
import { UserView } from 'src/app/shared/models/admin/userView';
import { SharedService } from 'src/app/shared/shared.service';
import { AdminService } from '../admin.service';
import { ContactToView } from 'src/app/shared/models/contact/contactToView';

@Component({
  selector: 'app-contact-of-user',
  templateUrl: './contact-of-user.component.html',
  styleUrls: ['./contact-of-user.component.scss'],
})
export class ContactOfUserComponent implements OnInit {
  displayedColumns: string[] = ['Email', 'DateCreated', 'Title', 'Options']; // tên cột của bảng
  data: ContactToView[] = [];

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
    this.InitListContacts();
    // khi thay đổi this.paginator.pageIndex thì sự kiện này xảy ra
    merge(this.paginator.page).subscribe({
      next: (res) => {
        this.adminService
          .getContacts(this.paginator.pageIndex, this.pageSize)
          .subscribe({
            next: (res: any) => {
              this.data = res.value.contacts;
            },
            error: (err) => {
              console.log(err);
            },
          });
      },
    });
  }

  private InitListContacts() {
    this.adminService
      .getContacts(this.paginator.pageIndex, this.pageSize)
      .subscribe({
        next: (res: any) => {
          this.data = res.value.contacts;
          this.length = res.value.size;
        },
        error: (err) => {
          console.log(err);
        },
      });
  }

  deleteContact(contact: ContactToView) {
    this.adminService.deleteContact(contact.id).subscribe({
      next: (_) => {
        this.sharedService.showDialog(
          true,
          'Delete contact successfully',
          'Delete contact successfully'
        );
        this.updateListContact(contact);
      },
      error: (_) => {
        this.sharedService.showDialog(
          false,
          'Error',
          'Something error when delete contact'
        );
      },
    });
  }

  deleteAllContacts() {
    this.adminService.deleteAllContacts().subscribe({
      next: (_) => {
        this.sharedService.showDialog(
          true,
          'Delete all contact successfully',
          'Delete all contact successfully'
        );
        this.data = [];
      },
      error: (_) => {
        this.sharedService.showDialog(
          false,
          'Error',
          'Something error when delete contact'
        );
      },
    })
  }

  private updateListContact(_contact: ContactToView) {
    const contact = this.data.find((c) => c.id === _contact.id);
    if (contact !== undefined) {
      let newListContact = this.data.filter((c) => c.id !== contact.id);
      this.data = newListContact;
      this.length--;
    }
  }

  showDescription(description: string) {
    this.sharedService.showText(description)
  }
}
