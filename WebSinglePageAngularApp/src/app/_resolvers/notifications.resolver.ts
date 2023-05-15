import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Device } from '../_models/device';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { NotificationsService } from '../_services/notifications.service';

@Injectable()
export class NotificationResolver implements Resolve<Device[]> {
  constructor(
    private notificationService: NotificationsService,
    private router: Router,
    private alertify: AlertifyService,
    private authService: AuthService
  ) {}

  resolve(route: ActivatedRouteSnapshot): Observable<Device[]> {
    let userId = this.authService.decodedToken?.nameid;
    return this.notificationService.getUserNotifications(userId).pipe(
      catchError(() => {
        this.alertify.error('Problem retrieving data');
        this.router.navigate(['/home']);
        return of(null);
      })
    );
  }
}
