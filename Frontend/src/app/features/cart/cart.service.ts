import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../auth/auth.service';

export interface CartItem {
  productId: number;
  productName: string;
  price: number;
  quantity: number;
}

@Injectable({
  providedIn: 'root',
})
export class CartService {
  private http = inject(HttpClient);
  private authService = inject(AuthService);

  private cartItems = signal<CartItem[]>([]);

  public totalItems = computed(() => {
    return this.cartItems().reduce((total, item) => total + item.quantity, 0);
  });

  public totalPrice = computed(() => {
    return this.cartItems().reduce(
      (total, item) => total + item.price * item.quantity,
      0,
    );
  });

  constructor() {
    if (this.authService.isLoggedIn()) {
      this.loadCart();
    }
  }

  public getCart() {
    return this.cartItems();
  }

  public loadCart() {
    this.http.get<any>('https://localhost:7119/api/cart').subscribe({
      next: (response) => {
        const dbItems = response.items || [];

        const mappedItems: CartItem[] = dbItems.map((dbItem: any) => ({
          productId: dbItem.productId,
          productName: dbItem.name,
          price: dbItem.price,
          quantity: dbItem.quantity,
        }));

        this.cartItems.set(mappedItems);
      },
      error: (err) => console.error('Failed to load cart', err),
    });
  }

  public addToCart(product: any) {
    this.cartItems.update((currentItems) => {
      const existingItem = currentItems.find((i) => i.productId === product.id);

      if (existingItem) {
        return currentItems.map((i) =>
          i.productId === product.id ? { ...i, quantity: i.quantity + 1 } : i,
        );
      } else {
        const newItem: CartItem = {
          productId: product.id,
          productName: product.name,
          price: product.price,
          quantity: 1,
        };
        return [...currentItems, newItem];
      }
    });

    this.http
      .post('https://localhost:7119/api/cart', {
        productId: product.id,
        quantity: 1,
      })
      .subscribe({
        error: (err) => console.error('Add to cart failed', err),
      });
  }

  public updateQuantity(productId: number, newQuantity: number) {
    if (newQuantity <= 0) {
      this.removeFromCart(productId);
      return;
    }

    this.cartItems.update((items) =>
      items.map((i) =>
        i.productId === productId ? { ...i, quantity: newQuantity } : i,
      ),
    );

    this.http
      .put('https://localhost:7119/api/cart', {
        productId: productId,
        newQuantity: newQuantity,
      })
      .subscribe({
        error: (err) => console.error('Quantity update failed', err),
      });
  }

  public removeFromCart(productId: number) {
    this.cartItems.update((items) =>
      items.filter((i) => i.productId !== productId),
    );

    this.http.delete(`https://localhost:7119/api/cart/${productId}`).subscribe({
      error: (err) => console.error('Remove from cart failed', err),
    });
  }

  public clearCart() {
    this.cartItems.set([]);
  }
}
