import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ChartDataSets } from 'chart.js';
import { Color, Label } from 'ng2-charts';
import { StatisticsEntry } from '../_models/statisticsEntry';
import { DatePipe } from '@angular/common'
import {Location} from '@angular/common';
import { StatisticsService } from '../_services/statistics.service';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-statistics',
  templateUrl: './statistics.component.html',
  styleUrls: ['./statistics.component.scss'],
})
export class StatisticsComponent implements OnInit {
  lineChartData: ChartDataSets[]

  lineChartLabels: Label[];

  lineChartOptions = {
    responsive: true,
  };

  lineChartColors: Color[] = [
    {
      borderColor: 'black',
      backgroundColor: 'rgb(195,238,255)',
    },
  ];

  lineChartLegend = true;
  lineChartPlugins = [];
  lineChartType = 'line';
  
  statistics: StatisticsEntry[]
  fieldName: string
  
  constructor(private route: ActivatedRoute, private _location: Location, private statistictService: StatisticsService) {
    this.route.data.subscribe((data) => {
      this.statistics = data["statistics"]["values"];
      this.fieldName = data["statistics"]["fieldName"];
      this.setProperties()
    });
  }

  ngOnInit() {
    
  }

  backClicked() {
    this._location.back();
  }

  onOptionsSelected(value:string){
    var deviceId = this.route.snapshot.paramMap.get('id')
    let fieldId = this.route.snapshot.paramMap.get('fieldId')
    this.statistictService.getStatistics(deviceId,fieldId, Number.parseInt(value))
    .subscribe(data => {
      var values = data["values"]
      this.statistics = values
      this.fieldName = data["fieldName"];
      this.setProperties()
    });
}

setProperties(){
  this.lineChartData = [
    { data: Array.from(this.statistics, d => d.value), label: this.fieldName },
  ];
  this.lineChartLabels = Array.from(this.statistics, d =>{ 
    var datePipe = new DatePipe('en-US');
    console.log(d.date)
    var newDate = datePipe.transform(d.date,'short')
    return newDate;
  })
}

}
