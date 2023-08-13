import { Component, OnInit } from '@angular/core';
import { AccountService } from '../account.service';
import { ActivatedRoute, Router } from '@angular/router';
import { take } from 'rxjs';
import { User } from 'src/app/shared/models/account/user';
import { ConfirmEmail } from 'src/app/shared/models/account/confirmEmail';
import { SharedService } from 'src/app/shared/shared.service';

@Component({
  selector: 'app-confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrls: ['./confirm-email.component.scss'],
})
export class ConfirmEmailComponent implements OnInit {
  errorMessages: string[] = [];

  constructor(
    private accountService: AccountService,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private sharedService: SharedService
  ) {}

  ngOnInit(): void {
    this.accountService.user$.pipe(take(1)).subscribe({
      next: (user: User | null) => {
        if (user) {
          this.router.navigateByUrl('/');
          this.sharedService.showDialog(true,"You are logged in", "Please logout to verify email")
        } else {
          this.activatedRoute.queryParamMap.subscribe({
            next: (params: any) => {
              const confirm: ConfirmEmail = {
                token: params.get('token'),
                email: params.get('email'),
              };

              this.accountService.confirmEmail(confirm).subscribe({
                next: (res: any) => {
                  const title = res.value.title;
                  const message = res.value.message;
                  this.router.navigate(['/account/send-email'], {
                    queryParams: {
                      state: true,
                      title: res.value.title,
                      message: res.value.message,
                    },
                  });
                },
                error: (err) => {
                  this.router.navigate(['/account/send-email'], {
                    queryParams: {
                      state: false,
                      title: err.error.value.title,
                      message: err.error.value.message,
                    },
                  });
                },
              });
            },
            error: (err) => {
              this.router.navigate(['/account/send-email'], {
                queryParams: {
                  state: false,
                  title: err.error.value.title,
                  message: err.error.value.message,
                },
              });
            },
          });
        }
      },
      error: (err) => {
        this.router.navigate(['/account/send-email'], {
          queryParams: {
            state: false,
            title: err.error.value.title,
            message: err.error.value.message,
          },
        });
      },
    });
  }
}
