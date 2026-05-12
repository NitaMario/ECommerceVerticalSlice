import { Component, inject, OnInit, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CurrencyPipe, DatePipe } from '@angular/common';

export interface OrderItem {
  productId: number;
  productName: string;
  unitPrice: number;
  quantity: number;
  imageUrl: string;
}

export interface Order {
  orderId: number;
  totalAmount: number;
  shippingAddress: string;
  orderDate: string;
  items: OrderItem[];
}
@Component({
  selector: 'app-order-history',
  standalone: true,
  imports: [CurrencyPipe, DatePipe],
  templateUrl: './order-history.component.html',
  styleUrl: './order-history.component.scss',
})
export class OrderHistoryComponent implements OnInit {
  private http = inject(HttpClient);

  public orders = signal<Order[]>([]);
  public isLoading = signal<boolean>(true);
  public errorMessage = signal<string>('');

  ngOnInit(): void {
    this.fetchOrders();
  }

  private fetchOrders() {
    this.http.get<any>('https://localhost:7119/api/orders').subscribe({
      next: (response) => {
        this.orders.set(response.orders || []);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Failed to load orders', err);
        this.errorMessage.set('Could not load order history');
        this.isLoading.set(false);
      },
    });
  }
}
