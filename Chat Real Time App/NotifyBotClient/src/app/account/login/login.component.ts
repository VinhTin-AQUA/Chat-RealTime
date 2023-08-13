import { Component, OnInit } from '@angular/core';
import { AccountService } from '../account.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SharedService } from 'src/app/shared/shared.service';
import { Router } from '@angular/router';
import { take } from 'rxjs';
import { User } from 'src/app/shared/models/account/user';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit {
  loginForm: FormGroup = new FormGroup({});
  submitted: boolean = false;
  errorMessages: string[] = [];

  constructor(
    private accountService: AccountService,
    private formBuilder: FormBuilder,
    private sharedService: SharedService,
    private router: Router
  ) {
    this.accountService.user$.pipe(take(1)).subscribe({

      /*
      nếu đã đăng nhập (user$ !== null) thì chuyển hướng đến Home nếu người dùng cố ý truy cập 
      đên Login Component
      */
      next: (user: User | null) => {
        if(user) {
          this.router.navigateByUrl('/');
        }
      },
    });
  }

  ngOnInit(): void {
    this.loginForm = this.formBuilder.group({
      email: ['', [Validators.required]],
      password: ['', [Validators.required]],
    });
  }

  validateInputContainer(field: string) {
    if (this.submitted === true) {
      if (field === 'email') {
        if (this.loginForm.hasError('required', field)) {
          return 'input-container-error';
        }
      } else if (field === 'password') {
        if (this.loginForm.hasError('required', field)) {
          return 'input-container-error';
        }
      }
    }

    return 'input-container';
  }

  validateInputField(field: string) {
    if (this.submitted === true) {
      if (field === 'email') {
        if (this.loginForm.hasError('required', field)) {
          return 'input-icon-error';
        }
      } else if (field === 'password') {
        if (this.loginForm.hasError('required', field)) {
          return 'input-icon-error';
        }
      }
    }

    return 'input-icon';
  }

  login() {
    
    this.submitted = true;
    this.errorMessages = [];
    if (this.loginForm.valid) {
      this.accountService.login(this.loginForm.value).subscribe({
        next: (res) => {
          //this.sharedService.showDialog(
          //  true,
          //  'Login Successfully',
          //  'Welcomto my app'
          //);
          this.router.navigateByUrl('/');
          
        },
        error: (error) => {
          if (error.errors) {
            this.errorMessages = error.errors;

          } else {
            this.errorMessages.push(error.error);
          }
          if(this.errorMessages.includes("Please confirm your email")) {
            this.router.navigateByUrl('/account/resend-confirmation-email')
          }
        },
      });
    }
  }
}
