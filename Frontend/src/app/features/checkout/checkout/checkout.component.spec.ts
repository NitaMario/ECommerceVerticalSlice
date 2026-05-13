import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { By } from '@angular/platform-browser';
import { CheckoutComponent } from './checkout.component';
import { CartService } from '../../cart/cart.service';

describe('CheckoutComponent', () => {
  let component: CheckoutComponent;
  let fixture: ComponentFixture<CheckoutComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CheckoutComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    }).compileComponents();

    fixture = TestBed.createComponent(CheckoutComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should disable the Place Order button if the shipping address is empty', () => {
    // Arrange
    const buttonElement = fixture.debugElement.query(
      By.css('button[type="submit"]'),
    ).nativeElement;

    // Act
    component.shippingDetails.address = '';
    fixture.detectChanges();

    // Assert
    expect(buttonElement.disabled).toBeTrue();
  });

  it('should enable the Place Order button when cart has items and is not submitting', async () => {
    // Arrange
    const cartService = TestBed.inject(CartService);

    // Act
    cartService.addToCart({ id: 1, name: 'Test Shirt', price: 10.0 });
    component.isSubmitting = false;
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    // Assert
    const buttonElement = fixture.debugElement.query(
      By.css('button[type="submit"]'),
    ).nativeElement;

    expect(buttonElement.disabled).toBeFalse();
  });
});
