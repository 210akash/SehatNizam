import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthenticationService } from './authentication.service';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private router: Router, private authenticationService: AuthenticationService, private jwtHelper: JwtHelperService) {

  }
  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    const currentUser: any = this.authenticationService.currentUserValue;
    // if (currentUser == null) {
    //   window.location.href = '/#/login?returnUrl=' + state.url;
    //   return false;
    // }
    let url: string = state.url;
    var token = currentUser?.token;
    var token1 = currentUser;


    if (token != null && this.jwtHelper.isTokenExpired(token)) {
      //let userData = this.jwtHelper.decodeToken(token);
      localStorage.removeItem('currentUser');
      window.location.href = '/login?returnUrl=' + state.url;
      return false;
    }
    else if (token == null) {
      localStorage.removeItem('currentUser');
      window.location.href = '/login?returnUrl=' + state.url;
      return false;
    }


    const requiredRoles = Array.isArray(route.data['roles'])
    ? route.data['roles'].flatMap((role: string) => role.split(',').map(r => r.trim().toLowerCase()))
    : route.data['roles']?.split(',').map((role: string) => role.trim().toLowerCase());
  
  if (requiredRoles && requiredRoles.length > 0) {
    const userRoles = currentUser.role
      .split(',')
      .map((role: string) => role.trim().toLowerCase())
      .filter((role: string) => role !== '');
  
    const hasRole = requiredRoles.some((requiredRole: string) =>
      userRoles.includes(requiredRole)
    );
  
    console.log('requiredRoles:', requiredRoles);
    console.log('userRoles:', userRoles);
    console.log('hasRole:', hasRole);
  
    return hasRole;
  }

    // const array1 = route.data['roles'];
    // if (array1 != null) {
    //   const array2: any = currentUser.role.split(',');
    //   const isEqual = array1.some((r: any) => array2.indexOf(r) >= 0)

    //   //this.router.navigate(['/']);
    //   console.log(isEqual);
    //   return isEqual;
    // }


    //this.route.snapshot.queryParams['returnUrl'] || '/';

    //this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
    return true;
  }

  canActivateChild(
    next: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
      debugger;
      window.location.href = '/login?returnUrl='+state.url;
    return false;
  }

}
