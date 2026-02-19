import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from "rxjs/operators";
import { Router, RouterStateSnapshot } from "@angular/router";

import { JwtHelperService } from '@auth0/angular-jwt';
import { AuthenticationService } from './authentication.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    constructor(private authenticationService: AuthenticationService, private jwtHelper: JwtHelperService) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        // add authorization header with jwt token if available
        let currentUser = this.authenticationService.currentUserValue;
        if(currentUser == null && !request.url.toLowerCase().includes("login"))
        {
            this.authenticationService.logout();
        }
        // else if(currentUser != null && this.jwtHelper.isTokenExpired(currentUser.token))
        // {
        //     this.authenticationService.logout();
        // }
        // else if(currentUser != null && this.jwtHelper.isTokenExpired())
        //   {
        //       this.authenticationService.logout();
        //   }
        else if (currentUser && !request.url.toLowerCase().includes("https://localhost:53779")) {
            request = request.clone({
                setHeaders: {
                    Authorization: `Bearer ${currentUser.token}`
                }
            });
            console.log(request.url.toLowerCase() + ' 1 st');
            return next.handle(request).pipe(
                tap(
                    succ => { },
                    err => {
                        // if (err.status == 0){
                        //     this.authenticationService.logout();
                        // }
                    }
                )
            )
        }

        else if (currentUser && request.url.toLowerCase().includes("https://localhost:53779")) {
            console.log('https://localhost:53779');
            request = request.clone({
                setHeaders: {
                    Cookie: `username=202392512572_2130706433`
                }
            });
            console.log(request.url.toLowerCase() + ' 2 st');
            return next.handle(request).pipe(
                tap(
                    succ => { },
                    err => {
                        // if (err.status == 0){
                        //     this.authenticationService.logout();
                        // }
                    }
                )
            )
        }

        return next.handle(request);
    }
}
