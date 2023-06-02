export const environment = {
  production: false,

  authDomain:
    'authservice.greenflower-4193fb10.northeurope.azurecontainerapps.io',
  authService:
    'https://authservice.greenflower-4193fb10.northeurope.azurecontainerapps.io/api/auth/',

  usersDomain: 'localhost:8081',
  usersService: 'http://localhost:8081/api/users/',

  devicesDomain: 'localhost:8082',
  devicesService: 'http://localhost:8082/api/devices/',

  notificationsDomain: 'localhost:8083',
  notificationsService: 'http://localhost:8083/api/notifications/',

  statisticsDomain: 'localhost:8085',
  statisticsService: 'http://localhost:8085/api/statistics/',
  hubUrl: 'http://localhost:8082/hubs/',
  // apiUrl: 'http://192.168.0.104:5000/api/',
  // hubUrl: 'http://192.168.0.104:5000/hubs/'
};
