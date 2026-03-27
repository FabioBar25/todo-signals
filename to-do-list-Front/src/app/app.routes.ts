import { Routes } from '@angular/router';

import { authRequiredGuard } from './core/auth/auth.guards';
import { TodoComponent } from './todo/todo';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'login'
  },
  {
    path: 'login',
    loadComponent: () => import('./auth/login/login').then((module) => module.LoginComponent)
  },
  {
    path: 'register',
    loadComponent: () => import('./auth/register/register').then((module) => module.RegisterComponent)
  },
  {
    path: 'tasks',
    canActivate: [authRequiredGuard],
    component: TodoComponent
  }
];
