import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { ConstantService } from '../../../../Service/constant.service';
import { UserService } from '../../user.service';

@Component({
    selector: 'app-add-role',
    templateUrl: './add-role.component.html',
    styleUrl: './add-role.component.css',
    standalone: false
})

export class AddRoleComponent {
  roleForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  rolesList: any;
  selectedRolls: any;
  officeList: any;
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
    this.roleForm = this.formBuilder.group({
      id: [''],
      name: ['', Validators.required],
    });

    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.roleForm);
    }
  }

  async SaveData() {
    if (this.roleForm.invalid) {
      this.constantService.markFormGroupTouched(this.roleForm);
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.roleForm.value);

    (await this.userService.saveRole(_clienttemperatureForm)).subscribe({
      next: (data) => {
        if (data.Status == 200) {
          this.notificationsService.showNotification(data.Data, 'snack-bar-success');
          this.dialog.closeAll();
        }
        else
          this.notificationsService.showNotification(data.Data, 'snack-bar-danger');
        this.isLoading = false;
      },
      error: (error) => {
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        console.error(error);
        this.isLoading = false;
      }
    });
  }


}
