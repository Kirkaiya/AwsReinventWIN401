import { Component, OnInit } from '@angular/core';
import { faShoppingCart, faTrash } from '@fortawesome/free-solid-svg-icons';
import { AmplifyService } from 'aws-amplify-angular';
import { CartService } from '../services/cart.service';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';
import { CartItem } from '../models/cart-item';

@Component({
  selector: 'app-shopping-cart',
  templateUrl: './shopping-cart.component.html',
  styleUrls: ['./shopping-cart.component.css']
})
export class ShoppingCartComponent implements OnInit {

  faShoppingCart = faShoppingCart
  cartItems: CartItem[] = [];
  faTrash = faTrash;
  constructor(private amplifyService: AmplifyService, private cartService: CartService) { }

  ngOnInit() {
    //this.getCart();
  }

  async cartClicked(popover) {
    if (popover.isOpen()) {
      popover.close();
    } else {
      await this.getCart();
      popover.open();
    }
  }

    async getCart() {

    this.cartService.getCart().subscribe(cart => this.cartItems = cart);

    }
    
  async removeFromCart(productId) {
   this.cartService.removeItemFromCartAuth(productId);
  }

  

}
