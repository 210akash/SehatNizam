import { Component, OnInit, Inject } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { SalesTargetService } from '../sales-target.service';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { TerritoryService } from '../../territory/territory.service';

@Component({
  selector: 'app-create-territory-target',
  templateUrl: './create-territory-target.component.html',
  styleUrls: ['./create-territory-target.component.css'], standalone: false
})

export class CreateTerritoryTargetComponent implements OnInit {
  createTerritoryTargetForm!: FormGroup;
  isLoading = false;
  isEditMode = false;

  territoryList: any;

  constructor(private territoryService: TerritoryService, private notificationsService: NotificationsService, private dialog: MatDialog, private formBuilder: FormBuilder,
    private constantService: ConstantService, private salesTargetService: SalesTargetService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.createTerritoryTargetForm = this.formBuilder.group({
      id: [0],
      zoneId: [0],
      targetMonth: [null, Validators.required],
      territoriesTargetList: this.formBuilder.array([])
    });

    this.LoadData(this.data.element);
    this.getTerritoryTargetsByZoneId();
  }

  get f() {
    return this.createTerritoryTargetForm.controls;
  }

  LoadData(element: any) {
    console.log(element);
    this.createTerritoryTargetForm.get('zoneId')?.patchValue(element.zoneId);
    this.createTerritoryTargetForm.get('targetMonth')?.patchValue(element.targetMonth);
  }

  async getTerritoryByAreaId() {
    this.territoryList = [];
    (await this.territoryService.getTerritoryByAreaId(this.data.element.zoneId)).subscribe(
      {
        next: (data) => {
          this.territoryList = data;
          this.setAddTerritoriesTarget(data);
        },
        error: (error) => {
          console.log(error);
          this.isLoading = false;
        }
      });
  }

  async getTerritoryTargetsByZoneId() {
    this.territoryList = [];
    (await this.salesTargetService.getTerritoryTargetsByZoneId(this.data.element.zoneId, this.data.element.targetMonth)).subscribe(
      {
        next: (data: string | any[]) => {
          if (data.length > 0) {
            this.territoryList = data;
            this.setEditTerritoriesTarget(data);
          }
          else {
            this.getTerritoryByAreaId();
          }
        },
        error: (error: any) => {
          console.log(error);
          this.isLoading = false;
        }
      });
  }

  async saveSalesTarget() {
    this.isLoading = true;
    console.log(this.createTerritoryTargetForm);
    if (this.createTerritoryTargetForm.invalid) {
      this.constantService.markFormGroupTouched(this.createTerritoryTargetForm);
      return;
    }
    let _createTerritoryTargetForm: any = {};
    _createTerritoryTargetForm = Object.assign(_createTerritoryTargetForm, this.createTerritoryTargetForm.value);

    var EnterTarget = _createTerritoryTargetForm.territoriesTargetList.reduce((acc: any, item: { target: any; }) => acc + item.target, 0);
    if (EnterTarget > this.data.element.target) {
      this.notificationsService.showNotification('Total Territory Target should be equal to or less than the Zone Target!', 'snack-bar-warning');
      this.isLoading = false;
      return;
    }

    let parts = _createTerritoryTargetForm.targetMonth.split('-');
    let year = parseInt(parts[0], 10);
    let month = parseInt(parts[1], 10) - 1;
    _createTerritoryTargetForm.targetMonth = new Date(year, month, 2);
    (await this.salesTargetService.saveTerritoryTarget(_createTerritoryTargetForm)).subscribe(
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
    const territoriesArray = this.createTerritoryTargetForm.get('territoriesTargetList') as FormArray;
    data.forEach((data: { id: any; name: any; }) => {
      territoriesArray.push(this.formBuilder.group({
        territoryId: new FormControl(data.id),
        territoryName: new FormControl(data.name),
        target: new FormControl(0),
      }));
    });
  }

  async setEditTerritoriesTarget(data: any) {
    const territoriesArray = this.createTerritoryTargetForm.get('territoriesTargetList') as FormArray;
    data.forEach((data: { id: any; territoryId: any; territory: { name: any; }; target: any; }) => {
      territoriesArray.push(this.formBuilder.group({
        id: new FormControl(data.id),
        territoryId: new FormControl(data.territoryId),
        territoryName: new FormControl(data.territory?.name),
        target: new FormControl(data.target),
      }));
    });
  }

  get territoriesTargetList(): FormArray {
    return this.createTerritoryTargetForm.get('territoriesTargetList') as FormArray;
  }


}