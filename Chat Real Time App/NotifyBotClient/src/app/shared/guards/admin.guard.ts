import { Injectable } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  Router,
  RouterStateSnapshot,
  UrlTree,
} from '@angular/router';
import { Observable, map } from 'rxjs';
import { User } from '../models/account/user';
import { AccountService } from 'src/app/account/account.service';
import { SharedService } from '../shared.service';
import jwt_decode from 'jwt-decode';

@Injectable({
  providedIn: 'root',
})
export class AdminGuard {
  constructor(
    private accountService: AccountService,
    private router: Router,
    private sharedService: SharedService
  ) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> {
    return this.accountService.user$.pipe(
      map((user: User | null) => {
        if (user) {
          const decodeToken: any = jwt_decode(user.jwt);

          if (this.isAdmin(decodeToken.role)) {
            return true;
          }

          this.router.navigateByUrl('/');
          this.sharedService.showDialog(
            false,
            'Only Admin',
            'Only admin can access feature'
          );
          return false;
        }
        this.router.navigateByUrl('/');
        this.sharedService.showDialog(
          false,
          'Only Admin',
          'Only admin can access feature'
        );
        return false;
      })
    );
  }

  private isAdmin(roles: string | string[]) {
    if (typeof roles === 'string') {
      return roles === 'Admin';
    } else {
      return roles.includes('Admin');
    }
  }
}
