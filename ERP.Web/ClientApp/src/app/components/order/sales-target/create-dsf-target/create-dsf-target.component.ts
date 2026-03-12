import { Component, OnInit, Inject } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { SalesTargetService } from '../sales-target.service';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { TerritoryService } from '../../territory/territory.service';

@Component({
  selector: 'app-create-dsf-target',
  templateUrl: './create-dsf-target.component.html',
  styleUrls: ['./create-dsf-target.component.css'], standalone: false
})

export class CreateDSFTargetComponent implements OnInit {
  createSalesTargetForm!: FormGroup;
  isLoading = false;
  isEditMode = false;
  isTerritoryTargetShow = false;
  territoryName = '';
  territoryTarget = 0;
  territoryTargetMonth = '';

  // dsfList: any[] = [];
  territoryList: any[] = [];

  constructor(private territoryService: TerritoryService, private notificationsService: NotificationsService, private dialog: MatDialog, private formBuilder: FormBuilder, private constantService: ConstantService, private salesTargetService: SalesTargetService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.createSalesTargetForm = this.formBuilder.group({
      id: [0],
      zoneId: [0],
      territoryId: [0],
      targetMonth: [null, Validators.required],
      dsfTargetList: this.formBuilder.array([])
    });

    this.LoadData(this.data.element);
    this.getTerritoryByAreaId();
  }

  get f() {
    return this.createSalesTargetForm.controls;
  }

  LoadData(element: any) {
    this.createSalesTargetForm.get('zoneId')?.patchValue(element.zoneId);
    this.createSalesTargetForm.get('targetMonth')?.patchValue(element.targetMonth);
  }

  async getTerritoryByAreaId() {
    this.territoryList = [];
    (await this.territoryService.getTerritoryByAreaId(this.data.element.zoneId)).subscribe(
      {
        next: (data) => {
          this.territoryList = data;
        },
        error: (error) => {
          console.log(error);
          this.isLoading = false;
        }
      });
  }

  // async getDsfByTerritoryId(territoryId: any) {
  //   this.dsfList = [];
  //   (await this.territoryService.getDsfByTerritoryId(territoryId)).subscribe(
  //     {
  //       next: (data) => {
  //         this.dsfList = data;
  //         this.getDSFTargetsByTerritoryId(territoryId, this.createSalesTargetForm.get('targetMonth').value);
  //       },
  //       error: (error) => {
  //         console.log(error);
  //         this.isLoading = false;
  //       }
  //     });
  // }

  async getDSFTargetsByTerritoryId() {

    let territoryId = this.createSalesTargetForm.get('territoryId')?.value;

    if (territoryId > 0) {
      (await this.salesTargetService.getDSFTargetsByTerritoryId(territoryId, this.createSalesTargetForm.get('targetMonth')?.value)).subscribe(
        {
          next: (data: any) => {
            this.setAddTerritoriesTarget(data);
          },
          error: (error: any) => {
            console.log(error);
            this.isLoading = false;
          }
        });
    }
    else {
      this.isTerritoryTargetShow = false;
      const territoriesArray = this.createSalesTargetForm.get('dsfTargetList') as FormArray;
      territoriesArray.clear();
    }
  }

  async saveSalesTarget() {
    this.isLoading = true;
    if (this.createSalesTargetForm.invalid) {
      this.constantService.markFormGroupTouched(this.createSalesTargetForm);
      return;
    }
    let _createSalesTargetForm: any = {};
    _createSalesTargetForm = Object.assign(_createSalesTargetForm, this.createSalesTargetForm.value);

    var EnterTarget = _createSalesTargetForm.dsfTargetList.reduce((acc: any, item: { target: any; }) => acc + item.target, 0);
    if (EnterTarget > this.territoryTarget) {
      this.notificationsService.showNotification('Total DSF Target should be equal to or less than the Territory Target!', 'snack-bar-warning');
      this.isLoading = false;
      return;
    }

    _createSalesTargetForm['territoryId'] = this.createSalesTargetForm.get("territoryId")?.value;

    let parts = _createSalesTargetForm.targetMonth.split('-');
    let year = parseInt(parts[0], 10);
    let month = parseInt(parts[1], 10) - 1;
    _createSalesTargetForm.targetMonth = new Date(year, month, 2);
    (await this.salesTargetService.saveDSFTarget(_createSalesTargetForm)).subscribe(
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

  async setAddTerritoriesTarget(data: any) {
    const territoriesArray = this.createSalesTargetForm.get('dsfTargetList') as FormArray;
    territoriesArray.clear();
    if (data[0].territoryTarget == null) {
      this.notificationsService.showNotification('Enter Territory target first!', 'snack-bar-warning');
    }
    else {
      if (data != null && data.length > 0) {
        this.isTerritoryTargetShow = true;
        this.territoryName = data[0].territoryTarget.territory.name;
        this.territoryTarget = data[0].territoryTarget.target;
        this.territoryTargetMonth = data[0].territoryTarget.targetMonth;
      }
      else {
        this.isTerritoryTargetShow = false;
      }
    }


    data.forEach((data: { target: { id: null; target: any; }; userName: any; userId: any; }) => {
      territoriesArray.push(this.formBuilder.group({
        id: new FormControl(data.target?.id == null ? 0 : data.target?.id),
        userName: new FormControl(data.userName),
        dsfId: new FormControl(data.userId),
        target: new FormControl(data.target?.target),
      }));
    });
  }

  get dsfTargetList(): FormArray {
    return this.createSalesTargetForm.get('dsfTargetList') as FormArray;
  }


}