import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { AccountService } from '../account.service';
import { Router } from '@angular/router';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-resend-confirmation-email',
  templateUrl: './resend-confirmation-email.component.html',
  styleUrls: ['./resend-confirmation-email.component.scss'],
})
export class ResendConfirmationEmailComponent implements OnInit {
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

  resend() {
    if (this.formGroup.valid == true) {
      this.accountService
        .resendConfirmationEmail(this.formGroup.value.email)
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
            
          },
        });
    }
  }
}
