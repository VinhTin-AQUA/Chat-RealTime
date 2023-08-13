import { Component, ElementRef, ViewChild } from '@angular/core';
import { ChatService } from '../chat.service';
import { MatDialog } from '@angular/material/dialog';
import { SharedService } from 'src/app/shared/shared.service';
import { GroupToView } from 'src/app/shared/models/chat/groupToView';
import { InputDataComponent } from 'src/app/shared/components/input-data/input-data.component';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss'],
})
export class ChatComponent {
  @ViewChild('messBox', { static: false }) messBox?: ElementRef;
  content: string = '';
  constructor(
    public chatService: ChatService,
    public dialog: MatDialog,
    private sharedService: SharedService
  ) {}

  ngAfterViewChecked() {
    this.scrollToBottom();
  }

  scrollToBottom(): void {
    if (this.messBox) {
      const nativeElement = this.messBox.nativeElement;
      nativeElement.scrollTop = nativeElement.scrollHeight;
    }
  }

  async sendMessage(inpurRef: any) {
    inpurRef.focus();
    await this.chatService.sendMessage(this.content);
    this.content = '';
  }

  addUserToGroup() {
    const dialogRef = this.dialog.open(InputDataComponent, {
      data: {
        title: 'Enter email',
        data: 'hello',
      },
    });

    dialogRef.afterClosed().subscribe((data) => {
      if (data === undefined) {
      } else if (data.trim() !== '') {
        this.chatService.addMemberToGroup(data);
      }
    });
  }

  addGroup(groupName: HTMLInputElement) {
    if (groupName.value.trim() !== '') {
      this.chatService.addGroupUser(groupName.value);
      groupName.value = '';
    } else {
      this.sharedService.showDialog(false, 'Error', 'Group name is required');
    }
  }

  leaveGroup(group: GroupToView) {
    this.chatService.leaveGroup(group);
  }

  switchGroup(group: GroupToView) {
    this.chatService.switchGroup(group);
  }

  active(group: GroupToView): string {
    if (group.name === this.chatService.groupName) {
      return 'bg-blue-600 text-white';
    } else if (group.hasNewMessage === true) {
      return 'bg-green-600 text-white';
    }
    return 'bg-blue-200 text-blue-900';
  }
}
