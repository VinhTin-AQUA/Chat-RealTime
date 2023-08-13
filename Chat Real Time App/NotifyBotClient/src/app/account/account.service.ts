import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { RegisterUser } from '../shared/models/account/registerUser';
import { environment } from 'src/environments/environment.development';
import { Router } from '@angular/router';
import { LoginUser } from '../shared/models/account/loginUser';
import { User } from '../shared/models/account/user';
import { ReplaySubject, map, of } from 'rxjs';
import { ConfirmEmail } from '../shared/models/account/confirmEmail';
import { Resetpassword } from '../shared/models/account/resetPassword';
import { UpdateUser } from '../shared/models/account/updateUser';
import { SharedService } from '../shared/shared.service';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private userSource: ReplaySubject<User | null> = new ReplaySubject(1);
  user$ = this.userSource.asObservable();

  constructor(private http: HttpClient, private router: Router, private sharedService: SharedService) {}

  register(model: RegisterUser) {
    return this.http.post(`${environment.appUrl}/api/account/register`, model);
  }

  login(model: LoginUser) {
    return this.http
      .post<User>(`${environment.appUrl}/api/account/login`, model)
      .pipe(
        map((user: User) => {
          if (user) {
            this.saveJWTUser(user);
            //return user;
          }
          //return null;
        })
      );
  }

  logout() {
    //window.location.reload();
    localStorage.removeItem(environment.userKey);
    this.userSource.next(null);
    this.router.navigateByUrl('/');
  }

  notLogin() {
    this.sharedService.setLoading(false);
    this.userSource.next(null);
    this.router.navigateByUrl('/');
  }

  private saveJWTUser(user: User) {
    localStorage.setItem(environment.userKey, JSON.stringify(user));
    this.userSource.next(user);
  }

  getJwtUser() {
    const key = localStorage.getItem(environment.userKey);
    if (key) {
      const user: User = JSON.parse(key);
      return user.jwt;
    }
    return null;
  }

  refreshUser(jwt: string | null) {
    if (jwt === null) {
      this.userSource.next(null);
      return of(undefined);
    }

    let headers = new HttpHeaders();
    headers = headers.set('Authorization', 'Bearer ' + jwt);
    return this.http
      .get<User>(`${environment.appUrl}/api/account/refresh-user-token`, {
        headers: headers,
      })
      .pipe(
        map((user: User) => {
          if (user) {
            this.saveJWTUser(user);
          }
        })
      );
  }

  confirmEmail(confirm: ConfirmEmail) {
    return this.http.put(`${environment.appUrl}/api/account/confirm-email`, confirm);
  }

  resendConfirmationEmail(email: string) {
    return this.http.post(`${environment.appUrl}/api/account/resend-confirmation-email/${email}`,email);
  }

  sendEmailForgotPassword(email: string) {
    return this.http.post(`${environment.appUrl}/api/account/forgot-password/${email}`, email)
  }

  resetPassword(model: Resetpassword) {
    return this.http.put(`${environment.appUrl}/api/account/reset-password`, model)
  }

  getUser(userId: string) {
    return this.http.get(`${environment.appUrl}/api/account/get-user/${userId}`)
  }

  updateUser(model: UpdateUser) {
    return this.http.put(`${environment.appUrl}/api/account/update-user`,model);
  }

}
