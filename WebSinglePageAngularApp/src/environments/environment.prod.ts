export const environment = {
  production: false,

  authDomain:
    'authservice.greenflower-4193fb10.northeurope.azurecontainerapps.io',
  authService:
    'https://authservice.greenflower-4193fb10.northeurope.azurecontainerapps.io/api/auth/',

  usersDomain:
    'usersservice.greenflower-4193fb10.northeurope.azurecontainerapps.io',
  usersService:
    'https://usersservice.greenflower-4193fb10.northeurope.azurecontainerapps.io/api/users/',

  devicesDomain:
    'devicesservice.greenflower-4193fb10.northeurope.azurecontainerapps.io',
  devicesService:
    'https://devicesservice.greenflower-4193fb10.northeurope.azurecontainerapps.io/api/devices/',

  notificationsDomain: 'localhost:8083',
  notificationsService: 'http://localhost:8083/api/notifications/',

  statisticsDomain: 'localhost:8085',
  statisticsService: 'http://localhost:8085/api/statistics/',
  hubUrl: 'http://localhost:8082/hubs/',
  // apiUrl: 'http://192.168.0.104:5000/api/',
  // hubUrl: 'http://192.168.0.104:5000/hubs/'
};
