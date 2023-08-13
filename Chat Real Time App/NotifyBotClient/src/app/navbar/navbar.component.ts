import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AccountService } from '../account/account.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss'],
})
export class NavbarComponent {
  collapsed: boolean = false;
  name: string = '';

  constructor(public accountService: AccountService, private router: Router) {}

  showMenu() {
    this.collapsed = !this.collapsed;
  }

  logout() {
    this.router.navigateByUrl('/account/login');
    this.accountService.logout();
  }
}
