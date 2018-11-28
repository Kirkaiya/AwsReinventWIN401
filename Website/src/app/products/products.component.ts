import { Component, OnInit } from '@angular/core';
import { faTshirt, faTrash, faPlusSquare } from '@fortawesome/free-solid-svg-icons';
import { Product } from '../models/product';
import { ProductsService } from '../services/products-service.service';
import { IdentityService } from '../services/identity.service';
import { CartService } from '../services/cart.service';
import { NotLoggedInWarningComponent } from '../not-logged-in-warning/not-logged-in-warning.component';
import { AddProductModalComponent } from '../add-product-modal/add-product-modal.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AmplifyService } from 'aws-amplify-angular';

@Component({
  selector: 'app-products',
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.css']
})
export class ProductsComponent implements OnInit {

  faTshirt = faTshirt;
  faTrash = faTrash;
  faAdd = faPlusSquare;
  products: Product[] = [];
  notLoggedIn = false;
  isAdmin = false;
  constructor(
    private productsService: ProductsService, 
    private modalService: NgbModal,
    private amplify: AmplifyService,
    private cart: CartService,
    private identity: IdentityService) { }

  ngOnInit() {
      this.retrieveProducts();
      this.setRole();
      this.productsService.listen().subscribe((products:any) => {
        this.products = products;
      })
      this.amplify.authStateChange$.subscribe(authState => {
        if(authState.state === "signedIn"){
        this.notLoggedIn = false;
        }
        else{
          this.notLoggedIn = true;
        }
      });
  }

 async setRole() {
    await this.identity.getIdentity();
    this.isAdmin = this.identity.isAdmin;
  }
  
  retrieveProducts() {
    this.productsService.retrieveProducts()
    .subscribe(products => this.products = products);

  }

  displayPrice(price){
    const priceToDisplay = price.toFixed(2);
    return "$" + priceToDisplay;
  }

  addItemToCart(product) {
    this.cart.addItemToCart(product).subscribe(response => this.cart.getCart());
  }

  addProduct(){
    this.modalService.open(AddProductModalComponent);
  }

  async deleteProduct(productId){
    try {
     const session = await this.amplify.auth().currentSession();
     
    } catch (e) {
      this.notLoggedIn = true;
      console.log('not logged in');
      this.modalService.open(NotLoggedInWarningComponent)
      return;
    }
    await this.productsService.deleteProduct(productId).then(response => this.retrieveProducts());
  
  }
    
}
