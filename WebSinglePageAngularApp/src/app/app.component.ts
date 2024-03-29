import { Component, OnInit } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { AuthService } from './_services/auth.service';
import { MessageService } from './_services/message.service';
import { PresenceService } from './_services/presence.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  title = 'IoTHub';
  jwtHelper = new JwtHelperService();
  
  constructor(private authService: AuthService, private presence: PresenceService, private message: MessageService){

  }
  ngOnInit(): void {
    const token = localStorage.getItem('token');
    if(token){
      this.authService.decodedToken = this.jwtHelper.decodeToken(token);
      this.presence.createHubConnection();
      //this.message.createHubConnection();

    }
  }

}
