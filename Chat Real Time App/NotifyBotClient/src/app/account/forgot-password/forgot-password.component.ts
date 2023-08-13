import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { SharedService } from 'src/app/shared/shared.service';
import { AccountService } from '../account.service';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss'],
})
export class ForgotPasswordComponent {
  formGroup: FormGroup = new FormGroup({});

  constructor(
    private accountService: AccountService,
    private router: Router,
    private sharedService: SharedService,
    private formBuilder: FormBuilder
  ) {}

  ngOnInit(): void {
    this.formGroup = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]],
    });
  }

  SendEmailForgotPassword() {
    if (this.formGroup.valid == true) {
      this.accountService
        .sendEmailForgotPassword(this.formGroup.value.email)
        .subscribe({
          next: (res: any) => {
            this.router.navigate(['/account/send-email'], {
              queryParams: {
                state: true,
                title: res.value.title,
                message: res.value.message,
              },
            });
          },
          error: (err) => {
            this.sharedService.showDialog(false, 'Error', err.error);
            console.log(err);
            
          },
        });
    }
  }
}
