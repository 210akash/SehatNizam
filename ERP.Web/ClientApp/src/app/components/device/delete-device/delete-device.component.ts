import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { ConstantService } from '../../../Service/constant.service';
import { NotificationsService } from '../../../Service/notification.service';
import { DeviceService } from '../device.service';

@Component({
  selector: 'app-delete-device',
  templateUrl: './delete-device.component.html',
  styleUrls: ['./delete-device.component.css'],
  standalone: false,
})
export class DeleteDeviceComponent implements OnInit {
  deleteDeviceForm!: FormGroup;
  isLoading = false;
  dialogRef: any;

  constructor(
    private notificationsService: NotificationsService,
    private dialog: MatDialog,
    private deviceService: DeviceService,
    private formBuilder: FormBuilder,
    private constantService: ConstantService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) {}

  ngOnInit(): void {
    this.deleteDeviceForm = this.formBuilder.group({
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

  LoadData(element: any) {
    this.constantService.LoadData(element, this.deleteDeviceForm);
  }

  async delete() {
    (
      await this.deviceService.deleteDevice(this.data.element.id)
    ).subscribe({
      next: (data) => {
        if (data.Status == 200) {
          this.isLoading = false;
          this.notificationsService.showNotification(
            data.Message,
            'snack-bar-success'
          );
          this.dialog.closeAll();
        } else {
          this.isLoading = false;
          this.notificationsService.showNotification(
            data.Message,
            'snack-bar-danger'
          );
        }
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      },
    });
  }
}
