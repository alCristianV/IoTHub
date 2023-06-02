import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ChartsModule } from 'ng2-charts';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { JwtModule } from '@auth0/angular-jwt';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { appRoutes } from './routes';
import { AppComponent } from './app.component';
import { NavComponent } from './nav/nav.component';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { ErrorInterceptorProvider } from './_services/error.interceptor';
import { DevicesComponent } from './devices/devices.component';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { MatDialogModule } from '@angular/material/dialog';
import { DialogBoxComponent } from './dialog-box/dialog-box.component';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatExpansionModule } from '@angular/material/expansion';
import { DeviceDetailComponent } from './device-detail/device-detail.component';
import { AuthService } from './_services/auth.service';
import { AlertifyService } from './_services/alertify.service';
import { DeviceDetailResolver } from './_resolvers/device-detail.resolver';
import { DevicesListResolver } from './_resolvers/devices-list.resolver';
import { EditInputComponent } from './edit-input/edit-input.component';
import { AutofocusDirective } from './edit-input/autofocus.directive';
import { FieldDialogBoxComponent } from './device-detail/field-dialog-box/field-dialog-box.component';
import { ActionDialogBoxComponent } from './device-detail/action-dialog-box/action-dialog-box.component';
import { ClipboardModule } from 'ngx-clipboard';
import { LoadingScreenComponent } from './loading-screen/loading-screen.component';
import { LoadingScreenInterceptor } from './loading-screen/loading.interceptor';
import { NotificationsComponent } from './notifications/notifications.component';
import { NotificationResolver } from './_resolvers/notifications.resolver';
import { StatisticsComponent } from './statistics/statistics.component';
import { StatisticsResolver } from './_resolvers/statistics.resolver';
import { environment } from 'src/environments/environment';

export function tokenGetter() {
  return localStorage.getItem('token');
}

@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    HomeComponent,
    RegisterComponent,
    DevicesComponent,
    DialogBoxComponent,
    DeviceDetailComponent,
    EditInputComponent,
    AutofocusDirective,
    FieldDialogBoxComponent,
    ActionDialogBoxComponent,
    LoadingScreenComponent,
    NotificationsComponent,
    StatisticsComponent,
  ],
  imports: [
    BrowserModule,
    FormsModule,
    ChartsModule,
    HttpClientModule,
    BsDropdownModule.forRoot(),
    BrowserAnimationsModule,
    RouterModule.forRoot(appRoutes),
    MatProgressSpinnerModule,
    MatTableModule,
    MatDialogModule,
    MatFormFieldModule,
    MatExpansionModule,
    ClipboardModule,
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        // allowedDomains: ['192.168.0.104:5000'],
        // disallowedRoutes: ['192.168.0.104:5000/api/auth']
        //allowedDomains: ['localhost:8082'],
        allowedDomains: [
          environment.authDomain,
          environment.usersDomain,
          environment.devicesDomain,
          environment.statisticsDomain,
          environment.statisticsDomain,
          environment.notificationsDomain,
        ],
        disallowedRoutes: [environment.authService],
      },
    }),
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: LoadingScreenInterceptor,
      multi: true,
    },
    ErrorInterceptorProvider,
    AuthService,
    AlertifyService,
    DeviceDetailResolver,
    DevicesListResolver,
    NotificationResolver,
    StatisticsResolver,
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
