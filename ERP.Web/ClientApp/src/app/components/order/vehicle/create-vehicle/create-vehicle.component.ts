import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { VehicleService } from '../vehicle.service';
import { createMask } from '@ngneat/input-mask';
import { NotificationsService } from '../../../../Service/notification.service';
import { DealershipService } from '../../dealership/dealership.service';
import { ConstantService } from '../../../../Service/constant.service';

@Component({
  selector: 'app-create-vehicle',
  templateUrl: './create-vehicle.component.html',
  styleUrls: ['./create-vehicle.component.css'],standalone: false
})

export class CreateVehicleComponent implements OnInit {
  createVehicleForm!: FormGroup;
  isLoading = false;
  vehicleListFilerForm!: FormGroup;
  dataSource!: any;
  roleList: any;
  dropdownSettings: any;
  isEditMode: boolean = false;
  isHeadOfficeVehicle : any;
  dealershipList: any[] = [];

  phoneNoInputMask = createMask('0399-9999999');

  constructor(private notificationsService: NotificationsService, private dialog: MatDialog, private formBuilder: FormBuilder, private constantService: ConstantService, private vehicleService: VehicleService,
    private dealershipService: DealershipService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.createVehicleForm = this.formBuilder.group({
      id: [0],
      vehicleName: ['', Validators.required],
      registrationNumber: ['', Validators.required],
      driverName: ['', Validators.required],
      driverPhoneNo: ['', Validators.required],
      isHeadOfficeVehicle: [0, Validators.required],
      dealershipId: [0],
      logisticPartner: [''],
      loadCapacity: ['', Validators.required]
    });

    this.getAllDealership();
    this.LoadData(this.data.element);
  }

  get f() {
    return this.createVehicleForm.controls;
  }

  async saveVehicle() {
    this.isLoading = true;
    
    if (this.createVehicleForm.invalid) {
      this.constantService.markFormGroupTouched(this.createVehicleForm);
      return;
    }
    let _createVehicleForm: any = {};
    _createVehicleForm = Object.assign(_createVehicleForm, this.createVehicleForm.value);

    (await this.vehicleService.saveVehicle(_createVehicleForm)).subscribe(
      {
        next: (data:any) => {
          if (data.Status == 200) {
            this.notificationsService.showNotification('Shop Type Saved Successfully!', 'snack-bar-success');
            this.dialog.closeAll();
            this.isLoading = false;
          }
          else if (data.Status == 409) {
            this.notificationsService.showNotification('Name already exist!', 'snack-bar-danger');
            this.isLoading = false;
          }
        },
        error: (error:any) => {
          this.notificationsService.showNotification( 'Please Fill the required fields!', 'snack-bar-danger');
          console.log(error);
          this.isLoading = false;
        }
      });
  }

  LoadData(element: any) {
    if (this.data.element?.id != null) {
      this.isEditMode = true;
      if(element.isHeadOfficeVehicle == true)
      {
        this.isHeadOfficeVehicle = true;
      }
      else
      {
        this.isHeadOfficeVehicle = false;
      }
      this.constantService.LoadData(element, this.createVehicleForm);

    }
    console.log(this.createVehicleForm);
  }

  async getAllDealership() {

    let dealershipForm = {
      'dealershipTypeId': 1
    };
    
    (await this.dealershipService.getAllDealership(dealershipForm)).subscribe({
      next: (data) => {
        this.dealershipList = data.item1;
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  resetDistributorValue() {
    this.createVehicleForm.get('dealershipId')?.patchValue(0);
    this.isHeadOfficeVehicle = this.createVehicleForm.get('isHeadOfficeVehicle')?.value;

    if (this.isHeadOfficeVehicle  == false) {
      this.createVehicleForm.get('dealershipId')?.setValidators(Validators.required);
      this.createVehicleForm.get('dealershipId')?.updateValueAndValidity;
    }
    else{
      this.createVehicleForm.get('dealershipId')?.clearValidators;
      this.createVehicleForm.get('dealershipId')?.updateValueAndValidity;
    }
  }


}