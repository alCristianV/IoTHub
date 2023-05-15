import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import {Notification} from '../_models/notification';
import { AlertifyService } from '../_services/alertify.service';
import { NotificationsService } from '../_services/notifications.service';

@Component({
  selector: 'app-notifications',
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.scss']
})
export class NotificationsComponent implements OnInit {

  notifications: Notification[];
  constructor(private route: ActivatedRoute, private notificationsService: NotificationsService,private alertify: AlertifyService,) {
    this.route.data.subscribe((data) => {
      this.notifications = data['notifications'];
      console.log(data);
    });
   }
   
  ngOnInit() {
  }

  getFirstLine(notification: string):string{
    return notification.split('\n')[0];
  }

  deleteNotification(notification: Notification){
    this.notificationsService.deleteNotification(notification?.id).subscribe(() => {
      this.notifications = this.notifications.filter(obj => obj.id !== notification?.id);
      this.alertify.success("Notification removed successfully");
    },
    (error) => {
      this.alertify.error(error);
    })
  }
  }

