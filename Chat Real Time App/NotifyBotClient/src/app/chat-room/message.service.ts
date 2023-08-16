import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class MessageService {
  constructor(private http: HttpClient) {}

  getOldMessages(groupId: string) {
    return this.http.get(
      `${environment.appUrl}/messagechat/messages-of-group?groupId=${groupId}`
    );
  }
}
