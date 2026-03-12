import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { VehicleService } from '../vehicle.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { ConstantService } from '../../../../Service/constant.service';

@Component({
  selector: 'app-delete-vehicle',
  templateUrl: './delete-vehicle.component.html',
  styleUrls: ['./delete-vehicle.component.css'],standalone: false
})

export class DeleteVehicleComponent implements OnInit {
  deleteVehicleForm!: FormGroup;
  isLoading = false;
  dataSource!: any;
  roleList: any;

  constructor(private notificationsService: NotificationsService, private dialog: MatDialog, private vehicleService: VehicleService, private formBuilder: FormBuilder, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.deleteVehicleForm = this.formBuilder.group({
      id: [0],
      vehicleName: [''],
      registrationNumber: [''],
      driverName: [''],
      driverPhoneNo: [''],
      isHeadOfficeVehicle: [null],
      dealershipId: [0],
      loadCapacity: ['', Validators.required]
    });

    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    this.constantService.LoadData(element, this.deleteVehicleForm);
    this.deleteVehicleForm.get('dealershipId')?.patchValue(element.dealership?.name);
    this.deleteVehicleForm.get('isHeadOfficeVehicle')?.patchValue(element.isHeadOfficeVehicle === true ? 'Yes' : 'No');
  }

  async delete() {
    (await this.vehicleService.deleteVehicle(this.data.element.id)).subscribe({
      next: (data: any) => {
        if (data.Status == 200) {
          this.isLoading = false;
          this.notificationsService.showNotification( data.Message, 'snack-bar-success');
          this.dialog.closeAll();
        }
        else {
          this.isLoading = false;
          this.notificationsService.showNotification(data.Message, 'snack-bar-danger');
        }
      },
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }


}
