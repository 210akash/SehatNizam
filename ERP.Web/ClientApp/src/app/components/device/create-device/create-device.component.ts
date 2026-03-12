import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { createMask } from '@ngneat/input-mask';
import { ConstantService } from '../../../Service/constant.service';
import { NotificationsService } from '../../../Service/notification.service';
import { DeviceService } from '../device.service';

@Component({
  selector: 'app-create-device',
  templateUrl: './create-device.component.html',
  styleUrls: ['./create-device.component.css'],
  standalone: false,
})

export class CreateDeviceComponent implements OnInit {
  createDeviceForm!: FormGroup;
  isLoading = false;
  dataSource!: any;
  roleList: any;
  isEditMode: boolean = false;

  phoneNoInputMask = createMask('0399-9999999');

  dialogRef: any;

  constructor(
    private notificationsService: NotificationsService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private constantService: ConstantService,
    private deviceService: DeviceService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) {}

  ngOnInit(): void {
    this.createDeviceForm = this.formBuilder.group({
      id: [0],
      name: ['', Validators.required],
      phoneNo: [''],
      address: [''],
      iPAddress: ['', Validators.required],
      port: [0, Validators.required],
      isActive: [true, Validators.required],
    });

    this.LoadData(this.data.element);
  }

  get f() {
    return this.createDeviceForm.controls;
  }

  async saveDevice() {
    this.isLoading = true;
    if (this.createDeviceForm.invalid) {
      this.constantService.markFormGroupTouched(this.createDeviceForm);
      return;
    }
    let _createDeviceForm: any = {};
    _createDeviceForm = Object.assign(
      _createDeviceForm,
      this.createDeviceForm.value
    );

    (await this.deviceService.saveDevice(_createDeviceForm)).subscribe({
      next: (data) => {
        if (data.Status == 200) {
          this.notificationsService.showNotification(
            'Device Saved Successfully!',
            'snack-bar-success'
          );
          this.dialog.closeAll();
          this.isLoading = false;
        } else if (data.Status == 409) {
          this.notificationsService.showNotification(
            'Name already exist!',
            'snack-bar-danger'
          );
          this.isLoading = false;
        }
      },
      error: (error) => {
        this.notificationsService.showNotification(
          'Please Fill the required fields!',
          'snack-bar-danger'
        );
        console.log(error);
        this.isLoading = false;
      },
    });
  }

  LoadData(element: any) {
    if (this.data.element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.createDeviceForm);
    }
    console.log(this.createDeviceForm);
  }
}
