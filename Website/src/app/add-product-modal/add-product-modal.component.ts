import { Component, OnInit } from '@angular/core';
import { Product } from '../models/product';
import { ProductsService } from '../services/products-service.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ProductsComponent } from '../products/products.component';
@Component({
  selector: 'app-add-product-modal',
  templateUrl: './add-product-modal.component.html',
  styleUrls: ['./add-product-modal.component.css']
})
export class AddProductModalComponent implements OnInit {
  categories: string[] = ["SummitSwag", "CoolStuff"]
  model = new Product();
  constructor(private products: ProductsService, private modalService: NgbModal) {

   }

  ngOnInit() {
  }

 async addProduct() {
   await this.products.addProduct(this.model).then(response => {
    this.products.getProducts();
     this.modalService.dismissAll();
   });
 }

}
