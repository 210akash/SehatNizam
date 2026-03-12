import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { createMask } from '@ngneat/input-mask';
import { UserTerritoryService } from '../user-territory.service';
import { UserService } from '../../../user-management/user.service';
import { ZoneService } from '../../zone/zone.service';
import { ConstantService } from '../../../../Service/constant.service';
import { TerritoryService } from '../../territory/territory.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { AreaService } from '../../area/area.service';
import { RegionService } from '../../region/region.service';
import { ShopService } from '../../shop/shop.service';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';

@Component({
  selector: 'app-create-user-territory',
  templateUrl: './create-user-territory.component.html',
  styleUrls: ['./create-user-territory.component.css'], standalone: false
})

export class CreateUserTerritoryComponent implements OnInit {
  createUserTerritoryForm!: FormGroup;
  isLoading = false;
  dataSource!: any;
  roleList: any;
  dropdownSettings: any;
  isEditMode: boolean = false;

  phoneNoInputMask = createMask('0399-9999999');
  cnicInputMask = createMask('99999-9999999-9');

  userList: any;
  zoneList: any;
  territoryList: any;
  rolesList: any;

  filteredZone: any;
  filteredTerritory: any;

  isTerritoryDisabled: boolean = false;
  isCheckDisabled: boolean = false;

  regionList: any;
  areaList: any;

  isRegionalRole: boolean = false;
  isZoneRole: boolean = false;
  isAreaRole: boolean = false;
  isTerritoryRole: boolean = false;

  shopList: any;

  constructor(private userService: UserService, private zoneService: ZoneService, private territoryService: TerritoryService,
    private notificationsService: NotificationsService, private dialog: MatDialog, private formBuilder: FormBuilder,
    private constantService: ConstantService, private userTerritoryService: UserTerritoryService, private areaService: AreaService,
    private regionService: RegionService, private shopService: ShopService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.createUserTerritoryForm = this.formBuilder.group({
      id: [0],
      userId: ['', Validators.required],
      roleId: ['', Validators.required],
      regionId: [],
      zoneId: [],
      areaId: [],
      territoryId: [],
      isAllTerritoryCheck: [false],
      shop: [null],
      shopId: [null],
      shopName: [''],
    });

    this.getAllRoles();
    this.LoadData(this.data.element);

    this.createUserTerritoryForm.get('isAllTerritoryCheck')?.valueChanges.subscribe(enabled => {
      this.createUserTerritoryForm.get('territoryId')?.patchValue(0);
      if (!enabled) {
        this.createUserTerritoryForm.get('territoryId')?.enable();
      } else {
        this.createUserTerritoryForm.get('territoryId')?.disable();
      }
    });
  }

  get f() {
    return this.createUserTerritoryForm.controls;
  }

  async getTerritoryByAreaId() {

    // if (zoneId > 0) {
    //   this.filteredZone = this.zoneList?.find(zone => zone.id === zoneId);
    // }
    // else {
    //   this.filteredZone = null;
    // }

    this.territoryList = [];
    this.createUserTerritoryForm.get('territoryId')?.patchValue(null);
    (await this.territoryService.getTerritoryByAreaId(this.createUserTerritoryForm.get('areaId')?.value)).subscribe(
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

  async getZonesByUserInTerritory(userId: any) {
    const roleId = this.createUserTerritoryForm.get('roleId')?.value;
    (await this.userTerritoryService.getZonesByUserInTerritory(userId, roleId)).subscribe(
      {
        next: (data) => {
          this.zoneList = data;
        },
        error: (error) => {
          console.log(error);
          this.isLoading = false;
        }
      });
  }

  onTerritoryChange(selectedTerritoryId: number): void {
    if (selectedTerritoryId > 0) {
      this.filteredTerritory = this.territoryList?.find((zone: { id: number; }) => zone.id === selectedTerritoryId);
    }
    else {
      this.filteredTerritory = null;
    }
  }

  async saveUserTerritory() {
    this.isLoading = true;
    if (this.createUserTerritoryForm.invalid) {
      this.constantService.markFormGroupTouched(this.createUserTerritoryForm);
      return;
    }
    let _createUserTerritoryForm: any = {};
    _createUserTerritoryForm = Object.assign(_createUserTerritoryForm, this.createUserTerritoryForm.value);

    (await this.userTerritoryService.saveUserTerritory(_createUserTerritoryForm)).subscribe(
      {
        next: (data) => {
          if (data.Status == 200) {
            this.notificationsService.showNotification('User Territory Saved Successfully!', 'snack-bar-success');
            this.dialog.closeAll();
            this.isLoading = false;
          }
          else if (data.Status == 409) {
            this.notificationsService.showNotification('Same Territory already map with this User!', 'snack-bar-warning');
            this.isLoading = false;
          }
        },
        error: (error) => {
          this.notificationsService.showNotification('Please Fill the required fields!', 'snack-bar-danger');
          console.log(error);
          this.isLoading = false;
        }
      });
  }

  LoadData(element: any) {
    if (this.data?.element?.id != null) {
      this.isEditMode = true;
      this.getTerritoryByAreaId();
      this.constantService.LoadData(element, this.createUserTerritoryForm);

      this.createUserTerritoryForm.get('roleId')?.patchValue(element.user?.aspNetUserRoles[0]?.roleId);
      const roleId = this.createUserTerritoryForm.get('roleId');
      this.getAllUsersByRole(roleId);
      this.onRoleChange();
    }
    console.log(this.createUserTerritoryForm);
  }

  async getAllRoles() {

    (await this.userService.getAllRoles()).subscribe({
      next: (data) => {
        this.rolesList = data;
        this.isLoading = false;
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  onRoleChange() {
    const roleId = this.createUserTerritoryForm.get('roleId')?.value;
    if (roleId != undefined) {

      const regionIdControl = this.createUserTerritoryForm.get('regionId');
      const zoneIdControl = this.createUserTerritoryForm.get('zoneId');
      const areaIdControl = this.createUserTerritoryForm.get('areaId');
      const territoryIdControl = this.createUserTerritoryForm.get('territoryId');

      regionIdControl?.clearValidators();
      zoneIdControl?.clearValidators();
      areaIdControl?.clearValidators();
      // territoryIdControl?.clearValidators();


      this.userList = [];
      this.zoneList = [];
      this.territoryList = [];
      this.regionList = [];
      this.areaList = [];

      var selectedRole = this.rolesList.find((r: { id: any; }) => r.id === roleId);

      if (selectedRole.name == "RSM") {
        regionIdControl?.setValidators(Validators.required);
        this.isRegionalRole = true;
        this.isZoneRole = false;
        this.isAreaRole = false;
        this.isTerritoryRole = false;
      }
      else if (selectedRole.name == "ZSM") {
        regionIdControl?.setValidators(Validators.required);
        zoneIdControl?.setValidators(Validators.required);

        this.isRegionalRole = true;
        this.isZoneRole = true;
        this.isAreaRole = false;
        this.isTerritoryRole = false;
      }
      else if (selectedRole.name == "ASM" || selectedRole.name == "ASE") {
        regionIdControl?.setValidators(Validators.required);
        zoneIdControl?.setValidators(Validators.required);
        areaIdControl?.setValidators(Validators.required);

        this.isRegionalRole = true;
        this.isZoneRole = true;
        this.isAreaRole = true;
        this.isTerritoryRole = false;
      }
      else {
        regionIdControl?.setValidators(Validators.required);
        zoneIdControl?.setValidators(Validators.required);
        areaIdControl?.setValidators(Validators.required);

        this.isRegionalRole = true;
        this.isZoneRole = true;
        this.isAreaRole = true;
        this.isTerritoryRole = true;
      }

      regionIdControl?.updateValueAndValidity();
      zoneIdControl?.updateValueAndValidity();
      areaIdControl?.updateValueAndValidity();
      // territoryIdControl?.updateValueAndValidity();

      if (selectedRole.name == 'DSF') { // DSF
        this.isCheckDisabled = true;
        this.isTerritoryDisabled = false;
        this.createUserTerritoryForm.get('isAllTerritoryCheck')?.patchValue(false);
      }
      else if (selectedRole.name == 'ZSM') { // ZSM
        this.isTerritoryDisabled = true;
        this.isCheckDisabled = true;
        // this.createUserTerritoryForm.get('isAllTerritoryCheck').patchValue(true);
      }
      else if (selectedRole.name == 'ASE') { // SALE Executive
        this.isTerritoryDisabled = false;
        this.isCheckDisabled = true;
      }
      else {
        this.isTerritoryDisabled = false;
        this.isCheckDisabled = true;
      }

      this.getAllUsersByRole(roleId);
    }
    else {
      this.isRegionalRole = false;
      this.isZoneRole = false;
      this.isAreaRole = false;
      this.isTerritoryRole = false;
    }
  }

  async getAllUsersByRole(roleId: any) {
    this.userList = [];
    (await this.userService.getAllUsersByRole(roleId)).subscribe({
      next: (data) => {
        this.userList = data;
        this.isLoading = false;
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  onUserChange() {
    // const userId = this.createUserTerritoryForm.get('userId')?.value;
    this.zoneList = [];
    this.territoryList = [];
    this.areaList = [];
    this.regionList = [];
    this.getRegions();
    // this.getZonesByUserInTerritory(userId);
  }

  async getZoneByRegionId() {

    this.zoneList = [];
    this.areaList = [];
    this.territoryList = [];

    (await this.zoneService.getZoneByRegionId(this.createUserTerritoryForm.get('regionId')?.value)).subscribe({
      next: (data) => {
        this.zoneList = data;
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
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

  async getAreaByZoneId() {

    this.areaList = [];
    this.territoryList = [];

    (await this.areaService.getAreaByZoneId(this.createUserTerritoryForm.get('zoneId')?.value)).subscribe({
      next: (data) => {
        this.areaList = data;
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  async getShopsList(event: any) {
    const filter = event.currentTarget.value;
    this.shopList = []; // Empty the list before updating
    (await this.shopService.searchShopsByTerritoryId(this.createUserTerritoryForm.get('territoryId')?.value, filter)).subscribe(
      (data: any) => {
        this.shopList = data || []; // Ensure it's an array even if no data is returned
      },
      (error) => {
        console.error('Error fetching vehicle list:', error);
        this.shopList = []; // Reset in case of an error
      }
    );
  }

  onShopSelected(event: MatAutocompleteSelectedEvent): void {
    const selectedValue = event.option.value;

    if (!selectedValue) {
      console.error(
        'Option value is undefined. Ensure mat-option [value] is correctly bound.'
      );
      return;
    }

    const selectedItem = this.getShop(selectedValue.id);
    if (!selectedItem) {
      console.error('Selected item not found.');
      return;
    }

    this.createUserTerritoryForm.get('shopId')?.patchValue(selectedValue.id);
    this.createUserTerritoryForm.get('shopName')?.patchValue(selectedValue.name + ' : ' + selectedValue.address);
    this.createUserTerritoryForm.get('shop')?.patchValue(selectedValue);
  }

  getShop(shopId: any) {
    return this.shopList.find((option: { id: any; }) => option.id === shopId);
  }


}