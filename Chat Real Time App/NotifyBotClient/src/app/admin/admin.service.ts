import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { UserToAdd } from '../shared/models/admin/userToAdd';
import { ReplaySubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  

  constructor(private http: HttpClient) {}

  
  getUsers(pageIndex: number, pageSize: number) {
    return this.http.get(
      `${environment.appUrl}/admin/get-users?pageIndex=${pageIndex}&pageSize=${pageSize}`
    );
  }

  countUsers() {
    return this.http.get(`${environment.appUrl}/admin/count-users`);
  }

  deleteUser(userId: string) {
    return this.http.delete(
      `${environment.appUrl}/admin/delete-user/${userId}`
    );
  }

  addNewUser(model: UserToAdd) {
    return this.http.post(`${environment.appUrl}/admin/add-user`, model);
  }

  searchUserByName(searchName: string, pageIndex: number, pageSize: number) {
    return this.http.get(
      `${environment.appUrl}/admin/search-users?searchString=${searchName}&pageIndex=${pageIndex}&pageSize=${pageSize}`
    );
  }

  lockUser(userId: string) {
    return this.http.put(
      `${environment.appUrl}/admin/lock-user/${userId}`,
      {}
    );
  }

  unlockUser(userId: string) {
    return this.http.put(
      `${environment.appUrl}/admin/unlock-user/${userId}`,
      {}
    );
  }

  deleteAllUsers() {
    return this.http.post(
      `${environment.appUrl}/admin/delete-all-users`,
      {}
    );
  }

  seedUsers(numberOfUsers: number, password: string) {
    return this.http.post(
      `${environment.appUrl}/admin/seed-users?numberOfUsers=${numberOfUsers}&password=${password}`,
      {}
    );
  }

  getApplicationRoles() {
    return this.http.get(
      `${environment.appUrl}/admin/get-application-roles`
    );
  }

  setRolesUser(userId: string | null, rolesUser: string[] | null) {
    return this.http.put(
      `${environment.appUrl}/admin/set-roles-user/${userId}`,
      rolesUser
    );
  }

  // contact
  getContacts(pageIndex: number, pageSize: number) {
    return this.http.get(
      `${environment.appUrl}/contact/get-contacts?pagIndex=${pageIndex}&pageSize=${pageSize}`
    );
  }

  deleteContact(contactId: string) {
    return this.http.delete(
      `${environment.appUrl}/contact/delete-contact?contactId=${contactId}`
    );
  }

  deleteAllContacts() {
    return this.http.delete(
      `${environment.appUrl}/contact/delete-all-contacts`
    );
  }
}
