import { Component, inject, OnInit, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CartService } from '../../cart/cart.service';

export interface Product {
  id: number;
  name: string;
  description: string;
  price: number;
  imageUrl: string;
}

export interface ProductResponse {
  products: Product[];
}

@Component({
  selector: 'app-product-list',
  standalone: true,
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.scss',
})
export class ProductListComponent implements OnInit {
  private http = inject(HttpClient);
  private cartService = inject(CartService);

  public products = signal<Product[]>([]);

  ngOnInit() {
    this.fetchProducts();
  }

  private fetchProducts() {
    this.http
      .get<ProductResponse>('https://localhost:7119/api/products')
      .subscribe({
        next: (data) => {
          this.products.set(data.products);
        },
        error: (err) => {
          console.error(
            'Failed to fetch products. Check if the .NET API is running?',
            err,
          );
        },
      });
  }

  public onAddToCart(product: Product) {
    this.cartService.addToCart(product);
  }
}
