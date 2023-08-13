import { Component, OnInit } from '@angular/core';
import { AdminService } from '../admin.service';
import { SharedService } from 'src/app/shared/shared.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-options',
  templateUrl: './options.component.html',
  styleUrls: ['./options.component.scss'],
})
export class OptionsComponent implements OnInit {
  formGroup: FormGroup = new FormGroup({});
  submitted: boolean = false;
  errorMessages: string[] = [];

  constructor(
    private adminService: AdminService,
    private sharedService: SharedService,
    private formBuilder: FormBuilder
  ) {}

  ngOnInit(): void {
    this.initializeForm();
  }

  private initializeForm() {
    this.formGroup = this.formBuilder.group({
      numberOfUsers: [5, [Validators.min(1)]],
      password: [
        'abc123',
        [
          Validators.required,
          Validators.maxLength(16),
          Validators.minLength(6),
        ],
      ],
    });
  }

  deleteAllusers() {
    this.adminService.deleteAllUsers().subscribe({
      next: (res) => {
        this.sharedService.showDialog(
          true,
          'Delete all users successfully',
          'Now only you can access application.'
        );
      },
      error: (err) => {
        this.sharedService.showDialog(
          false,
          'Error',
          'Something error when deleting all users.'
        );
      },
    });
  }

  seedUsers() {
    this.submitted = true;
    this.errorMessages = [];
    //this.setLoading(true);
    if(this.formGroup.valid) {
      this.adminService
      .seedUsers(
        this.formGroup.get('numberOfUsers')?.value,
        this.formGroup.get('password')?.value
      )
      .subscribe({
        next: (_) => {
          //this.setLoading(false);
          this.sharedService.showDialog(
            true,
            'Seed users successfully',
            `Increase ${this.formGroup.get('numberOfUsers')?.value} users.`
          );
        },
        error: (error) => {
          //this.setLoading(false);
          if (error.value) {
            this.sharedService.showDialog(
              false,
              error.value.title,
              error.value.message
            );
          } else if (error.error.errors) {
            this.errorMessages = error.error.errors;
          } else {
            this.errorMessages.push(error.error);
          }
        },
      });
    }
  }

  //private setLoading(isLoading: boolean) {
  //  this.adminService.setLoading(isLoading);
  //}
}
