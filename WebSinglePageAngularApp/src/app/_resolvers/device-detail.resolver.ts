import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Device } from '../_models/device';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { DevicesService } from '../_services/devices.service';

@Injectable()
export class DeviceDetailResolver implements Resolve<Device> {
  constructor(
    private devicesService: DevicesService,
    private router: Router,
    private alertify: AlertifyService,
    private authService: AuthService
  ) {}

  resolve(route: ActivatedRouteSnapshot): Observable<Device> {
    let userId = this.authService.decodedToken?.nameid;
    return this.devicesService.getUserDevice(userId, route.params['id']).pipe(
      catchError((error) => {
        this.alertify.error('You don`t have access to this data');
        this.router.navigate(['/devices']);
        return of(null);
      })
    );
  }
}
