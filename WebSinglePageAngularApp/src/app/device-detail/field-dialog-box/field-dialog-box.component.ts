import { Component, Inject, Optional } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Field } from 'src/app/_models/field';

@Component({
  selector: 'app-field-dialog-box',
  templateUrl: './field-dialog-box.component.html',
  styleUrls: ['./field-dialog-box.component.scss']
})
export class FieldDialogBoxComponent{

  action:string;
  local_data:any;

  constructor(
    public dialogRef: MatDialogRef<FieldDialogBoxComponent>,
    //@Optional() is used to prevent error if no data is passed
    @Optional() @Inject(MAT_DIALOG_DATA) public data: Field) {
    console.log(data);
    this.local_data = {...data};
    this.local_data.type = "Numeric"
    this.action = this.local_data.action;
  }

  doAction(){
    this.dialogRef.close({event:this.action,data:this.local_data});
  }

  closeDialog(){
    this.dialogRef.close({event:'Cancel'});
  }

}
