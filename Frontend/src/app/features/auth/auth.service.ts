import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs/operators';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);

  public isLoggedIn = signal<boolean>(!!sessionStorage.getItem('jwt_token'));

  public login(credentials: any) {
    return this.http
      .post<any>('https://localhost:7119/api/identity/login', credentials)
      .pipe(
        tap((response) => {
          sessionStorage.setItem('jwt_token', response.token);
          this.isLoggedIn.set(true);
        }),
      );
  }

  public logout() {
    sessionStorage.removeItem('jwt_token');
    this.isLoggedIn.set(false);
    this.router.navigate(['/']);
  }

  public getToken(): string | null {
    return sessionStorage.getItem('jwt_token');
  }
}
