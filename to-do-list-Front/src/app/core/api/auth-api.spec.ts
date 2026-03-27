import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';

import { AuthApi } from './auth-api';
import { apiCredentialsInterceptor } from './api-credentials.interceptor';

describe('AuthApi', () => {
  let service: AuthApi;
  let httpTestingController: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([apiCredentialsInterceptor])),
        provideHttpClientTesting()
      ]
    });
    service = TestBed.inject(AuthApi);
    httpTestingController = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTestingController.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('posts login details to the login endpoint', () => {
    service.login({ email: 'test@example.com', password: 'secret123' }).subscribe();

    const request = httpTestingController.expectOne('/api/auth/login');
    expect(request.request.method).toBe('POST');
    expect(request.request.withCredentials).toBe(true);
    expect(request.request.body).toEqual({
      email: 'test@example.com',
      password: 'secret123'
    });

    request.flush({});
  });

  it('posts registration details to the register endpoint', () => {
    service
      .register({
        fullName: 'Fabio Bareiro',
        email: 'test@example.com',
        password: 'secret123'
      })
      .subscribe();

    const request = httpTestingController.expectOne('/api/auth/register');
    expect(request.request.method).toBe('POST');
    expect(request.request.withCredentials).toBe(true);
    expect(request.request.body).toEqual({
      fullName: 'Fabio Bareiro',
      email: 'test@example.com',
      password: 'secret123'
    });

    request.flush({});
  });

  it('loads the current user from the session endpoint', () => {
    service.getCurrentUser().subscribe();

    const request = httpTestingController.expectOne('/api/auth/me');
    expect(request.request.method).toBe('GET');
    expect(request.request.withCredentials).toBe(true);

    request.flush({ id: 1, fullName: 'Fabio Bareiro', email: 'test@example.com' });
  });
});
