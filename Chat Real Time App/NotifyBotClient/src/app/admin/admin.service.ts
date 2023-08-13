import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';
import { UserToAdd } from '../shared/models/admin/userToAdd';
import { ReplaySubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  

  constructor(private http: HttpClient) {}

  
  getUsers(pageIndex: number, pageSize: number) {
    return this.http.get(
      `${environment.appUrl}/api/admin/get-users?pageIndex=${pageIndex}&pageSize=${pageSize}`
    );
  }

  countUsers() {
    return this.http.get(`${environment.appUrl}/api/admin/count-users`);
  }

  deleteUser(userId: string) {
    return this.http.delete(
      `${environment.appUrl}/api/admin/delete-user/${userId}`
    );
  }

  addNewUser(model: UserToAdd) {
    return this.http.post(`${environment.appUrl}/api/admin/add-user`, model);
  }

  searchUserByName(searchName: string, pageIndex: number, pageSize: number) {
    return this.http.get(
      `${environment.appUrl}/api/admin/search-users?searchString=${searchName}&pageIndex=${pageIndex}&pageSize=${pageSize}`
    );
  }

  lockUser(userId: string) {
    return this.http.put(
      `${environment.appUrl}/api/admin/lock-user/${userId}`,
      {}
    );
  }

  unlockUser(userId: string) {
    return this.http.put(
      `${environment.appUrl}/api/admin/unlock-user/${userId}`,
      {}
    );
  }

  deleteAllUsers() {
    return this.http.post(
      `${environment.appUrl}/api/admin/delete-all-users`,
      {}
    );
  }

  seedUsers(numberOfUsers: number, password: string) {
    return this.http.post(
      `${environment.appUrl}/api/admin/seed-users?numberOfUsers=${numberOfUsers}&password=${password}`,
      {}
    );
  }

  getApplicationRoles() {
    return this.http.get(
      `${environment.appUrl}/api/admin/get-application-roles`
    );
  }

  setRolesUser(userId: string | null, rolesUser: string[] | null) {
    return this.http.put(
      `${environment.appUrl}/api/admin/set-roles-user/${userId}`,
      rolesUser
    );
  }

  // contact
  getContacts(pageIndex: number, pageSize: number) {
    return this.http.get(
      `${environment.appUrl}/api/contact/get-contacts?pagIndex=${pageIndex}&pageSize=${pageSize}`
    );
  }

  deleteContact(contactId: string) {
    return this.http.delete(
      `${environment.appUrl}/api/contact/delete-contact?contactId=${contactId}`
    );
  }

  deleteAllContacts() {
    return this.http.delete(
      `${environment.appUrl}/api/contact/delete-all-contacts`
    );
  }
}
