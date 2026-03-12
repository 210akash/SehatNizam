import { Component, Inject, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { UserService } from '../../../user-management/user.service';
import { DeviceService } from '../../../device/device.service';
import { EmployeeDeviceService } from '../employee-device.service';

@Component({
  selector: 'app-employee-device',
  templateUrl: './employee-device.component.html',
  styleUrl: './employee-device.component.css',
  standalone: false
})
export class EmployeeDeviceComponent implements OnInit {
  employeedeviceForm!: FormGroup;
  isLoading = false;
  isEditMode = false;
  devices: any[] = [];

  constructor(
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private notificationsService: NotificationsService,
    private employeeService: UserService,
    private constantService: ConstantService,
    private deviceService: DeviceService,
    private employeeDeviceService: EmployeeDeviceService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  ngOnInit(): void {
    this.employeedeviceForm = this.formBuilder.group({
      employeeId: [this.data.element.id || 0],
      employeeDevices: this.formBuilder.array([], Validators.required)
    });

    // ✅ Load devices before calling LoadData
    let _DevicesForm: any = {};
    this.deviceService.getAllDevices(_DevicesForm).subscribe(devices => {
      this.devices = devices.item1;
      this.LoadData(this.data.element);
    });
  }

  get employeeDevices(): FormArray {
    return this.employeedeviceForm.get('employeeDevices') as FormArray;
  }

  LoadData(element: any) {
    // Clear the FormArray
    this.employeeDevices.clear();
    this.employeedeviceForm.patchValue({ employeeId: element.id });
    if (element.employeeDevice.length > 0) {
      this.isEditMode = true;
    }

    this.employeedeviceForm.patchValue({ employeeId: element.id });
    const assignedDevices = element.employeeDevice || [];

    this.devices.forEach((device, index) => {
      const assigned = assignedDevices.find((d: any) => d.deviceId === device.id);
      this.addEmployeeDevice(index - 1, device, assigned); // insert after previous
    });
  }

  private createEmployeeDeviceGroup(device: any, assigned: any): FormGroup {
    return this.formBuilder.group({
      id: [assigned ? assigned.id :  0],
      isChecked: [!!assigned],
      isSyned: [assigned ? assigned.isSyned :  false],
      deviceId: [device.id],
      deviceName: [device.name],
      enrollmentNo: [assigned ? assigned.enrollmentNo : '']
    });
  }

  addEmployeeDevice(index: number, device: any, assigned: any = null): void {
    const newDeviceGroup = this.createEmployeeDeviceGroup(device, assigned);
    this.employeeDevices.insert(index + 1, newDeviceGroup);
  }

  async submit() {
    const selectedDevices = this.employeeDevices.controls
      .map(control => control.value)
      .filter(device => device.isChecked)
      .map(device => ({
        id: device.id,
        deviceId: device.deviceId,
        enrollmentNo: device.enrollmentNo
      }));

    if (selectedDevices.length === 0) {
      this.notificationsService.showNotification('Please select at least one device.', 'snack-bar-danger');
      return;
    }

    const payload = {
      employeeId: this.employeedeviceForm.get('employeeId')?.value,
      employeeDevices: selectedDevices
    };

    (await this.employeeDeviceService.saveEmployeeDevice(payload)).subscribe({
      next: (data) => {
        this.isLoading = false;
        if (data.item1 == 200) {
          this.notificationsService.showNotification(data.item2, 'snack-bar-success');
          this.dialog.closeAll();
        } else {
          this.notificationsService.showNotification(data.item2, 'snack-bar-danger');
        }
      },
      error: (error) => {
        console.error(error);
        this.notificationsService.showNotification('Failed to save devices.', 'snack-bar-danger');
        this.isLoading = false;
      }
    });
  }
}
