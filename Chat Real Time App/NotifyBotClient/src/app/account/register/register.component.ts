import { Component, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { AccountService } from '../account.service';
import { SharedService } from 'src/app/shared/shared.service';
import { take } from 'rxjs';
import { User } from 'src/app/shared/models/account/user';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
})
export class RegisterComponent implements OnInit {
  submitted: boolean = false;
  errorMessages: string[] = [];
  registerForm: FormGroup = new FormGroup({});

  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private accountService: AccountService,
    private sharedService: SharedService
  ) {
    this.accountService.user$.pipe(take(1)).subscribe({
      next: (user: User | null) => {
        if (user) {
          this.router.navigateByUrl('/');
        }
      },
    });
  }

  ngOnInit(): void {
    this.registerForm = this.formBuilder.group({
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
      reEnterPassword: ['', Validators.required],
    });
  }

  register() {
    this.submitted = true;
    this.errorMessages = [];
    if (this.registerForm.valid) {
      this.accountService.register(this.registerForm.value).subscribe({
        next: (res: any) => {
          this.router.navigate(['/account/send-confirmation-email'], {
            queryParams: {
              state: true,
              title: res.value.title,
              message: res.value.message,
            },
          });
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
        if (this.registerForm.hasError('required', field)) {
          return 'input-container-error';
        }
      } else if (field === 'lastName') {
        if (this.registerForm.hasError('required', field)) {
          return 'input-container-error';
        }
      } else if (field === 'email') {
        if (
          this.registerForm.hasError('email', field) ||
          this.registerForm.hasError('required', field)
        ) {
          return 'input-container-error';
        }
      } else if (field === 'password') {
        if (
          this.registerForm.hasError('maxlength', field) ||
          this.registerForm.hasError('minlength', field) ||
          this.registerForm.hasError('required', field)
        ) {
          return 'input-container-error';
        }
      } else if (field === 'reEnterPassword') {
        if (this.registerForm.hasError('required', field)) {
          return 'input-container-error';
        }
      }
    }

    return 'input-container';
  }

  validateInputField(field: string) {
    if (this.submitted === true) {
      if (field === 'firstName') {
        if (this.registerForm.hasError('required', field)) {
          return 'input-icon-error';
        }
      } else if (field === 'lastName') {
        if (this.registerForm.hasError('required', field)) {
          return 'input-icon-error';
        }
      } else if (field === 'email') {
        if (
          this.registerForm.hasError('email', field) ||
          this.registerForm.hasError('required', field)
        ) {
          return 'input-icon-error';
        }
      } else if (field === 'password') {
        if (
          this.registerForm.hasError('maxlength', field) ||
          this.registerForm.hasError('minlength', field) ||
          this.registerForm.hasError('required', field)
        ) {
          return 'input-icon-error';
        }
      } else if (field === 'reEnterPassword') {
        if (this.registerForm.hasError('required', field)) {
          return 'input-icon-error';
        }
      }
    }

    return 'input-icon';
  }
}
