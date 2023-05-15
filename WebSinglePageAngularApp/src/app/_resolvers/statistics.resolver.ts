import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { StatisticsEntry } from '../_models/statisticsEntry';
import { AlertifyService } from '../_services/alertify.service';
import { StatisticsService } from '../_services/statistics.service';

@Injectable()
export class StatisticsResolver implements Resolve<StatisticsEntry[]> {
  constructor(
    private statisticsService: StatisticsService,
    private router: Router,
    private alertify: AlertifyService,
  ) {}

  resolve(route: ActivatedRouteSnapshot): Observable<StatisticsEntry[]> {
    let deviceId = route.params['id'];
    let fieldId = route.params['fieldId'];
    return this.statisticsService.getStatistics(deviceId, fieldId, 0).pipe(
      catchError((error) => {
        this.alertify.error('You don`t have access to this data');
        this.router.navigate(['/devices/' + deviceId]);
        return of(null);
      })
    );
  }
}