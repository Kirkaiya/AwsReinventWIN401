import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';
import { environment } from './environments/environment';
import Amplify from 'aws-amplify';

Amplify.configure({
  Auth:{
    identityPoolId: 'us-west-2:e771c849-abbc-48df-8539-1ccf1c97a0f9',
    region: 'us-west-2',
    userPoolId: 'us-west-2_megh2msxd',
    userPoolWebClientId: '288seubnkumcdnj3odpftsvbjl',
  }
});

if (environment.production) {
  enableProdMode();
}

platformBrowserDynamic().bootstrapModule(AppModule)
  .catch(err => console.error(err));
