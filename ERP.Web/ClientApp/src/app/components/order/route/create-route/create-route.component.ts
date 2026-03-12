import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { RouteService } from '../route.service';
import { AuthenticationService } from '../../../../Auth/authentication.service';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { RegionService } from '../../region/region.service';
import { AreaService } from '../../area/area.service';
import { TerritoryService } from '../../territory/territory.service';
import { ZoneService } from '../../zone/zone.service';

@Component({
  selector: 'app-create-route',
  templateUrl: './create-route.component.html',
  styleUrls: ['./create-route.component.css'],standalone: false
})

export class CreateRouteComponent implements OnInit {
  createRouteForm!: FormGroup;
  isLoading = false;
  dataSource!: any;
  roleList: any;
  dropdownSettings: any;
  isEditMode: boolean = false;
  currentuser: any;
  isDropDownShow = true;

  regionList: any;
  zoneList: any;
  areaList: any;
  territoryList: any;

  zoneId: any;
  weekDays: string[] = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
  gRoute: any;

  constructor(private auth: AuthenticationService, private notificationsService: NotificationsService, private territoryService: TerritoryService, private zoneService: ZoneService, private dialog: MatDialog,
    private formBuilder: FormBuilder, private constantService: ConstantService, private routeService: RouteService, private areaService: AreaService, private regionService: RegionService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {

    this.currentuser = this.auth.currentUserValue;
    const rolesArray = this.currentuser.role.split(',').map((role: string) => role.trim());

    this.createRouteForm = this.formBuilder.group({
      id: [0],
      name: ['', Validators.required],
      territoryId: [0, Validators.required],
      zoneId: [0],
      // visitDay: ['', Validators.required]
      regionId: [0],
      areaId: [0],
    });

    this.getRegions();

    if (this.data.element != null) {
      this.getRouteById(this.data.element?.id);

      this.zoneId = this.gRoute?.territory?.area?.zone?.id;
      this.createRouteForm.get('zoneId')?.patchValue(this.zoneId);
      this.getTerritoryByAreaId();
    }

    if (rolesArray.includes("ASE")) {
      this.isDropDownShow = false;
      this.createRouteForm.get('zoneId')?.patchValue(this.currentuser.zoneId);
      this.createRouteForm.get('territoryId')?.patchValue(this.currentuser.territoryId);
    }
  }

  get f() {
    return this.createRouteForm.controls;
  }

  async saveRoute() {
    this.isLoading = true;
    if (this.createRouteForm.invalid) {
      this.constantService.markFormGroupTouched(this.createRouteForm);
      return;
    }
    let _createRouteForm: any = {};
    _createRouteForm = Object.assign(_createRouteForm, this.createRouteForm.value);

    (await this.routeService.saveRoute(_createRouteForm)).subscribe(
      {
        next: (data: { Status: number; }) => {
          if (data.Status == 200) {
            this.notificationsService.showNotification('Routed Saved Successfully', 'snack-bar-success');
            this.dialog.closeAll();
            this.isLoading = false;
          }
          else if (data.Status == 409) {
            this.notificationsService.showNotification('Name already exist!', 'snack-bar-danger');
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
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.createRouteForm);

      this.createRouteForm.get('regionId')?.patchValue(element.territory?.area?.zone?.regionId);
      this.getZoneByRegionId();

      this.createRouteForm.get('zoneId')?.patchValue(element.territory?.area?.zoneId);
      this.getAreaByZoneId();

      this.createRouteForm.get('areaId')?.patchValue(element.territory?.areaId);
      this.getTerritoryByAreaId();

      this.createRouteForm.get('territoryId')?.patchValue(element.territoryId);
    }
    console.log(this.createRouteForm);
  }

  async getTerritoryByAreaId() {
    this.territoryList = [];
    this.createRouteForm.get('territoryId')?.patchValue(0);
    (await this.territoryService.getTerritoryByAreaId(this.createRouteForm.get('areaId')?.value)).subscribe(
      {
        next: (data: any) => {
          this.territoryList = data;
        },
        error: (error: any) => {
          console.log(error);
          this.isLoading = false;
        }
      });
  }

  async getZoneByRegionId() {

    this.zoneList = [];
    this.areaList = [];
    this.territoryList = [];

    (await this.zoneService.getZoneByRegionId(this.createRouteForm.get('regionId')?.value)).subscribe({
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

    (await this.areaService.getAreaByZoneId(this.createRouteForm.get('zoneId')?.value)).subscribe({
      next: (data) => {
        this.areaList = data;
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  async getRouteById(routeId: any) {
    (await this.routeService.getRouteById(routeId)).subscribe({
      next: (data: any) => {
        this.gRoute = data;
        this.LoadData(data);
      },
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }


}