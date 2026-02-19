import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, firstValueFrom } from 'rxjs';
import { map } from 'rxjs/operators';
import { AuthEndPoints } from './auth.endpoints';
import { BaseService } from '../Service/base.service';
import { User } from '../_model/user';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })

export class AuthenticationService extends BaseService<any> {
    endPointControllerName = 'Auth';
    private currentUserSubject: BehaviorSubject<User>;
    public currentUser: Observable<User>;

    constructor(private http: HttpClient, httpClient: HttpClient, private authEndPoints: AuthEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
        this.currentUserSubject = new BehaviorSubject<User>(JSON.parse(localStorage.getItem('currentUser') || 'null'));
        this.currentUser = this.currentUserSubject.asObservable();
    }

    public get currentUserValue(): User {
        return this.currentUserSubject.value;
    }

    async login(loginForm: any): Promise<any> {
        try {
            const response = await firstValueFrom(this.post(loginForm, this.endPointControllerName + this.authEndPoints.login));
            if (response != null && response.token != undefined) {
                localStorage.setItem('currentUser', JSON.stringify(response));
                this.currentUserSubject.next(response);
            }
            return response;
        } catch (error) {
            // Handle error appropriately here
            console.error('Login failed', error);
            throw error;
        }
    }

    // logout() {
    //     localStorage.removeItem('currentUser');
    //     this.currentUserSubject.next(null!);
    //     // this.activatedRoute.url.subscribe(params => {
    //     //     this.path = params[0].path;
    //     //     this.parameters = params[0].parameters; // Print the parameter to the console.
    //     // });
    //     window.location.href = '/login';

    //     // remove user from local storage to log user out
    //      localStorage.removeItem('currentUser');
    //      this.currentUserSubject.next(null!);
    // }

    logout() {

        this._logout().subscribe(
            data => {
                localStorage.removeItem('currentUser');
                this.currentUserSubject.next(null!);
                //  window.location.href = '/login';
            }

        );
        localStorage.removeItem('currentUser');
        this.currentUserSubject.next(null!);
        // this.activatedRoute.url.subscribe(params => {
        //     this.path = params[0].path;
        //     this.parameters = params[0].parameters; // Print the parameter to the console. 
        // });
        // this.get('',this.endPointControllerName + this.authEndPoints.logOut).pipe(map((data: any) =>
        //     {
        //         if(data != null)
        //             {
        //                 localStorage.removeItem('currentUser');
        //                 this.currentUserSubject.next(null!);
        //                 window.location.href = '/#/login';
        //             }
        //     }
        //     ));
        // localStorage.removeItem('currentUser');
        //this.currentUserSubject.next(null!);
        // window.location.href = '/#/login';
        // remove user from local storage to log user out
        // localStorage.removeItem('currentUser');
        // this.currentUserSubject.next(null!);
    }

    _logout() {
        return this.get(this.endPointControllerName + this.authEndPoints.logOut)
            .pipe(map((data: any) => data));
    }

    verifyPassword(LoginForm: any) {
        return this.post(LoginForm, this.endPointControllerName + this.authEndPoints.verifyPassword).pipe(map((data: any) => data));
    }

    verifyOtp(LoginForm: any) {
        return this.post(LoginForm, this.endPointControllerName + this.authEndPoints.verifyOtp).pipe(map((data: any) => data));
    }

    send2FaOtp(LoginForm: any) {
        return this.post(LoginForm, this.endPointControllerName + this.authEndPoints.send2FaOtp).pipe(map((data: any) => data));
    }

    register(LoginForm: any) {
        return this.post(LoginForm, this.endPointControllerName + this.authEndPoints.register).pipe(map((data: any) => data));
    }

    forgetPassword(LoginForm: any) {
        return this.post(LoginForm, this.endPointControllerName + this.authEndPoints.forgetPassword).pipe(map((data: any) => data));
    }

    resetPassword(LoginForm: any) {
        return this.post(LoginForm, this.endPointControllerName + this.authEndPoints.resetPassword).pipe(map((data: any) => data));
    }

    changePassword(LoginForm: any) {
        return this.post(LoginForm, this.endPointControllerName + this.authEndPoints.changePassword).pipe(map((data: any) => data));
    }

    changeEmail(LoginForm: any) {
        return this.post(LoginForm, this.endPointControllerName + this.authEndPoints.changeEmail).pipe(map((data: any) => data));
    }

    confirmChangeEmail(LoginForm: any) {
        return this.post(LoginForm, this.endPointControllerName + this.authEndPoints.confirmChangeEmail).pipe(map((data: any) => data));
    }

    resendEmailConfirmation(LoginForm: any) {
        return this.post(LoginForm, this.endPointControllerName + this.authEndPoints.resendEmailConfirmation).pipe(map((data: any) => data));
    }

    isValidPhoneNumber(LoginForm: any) {
        return this.post(LoginForm, this.endPointControllerName + this.authEndPoints.isValidPhoneNumber).pipe(map((data: any) => data));
    }

    addPhoneNumber(LoginForm: any) {
        return this.post(LoginForm, this.endPointControllerName + this.authEndPoints.addPhonenumber).pipe(map((data: any) => data))
    }

    confirmPhoneNumber(LoginForm: any) {
        return this.post(LoginForm, this.endPointControllerName + this.authEndPoints.confirmPhoneNumber).pipe(map((data: any) => data));
    }

    completeDeviceWizard(LoginForm: any) {
        return this.post(LoginForm, this.endPointControllerName + this.authEndPoints.completeDeviceWizard).pipe(map((data: any) => data))
    }
}
