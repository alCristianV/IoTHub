import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { StatisticsEntry } from '../_models/statisticsEntry';
import { Observable } from 'rxjs';

export enum StatisticsType {
  Daily,
  Weekly,
  Monthly,
}

@Injectable({
  providedIn: 'root',
})
export class StatisticsService {
  baseUrl = environment.statisticsService;
  constructor(private http: HttpClient) {}

  getStatistics(
    deviceId: string,
    fieldId: string,
    statisticsType: StatisticsType
  ): Observable<any> {
    return this.http.get<StatisticsEntry[]>(
      this.baseUrl + deviceId + '/' + fieldId + '/' + statisticsType
    );
  }
}
