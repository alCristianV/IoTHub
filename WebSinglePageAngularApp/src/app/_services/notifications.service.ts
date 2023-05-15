import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class NotificationsService {
baseUrl = environment.apiUrl + 'notifications/';
constructor(private http: HttpClient) { }

getUserNotifications(userId: string): Observable<Notification[]> {
  return this.http.get<Notification[]>(this.baseUrl + userId);
}

deleteNotification(notificationId: string) {
  return this.http.delete(this.baseUrl + notificationId);
}
}
