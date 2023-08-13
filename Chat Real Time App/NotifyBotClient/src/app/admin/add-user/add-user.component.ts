import { Component } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { take } from 'rxjs';
import { AccountService } from 'src/app/account/account.service';
import { User } from 'src/app/shared/models/account/user';
import { SharedService } from 'src/app/shared/shared.service';
import { AdminService } from '../admin.service';

@Component({
  selector: 'app-add-user',
  templateUrl: './add-user.component.html',
  styleUrls: ['./add-user.component.scss'],
})
export class AddUserComponent {
  submitted: boolean = false;
  errorMessages: string[] = [];
  addUserForm: FormGroup = new FormGroup({});

  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private adminService: AdminService,
    private sharedService: SharedService
  ) {}

  ngOnInit(): void {
    this.addUserForm = this.formBuilder.group({
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      email: ['', [Validators.email, Validators.required]],
      password: [
        '',
        [
          Validators.required,
          Validators.maxLength(16),
          Validators.minLength(6),
        ],
      ],
    });
  }

  addNewUser() {
    this.submitted = true;
    this.errorMessages = [];
    
    if (this.addUserForm.valid) {
      this.adminService.addNewUser(this.addUserForm.value).subscribe({
        next: (res: any) => {
          this.router.navigateByUrl('/admin')
          this.sharedService.showDialog(
            true,
            res.value.title,
            res.value.message
          );
        },
        error: (error) => {
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

  validateInputContainer(field: string) {
    if (this.submitted === true) {
      if (field === 'firstName') {
        if (this.addUserForm.hasError('required', field)) {
          return 'input-container-error';
        }
      } else if (field === 'lastName') {
        if (this.addUserForm.hasError('required', field)) {
          return 'input-container-error';
        }
      } else if (field === 'email') {
        if (
          this.addUserForm.hasError('email', field) ||
          this.addUserForm.hasError('required', field)
        ) {
          return 'input-container-error';
        }
      } else if (field === 'password') {
        if (
          this.addUserForm.hasError('maxlength', field) ||
          this.addUserForm.hasError('minlength', field) ||
          this.addUserForm.hasError('required', field)
        ) {
          return 'input-container-error';
        }
      } else if (field === 'reEnterPassword') {
        if (this.addUserForm.hasError('required', field)) {
          return 'input-container-error';
        }
      }
    }

    return 'input-container';
  }

  validateInputField(field: string) {
    if (this.submitted === true) {
      if (field === 'firstName') {
        if (this.addUserForm.hasError('required', field)) {
          return 'input-icon-error';
        }
      } else if (field === 'lastName') {
        if (this.addUserForm.hasError('required', field)) {
          return 'input-icon-error';
        }
      } else if (field === 'email') {
        if (
          this.addUserForm.hasError('email', field) ||
          this.addUserForm.hasError('required', field)
        ) {
          return 'input-icon-error';
        }
      } else if (field === 'password') {
        if (
          this.addUserForm.hasError('maxlength', field) ||
          this.addUserForm.hasError('minlength', field) ||
          this.addUserForm.hasError('required', field)
        ) {
          return 'input-icon-error';
        }
      } else if (field === 'reEnterPassword') {
        if (this.addUserForm.hasError('required', field)) {
          return 'input-icon-error';
        }
      }
    }

    return 'input-icon';
  }
}
