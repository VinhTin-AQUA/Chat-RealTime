import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { map, take } from 'rxjs';
import { User } from 'src/app/shared/models/account/user';
import { SharedService } from 'src/app/shared/shared.service';
import { AccountService } from '../account.service';
import { RegisterUser } from 'src/app/shared/models/account/registerUser';
import { UpdateUser } from 'src/app/shared/models/account/updateUser';

@Component({
  selector: 'app-edit-profile',
  templateUrl: './edit-profile.component.html',
  styleUrls: ['./edit-profile.component.scss'],
})
export class EditProfileComponent implements OnInit {
  submitted: boolean = false;
  errorMessages: string[] = [];
  updateForm: FormGroup = new FormGroup({});

  user: UpdateUser = {
    email: '',
    firstName: '',
    lastName: '',
    oldPassword: '',
    newPassword: '',
    reEnterPassword: '',
  };

  constructor(
    private formBuilder: FormBuilder,
    private accountService: AccountService,
    private sharedService: SharedService,
    private router: Router
  ) {
    this.accountService.user$.pipe(take(1)).subscribe({
      next: (user: User | null) => {
        if (user === null) {
          this.router.navigateByUrl('/account/login');
        }
      },
    });
  }

  ngOnInit(): void {
    this.initUserToForm();

    /*
    Gọi lại hàm initializeForm() sau khi gán giá trị
    => để các giá trị có thể thay đổi sau khi nhập giá trị vào input
    */
    this.initializeForm();
  }

  // lấy dữ liệu của user và gán vào updateForm
  private initUserToForm() {
    // get user id
    let userId = '';
    this.accountService.user$.pipe(take(1)).subscribe({
      next: (user: User | null) => {
        if (user) {
          userId = user.id;
        }
      },
      error: (error) => {},
    });

    // get user
    this.accountService.getUser(userId).subscribe({
      next: (res: any) => {
        this.user = res;
        this.initializeForm();
      },
      error: (_) => {
        this.sharedService.showDialog(
          false,
          'Error',
          'Something error when get user id.'
        );
      },
    });
  }

  private initializeForm() {
    this.updateForm = this.formBuilder.group({
      firstName: [this.user.firstName],
      lastName: [this.user.lastName],
      email: [this.user.email],
      oldPassword: [this.user.oldPassword],
      newPassword: [this.user.newPassword],
      reEnterPassword: [this.user.reEnterPassword],
    });
  }

  update() {
    this.submitted = true;
    this.errorMessages = [];
    if (this.updateForm.valid) {
      this.accountService.updateUser(this.updateForm.value).subscribe({
        next: (res: any) => {
          this.sharedService.showDialog(
            true,
            res.value.title,
            res.value.message
          );
          this.user = this.updateForm.value;
        },
        error: (error) => {
          //this.sharedService.setLoading(false);
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
