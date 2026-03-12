import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormControl,
  FormGroup,
  FormGroupDirective,
  ValidationErrors,
  ValidatorFn,
  Validators,
  AbstractControl
} from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../Service/notification.service';
import { AuthenticationService } from '../authentication.service';
import { ConstantService } from '../../Service/constant.service';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.css'],
  standalone: false
})
export class ResetPasswordComponent implements OnInit {
  resetPasswordForm!: FormGroup;
  PasswordCurrent: boolean = false;
  PasswordNew: boolean = false;
  PasswordConfirm: boolean = false;
  isLoading = false;
  dataSource: any;
  user: any;

  passwordRequirements = {
    hasLowerCase: false,
    hasUpperCase: false,
    hasNumber: false,
    hasMinimumLength: false,
    hasSpecialCharacter: false
  };

  constructor(
    private dialog: MatDialog,
    private notificationService: NotificationsService,
    private formBuilder: FormBuilder,
    private constantService: ConstantService,
    private authService: AuthenticationService,
  ) { }

  ngOnInit(): void {
    this.resetPasswordForm = new FormGroup({
      oldPassword: new FormControl('', Validators.required),
      newPassword: new FormControl('', [
        Validators.required,
        Validators.pattern('(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-z\\d$@$!%*?&]{8,}')
      ]),
      confirmPassword: new FormControl('', Validators.required),
      userId: new FormControl('')
    }, { validators: passwordMatchValidator });

    this.resetPasswordForm.get('newPassword')?.valueChanges.subscribe(value => {
      this.checkPasswordRequirements(value);
    });
  }

  checkPasswordRequirements(password: string) {
    this.passwordRequirements.hasLowerCase = /[a-z]/.test(password);
    this.passwordRequirements.hasUpperCase = /[A-Z]/.test(password);
    this.passwordRequirements.hasNumber = /[0-9]/.test(password);
    this.passwordRequirements.hasMinimumLength = password.length >= 8;
    this.passwordRequirements.hasSpecialCharacter = /[!@#$%^&*(),.?":{}|<>]/.test(password);
  }

  async saveResetPassword() {
    if (this.resetPasswordForm.invalid) {
      this.constantService.markFormGroupTouched(this.resetPasswordForm);
      return;
    }
    this.isLoading = true;

    const currentUserString = localStorage.getItem('currentUser');
    if (!currentUserString) {
      this.notificationService.showNotification('User not logged in.', 'danger');
      this.isLoading = false;
      return;
    }

    const userIdObj = JSON.parse(currentUserString);
    const userId = userIdObj.userId;

    const newPassword = this.resetPasswordForm.controls['newPassword'].value;
    const confirmPassword = this.resetPasswordForm.controls['confirmPassword'].value;

    if (newPassword === confirmPassword) {
      this.resetPasswordForm.controls['userId'].patchValue(userId);
      const _resetPasswordForm = { ...this.resetPasswordForm.value };
      this.user = userIdObj;

      try {
        const response = await this.authService.changePassword(_resetPasswordForm).toPromise();
        if (response.Status === 500) {
          this.notificationService.showNotification(response.Message, 'danger');
        } else {
          this.notificationService.showNotification(response.Message, 'success');
          this.dialog.closeAll();
          this.logout();
        }
      } catch (error) {
        console.error(error);
        this.notificationService.showNotification('An error occurred. Please try again.', 'danger');
      } finally {
        this.isLoading = false;
      }
    } else {
      this.notificationService.showNotification('New passwords do not match.', 'danger');
      this.isLoading = false;
    }
  }

  PasswordVisibilityCurrent(): void {
    this.PasswordCurrent = !this.PasswordCurrent;
  }

  PasswordVisibilityNew(): void {
    this.PasswordNew = !this.PasswordNew;
  }

  PasswordVisibilityConfirm(): void {
    this.PasswordConfirm = !this.PasswordConfirm;
  }

  passwordErrorMatcher = {
    isErrorState: (control: FormControl, form: FormGroupDirective | null): boolean => {
      const controlInvalid = control.touched && control.invalid;
      const formInvalid = control.touched && (this.resetPasswordForm.get('confirmPassword')?.touched ?? false) && this.resetPasswordForm.invalid;
      return controlInvalid || formInvalid;
    }
  }

  confirmErrorMatcher = {
    isErrorState: (control: FormControl, form: FormGroupDirective | null): boolean => {
      const controlInvalid = control.touched && control.invalid;
      const formInvalid = control.touched && (this.resetPasswordForm.get('newPassword')?.touched ?? false) && this.resetPasswordForm.invalid;
      return controlInvalid || formInvalid;
    }
  }

  getErrorMessage(controlName: string): string {
    if (this.resetPasswordForm.get(controlName)?.hasError('required')) {
      return 'This field is required.';
    }
    if (controlName === 'confirmPassword' && this.resetPasswordForm.hasError('mismatch')) {
      return 'Passwords must match.';
    }
    return 'Invalid input.';
  }

  logout() {
    this.authService.logout();
    window.location.href = '/login';
  }

}

// Updated password match validator function to work with AbstractControl
export const passwordMatchValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  if (!(control instanceof FormGroup)) {
    return null;
  }
  const newPassword = control.get('newPassword')?.value;
  const confirmPassword = control.get('confirmPassword')?.value;
  return newPassword === confirmPassword ? null : { mismatch: true };
};
