import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Subject } from 'rxjs/Subject';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Product } from '../models/product';
import { AmplifyService } from 'aws-amplify-angular';

@Injectable({
  providedIn: 'root'
})
export class ProductsService {
  api: String;
  token: String;
  private _listeners = new Subject<any>();
  constructor(private http: HttpClient, private amplify: AmplifyService) {
    this.api = environment.apiUrl;
   }

   listen(): Observable<any> {
     return this._listeners = new Subject<any>();
   }

   getProducts() {
     return this.retrieveProducts().subscribe(items => this._listeners.next(items));
   }

   retrieveProducts(): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.api}/products`);
  }

  async addProduct(product) {
    const path = `${this.api}/products`;

    await this.amplify.auth().currentSession()
      .then((cognitoUserSession) => {
      this.token = cognitoUserSession.getIdToken().getJwtToken();
    });

    const headers: HttpHeaders = new HttpHeaders()
                .append('Content-Type', 'application/json;')
                .append('Authorization', 'Bearer ' + this.token)
                .append("Accept", "application/json");
    const httpOptions = {
                headers: headers
            };
    return this.http.post(path, product, httpOptions).toPromise();
  }

  async deleteProduct(productId: string) {
    const path = `${this.api}/products/${productId}`;

    await this.amplify.auth().currentSession()
      .then((cognitoUserSession) => {
      this.token = cognitoUserSession.getIdToken().getJwtToken();
    });
 
    const headers: HttpHeaders = new HttpHeaders()
                .append('Content-Type', 'application/json;')
                .append('Authorization', 'Bearer ' + this.token)
                .append("Accept", "application/json");
    const httpOptions = {
                headers: headers
            };
    return this.http.delete(path, httpOptions).toPromise();
  }
}
