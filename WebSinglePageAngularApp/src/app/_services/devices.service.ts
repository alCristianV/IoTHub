import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Action } from '../_models/action';
import { Device } from '../_models/device';
import { Field } from '../_models/field';
import { MessageService } from './message.service';


@Injectable({
  providedIn: 'root',
})
export class DevicesService {
  baseUrl = environment.apiUrl + 'devices/';

  constructor(private http: HttpClient, private messagesService: MessageService) {}

  getUserDevices(userId: string): Observable<Device[]> {
    return this.http.get<Device[]>(this.baseUrl + 'GetUserDevices/' + userId);
  }

  getUserDevice(userId: string, deviceId: string): Observable<Device> {
    return this.http.get<Device>(
      this.baseUrl + 'GetUserDevice/' + userId + '/' + deviceId
    );
  }

  getDevice(deviceId: string): Observable<Device> {
    return this.http.get<Device>(this.baseUrl + deviceId);
  }

  addUserDevice(userId: string, device: Device) {
    return this.http.post(this.baseUrl + userId, device);
  }

  updateDevice(device: Device) {
    return this.http.put(this.baseUrl + device.id, device);
  }

  deleteDevice(deviceId: string) {
    return this.http.delete(this.baseUrl + deviceId);
  }

  getFieldsForDevice(deviceId: string): Observable<Field[]>{
    return this.http.get<Field[]>(this.baseUrl + 'GetDeviceFields/' + deviceId);
  }

  addFieldToDevice(deviceId: string, field: Field){
    return this.http.post(this.baseUrl +  deviceId + '/fields', field)
  }

  updateField(field: Field) {
    return this.http.put(this.baseUrl + 'fields/' + field.id, field);
  }

  deleteFieldFromDevice(deviceId: string,fieldId: string){
    return this.http.delete(this.baseUrl +  deviceId + '/fields/' + fieldId)
  }

  getActionsForDevice(deviceId: string): Observable<Action[]>{
    return this.http.get<Action[]>(this.baseUrl + 'GetDeviceActions/' + deviceId);
  }

  addActionToDevice(deviceId: string, action: Action){
    return this.http.post(this.baseUrl +  deviceId + '/actions', action)
  }

  updateAction(action: Action) {
    return this.http.put(this.baseUrl + 'actions/' + action.id, action);
  }

  deleteActionFromDevice(deviceId: string,actionId: string){
    return this.http.delete(this.baseUrl +  deviceId + '/actions/' + actionId)
  }

  addCollaboratorToDevice(deviceId: string, collaboratorEmail: string){
    console.log(collaboratorEmail)
    return this.http.post(this.baseUrl +  deviceId + '/collaborators?collaboratorEmail=' +collaboratorEmail , collaboratorEmail)
  }

  deleteCollaboratorFromDevice(deviceId: string, collaboratorId: string){
    return this.http.delete(this.baseUrl +  deviceId + '/collaborators/'+ collaboratorId)
  }

}
