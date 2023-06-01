export const environment = {
  production: false,

  authDomain: 'localhost:8080',
  authService: 'http://localhost:8080/api/auth/',

  usersDomain: 'localhost:8081',
  usersService: 'http://localhost:8081/api/users/',

  devicesDomain: 'localhost:8082',
  devicesService: 'http://localhost:8082/api/devices/',

  notificationDomain: 'localhost:8083',
  notificationsService: 'http://localhost:8083/api/notifications/',

  statisticsDomain: 'localhost:8085',
  statisticsService: 'http://localhost:8085/api/statistics/',
  hubUrl: 'http://localhost:8082/hubs/',
  // apiUrl: 'http://192.168.0.104:5000/api/',
  // hubUrl: 'http://192.168.0.104:5000/hubs/'
};
