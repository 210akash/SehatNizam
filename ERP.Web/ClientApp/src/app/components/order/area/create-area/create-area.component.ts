import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { AreaService } from '../area.service';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { DrawMapComponent } from '../../gmap/draw-map/draw-map.component';
import { RegionService } from '../../region/region.service';
import { ZoneService } from '../../zone/zone.service';

@Component({
  selector: 'app-create-area',
  templateUrl: './create-area.component.html',
  styleUrls: ['./create-area.component.css'],standalone: false
})

export class CreateAreaComponent implements OnInit {
  createAreaForm!: FormGroup;
  isLoading = false;
  areaListFilerForm!: FormGroup;
  dataSource!: any;
  dropdownSettings: any;
  isEditMode: boolean = false;
  pageSize = 1000;
  currentPage = 0;
  zoneList: any;
  regionList: any;

  filteredRegion: any;
  filteredZone: any;

  constructor(private notificationsService: NotificationsService, private dialog: MatDialog, private formBuilder: FormBuilder,
    private constantService: ConstantService, private areaService: AreaService, private zoneService: ZoneService, private regionService: RegionService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.createAreaForm = this.formBuilder.group({
      id: [0],
      name: ['', Validators.required],
      description: [''],
      coordinates: ['', Validators.required],
      zoneId: ['', Validators.required],
      regionId: ['', Validators.required]
    });

    this.areaListFilerForm = this.formBuilder.group({
      name: [''],
    });

    this.getRegions();
    this.LoadData(this.data.element);
  }

  get f() {
    return this.createAreaForm.controls;
  }

  async saveArea() {
    this.isLoading = true;
    if (this.createAreaForm.invalid) {
      this.constantService.markFormGroupTouched(this.createAreaForm);
      return;
    }
    let _createAreaForm: any = {};
    _createAreaForm = Object.assign(_createAreaForm, this.createAreaForm.value);

    (await this.areaService.saveArea(_createAreaForm)).subscribe(
      {
        next: (data: { Status: number; }) => {
          if (data.Status == 200) {
            this.notificationsService.showNotification('Area Saved Successfully!', 'snack-bar-success');
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
    if (this.data.element?.id != null) {
      this.isEditMode = true;

      this.createAreaForm.get('regionId')?.patchValue(this.data.element.zone?.regionId);
      this.getZonesByRegionId();
      
      this.constantService.LoadData(element, this.createAreaForm);

      this.filteredRegion = this.regionList.find((r: { id: any; }) => r.id === this.data.element.zone.regionId);
      // this.filteredZone = this.zoneList.find(r => r.id === this.data.element.zoneId);
    }
    console.log(this.createAreaForm);
  }

  drawArea(): void {
    if (this.filteredRegion == null) {
      this.notificationsService.showNotification('Please Select Region First!', 'snack-bar-danger');
      return;
    }

    if (this.filteredZone == null) {
      this.notificationsService.showNotification('Please Select Zone First!', 'snack-bar-danger');
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


    this.filteredZone.area.forEach((item: { isActive: boolean; coordinates: any; name: string; }) => {
      if(item.isActive == true)
      {
        coordinatesList.push({
          typeId: 4,
          coordinates: item.coordinates,
          name : 'Area-' + item.name
        });
      }
    });

    const element = {
      caption: 'Draw Area (' + this.filteredRegion.name + '-Region)',
      fromComponent: 'createArea',
      drawingPolygon: true,
      drawingMarker: false,
      coordinates: coordinatesList,
      typeId: 4,
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
        this.createAreaForm.get('coordinates')?.setValue(result);
      }
    });
  }


  async editArea() {

    const coordinatesList: any[] = [];

    coordinatesList.push({
      typeId: 4,
      coordinates: this.data.element.coordinates,
      name: 'Area-' + this.data.element.name
    });

    const element = {
      caption: 'Draw Area (' + this.filteredRegion.name + '-Region)',
      fromComponent: 'createArea',
      drawingPolygon: false,
      drawingMarker: false,
      coordinates: coordinatesList,
      typeId: 4,
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
        this.createAreaForm.get('coordinates')?.setValue(result);
      }
    });
  }


  async drawAreaOld() {

    const pagingData = {
      currentPage: this.currentPage,
      take: this.pageSize
    }
    const coordinatesList: any[] = [];

    let _areaListFilerForm: any = {};
    _areaListFilerForm = Object.assign(_areaListFilerForm, this.areaListFilerForm.value);
    _areaListFilerForm["PagingData"] = pagingData;

    (await this.areaService.getAllArea(_areaListFilerForm)).subscribe({
      next: (data: { item1: any[]; }) => {
        data.item1.forEach((item: { coordinates: any; name: string; }) => {
          coordinatesList.push({
            typeId: 4,
            coordinates: item.coordinates,
            name: 'Area-' + item.name
          });
        });

        const element = {
          caption: 'Draw Area',
          fromComponent: 'createArea',
          drawingPolygon: true,
          drawingMarker: false,
          typeId: 4,
          coordinates: coordinatesList,
          isFocusDrawPolygon: false,
          isShowAreaCaption: true,
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
            this.createAreaForm.get('coordinates')?.setValue(result);
          }
        });

      },
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  async getZonesByRegionId() {
    (await this.zoneService.getZoneByRegionId(this.createAreaForm.get('regionId')?.value)).subscribe({
      next: (data) => {
        this.zoneList = data;
        if (this.data.element?.id != null) {
          this.filteredZone = this.zoneList.find((r: { id: any; }) => r.id === this.data.element.zoneId);
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
      next: (data: { item1: any; }) => {
        this.regionList = data.item1;
        this.LoadData(this.data.element);
      },
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  onRegionChange(selectedRegionId: number): void {
    if (selectedRegionId > 0) {
      this.filteredRegion = this.regionList.find((x: { id: number; }) => x.id === selectedRegionId);
    }
    else {
      this.filteredRegion = null;
    }
  }
  onZoneChange(selectedZoneId: number): void {
    if (selectedZoneId > 0) {
      this.filteredZone = this.zoneList.find((x: { id: number; }) => x.id === selectedZoneId);
    }
    else {
      this.filteredZone = null;
    }
  }

}