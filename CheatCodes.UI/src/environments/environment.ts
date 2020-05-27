// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.

export const environment = {
  production: false,
  categoryUrl: 'https://localhost:57245/api/categories',
  categorySearchUrl: 'https://localhost:57246/api/CategoriesSearch',

  clientRoot: 'http://localhost:3214/',
  apiRoot: 'http://localhost:57243',
  stsAuthority: 'https://localhost:5002/',
  clientId: 'mainApp-api'
};
