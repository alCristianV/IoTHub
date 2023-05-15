import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTable } from '@angular/material/table';
import { ActivatedRoute } from '@angular/router';
import { DialogBoxComponent } from '../dialog-box/dialog-box.component';
import { Device } from '../_models/device';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { DevicesService } from '../_services/devices.service';
import { PresenceService } from '../_services/presence.service';
import { StatisticsService, StatisticsType } from '../_services/statistics.service';
@Component({
  selector: 'app-devices',
  templateUrl: './devices.component.html',
  styleUrls: ['./devices.component.scss']
})
export class DevicesComponent {
  displayedColumns: string[] = [
    'Device Name',
    'Status',
  ];
  dataSource: Device[];

  @ViewChild(MatTable, { static: true }) table: MatTable<any>;

  constructor(
    public dialog: MatDialog,
    private devicesService: DevicesService,
    private authService: AuthService,
    private route: ActivatedRoute,
    private alertify: AlertifyService,
    public presence: PresenceService
  ) {
    this.route.data.subscribe((data) => {
      this.dataSource = data['devices'];
      console.log(data);
    });
  }

  openDialog(action, obj) {
    obj.action = action;
    const dialogRef = this.dialog.open(DialogBoxComponent, {
      data: obj,
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result.event == 'Add') {
        this.addRowData(result.data);
      } else if (result.event == 'Update') {
        this.updateRowData(result.data);
      } else if (result.event == 'Delete') {
        this.deleteRowData(result.data);
      }
    });

  }

  addRowData(row_obj) {
    var d = new Date();
    var device: Device = { id: row_obj.id, name: row_obj.name };
    this.devicesService
      .addUserDevice(this.authService.decodedToken?.nameid, device)
      .subscribe(
        (result: Device) => {
          this.dataSource.push(result);
        },
        (error) => {
          this.alertify.error(error);
        }
      );
  }
  updateRowData(row_obj) {
    this.dataSource = this.dataSource.filter((value, key) => {
      if (value.id == row_obj.id) {
        value.name = row_obj.name;
      }
      return true;
    });
  }
  deleteRowData(row_obj) {
    this.dataSource = this.dataSource.filter((value, key) => {
      return value.id != row_obj.id;
    });
  }

  singleClick(row) {
    console.log(row);
  }
}
