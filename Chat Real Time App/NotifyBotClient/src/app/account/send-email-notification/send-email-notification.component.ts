import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-send-email-notification',
  templateUrl: './send-email-notification.component.html',
  styleUrls: ['./send-email-notification.component.scss']
})
export class SendEmailNotificationComponent {
  state: string | null = '';
  title: string | null = '';
  message: string | null = '';

  constructor(private activatedRoute: ActivatedRoute) {}

  // Trong phương thức hoặc sự kiện nào đó
  ngOnInit() {
    this.title = this.activatedRoute.snapshot.queryParamMap.get('title');
    this.message = this.activatedRoute.snapshot.queryParamMap.get('message');
    this.state = this.activatedRoute.snapshot.queryParamMap.get('state');
  }
}
