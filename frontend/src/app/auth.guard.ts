import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';

export const guestGuard: CanActivateFn = (route, state) => {
    const router = inject(Router);
    const token = typeof window !== "undefined" ? localStorage.getItem('token') : null

    if(token)
        return router.parseUrl("/main");

    return true;
};

export const authGuard: CanActivateFn = (route, state) => {
    const router = inject(Router);
    const token = typeof window !== "undefined" ? localStorage.getItem('token') : null

    if(!token)
        return router.parseUrl("/login");

    return true;
};