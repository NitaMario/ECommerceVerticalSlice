import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { CurrencyPipe } from '@angular/common';
import { CartService } from '../../cart/cart.service';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [FormsModule, CurrencyPipe],
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.scss',
})
export class CheckoutComponent {
  public cartService = inject(CartService);
  private http = inject(HttpClient);
  private router = inject(Router);

  public shippingDetails = {
    fullName: '',
    address: '',
  };

  public isSubmitting = false;

  public onPlaceOrder() {
    this.isSubmitting = true;

    const orderPayload = {
      fullName: this.shippingDetails.fullName,
      shippingAddress: this.shippingDetails.address,
      items: this.cartService.getCart().map((item) => ({
        productId: item.productId,
        quantity: item.quantity,
      })),
    };

    this.http
      .post('https://localhost:7119/api/orders', orderPayload)
      .subscribe({
        next: (response) => {
          this.cartService.clearCart();

          alert('Order Placed Succesfully! Returning to store.');
          this.router.navigate(['/']);
        },
        error: (err) => {
          console.error('Order failed, check if the API is running.', err);
          this.isSubmitting = false;
        },
      });
  }
}
