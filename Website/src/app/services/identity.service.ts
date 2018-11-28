import { Injectable } from '@angular/core';
import { AmplifyService } from 'aws-amplify-angular';

@Injectable({
  providedIn: 'root'
})
export class IdentityService {
  userRole: [];
  isAdmin: false;

  constructor(private amplify: AmplifyService) { 
 
  }

  async getIdentity() {
    await this.amplify.auth().currentSession()
    .then((cognitoUserSession) => {
    const payload = cognitoUserSession.getIdToken().decodePayload();
    const groups = payload["cognito:groups"];
      this.userRole = groups;
      this.isAdmin = groups.includes("SiteAdmin");
  });
  }
}
