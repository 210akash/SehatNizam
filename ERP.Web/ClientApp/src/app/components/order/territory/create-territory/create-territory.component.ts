import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { TerritoryService } from '../territory.service';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { AreaService } from '../../area/area.service';
import { DrawMapComponent } from '../../gmap/draw-map/draw-map.component';
import { RegionService } from '../../region/region.service';
import { ZoneService } from '../../zone/zone.service';

@Component({
  selector: 'app-create-territory',
  templateUrl: './create-territory.component.html',
  styleUrls: ['./create-territory.component.css'],standalone: false
})

export class CreateTerritoryComponent implements OnInit {
  createTerritoryForm!: FormGroup;
  isLoading = false;
  dataSource!: any;
  roleList: any;
  dropdownSettings: any;
  isEditMode: boolean = false;

  regionList: any;
  zoneList: any;
  areaList: any;

  filteredZone: any;
  filteredRegion: any;
  filteredArea: any;

  constructor(private notificationsService: NotificationsService, private zoneService: ZoneService, private dialog: MatDialog, private formBuilder: FormBuilder,
    private constantService: ConstantService, private territoryService: TerritoryService, private areaService: AreaService, private regionService: RegionService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.createTerritoryForm = this.formBuilder.group({
      id: [0],
      name: ['', Validators.required],
      description: [''],
      coordinates: ['', Validators.required],
      regionId: [0, Validators.required],
      zoneId: [0, Validators.required],
      areaId: [0, Validators.required],
      saleModel: ['', Validators.required],
    });

    this.getRegions();
    this.LoadData(this.data.element);
  }

  get f() {
    return this.createTerritoryForm.controls;
  }

  async saveTerritory() {
    this.isLoading = true;
    if (this.createTerritoryForm.invalid) {
      this.constantService.markFormGroupTouched(this.createTerritoryForm);
      return;
    }
    let _createTerritoryForm: any = {};
    _createTerritoryForm = Object.assign(_createTerritoryForm, this.createTerritoryForm.value);

    (await this.territoryService.saveTerritory(_createTerritoryForm)).subscribe(
      {
        next: (data: { Status: number; }) => {
          if (data.Status == 200) {
            this.notificationsService.showNotification('Territory Saved Successfully', 'snack-bar-success');
            this.dialog.closeAll();
            this.isLoading = false;
          }
          else if (data.Status == 409) {
            this.notificationsService.showNotification('Name already exist!', 'snack-bar-danger');
            this.isLoading = false;
          }
        },
        error: (error: any) => {
          this.notificationsService.showNotification('Please Fill the required fields', 'snack-bar-danger');
          console.log(error);
          this.isLoading = false;
        }
      });
  }

  LoadData(element: any) {
    if (this.data.element != null) {

      this.isEditMode = true;
      this.constantService.LoadData(element, this.createTerritoryForm);

      this.createTerritoryForm.get('regionId')?.patchValue(this.data.element.area?.zone?.regionId);
      this.getZoneByRegionId();

      this.createTerritoryForm.get('zoneId')?.patchValue(this.data.element.area?.zoneId);
      this.getAreaByZoneId();

    }
    console.log(this.createTerritoryForm);
  }

  async getAreaByZoneId() {
    (await this.areaService.getAreaByZoneId(this.createTerritoryForm.get('zoneId')?.value)).subscribe({
      next: (data) => {
        this.areaList = data;
        if (this.data.element?.id != null) {
          this.filteredArea = this.areaList.find((r: { id: any; }) => r.id === this.data.element.areaId);
        }

      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  drawTerritory(): void {
    if (this.filteredRegion == null) {
      this.notificationsService.showNotification('Please Select Region First!', 'snack-bar-danger');
      return;
    }

    if (this.filteredZone == null) {
      this.notificationsService.showNotification('Please Select Zone First!', 'snack-bar-danger');
      return;
    }

    if (this.filteredArea == null) {
      this.notificationsService.showNotification('Please Select Area First!', 'snack-bar-danger');
      return;
    }

    const coordinatesList: any[] = [];

    coordinatesList.push({
      typeId: 3,
      coordinates: this.filteredRegion.coordinates,
      name: 'Region-' + this.filteredRegion.name,
    });

    coordinatesList.push({
      typeId: 1,
      coordinates: this.filteredZone.coordinates,
      name: 'Zone-' + this.filteredZone.name,
    });

    coordinatesList.push({
      typeId: 4,
      coordinates: this.filteredArea.coordinates,
      name: 'Area-' + this.filteredArea.name,
    });


    this.filteredArea.territory.forEach((item: { isActive: boolean; coordinates: any; name: string; }) => {
      if(item.isActive == true)
      {
        coordinatesList.push({
          typeId: 2,
          coordinates: item.coordinates,
          name : 'Territory-' + item.name
        });
      }
    });


    const element = {
      caption: 'Draw Territory (' + this.filteredZone.name + '-Zone)',
      fromComponent: 'createTerritory',
      drawingPolygon: true,
      drawingMarker: false,
      coordinates: coordinatesList,
      typeId: 2,
      isFocusDrawPolygon: true,
      isShowInfoBox: true
    };

    const dialogRef = this.dialog.open(DrawMapComponent, {
      width: '70%',
      height: 'auto',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.createTerritoryForm.get('coordinates')?.setValue(result);
      }
    });
  }

  async editTerritory() {

    const coordinatesList: any[] = [];

    coordinatesList.push({
      typeId: 2,
      coordinates: this.data.element.coordinates,
      name: 'Territory-' + this.data.element.name
    });
    const element = {
      caption: 'Draw Territory (' + this.filteredZone.name + '-Zone)',
      fromComponent: 'createTerritory',
      drawingPolygon: false,
      drawingMarker: false,
      coordinates: coordinatesList,
      typeId: 2,
      isFocusDrawPolygon: true,
      isShowInfoBox: false,
      isExpandPolygon: true
    };


    const dialogRef = this.dialog.open(DrawMapComponent, {
      width: '70%',
      height: 'auto',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.createTerritoryForm.get('coordinates')?.setValue(result);
      }
    });
  }

  onZoneChange(selectedZoneId: number): void {
    if (selectedZoneId > 0) {
      this.filteredZone = this.zoneList.find((zone: { id: number; }) => zone.id === selectedZoneId);
    }
    else {
      this.filteredZone = null;
    }
  }

  async getZoneByRegionId() {
    (await this.zoneService.getZoneByRegionId(this.createTerritoryForm.get('regionId')?.value)).subscribe({
      next: (data) => {
        this.zoneList = data;
        if (this.data.element != null) {
          this.filteredZone = this.zoneList.find((zone: { id: any; }) => zone.id === this.data.element.area?.zoneId);
        }
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
        if (this.data.element != null) {
        this.filteredRegion = this.regionList.find((r: { id: any; }) => r.id === this.data.element.area.zone.regionId);
        }
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }
  onRegionChange(selectedRegionId: number): void {
    if (selectedRegionId > 0) {
      this.filteredRegion = this.regionList.find((x: { id: number; isActive: boolean; }) => x.id === selectedRegionId && x.isActive == true);
    }
    else {
      this.filteredRegion = null;
    }
  }
  onAreaChange(selectedAreaId: number): void {
    if (selectedAreaId > 0) {
      this.filteredArea = this.areaList.find((x: { id: number; isActive: boolean; }) => x.id === selectedAreaId && x.isActive == true);
    }
    else {
      this.filteredArea = null;
    }
  }
}