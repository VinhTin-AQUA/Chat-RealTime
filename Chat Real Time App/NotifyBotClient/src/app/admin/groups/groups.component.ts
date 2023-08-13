import { Component, OnInit } from '@angular/core';
import { GroupService } from 'src/app/chat-room/group.service';
import { GroupToView } from 'src/app/shared/models/chat/groupToView';
import { lastValueFrom } from 'rxjs';

@Component({
  selector: 'app-groups',
  templateUrl: './groups.component.html',
  styleUrls: ['./groups.component.scss'],
})
export class GroupsComponent implements OnInit {
  groups: GroupToView[] = [];
  members: string[] = [];
  onlines: string[] = [];

  constructor(private groupService: GroupService) {}

  ngOnInit(): void {
    this.getAllGroups();
  }

  private getAllGroups() {
    this.groupService.getAllGroup().subscribe({
      next: (res: any) => {
        this.groups = res;
      },
      error: (err) => {
        console.log(err);
      },
    });
  }

  private async getMembersOfGroup(group: GroupToView) {
    const result = await lastValueFrom(
      this.groupService.getUsersOFGroup(group.id)
    )
      .then((data: any) => {
        this.members = data;
      })
      .catch((err) => {
        console.log(err);
      });
    return result;
  }

  private async getOnlinesOfGroup(group: GroupToView) {
    const result = await lastValueFrom(
      this.groupService.getUsersOnline(group.name)
    )
      .then((data: any) => {
        this.onlines = data;
      })
      .catch((err) => {
        console.log(err);
      });

    return result;
  }

  async getOnlinesAndMembers(group: GroupToView) {
    await this.getMembersOfGroup(group);
    await this.getOnlinesOfGroup(group);
  }
}
