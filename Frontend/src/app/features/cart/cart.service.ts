import { Injectable, computed, signal } from '@angular/core';

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

  public getCart() {
    return this.cartItems();
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
  }

  public removeFromCart(productId: number) {
    this.cartItems.update((items) =>
      items.filter((i) => i.productId !== productId),
    );
  }

  public clearCart() {
    this.cartItems.set([]);
  }
}
