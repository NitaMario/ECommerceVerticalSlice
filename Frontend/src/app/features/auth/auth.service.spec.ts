import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { AuthService } from './auth.service';

describe('AuthService', () => {
  it('should initialize isLoggedIn as true if token exists in sessionStorage', () => {
    // Arrange
    sessionStorage.setItem('jwt_token', 'fake_jwt_token');
    TestBed.configureTestingModule({
      providers: [AuthService, provideHttpClient(), provideHttpClientTesting()],
    });

    // Act
    const service = TestBed.inject(AuthService);

    // Assert
    expect(service.isLoggedIn()).toBeTrue();
    expect(service.getToken()).toBe('fake_jwt_token');

    sessionStorage.clear();
  });
});
