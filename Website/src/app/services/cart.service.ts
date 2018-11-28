import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { RequestOptions, Request, RequestMethod } from '@angular/http';
import { environment } from '../../environments/environment';
import { CartItem } from '../models/cart-item';
import { AmplifyService } from 'aws-amplify-angular';
import { CognitoAccessToken } from 'amazon-cognito-identity-js';
import { of } from 'rxjs/observable/of';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  api: String;
  token: String;
  headers: HttpHeaders;
  httpOptions;

  constructor(private http: HttpClient, private amplify: AmplifyService) {
    this.api = environment.apiUrl;
    this.headers = new HttpHeaders()
      .append('Content-Type', 'application/json;')
      .append('Access-Control-Allow-Origin', '*');
    this.httpOptions = {
      headers: this.headers,
      withCredentials: true
    };

   }

  getCart(): Observable<CartItem[]> {
    const httpOptions = {
      headers: this.headers,
      withCredentials: true
    };

    return this.http.get<CartItem[]>(`${this.api}/cart`, httpOptions);
  }


  addItemToCart(body) {
    const httpOptions = {
      headers: this.headers,
      withCredentials: true
    };
    
    return this.http.post(`${this.api}/cart`, body, httpOptions);
  }


  removeItemFromCartAuth(productId) {
    const httpOptions = {
      headers: this.headers,
      withCredentials: true
    };

    return this.http.delete(`${this.api}/cart/${productId}`, httpOptions).toPromise();
  }

}
