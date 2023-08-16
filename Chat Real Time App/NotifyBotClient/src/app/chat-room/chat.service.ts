
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { environment } from 'src/environments/environment';
import { MessageChatToView } from '../shared/models/chat/messageChatToView';
import { GroupService } from './group.service';
import { GroupToView } from '../shared/models/chat/groupToView';
import { MessageService } from './message.service';
import { MessageChatToSend } from '../shared/models/chat/messageChatToSend';
import { AccountService } from '../account/account.service';
import { lastValueFrom, take } from 'rxjs';
import { User } from '../shared/models/account/user';
import { SharedService } from '../shared/shared.service';

@Injectable({
  providedIn: 'root',
})
export class ChatService {
  private chatConnection?: HubConnection;

  /* thong tin */
  groupId: string = '';
  groupName: string = '';
  myName: string = '';
  userId: string = '';
  connectionId: string | null | undefined = '';

  messages: MessageChatToView[] = [];
  usersOnline: string[] = [];
  groups: GroupToView[] = [];

  constructor(
    private groupService: GroupService,
    private messageService: MessageService,
    private accountService: AccountService,
    private shareService: SharedService
  ) {}

  async createChatConnection() {
    if (this.chatConnection?.state !== 'Connected') {
      this.chatConnection = new HubConnectionBuilder()
        .withUrl(`${environment.hupUrl}/hubs/chat`)
        .withAutomaticReconnect()
        .build();

      await this.getMyName();
      await this.startConnection();

      await this.getGroupsOfUser();

      await this.JoinGroup();

      await this.getOldMessages();
      await this.addOnlineUser();
      await this.getOnlineUsers();
    } else {
    }

    this.chatConnection.on('UserConected', async (connectionId: string) => {});

    this.chatConnection.on('NewOnlineUser', async (userName: string) => {
      if (this.usersOnline.includes(userName) === false) {
        this.usersOnline = [...this.usersOnline, userName];
      }
    });

    this.chatConnection.on(
      'NewGroup',
      async (groupToView: GroupToView | string) => {
        if (typeof groupToView === 'string') {
          this.shareService.showDialog(false, 'Error', groupToView);
        } else {
          this.groups = [...this.groups, groupToView];
          this.groupName = groupToView.name;
          this.groupId = groupToView.id;
          await this.addOnlineUser();
          await this.getOnlineUsers();
          this.JoinGroup();
        }
      }
    );

    this.chatConnection.on('LeaveGroup', async (groupId: string) => {
      this.groups = this.groups.filter((g) => g.id !== groupId);
      if (this.groups.length > 0) {
        this.groupId = this.groups[0].id;
        this.groupName = this.groups[0].name;
        await this.getOldMessages();
        await this.addOnlineUser();
        await this.getOnlineUsers();
        this.JoinGroup();
      } else {
        this.groupId = '';
        this.groupName = '';
      }
    });

    this.chatConnection.on("LeaveGroupOthers", async (onlineUser: string) => {
      this.usersOnline = this.usersOnline.filter((u) => u !== onlineUser);
      //await this.getOnlineUsers();
    })

    this.chatConnection.on('NewMessage', async (message: MessageChatToView) => {
      this.messages = [...this.messages, message];
    });

    this.chatConnection.on('RemoveOnlineUser', async (onlineUser: string) => {
      this.usersOnline = this.usersOnline.filter((u) => u !== onlineUser);
    });

    this.chatConnection.on('AddedToGroup', async (groupToView: GroupToView) => {
      this.groups = [...this.groups, groupToView];
      if (this.groups.length == 1) {
        this.groupName = groupToView.name;
        this.groupId = groupToView.id;
        this.JoinGroup();
        await this.getOldMessages();
        await this.addOnlineUser();
        await this.getOnlineUsers();
      }
    });

    this.chatConnection.on('GroupHasNewMessage', async (groupName: string) => {
      const groupHasNewMess = this.groups.find((g) => g.name === groupName);
      if (groupHasNewMess && groupName !== this.groupName) {
        groupHasNewMess.hasNewMessage = true;
        await this.sethasNewMess(groupHasNewMess, true);
      }
    });
  }

  // ==========================================================
  private async startConnection() {
    await this.chatConnection?.start().catch((error) => {
      console.log(error);
    });
  }

  async disConnentGroup(groupName: string, onlineUser: string) {
    return this.chatConnection
      ?.invoke('DisConnectedGroup', groupName, this.myName)
      .catch((err) => {
        console.log(err);
      });
  }

  async stopConnection() {
    await this.removeOnlineUser(this.groupName, this.myName);
    await this.disConnentGroup(this.groupName, this.myName);
    await this.chatConnection?.stop().catch((err) => {
      console.log(err);
    });
  }

  private async getMyName() {
    this.accountService.user$.pipe(take(1)).subscribe({
      next: (user: User | null) => {
        if (user) {
          this.myName = user.firstName + ' ' + user.lastName;
        }
      },
      error: (err) => {
        console.log(err);
      },
    });
  }

  private async getGroupsOfUser() {
    const result = await lastValueFrom(
      this.groupService.getGroupsOfUser(this.userId)
    )
      .then((res: any) => {
        if (res.length > 0) {
          this.groups = res;
          this.groupName = res[0].name;
          this.groupId = res[0].id;
        }
      })
      .catch((err) => {
        console.log(err);
      });
    return result;
  }

  async JoinGroup() {
    return this.chatConnection
      ?.invoke('ConnectGroup', this.groupName, this.userId)
      .then(() => {
        this.connectionId = this.chatConnection?.connectionId;
      })
      .catch((err) => {
        console.log(err);
      });
  }

  addGroupUser(groupName: string) {
    this.messages = [];
    this.chatConnection
      ?.invoke('AddNewGroup', groupName, this.userId)
      .catch((err) => {
        console.log(err);
      });
  }

  async leaveGroup(group: GroupToView) {
    return this.chatConnection
      ?.invoke('LeaveGroup', group.id, this.userId)
      .then(async () => {
        await this.removeOnlineUser(group.name, this.myName);
        await this.disConnentGroup(group.name, this.myName);
      })
      .catch((err) => {
        console.log(err);
      });
  }

  async getOldMessages() {
    if (this.groupId === '') return;
    const result = await lastValueFrom(
      this.messageService.getOldMessages(this.groupId)
    )
      .then((data: any) => {
        if (data) {
          this.messages = data;
        }
      })
      .catch((err) => {
        console.log(err);
      });
    return result;
  }

  async sendMessage(content: string) {
    const messageToSend: MessageChatToSend = {
      sender: this.myName,
      content: content,
    };
    return this.chatConnection
      ?.invoke('RecieveMessage', this.groupName, this.groupId, messageToSend)
      .catch((err) => {
        console.log(err);
      });
  }

  async switchGroup(group: GroupToView) {
    this.removeOnlineUser(this.groupName, this.myName);
    this.disConnentGroup(this.groupName, this.myName);

    this.groupId = group.id;
    this.groupName = group.name;
    group.hasNewMessage = false;
    this.usersOnline = [];
    this.messages = [];
    this.JoinGroup();
    await this.getOldMessages();
    await this.addOnlineUser();
    await this.getOnlineUsers();
    await this.sethasNewMess(group, false);
  }

  async addOnlineUser() {
    if (this.groupName === '') return;
    const result = await lastValueFrom(
      this.groupService.addOnlineUser(this.groupName, this.myName)
    )
      .then((data: any) => {
        return this.chatConnection
          ?.invoke('AddUsersOnline', this.groupName, data.userOnline)
          .catch((err) => {
            console.log(err);
          });
      })
      .catch((err) => {
        console.log(err);
      });
    return result;
  }

  async removeOnlineUser(groupName: string, onlineUser: string) {
    return this.chatConnection
      ?.invoke('RemoveOnlineUser', groupName, onlineUser)
      .catch((err) => {
        console.log(err);
      });
  }

  async getOnlineUsers() {
    if (this.groupName === '') return;

    const result = await lastValueFrom(
      this.groupService.getUsersOnline(this.groupName)
    ).then((data: any) => {
      this.usersOnline = data;
    });
    return result;
  }

  addMemberToGroup(email: string) {
    this.groupService.addUserToGroup(email, this.groupId).subscribe({
      next: (_) => {
        this.chatConnection
          ?.invoke('AddedToGroup', email, this.groupId)
          .catch((err) => {
            console.log(err);
          });

        this.shareService.showDialog(
          true,
          'Success',
          'Add member successfully'
        );
      },
      error: (err) => {
        console.log(err);
      },
    });
  }

  async sethasNewMess(groupHasNewMess: GroupToView, hasNewMessage: boolean) {
    return this.chatConnection
      ?.invoke('SetHasNewMessage', groupHasNewMess.id, hasNewMessage)
      .catch((err) => {
        console.log(err);
      });
  }
}
