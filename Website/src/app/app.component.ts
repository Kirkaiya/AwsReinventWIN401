import { Component, OnInit } from '@angular/core';
import { AmplifyService } from 'aws-amplify-angular';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AuthComponent } from './auth/auth.component';
import { IdentityService } from './services/identity.service';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'ECommerce';
  isLoggedIn = false;
  username: string;
  admin = "Admin";

  constructor(
    private amplifyService: AmplifyService,
    private modalService: NgbModal,
    private identity: IdentityService) {
    }
      async ngOnInit() {
        try {
          const user = await this.amplifyService.auth().currentAuthenticatedUser();
          console.log(user);
          this.isLoggedIn = true;
          await this.identity.getIdentity();
          this.username = this.identity.isAdmin ? `${user.username} | Admin` : user.username;
        } catch (e) {
          this.isLoggedIn = false;
        }
      }
    
      async login() {
  
        this.modalService.open(AuthComponent, { size: 'lg' })
          .result.then((closed) => {
            if (closed === 'logged_in') {
              console.log('showing logged in.');
              this.isLoggedIn = true;
           
            }
          }, (reason) => {
            console.log('reason:');
            console.log(reason);
          });
      }
    
      async logout() {
        await this.amplifyService.auth().signOut();
        this.isLoggedIn = false;

      }
    }


  
  

