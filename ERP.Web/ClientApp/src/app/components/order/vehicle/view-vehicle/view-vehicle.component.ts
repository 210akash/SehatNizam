import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';

@Component({
  selector: 'app-view-vehicle',
  templateUrl: './view-vehicle.component.html',
  styleUrls: ['./view-vehicle.component.css'],standalone: false
})

export class ViewVehicleComponent implements OnInit {
  viewVehicleForm!: FormGroup;
  isLoading = false;
  dataSource!: any;
  roleList: any;
  constructor(private dialog: MatDialog, private formBuilder: FormBuilder,
     private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.viewVehicleForm = this.formBuilder.group({
      id: [0],
      vehicleName: [''],
      registrationNumber: [''],
      driverName: [''],
      driverPhoneNo: [''],
      isHeadOfficeVehicle: [null],
      dealershipId: [0],
    });

    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    this.constantService.LoadData(element, this.viewVehicleForm);
    this.viewVehicleForm?.get('dealershipId')?.patchValue(element.dealership?.name);
    this.viewVehicleForm?.get('isHeadOfficeVehicle')?.patchValue(element.isHeadOfficeVehicle === true ? 'Yes' : 'No');
  }


}
