import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, firstValueFrom } from 'rxjs';
import { map } from 'rxjs/operators';
import { AuthEndPoints } from './auth.endpoints';
import { BaseService } from '../Service/base.service';
import { User } from '../_model/user';
import { environment } from '../../environments/environment';
import { UserService } from '../components/user-management/user.service';
import { NotificationsService } from '../Service/notification.service';

@Injectable({ providedIn: 'root' })

export class AuthenticationService extends BaseService<any> {
    endPointControllerName = 'Auth';
    private currentUserSubject: BehaviorSubject<User>;
    public currentUser: Observable<User>;

    constructor(private http: HttpClient, httpClient: HttpClient, private notificationsService: NotificationsService, private authEndPoints: AuthEndPoints, private userService: UserService) {
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
            // Step 1: Call login API
            const response = await firstValueFrom(
                this.post(loginForm, this.endPointControllerName + this.authEndPoints.login)
            );
            if(response.Status  == 500){
              this.notificationsService.showNotification(response.Message, 'snack-bar-danger');
            }
            // Step 2: If login successful and token exists
            if (response && response.token) {
                // ✅ Store login response immediately
                localStorage.setItem('currentUser', JSON.stringify(response));

                // ✅ Update BehaviorSubject before calling getProfile
                this.currentUserSubject.next(response);

                // Step 3: Fetch and store profile after token is ready for interceptors
                const profile = await this.getProfile(response.userId);
                localStorage.setItem('profile', JSON.stringify(profile));

                const roleString = response?.role;

                if (typeof roleString === 'string') {
                    const roles = roleString
                        .split(',')
                        .map(r => r.trim().toLowerCase());

                    if (roles.includes('hr manager')) {
                        window.location.href = '/hrdashboard';
                    } else if (roles.includes('employee')) {
                        window.location.href = '/employeedashboard';
                    } else {
                        window.location.href = '/home';
                    }
                } else {
                    console.warn('Invalid role format in response:', roleString);
                    window.location.href = '/home';
                }

            }

            return response;
        } catch (error) {
            console.error('Login failed', error);
            throw error;
        }
    }

    async getProfile(userId: any): Promise<any> {
        try {
            const profile = await firstValueFrom(
                this.get('?userId=' + userId, this.endPointControllerName + this.authEndPoints.getById)
            );
            return profile; // Return instead of saving here
        } catch (error) {
            console.error('Failed to fetch profile', error);
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
                localStorage.removeItem('profile');
                this.currentUserSubject.next(null!);
                //  window.location.href = '/login';
            }

        );
        localStorage.removeItem('currentUser');
        localStorage.removeItem('profile');
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

    updateSelectedWarehouse(projectId: number) {
        const queryString = '?ProjectId=' + projectId;
        return this.get(queryString, this.endPointControllerName + this.authEndPoints.updateSelectedWarehouse)
            .pipe(map((data: any) => data));
    }

    private getUserFromStorage(): User | null {
        const userJson = localStorage.getItem('currentUser');
        return userJson ? JSON.parse(userJson) : null;
    }

    public updateToken(token: string): void {
        const user = this.getUserFromStorage();
        if (user) {
            user.token = token;
            localStorage.setItem('currentUser', JSON.stringify(user));
            this.currentUserSubject.next(user); // ✅ Update BehaviorSubject so interceptor sees new token
        }
    }

    getByName(_param: any) {
        return this.get('?name=' + _param, this.endPointControllerName + this.authEndPoints.getByName).pipe(map((data: any) => data))
    }


}