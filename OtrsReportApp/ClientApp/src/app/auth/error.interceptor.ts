import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable, throwError} from "rxjs";
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';


@Injectable()
export class ErrorInterceptor implements HttpInterceptor{
    /**
     *
     */
    constructor(private router: Router) {
        
    }
    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(req).pipe(catchError(err => {
            if(err.status === 401){
                localStorage.removeItem('token');
                localStorage.removeItem('otpToken');
                this.router.navigateByUrl('/login');
            }else if(err.status === 403){
                localStorage.removeItem('token');
                localStorage.removeItem('otpToken');
                this.router.navigateByUrl('/user/profile');
            }
            return throwError(err); 
        }))
    }
    
}