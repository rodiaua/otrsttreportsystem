import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
@Injectable()
export class JwtInterceptor implements HttpInterceptor {

    /**
     *
     */
    constructor(private router: Router) {

    }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        let token = localStorage.getItem("token");
        let otpToken = localStorage.getItem("otpToken");
        if (token) {
            request = request.clone({
                setHeaders: {
                    Authorization: `Bearer ${token}`
                }
            });
        }else if(otpToken) {
            request = request.clone({
                setHeaders: {
                    Authorization: `Bearer ${otpToken}`
                }
            });
        }
        return next.handle(request);
        /*
        Error interceptor implementation
         */
        // return next.handle(request).pipe(tap(
        //     success => {
        //     },
        //     error => {
        //         if(error.status == 401){
        //          localStorage.removeItem("token");
        //         this.router.navigateByUrl('/login');
        //}
        //     }
        // ));

    }

}