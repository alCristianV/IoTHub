// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  authService: 'http://localhost:8080/api/auth/',
  usersService: 'http://localhost:8081/api/users/',
  devicesService: 'http://localhost:8082/api/devices/',
  notificationsService: 'http://localhost:8083/api/notifications/',
  statisticsService: 'http://localhost:8085/api/statistics/',
  hubUrl: 'http://localhost:8082/hubs/',
  // apiUrl: 'http://192.168.0.104:5000/api/',
  // hubUrl: 'http://192.168.0.104:5000/hubs/'
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
