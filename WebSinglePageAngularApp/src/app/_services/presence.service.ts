import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root',
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  private onlineDevicesSource = new BehaviorSubject<string[]>([]);
  onlineDevices$ = this.onlineDevicesSource.asObservable();

  constructor() {}

  createHubConnection() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => localStorage.getItem('token'),
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch((error) => console.log(error));

    console.log('presenceHub connected');
    this.hubConnection.on('DeviceIsOnline', () => {});
    this.hubConnection.on('DeviceIsOffline', () => {});

    this.hubConnection.on('GetOnlineDevices', (devicesIds: string[]) => {
      this.onlineDevicesSource.next(devicesIds);
    });
  }

  stopHubConnection() {
    this.hubConnection.stop().catch((error) => console.log(error));
  }
}
