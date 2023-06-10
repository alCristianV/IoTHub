import { EventEmitter, Injectable, Output } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { environment } from 'src/environments/environment';
import { Action } from '../_models/action';
import { Field } from '../_models/field';

@Injectable({
  providedIn: 'root',
})
export class MessageService {
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  private message: any;

  @Output() onSignalRActionError: EventEmitter<any> = new EventEmitter();
  @Output() onSignalRFieldError: EventEmitter<any> = new EventEmitter();
  constructor() {}

  createHubConnection(deviceId: string) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'message?device=' + deviceId, {
        accessTokenFactory: () => localStorage.getItem('token'),
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch((error) => console.log(error));

    this.hubConnection.on('NewMessage', (msg) => {
      this.message = msg;
      console.log(this.message);
      //console.log(msg);
    });

    this.hubConnection.on('Error', (data) => {
      let obj: Action = JSON.parse(data);
      this.newActionError(obj);
    });

    this.hubConnection.on('FieldError', (data) => {
      let obj: Field = JSON.parse(data);
      this.newFieldError(obj);
    });
  }

  stopHubConnection() {
    this.hubConnection.stop().catch((error) => console.log(error));
  }

  getMessage() {
    return this.message;
  }

  invokeAction(action: Action) {
    this.hubConnection.send('InvokeAction', action);
  }

  fieldsChanged(changed: boolean) {
    this.hubConnection.send('ChangedField', true);
  }

  private newActionError(data) {
    this.onSignalRActionError.emit(data);
  }

  private newFieldError(data) {
    this.onSignalRFieldError.emit(data);
  }
}
