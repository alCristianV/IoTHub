import { Component, Inject, Optional } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Action } from 'src/app/_models/action';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-action-dialog-box',
  templateUrl: './action-dialog-box.component.html',
  styleUrls: ['./action-dialog-box.component.scss']
})
export class ActionDialogBoxComponent{

  action:string;
  local_data:any;

  constructor(
    public dialogRef: MatDialogRef<ActionDialogBoxComponent>,
    private messagesService: MessageService,
    //@Optional() is used to prevent error if no data is passed
    @Optional() @Inject(MAT_DIALOG_DATA) public data: Action) {
    console.log(data);
    this.local_data = {...data};
    this.action = this.local_data.action;
  }

  doAction(){
    this.dialogRef.close({event:this.action,data:this.local_data});
  }

  closeDialog(){
    this.dialogRef.close({event:'Cancel'});
  }
}
