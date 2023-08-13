import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { AccountRoutingModule } from './account-routing.module';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';

import { ReactiveFormsModule } from '@angular/forms';
import { ConfirmEmailComponent } from './confirm-email/confirm-email.component';
import { ResendConfirmationEmailComponent } from './resend-confirmation-email/resend-confirmation-email.component';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { ForgotPasswordComponent } from './forgot-password/forgot-password.component';
import { SendEmailNotificationComponent } from './send-email-notification/send-email-notification.component';
import { ResetPasswordComponent } from './reset-password/reset-password.component';
import { EditProfileComponent } from './edit-profile/edit-profile.component';
import { FormsModule } from '@angular/forms';

import {MatCardModule} from '@angular/material/card';



@NgModule({
  declarations: [
    RegisterComponent,
    LoginComponent,
    ConfirmEmailComponent,
    ResendConfirmationEmailComponent,
    ForgotPasswordComponent,
    SendEmailNotificationComponent,
    ResetPasswordComponent,
    EditProfileComponent,
  ],
  imports: [
    CommonModule,
    AccountRoutingModule,
    ReactiveFormsModule,
    MatIconModule,
    MatInputModule,
    MatButtonModule,FormsModule,MatCardModule
  ],
})
export class AccountModule {}
