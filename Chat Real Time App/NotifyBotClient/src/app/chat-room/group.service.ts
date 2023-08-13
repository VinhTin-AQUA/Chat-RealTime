import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class GroupService {
  constructor(private http: HttpClient) {}

  getGroupsOfUser(userId: string) {
    return this.http.get(
      `${environment.appUrl}/api/group/get-group-of-user?userId=${userId}`
    );
  }

  addUserToGroup(email: string, groupId: string) {
    return this.http.post(
      `${environment.appUrl}/api/group/add-user-to-group?groupId=${groupId}&email=${email}`,
      {}
    );
  }

  getUsersOnline(groupName: string) {
    return this.http.get(
      `${environment.appUrl}/api/group/get-users-online-of-group?groupName=${groupName}`
    );
  }

  addOnlineUser(groupName: string, userName: string) {
    return this.http.post(
      `${environment.appUrl}/api/group/add-user-online?groupName=${groupName}&userName=${userName}`,
      {}
    );
  }

  getAllGroup() {
    return this.http.get(`${environment.appUrl}/api/group/get-groups`);
  }

  getUsersOFGroup(groupId: string) {
    return this.http.get(`${environment.appUrl}/api/group/users-of-a-group?groupId=${groupId}`);
  }
}
