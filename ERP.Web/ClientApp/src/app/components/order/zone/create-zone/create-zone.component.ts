import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ZoneService } from '../zone.service';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { DrawMapComponent } from '../../gmap/draw-map/draw-map.component';
import { RegionService } from '../../region/region.service';

@Component({
  selector: 'app-create-zone',
  templateUrl: './create-zone.component.html',
  styleUrls: ['./create-zone.component.css'], standalone: false
})

export class CreateZoneComponent implements OnInit {
  createZoneForm!: FormGroup;
  isLoading = false;
  zoneListFilerForm!: FormGroup;
  dataSource!: any;
  roleList: any;
  dropdownSettings: any;
  isEditMode: boolean = false;
  pageSize = 1000;
  currentPage = 0;
  regionList: any;
  filteredRegion: any;

  constructor(private notificationsService: NotificationsService, private dialog: MatDialog, private formBuilder: FormBuilder,
    private constantService: ConstantService, private zoneService: ZoneService, private regionService: RegionService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.createZoneForm = this.formBuilder.group({
      id: [0],
      name: ['', Validators.required],
      description: [''],
      coordinates: ['', Validators.required],
      regionId: ['', Validators.required]
    });
    this.zoneListFilerForm = this.formBuilder.group({
      name: [''],
    });

    this.getRegions();
  }

  get f() {
    return this.createZoneForm.controls;
  }

  async saveZone() {
    this.isLoading = true;
    if (this.createZoneForm.invalid) {
      this.constantService.markFormGroupTouched(this.createZoneForm);
      return;
    }
    let _createZoneForm: any = {};
    _createZoneForm = Object.assign(_createZoneForm, this.createZoneForm.value);

    (await this.zoneService.saveZone(_createZoneForm)).subscribe(
      {
        next: (data) => {
          if (data.Status == 200) {
            this.notificationsService.showNotification('Zone Saved Successfully!', 'snack-bar-success');
            this.dialog.closeAll();
            this.isLoading = false;
          }
          else if (data.Status == 409) {
            this.notificationsService.showNotification('Name already exist!', 'snack-bar-danger');
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
    if (this.data.element?.id != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.createZoneForm);
      this.filteredRegion = this.regionList.find((r: { id: any; }) => r.id === this.data.element.regionId);
    }
    console.log(this.createZoneForm);
  }

  drawZone(): void {
    if (this.filteredRegion == null) {
      this.notificationsService.showNotification('Please Select Region First!', 'snack-bar-danger');
      return;
    }

    const coordinatesList: any[] = [];

    coordinatesList.push({
      typeId: 3,
      coordinates: this.filteredRegion.coordinates,
      name: 'Region-' + this.filteredRegion.name,
    });

    this.filteredRegion.zone.forEach((item: { isActive: boolean; coordinates: any; name: string; }) => {
      if (item.isActive == true) {
        coordinatesList.push({
          typeId: 1,
          coordinates: item.coordinates,
          name: 'Zone-' + item.name
        });
      }
    });




    const element = {
      caption: 'Draw Zone (' + this.filteredRegion.name + '-Region)',
      fromComponent: 'createZone',
      drawingPolygon: true,
      drawingMarker: false,
      coordinates: coordinatesList,
      typeId: 1,
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
        this.createZoneForm.get('coordinates')?.setValue(result);
      }
    });
  }

  async editZone() {

    const coordinatesList: any[] = [];

    coordinatesList.push({
      typeId: 1,
      coordinates: this.data.element.coordinates,
      name: 'Zone-' + this.data.element.name
    });

    const element = {
      caption: 'Draw Zone (' + this.filteredRegion.name + '-Region)',
      fromComponent: 'createZone',
      drawingPolygon: false,
      drawingMarker: false,
      coordinates: coordinatesList,
      typeId: 1,
      isFocusDrawPolygon: false,
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
        this.createZoneForm.get('coordinates')?.setValue(result);
      }
    });
  }

  async drawZoneOld() {

    const pagingData = {
      currentPage: this.currentPage,
      take: this.pageSize
    }
    const coordinatesList: any[] = [];

    let _zoneListFilerForm: any = {};
    _zoneListFilerForm = Object.assign(_zoneListFilerForm, this.zoneListFilerForm.value);
    _zoneListFilerForm["PagingData"] = pagingData;

    (await this.zoneService.getAllZone(_zoneListFilerForm)).subscribe({
      next: (data) => {
        data.item1.forEach((item: { coordinates: any; name: string; }) => {
          coordinatesList.push({
            typeId: 1,
            coordinates: item.coordinates,
            name: 'Zone-' + item.name
          });
        });

        const element = {
          caption: 'Draw Zone',
          fromComponent: 'createZone',
          drawingPolygon: true,
          drawingMarker: false,
          typeId: 1,
          coordinates: coordinatesList,
          isFocusDrawPolygon: false,
          isShowZoneCaption: true,
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
            this.createZoneForm.get('coordinates')?.setValue(result);
          }
        });

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
        this.LoadData(this.data.element);
      },
      error: (error) => {
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
}