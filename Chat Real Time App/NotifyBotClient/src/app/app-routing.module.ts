import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { ContactComponent } from './contact/contact.component';
import { AboutComponent } from './about/about.component';
import { AuthGuard } from './shared/guards/auth.guard';
import { AdminGuard } from './shared/guards/admin.guard';

const routes: Routes = [
  { path: '', component: HomeComponent, title: 'Home' },
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children: [
      {
        path: 'chat-rooms',
        loadChildren: () =>
          import('../app/chat-room/chat-room.module').then(
            (m) => m.ChatRoomModule
          ),
      },
    ],
  },
  { path: 'contact', component: ContactComponent, title: 'Contact us' },
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [AdminGuard],
    children: [
      {
        path: 'admin',
        loadChildren: () =>
          import('../app/admin/admin.module').then((m) => m.AdminModule),
      },
    ],
  },

  { path: 'about', component: AboutComponent, title: 'About' },
  // lazy loading
  {
    path: 'account',
    loadChildren: () =>
      import('../app/account/account.module').then((m) => m.AccountModule),
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { onSameUrlNavigation: 'reload' })],
  exports: [RouterModule],
})
export class AppRoutingModule {}
