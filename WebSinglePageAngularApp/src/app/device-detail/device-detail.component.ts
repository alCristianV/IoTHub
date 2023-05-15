import { Component, OnChanges, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Device } from '../_models/device';
import { AlertifyService } from '../_services/alertify.service';
import { DevicesService } from '../_services/devices.service';
import { MessageService } from '../_services/message.service';
import { MatDialog } from '@angular/material/dialog';
import { FieldDialogBoxComponent } from './field-dialog-box/field-dialog-box.component';
import { Field } from '../_models/field';
import { Action } from '../_models/action';
import { ActionDialogBoxComponent } from './action-dialog-box/action-dialog-box.component';
import { ClipboardService } from 'ngx-clipboard';
import { Value } from '../_models/value';
import { PresenceService } from '../_services/presence.service';
import { User } from '../_models/user';

@Component({
  selector: 'app-device-detail',
  templateUrl: './device-detail.component.html',
  styleUrls: ['./device-detail.component.scss'],
})
export class DeviceDetailComponent implements OnInit, OnDestroy {

  device: Device
  currentValues: Value[] = []
  collaboratorEmail: string

  constructor(
    public dialog: MatDialog,
    private deviceService: DevicesService,
    private alertify: AlertifyService,
    private route: ActivatedRoute,
    private router: Router,
    private messageService: MessageService,
    private _clipboardService: ClipboardService,
    public presence: PresenceService
    ) { }

//message: string = "{\n\tid: exempluMesaj,\n\ttemperature: 32.72\n\thumidity: 50%\n}";
messagesColumns: string[] = [
  'Field',
  'Value',
  ' '
];

actionsColumns: string[] = [
  'Action',
  ' '
];
  ngOnInit() {
    this.route.data.subscribe(data => {
      this.device = data.device
      this.deviceService.getFieldsForDevice(this.device.id).subscribe(fields => this.device.fields = fields)
      this.deviceService.getActionsForDevice(this.device.id).subscribe(actions => this.device.actions = actions)
      console.log(this.device)
    })
    this.displayMessage();
    this.messageService.onSignalRActionError.subscribe((action: Action) => {console.log(action?.error)
      var acc = this.device.actions.find(a => a.id === action.id);
      acc.error = action.error;
      console.log('dialog')
      this.openActionDialog('Update',acc)
    }
      )

      this.messageService.onSignalRFieldError.subscribe((field: Field) => {console.log(field?.error)
        var f = this.device.fields.find(a => a.id === field.id);
        f.error = field.error;
        var currentField = this.device.fields.find(f => f.id == field.id);
        currentField.error = field.error
      }
        )
  }

  openFieldDialog(action, obj) {
    obj.action = action;
    const dialogRef = this.dialog.open(FieldDialogBoxComponent, {
      data: obj,
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result.event == 'Add') {
        this.addFieldRowData(result.data);
        console.log(result.data)
      } else if (result.event == 'Update') {
        this.updateFieldRowData(result.data);
      } else if (result.event == 'Delete') {
        console.log(result.data);
        this.deleteFieldRowData(result.data);
      }
    });
  }

  deleteFieldRowData(row_obj){
    var fieldId = row_obj.id
    this.deviceService
      .deleteFieldFromDevice(this.device.id, fieldId)
      .subscribe(
        (result: Field) => {
          this.device.fields = this.device.fields.filter(obj => obj.id !== fieldId);
          this.messageService.fieldsChanged(true);
          this.alertify.success('Field Successfully Deleted');
        },
        (error) => {
          this.alertify.error(error);
        }
      );
  }

  addFieldRowData(row_obj) {
    var field: Field = {name: row_obj.name, type: row_obj.type, statistics: row_obj.statistics, code: row_obj.code };
    this.deviceService
      .addFieldToDevice(this.device.id, field)
      .subscribe(
        (result: Field) => {
          result.error = null
          this.device.fields.push(result);
          this.messageService.fieldsChanged(true);
          this.alertify.success('Field Successfully Created');
        },
        (error) => {
          this.alertify.error(error);
        }
      );
  }

  updateFieldRowData(row_obj) {
    console.log(row_obj)
    var field: Field = {name: row_obj.name, type: row_obj.type, statistics: row_obj.statistics, id:row_obj.id, code: row_obj.code };
    this.deviceService
      .updateField(field)
      .subscribe(
        (result: Field) => {
          console.log(result)          
          this.deviceService.getFieldsForDevice(this.device.id)
          .subscribe(fields => {this.device.fields = fields})
          this.messageService.fieldsChanged(true);
          this.alertify.success('Field Successfully Updated');
        },
        (error) => {
          this.alertify.error(error);
        }
      );
  }

  addActionRowData(row_obj) {
    var action: Action = {name: row_obj.name, code: row_obj.code };
    console.log(action)
    this.deviceService
      .addActionToDevice(this.device.id, action)
      .subscribe(
        (result: Action) => {
          this.device.actions.push(result);
          this.alertify.success('Action Successfully Created');
        },
        (error) => {
          this.alertify.error(error);
        }
      );
  }

  updateActionRowData(row_obj) {
    console.log(row_obj)
    var action: Action = {name: row_obj.name, id:row_obj.id, deviceId:row_obj.deviceId, code: row_obj.code };
    this.deviceService
      .updateAction(action)
      .subscribe(
        (result: Action) => {
          console.log(result)          
          this.deviceService.getActionsForDevice(this.device.id)
          .subscribe(actions => {this.device.actions = actions})
          this.alertify.success('Action Successfully Updated');
        },
        (error) => {
          this.alertify.error(error);
        }
      );
  }

  deleteActionRowData(row_obj){
    var actionId = row_obj.id
    this.deviceService
      .deleteActionFromDevice(this.device.id, actionId)
      .subscribe(
        (result: Field) => {
          this.device.actions = this.device.actions.filter(obj => obj.id !== actionId);
          this.alertify.success('Action Successfully Deleted');
        },
        (error) => {
          this.alertify.error(error);
        }
      );
  }

  openActionDialog(action, obj) {
    obj.action = action;
    const dialogRef = this.dialog.open(ActionDialogBoxComponent, {
      data: obj,
    });

    dialogRef.afterClosed().subscribe((result) => {
      if (result.event == 'Add') {
        this.addActionRowData(result.data);
      }else if (result.event == 'Update') {
        this.updateActionRowData(result.data);
        console.log(result.data);
      } else if (result.event == 'Delete') {
        console.log(result.data);
        this.deleteActionRowData(result.data);
      }
    });
  }

  saveName(value){
    var currentName = this.device.name;
    this.device.name=value;
    this.deviceService.updateDevice(this.device).subscribe(
      () => {
      },
      (error) => {
        this.alertify.error(error);
        this.device.name = currentName;
      }
    );
  }

  deleteDevice(){
    if(window.confirm('Are sure you want to delete this item ?')){
    this.deviceService.deleteDevice(this.device.id).subscribe(
      () => {
        this.router.navigate(['/devices']);
        this.alertify.success('Device deleted successfully!');
      },
      (error) => {
        this.alertify.error(error);
      }
    )
  }
}

displayMessage(){
  this.messageService.createHubConnection(this.device.id);
}

getMessageValue(fieldName: string): string{
  var values = this.messageService.getMessage()?.values
  var value: string = values?.find(x => x?.dataField?.name === fieldName)?.['value']
  if(value === undefined){
    var f = this.currentValues.find(d => d.dataField === fieldName)
    return f?.value
  }
  else{    
    var fieldValue: Value = {dataField: fieldName, value: value, date: this.messageService.getMessage()?.date}
    fieldValue.dataField = fieldName;
    fieldValue.value = value;
    this.currentValues = this.currentValues.filter(obj => obj.dataField !== fieldName);
    this.currentValues.push(fieldValue)
    return value;
  }
  
}

getMessageDate(fieldName: string): string{
  var date = this.messageService.getMessage()?.date
  return date;
}

copyConnectionString(){
  this._clipboardService.copy(this.device.connectionString)
  this.alertify.success("Connection String Copied");
}

addCollaborator(){
  console.log(this.collaboratorEmail)
  this.deviceService.addCollaboratorToDevice(this.device.id,this.collaboratorEmail).subscribe((m: User) => {
    console.log(m)
    this.device.collaborators.push(m)
    this.alertify.success(this.collaboratorEmail + " was added successfully as a collaborator");
    this.collaboratorEmail = null;
  },
  (error) => {
    this.alertify.error(error);
  })
}

removeCollaborator(collab : User){
  console.log(this.collaboratorEmail)
  this.deviceService.deleteCollaboratorFromDevice(this.device.id,collab.id).subscribe(() => {
    this.device.collaborators = this.device.collaborators.filter(obj => obj.id !== collab.id);
    this.alertify.success(collab.email + " was removed as a collaborator");
  },
  (error) => {
    this.alertify.error(error);
  })
}

invokeAction(action: Action){
  console.log(action)
  this.messageService.invokeAction(action);
}

ngOnDestroy(): void {
  this.messageService.stopHubConnection();
}

}
