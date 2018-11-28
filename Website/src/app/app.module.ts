import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { AmplifyAngularModule, AmplifyService } from 'aws-amplify-angular';
import { NgbModalModule, NgbDropdownModule, NgbPopoverModule } from '@ng-bootstrap/ng-bootstrap';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AuthComponent } from './auth/auth.component';
import { NotLoggedInWarningComponent } from './not-logged-in-warning/not-logged-in-warning.component';
import { ShoppingCartComponent } from './shopping-cart/shopping-cart.component';
import { ProductsComponent } from './products/products.component';
import { ProductsService } from './services/products-service.service';
import { CartService } from './services/cart.service';
import { AddProductModalComponent } from './add-product-modal/add-product-modal.component';
import { IdentityService } from './services/identity.service';

@NgModule({
  declarations: [
    AppComponent,
    AuthComponent,
    NotLoggedInWarningComponent,
    ShoppingCartComponent,
    ProductsComponent,
    AddProductModalComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AmplifyAngularModule,
    FormsModule,
    ReactiveFormsModule,
    NgbModalModule,
    NgbDropdownModule,
    NgbPopoverModule,
    AppRoutingModule,
    FontAwesomeModule
  ],
  providers: [AmplifyService, ProductsService, CartService, IdentityService],
  bootstrap: [AppComponent],
  entryComponents: [AuthComponent, NotLoggedInWarningComponent, ShoppingCartComponent, ProductsComponent, AddProductModalComponent]
})
export class AppModule { }
