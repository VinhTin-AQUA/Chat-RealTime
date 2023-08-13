import { Component, OnDestroy, OnInit } from '@angular/core';
import { ChatService } from './chat.service';
import { AccountService } from '../account/account.service';
import { take } from 'rxjs';
import { User } from '../shared/models/account/user';
import { SharedService } from '../shared/shared.service';
import { GroupToView } from '../shared/models/chat/groupToView';

@Component({
  selector: 'app-room',
  templateUrl: './room.component.html',
  styleUrls: ['./room.component.scss'],
})
export class RoomComponent implements OnInit, OnDestroy {
  constructor(
    public chatService: ChatService,
    private accountService: AccountService,
    private sharedService: SharedService
  ) {}

  ngOnDestroy(): void {
    this.chatService.stopConnection();
  }

  ngOnInit(): void {
    this.chatService.createChatConnection();
    this.getUserId();
  }

  private getUserId() {
    this.accountService.user$.pipe(take(1)).subscribe({
      next: (user: User | null) => {
        if (user) {
          this.chatService.userId = user.id;
        }
      },
      error: (err) => {
        console.log(err);
      },
    });
  }

  
}
