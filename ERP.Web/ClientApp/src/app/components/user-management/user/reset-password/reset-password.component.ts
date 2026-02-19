import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { ConstantService } from '../../../../Service/constant.service';
import { UserService } from '../../user.service';

@Component({
    selector: 'app-reset-password',
    templateUrl: './reset-password.component.html',
    styleUrl: './reset-password.component.css',
    standalone: false
})

export class ResetpasswordComponent {
  resetPasswordForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;

  PasswordNew: boolean = false;
  passwordRequirements = {
    hasLowerCase: false,
    hasUpperCase: false,
    hasNumber: false,
    hasMinimumLength: false,
    hasSpecialCharacter: false
  };

  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private userService: UserService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.resetPasswordForm = this.formBuilder.group({
      userId: [this.data.element.id, Validators.required],
      password: [
        '',
        [
          Validators.required,
          Validators.pattern('(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&].{8,}')
        ]
      ],
    });
    this.LoadData(this.data.element);
    this.resetPasswordForm.get('password')?.valueChanges.subscribe(value => {
      this.checkPasswordRequirements(value);
    });
  }

  checkPasswordRequirements(password: string) {
    this.passwordRequirements.hasLowerCase = /[a-z]/.test(password);
    this.passwordRequirements.hasUpperCase = /[A-Z]/.test(password);
    this.passwordRequirements.hasNumber = /[0-9]/.test(password);
    this.passwordRequirements.hasMinimumLength = password.length >= 9;
    this.passwordRequirements.hasSpecialCharacter = /[!@#$%^&*(),.?":{}|<>]/.test(password);
  }

  PasswordVisibilityNew(): void {
    this.PasswordNew = !this.PasswordNew;
  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.resetPasswordForm);
    }
  }

  async SaveData() {
    if (this.resetPasswordForm.invalid) {
      this.constantService.markFormGroupTouched(this.resetPasswordForm);
      return;
    }
    this.isLoading = true;
    let _resetPasswordForm: any = {};
    _resetPasswordForm = Object.assign(_resetPasswordForm, this.resetPasswordForm.value);
    (await this.userService.changeUserPassword(_resetPasswordForm)).subscribe({
      next: (data) => {
        if (data.Status == 200) {
          this.notificationsService.showNotification('Password Changed Successfully', 'snack-bar-success');
          this.dialog.closeAll();
        }
        else
          this.notificationsService.showNotification(data.Data, 'snack-bar-danger');
        this.isLoading = false;
      },
      error: (error) => {
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        this.isLoading = false;
      }
    });
  }
}
