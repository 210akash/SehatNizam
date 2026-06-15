import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthenticationService } from '../authentication.service';
import { NotificationsService } from '../../Service/notification.service';
import { environment } from '../../../environments/environment';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.css'],
    standalone: false
})
export class LoginComponent implements OnInit {
    title = environment.production ? 'Sign In (Live Server)' : 'Sign In (Testing Server)';
    loginForm!: FormGroup;
    loading = false;
    submitted = false;
    returnUrl!: string;
    error: any;
    bgClass = environment.backgroundStyle; // e.g., 'prod' or 'dev'
    colorClass = environment.color; // e.g., 'prod' or 'dev'
    name = environment.name; // e.g., 'prod' or 'dev'
    logo = environment.logo; // e.g., 'prod' or 'dev'

    constructor(private notificationsService: NotificationsService, private formBuilder: FormBuilder, private route: ActivatedRoute, private router: Router, private authenticationService: AuthenticationService) {
        // redirect to home if already logged in
        if (this.authenticationService.currentUserValue) {
            this.router.navigate(['/']);
        }
    }

    ngOnInit() {
        
        this.loginForm = this.formBuilder.group({
            email: ['', Validators.required],
            password: ['', Validators.required],
            isRemember: [false]
        });

        // get return url from route parameters or default to '/'
        this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
    }

    // convenience getter for easy access to form fields
    get f() { return this.loginForm.controls; }

    async onSubmit() {
        this.submitted = true;

        // stop here if form is invalid
        if (this.loginForm.invalid) {
            return;
        }

        let loginFormValue = {
            email: this.f['email'].value,
            password: this.f['password'].value
        }
        this.loading = true;

        try {
            const data = await this.authenticationService.login(loginFormValue);
            //   if (data != null) {
            //       if (data.token != '' && data.token != null) {
            //         const roles = data.role.split(',').map((role: string) => role.trim());
            //       if (roles.includes('Employee')) 
            //             window.location.href = '/employeedashboard';
            //         //   else if(data.role.replace(',','') == 'Team Manager')
            //         //       window.location.href = '/teammanagerdashboard';
            //         //   else if(data.role.replace(',','') == 'Manager')
            //         //       window.location.href = '/managerdashboard';
            //           else
            //           window.location.href = '/home';
            //       } else {
            //           this.notificationsService.showNotification(data.Message, 'snack-bar-danger');
            //           this.loading = false;
            //           // window.location.href = '/login';
            //       }
            //   }
        } catch (error) {
            this.error = error;
            this.loading = false;
        }
    }
}