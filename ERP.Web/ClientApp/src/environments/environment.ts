// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.

// export const environment = {
//   production: false
// };


export const environment = {
  production: false,
  productionUri: '',
 dev_uri: 'http://localhost:53779/api',
  //  dev_uri: 'https://api.khilafatcola.com/api',
   //dev_uri: 'http://202.166.160.200:9084/api',
  //dev_uri: 'http://110.39.5.82:3105/api',
  //reports_uri: 'http://report:Network@123@134.119.192.107:9097/',
  //reports_uri: 'http://report:Network@123@92.204.187.99:9097/',
  reports_uri: 'http://report:Network@123%202.166.160.200:9097/',
  backgroundStyle: 'dev',
  color : '#1e88e5',
  name  : 'Sehat Nizam',
  logo  : '/assets/Files/khilafatcola.png' 
}


