import { Routes } from '@angular/router';
import { Login } from './pages/login/login';
import { Main } from './pages/main/main';
import { authGuard, guestGuard } from './auth.guard';

export const routes: Routes = [
    {path: 'login', component: Login, canActivate: [guestGuard]},
    {path: 'main', component: Main, canActivate: [authGuard]},
    {path: '', redirectTo: 'login', pathMatch: 'full'}
];