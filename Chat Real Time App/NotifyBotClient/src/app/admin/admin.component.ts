import { Component, OnInit } from '@angular/core';
import { AdminService } from './admin.service';
import { UserView } from '../shared/models/admin/userView';

@Component({
  selector: 'app-admin-dashboard',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss']
})
export class AdminComponent{
  constructor(public adminService: AdminService) {}
}
