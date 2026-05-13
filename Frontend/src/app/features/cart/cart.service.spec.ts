import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { CartService } from './cart.service';
import { AuthService } from '../auth/auth.service';

describe('CartService', () => {
  let service: CartService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        CartService,
        AuthService,
        provideHttpClient(),
        provideHttpClientTesting(),
      ],
    });
    service = TestBed.inject(CartService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should optimistically update totalItems and totalPrice when adding a product ', () => {
    // Arrange
    const mockProduct = { id: 1, name: 'Test Shirt', price: 20.0 };

    // Act
    service.addToCart(mockProduct);

    // Assert (UI)
    expect(service.totalItems()).toBe(1);
    expect(service.totalPrice()).toBe(20.0);

    // Assert (Db Sync)
    const req = httpMock.expectOne('https://localhost:7119/api/cart');
    expect(req.request.method).toBe('POST');
    req.flush({});
  });

  it('should clear the cart UI immediately when clearCart is called', () => {
    // Arrange
    service.addToCart({ id: 1, name: 'Test Shirt', price: 20.0 });
    httpMock.expectOne('https://localhost:7119/api/cart').flush({});

    // Act
    service.clearCart();

    // Assert
    expect(service.totalItems()).toBe(0);
    expect(service.getCart().length).toBe(0);
  });
});
