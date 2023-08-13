import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AccountService } from '../account.service';
import { SharedService } from 'src/app/shared/shared.service';
import { Resetpassword } from 'src/app/shared/models/account/resetPassword';
import { ActivatedRoute, Router } from '@angular/router';
import { take } from 'rxjs';
import { User } from 'src/app/shared/models/account/user';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss'],
})
export class ResetPasswordComponent implements OnInit {
  resetGroup: FormGroup = new FormGroup({});
  errorMessages: string[] = [];
  resetModel: Resetpassword = {
    email: '',
    token: '',
    password: '',
    confirmPassword: '',
  };

  constructor(
    private formBuilder: FormBuilder,
    private accountService: AccountService,
    private sharedService: SharedService,
    private activatedRoute: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.accountService.user$.pipe(take(1)).subscribe({
      next: (user: User | null) => {
        if (user) {
          this.sharedService.showDialog(
            false,
            'Error',
            'You had logged in. To verify email, please logout.'
          );
          this.router.navigateByUrl('/');
        }
      },
      error: (err) => {
        console.log(err);
      },
    });

    this.initializeForm();
    this.getTokenAndEmail();
  }

  private initializeForm() {
    this.resetGroup = this.formBuilder.group({
      password: [
        '',
        [
          Validators.required,
          Validators.minLength(6),
          Validators.maxLength(16),
        ],
      ],
      confirmPassword: ['', [Validators.required]],
    });
  }

  private getTokenAndEmail() {
    this.activatedRoute.queryParamMap.subscribe({
      next: (params: any) => {
        this.resetModel.email = params.get('email');
        this.resetModel.token = params.get('token');
      },
      error: (error) => {
        console.log(error);
      },
    });

    const resetModel: Resetpassword = {
      email: this.resetGroup.get('email')?.value,
      token: this.resetGroup.get('email')?.value,
      password: this.resetGroup.get('email')?.value,
      confirmPassword: this.resetGroup.get('email')?.value,
    };
  }

  resetPassword() {
    if (this.resetGroup.valid) {
      this.resetModel.password = this.resetGroup.get('password')?.value;
      this.resetModel.confirmPassword =
        this.resetGroup.get('confirmPassword')?.value;

      this.accountService.resetPassword(this.resetModel).subscribe({
        next: (res: any) => {
          this.router.navigateByUrl('/account/login');
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
}
