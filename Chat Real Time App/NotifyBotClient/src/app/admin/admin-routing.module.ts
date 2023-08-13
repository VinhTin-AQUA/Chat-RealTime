import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminComponent } from './admin.component';
import { UsersComponent } from './users/users.component';
import { AddUserComponent } from './add-user/add-user.component';
import { OptionsComponent } from './options/options.component';
import { EditRoleComponent } from './edit-role/edit-role.component';
import { ContactOfUserComponent } from './contact-of-user/contact-of-user.component';
import { GroupsComponent } from './groups/groups.component';

const routes: Routes = [
  {
    path: '',
    component: AdminComponent,
    children: [
      { path: '', component: UsersComponent, title: 'Admin dashboard' },
      { path: 'add-user', component: AddUserComponent, title: 'Add user' },
      { path: 'options', component: OptionsComponent, title: 'Options' },
      { path: 'edit-role/:userId', component: EditRoleComponent, title: 'Edit role' },
      { path: 'contacts-of-user', component: ContactOfUserComponent, title: 'Contacts of user' },
      { path: 'groups', component: GroupsComponent, title: 'Groups' },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminRoutingModule {}
