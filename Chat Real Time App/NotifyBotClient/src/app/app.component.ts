import { Component, OnInit } from '@angular/core';
import { AccountService } from './account/account.service';
import { SharedService } from './shared/shared.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'NotifyBotClient';

  constructor(private accountService: AccountService, public sharedService: SharedService){}

  ngOnInit(): void {
    this.refreshUser()
  }

  private refreshUser() {
    const jwt = this.accountService.getJwtUser();
    if(jwt) {
      this.accountService.refreshUser(jwt).subscribe({
        next: _ => {},
        error: _ => {
          //this.accountService.logout();
          this.accountService.notLogin();
        }
      })
    } else {
      this.accountService.refreshUser(null).subscribe();
    }
  }
}
