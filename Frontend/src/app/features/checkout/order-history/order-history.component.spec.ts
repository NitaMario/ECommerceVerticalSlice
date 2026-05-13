import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { OrderHistoryComponent } from './order-history.component';

describe('OrderHistoryComponent', () => {
  let component: OrderHistoryComponent;
  let fixture: ComponentFixture<OrderHistoryComponent>;
  let httpMock: HttpTestingController;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OrderHistoryComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();

    fixture = TestBed.createComponent(OrderHistoryComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
    fixture.detectChanges();
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should render the order number when data is succesfully fetched', () => {
    // Arrange
    const mockApiResponse = {
      orders: [
        {
          orderId: 999,
          totalAmount: 50.0,
          shippingAddress: 'Test',
          orderDate: '2026-05-12T00:00:00',
          items: [],
        },
      ],
    };

    // Act
    const req = httpMock.expectOne('https://localhost:7119/api/orders');
    req.flush(mockApiResponse);

    fixture.detectChanges();

    // Assert
    const compiledHtml = fixture.nativeElement as HTMLElement;
    expect(compiledHtml.textContent).toContain('Order #999');
    expect(component.isLoading()).toBeFalse();
  });
});
