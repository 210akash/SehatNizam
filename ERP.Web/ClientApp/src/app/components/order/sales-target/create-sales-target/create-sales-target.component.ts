import { Component, Inject, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { SalesTargetService } from '../sales-target.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { AreaService } from '../../area/area.service';
import { RegionService } from '../../region/region.service';
import { TerritoryService } from '../../territory/territory.service';
import { ZoneService } from '../../zone/zone.service';

@Component({
  selector: 'app-create-sales-target',
  templateUrl: './create-sales-target.component.html',
  styleUrls: ['./create-sales-target.component.css'], standalone: false
})

export class CreateSalesTargetComponent implements OnInit {
  createSalesTargetForm!: FormGroup;
  isLoading = false;
  isEditMode = false;

  filteredRegion: any;
  filteredZone: any;
  filteredArea: any;
  filteredTerritory: any;
  filterDistributor: any;

  regionList: any;
  zoneList: any;
  areaList: any;
  territoryList: any;

  constructor(private dialog: MatDialog, private salesTargetService: SalesTargetService, private notificationsService: NotificationsService, private territoryService: TerritoryService, 
    private areaService: AreaService, private zoneService: ZoneService, private constantService: ConstantService, private regionService: RegionService, private formBuilder: FormBuilder,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.createSalesTargetForm = this.formBuilder.group({
      id: [0],
      targetMonth: [null, Validators.required],
      regionId: [0],
      zoneId: [0],
      areaId: [0],
      territoryId: [0, Validators.required],
      distributor: [''],
      dsfTargetList: this.formBuilder.array([])
    });

    this.getRegions();
    this.LoadData(this.data.element);
  }

  async getRegions() {
    let _regionFilterForm = {};
    (await this.regionService.getAllRegion(_regionFilterForm)).subscribe({
      next: (data) => {
        this.regionList = data.item1;
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  LoadData(element: any) {
    if (element?.id != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.createSalesTargetForm);

      var targetMonth = this.constantService.convertDateToMonth(element.targetMonth);
      this.createSalesTargetForm.get('targetMonth')?.patchValue(targetMonth);

      this.createSalesTargetForm.get('regionId')?.patchValue(element.user?.userTerritory[0]?.regionId);
      this.getZoneByRegionId();

      this.createSalesTargetForm.get('zoneId')?.patchValue(element.user?.userTerritory[0]?.zoneId);
      this.getAreaByZoneId();

      this.createSalesTargetForm.get('areaId')?.patchValue(element.user?.userTerritory[0]?.areaId);
      this.getTerritoryByAreaId();

      this.createSalesTargetForm.get('territoryId')?.patchValue(element.user?.userTerritory[0]?.territoryId);

      setTimeout(() => {
        this.getFilteredData();
        this.getDSFTargetsByTerritoryId();
      }, 400);
    }
  }

  async getZoneByRegionId() {

    this.zoneList = [];
    this.areaList = [];
    this.territoryList = [];
    this.createSalesTargetForm.get('zoneId')?.patchValue(0);
    this.createSalesTargetForm.get('areaId')?.patchValue(0);
    this.createSalesTargetForm.get('territoryId')?.patchValue(0);

    (await this.zoneService.getZoneByRegionId(this.createSalesTargetForm.get('regionId')?.value)).subscribe({
      next: (data) => {
        this.zoneList = data;
        // if (this.data.element != null) {
        //   this.filteredZone = this.zoneList.find(zone => zone.id === this.data.element.area?.zoneId);
        // }
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  get f() {
    return this.createSalesTargetForm.controls;
  }

  getFilteredData() {

    this.filterDistributor = null;
    this.filteredRegion = this.regionList?.find((region: { id: any; }) => region.id === this.createSalesTargetForm.get('regionId')?.value);
    this.filteredZone = this.zoneList?.find((zone: { id: any; }) => zone.id === this.createSalesTargetForm.get('zoneId')?.value);
    this.filteredArea = this.areaList?.find((area: { id: any; }) => area.id === this.createSalesTargetForm.get('areaId')?.value);
    this.filteredTerritory = this.territoryList?.find((territory: { id: any; }) => territory.id === this.createSalesTargetForm.get('territoryId')?.value);
    if (this.filteredTerritory?.dealership?.length > 0) {
      this.filterDistributor = this.filteredTerritory?.dealership?.find((y: { isActive: boolean; }) => y.isActive == true)
      this.createSalesTargetForm.get('distributor')?.patchValue(this.filterDistributor.name + ' (' + this.filterDistributor.address + ')');
    }
  }

  async getAreaByZoneId() {

    this.areaList = [];
    this.territoryList = [];
    this.createSalesTargetForm.get('areaId')?.patchValue(0);
    this.createSalesTargetForm.get('territoryId')?.patchValue(0);

    (await this.areaService.getAreaByZoneId(this.createSalesTargetForm.get('zoneId')?.value)).subscribe({
      next: (data) => {
        this.areaList = data;
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  async getTerritoryByAreaId() {

    this.territoryList = [];
    this.createSalesTargetForm.get('territoryId')?.patchValue(0);

    this.createSalesTargetForm.get('territoryId')?.patchValue('');
    (await this.territoryService.getTerritoryByAreaId(this.createSalesTargetForm.get('areaId')?.value)).subscribe(
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

  async saveShop() {
    this.isLoading = true;
    if (this.createSalesTargetForm.invalid) {
      this.constantService.markFormGroupTouched(this.createSalesTargetForm);
      return;
    }
    // let _createShopForm: any = {};
    // _createShopForm = Object.assign(_createShopForm, this.createShopForm.value);

    // _createShopForm['shopImages'] = this.documents;

    // (await this.shopService.saveShop(_createShopForm)).subscribe(
    //   {
    //     next: (data) => {
    //       if (data.Status == 200) {
    //         this.notificationsService.showNotification('top', 'right', 'Shop Saved Successfully', 'snack-bar-success');
    //         this.dialog.closeAll();
    //         this.isLoading = false;
    //       }
    //       else if (data.Status == 409) {
    //         this.notificationsService.showNotification('top', 'right', 'Name already exist!', 'snack-bar-danger');
    //         this.isLoading = false;
    //       }
    //       else if (data.Status == 412) {
    //         this.notificationsService.showNotification('top', 'right', 'Phone No already exist!', 'snack-bar-danger');
    //         this.isLoading = false;
    //       }
    //     },
    //     error: (error) => {
    //       this.notificationsService.showNotification('top', 'right', 'Please Fill the required fields!', 'snack-bar-danger');
    //       console.log(error);
    //       this.isLoading = false;
    //     }
    //   });
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
    // if (EnterTarget > this.territoryTarget) {
    //   this.notificationsService.showNotification('top', 'right', 'Total DSF Target should be equal to or less than the Territory Target!', 'snack-bar-warning');
    //   this.isLoading = false;
    //   return;
    // }

    _createSalesTargetForm['territoryId'] = this.createSalesTargetForm.get("territoryId")?.value;

    let parts = _createSalesTargetForm.targetMonth.split('-');
    let year = parseInt(parts[0], 10);
    let month = parseInt(parts[1], 10) - 1;
    _createSalesTargetForm.targetMonth = new Date(year, month, 2);
    (await this.salesTargetService.saveDSFTarget(_createSalesTargetForm)).subscribe(
      {
        next: (data: { Status: number; }) => {
          if (data.Status == 200) {
            this.notificationsService.showNotification('Sales Target Saved Successfully!', 'snack-bar-success');
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
      const territoriesArray = this.createSalesTargetForm.get('dsfTargetList') as FormArray;
      territoriesArray.clear();
    }
  }

  async setAddTerritoriesTarget(data: any) {
    const territoriesArray = this.createSalesTargetForm.get('dsfTargetList') as FormArray;
    territoriesArray.clear();
    // if (data[0].territoryTarget == null) {
    //   this.notificationsService.showNotification('top', 'right', 'Enter Territory target first!', 'snack-bar-warning');
    // }
    // else {
    //   if (data != null && data.length > 0) {
    //     this.isTerritoryTargetShow = true;
    //     this.territoryName = data[0].territoryTarget.territory.name;
    //     this.territoryTarget = data[0].territoryTarget.target;
    //     this.territoryTargetMonth = data[0].territoryTarget.targetMonth;
    //   }
    //   else {
    //     this.isTerritoryTargetShow = false;
    //   }
    // }


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
