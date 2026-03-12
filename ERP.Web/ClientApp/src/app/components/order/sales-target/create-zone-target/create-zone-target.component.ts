import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { createMask } from '@ngneat/input-mask';
import { SalesTargetService } from '../sales-target.service';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { ZoneService } from '../../zone/zone.service';

@Component({
  selector: 'app-create-zone-target',
  templateUrl: './create-zone-target.component.html',
  styleUrls: ['./create-zone-target.component.css'], standalone: false
})

export class CreateZoneTargetComponent implements OnInit {
  createSalesTargetForm!: FormGroup;
  isLoading = false;
  dataSource!: any;
  roleList: any;
  dropdownSettings: any;
  isEditMode: boolean = false;

  phoneNoInputMask = createMask('0399-9999999');
  cnicInputMask = createMask('99999-9999999-9');

  zoneList: any;
  territoryList: any;

  filteredZone: any;
  filteredTerritory: any;

  dsfList: any;

  zoneTargetExist: any;

  constructor(private zoneService: ZoneService, private notificationsService: NotificationsService, private dialog: MatDialog, private formBuilder: FormBuilder,
    private constantService: ConstantService, private salesTargetService: SalesTargetService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.createSalesTargetForm = this.formBuilder.group({
      id: [0],
      zoneId: [0],
      target: [0, Validators.required],
      targetMonth: [null, Validators.required],
    });

    this.getZones();
    this.LoadData(this.data.element);
  }

  get f() {
    return this.createSalesTargetForm.controls;
  }

  async getZones() {
    let _zoneFilterForm = {};
    (await this.zoneService.getAllZone(_zoneFilterForm)).subscribe(
      {
        next: (data) => {
          this.zoneList = data.item1;
        },
        error: (error) => {
          console.log(error);
          this.isLoading = false;
        }
      });
  }

  async saveSalesTarget() {
    this.isLoading = true;
    if (this.createSalesTargetForm.invalid) {
      this.constantService.markFormGroupTouched(this.createSalesTargetForm);
      return;
    }
    let _createSalesTargetForm: any = {};
    _createSalesTargetForm = Object.assign(_createSalesTargetForm, this.createSalesTargetForm.value);

    let parts = _createSalesTargetForm.targetMonth.split('-');
    let year = parseInt(parts[0], 10);
    let month = parseInt(parts[1], 10) - 1;
    _createSalesTargetForm.targetMonth = new Date(year, month, 2);
    (await this.salesTargetService.saveSalesTarget(_createSalesTargetForm)).subscribe(
      {
        next: (data: { Status: number; }) => {
          if (data.Status == 200) {
            this.notificationsService.showNotification('User Territory Saved Successfully!', 'snack-bar-success');
            this.dialog.closeAll();
            this.isLoading = false;
          }
          else if (data.Status == 409) {
            this.notificationsService.showNotification('Target of month against selected territory already added!', 'snack-bar-warning');
            this.isLoading = false;
          }
        },
        error: (error: any) => {
          this.notificationsService.showNotification('Please Fill the required fields!', 'snack-bar-danger');
          console.log(error);
          this.isLoading = false;
        }
      });
  }

  LoadData(element: any) {
    if (this.data?.element?.id != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.createSalesTargetForm);

      // Format the date as YYYY-MM
      let targetMonthDate = new Date(element.targetMonth);
      const targetMonthValue = targetMonthDate.toISOString().substring(0, 7);
      this.createSalesTargetForm.get('targetMonth')?.patchValue(targetMonthValue);
    }
    console.log(this.createSalesTargetForm);
  }


}