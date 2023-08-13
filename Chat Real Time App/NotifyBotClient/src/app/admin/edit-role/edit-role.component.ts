import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AdminService } from '../admin.service';
import { SharedService } from 'src/app/shared/shared.service';
import { FormControl } from '@angular/forms';

@Component({
  selector: 'app-edit-role',
  templateUrl: './edit-role.component.html',
  styleUrls: ['./edit-role.component.scss'],
})
export class EditRoleComponent implements OnInit {
  userId: string | null = '';
  name: string | null = '';
  applicationRoles: string[] = [];
  roles = new FormControl(['']);

  constructor(
    private activatedRoute: ActivatedRoute,
    private adminService: AdminService,
    private sharedService: SharedService
  ) {}

  ngOnInit(): void {
    this.userId = this.activatedRoute.snapshot.paramMap.get('userId');
    this.name = this.activatedRoute.snapshot.queryParamMap.get('name');

    this.adminService.getApplicationRoles().subscribe({
      next: (res: any) => {
        this.applicationRoles = res;
      },
      error: (err) => {
        this.sharedService.showDialog(
          false,
          'Error',
          'Something error when get application roles'
        );
      },
    });
  }

  save() {
    if (this.roles.value?.length === 0) {
      this.roles.value.push('');
    }
    this.adminService.setRolesUser(this.userId, this.roles?.value).subscribe({
      next: (res: any) => {
        this.sharedService.showDialog(true, res.value.title, res.value.message)
      },
      error: (err) => {
        this.sharedService.showDialog(false, "Error", err.error)
      },
    });
  }
}
