import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { ConfirmEmailComponent } from './confirm-email/confirm-email.component';
import { ResendConfirmationEmailComponent } from './resend-confirmation-email/resend-confirmation-email.component';
import { ForgotPasswordComponent } from './forgot-password/forgot-password.component';
import { SendEmailNotificationComponent } from './send-email-notification/send-email-notification.component';
import { ResetPasswordComponent } from './reset-password/reset-password.component';
import { EditProfileComponent } from './edit-profile/edit-profile.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent, title: 'Login' },
  { path: 'signup', component: RegisterComponent, title: 'Signup' },
  { path: 'confirm-email', component: ConfirmEmailComponent, title: 'confirm email', },
  { path: 'send-email', component: SendEmailNotificationComponent, title: 'Verify email', },
  { path: 'resend-confirmation-email', component: ResendConfirmationEmailComponent, title: 'Resend confirmation email', },
  { path: 'forgot-password', component: ForgotPasswordComponent, title: 'Forgot password', },
  { path: 'reset-password', component: ResetPasswordComponent, title: 'Reset password', },
  { path: 'edit-profile', component: EditProfileComponent, title: 'Edit profile', },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AccountRoutingModule {}
