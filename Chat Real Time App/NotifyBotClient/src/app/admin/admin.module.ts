import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AdminComponent } from './admin.component';
import { AdminNavComponent } from './admin-nav/admin-nav.component';
import { AdminRoutingModule } from './admin-routing.module';
import { UsersComponent } from './users/users.component';

import { MatPaginatorModule } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { MatSortModule } from '@angular/material/sort';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { AddUserComponent } from './add-user/add-user.component';
import { ReactiveFormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { OptionsComponent } from './options/options.component'; // Import FormsModule
import { MatDividerModule } from '@angular/material/divider';
import { EditRoleComponent } from './edit-role/edit-role.component';
import { MatSelectModule } from '@angular/material/select';
import { ContactOfUserComponent } from './contact-of-user/contact-of-user.component';
import { SharedModule } from '../shared/shared.module';
import { GroupsComponent } from './groups/groups.component';

@NgModule({
  declarations: [
    AdminComponent,
    AdminNavComponent,
    UsersComponent,
    AddUserComponent,
    OptionsComponent,
    EditRoleComponent,
    ContactOfUserComponent,
    GroupsComponent,
  ],
  imports: [
    CommonModule,
    AdminRoutingModule,
    MatPaginatorModule,
    MatTableModule,
    MatSortModule,
    MatButtonModule,
    MatIconModule,
    MatTooltipModule,
    ReactiveFormsModule,
    MatInputModule,
    FormsModule,
    MatDividerModule,
    MatSelectModule,
    SharedModule,
  ],
})
export class AdminModule {}
