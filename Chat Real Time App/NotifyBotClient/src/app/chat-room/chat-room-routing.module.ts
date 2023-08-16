import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RoomComponent } from './room.component';
import { SearchMemberComponent } from './search-member/search-member.component';
import { ChatComponent } from './chat/chat.component';

const routes: Routes = [
  {
    path: '',
    component: RoomComponent,
    title: 'chat rooms',
    children: [
      { path: '', component: ChatComponent, title: 'chat' },
      { path: 'search-member', component: SearchMemberComponent, title: 'search' },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ChatRoomRoutingModule {}
