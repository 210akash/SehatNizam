import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { ConstantService } from '../../../Service/constant.service';
import { UserService } from '../../user-management/user.service';


@Component({
    selector: 'app-register-mobile-device',
    templateUrl: './register-mobile-device.component.html',
    styleUrl: './register-mobile-device.component.css',
    standalone: false
})

export class RegisterMobileDeviceComponent {
  userattendanceForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  companyList: any;

  constructor(private dialog: MatDialog, private employeeService: UserService, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

ngOnInit(): void {
  this.userattendanceForm = this.formBuilder.group({
    id: [this.data.element.id],
    deviceId: [this.data.element.deviceId],
    modifiedDate: [this.data.element.modifiedDate, Validators.required],
    isAvailableForMobile: [this.data.element.isAvailableForMobile, Validators.required],
    isMobileDeviceRegister: [this.data.element.isMobileDeviceRegister, Validators.required],
    isAvailableForWeb: [this.data.element.isAvailableForWeb, Validators.required],
    isDistCompForAtten: [this.data.element.isDistCompForAtten, Validators.required],
  });
    this.handleMobileAvailability();
}

  async SaveData() {

this.userattendanceForm
    // if (this.userattendanceForm.get('deviceId')?.value ==  null) {
    //   this.notificationsService.showNotification('Device Request Not Received', 'snack-bar-danger');
    //   return;
    // }
    if (this.userattendanceForm.invalid) {
      this.constantService.markFormGroupTouched(this.userattendanceForm);
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.userattendanceForm.value);

    (await this.employeeService.registerMobileDevice(_clienttemperatureForm)).subscribe({
      next: (data:any) => {
        if (data.Status == 200) {
          this.notificationsService.showNotification(data.Message, 'snack-bar-success');
          this.dialog.closeAll();
        }
        else
          this.notificationsService.showNotification(data.Message, 'snack-bar-danger');
        this.isLoading = false;
      },
      error: (error) => {
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        console.error(error);
        this.isLoading = false;
      }
    });
  }

handleMobileAvailability(): void {
  const deviceIdCtrl = this.userattendanceForm.get('deviceId');
  const mobileCtrl = this.userattendanceForm.get('isAvailableForMobile');
  const deviceCtrl = this.userattendanceForm.get('isMobileDeviceRegister');
  const distanceCtrl = this.userattendanceForm.get('isDistCompForAtten');

  // One handler for all three
  const applyRule = () => {
    if (!mobileCtrl?.value && (deviceIdCtrl?.value != null || deviceIdCtrl?.value != '')) {
      if (deviceCtrl?.value !== false) {
        deviceCtrl?.setValue(false, { emitEvent: false });
      }
      if (distanceCtrl?.value !== false) {
        distanceCtrl?.setValue(false, { emitEvent: false });
      }
    }
  };

  // Initial check
  applyRule();

  // Listen to all three changes
  mobileCtrl?.valueChanges.subscribe(applyRule);
  deviceCtrl?.valueChanges.subscribe(applyRule);
  distanceCtrl?.valueChanges.subscribe(applyRule);
}

}
