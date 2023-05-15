import { Routes } from '@angular/router';
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { AuthGuard } from './_guards/auth.guard';
import { DevicesComponent } from './devices/devices.component';
import { DeviceDetailComponent } from './device-detail/device-detail.component';
import { DeviceDetailResolver } from './_resolvers/device-detail.resolver';
import { DevicesListResolver } from './_resolvers/devices-list.resolver';
import { NotificationsComponent } from './notifications/notifications.component';
import { NotificationResolver } from './_resolvers/notifications.resolver';
import { StatisticsComponent } from './statistics/statistics.component';
import { StatisticsResolver } from './_resolvers/statistics.resolver';

export const appRoutes: Routes = [
  { path: '', component: HomeComponent },
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children: [
      {
        path: 'devices',
        component: DevicesComponent,
        resolve: { devices: DevicesListResolver },
      },
      {
        path: 'devices/:id',
        component: DeviceDetailComponent,
        resolve: { device: DeviceDetailResolver },
      },
      {
        path: 'notifications',
        component: NotificationsComponent,
        resolve: { notifications: NotificationResolver },
      },
      {
        path: 'devices/:id/statistics/:fieldId',
        component: StatisticsComponent,
        resolve: {statistics: StatisticsResolver}
      }
    ],
  },

  { path: '**', redirectTo: '', pathMatch: 'full' },
];
