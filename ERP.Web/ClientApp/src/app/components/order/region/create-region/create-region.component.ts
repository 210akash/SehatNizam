import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { RegionService } from '../region.service';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { DrawMapComponent } from '../../gmap/draw-map/draw-map.component';

@Component({
  selector: 'app-create-region',
  templateUrl: './create-region.component.html',
  styleUrls: ['./create-region.component.css'],standalone: false
})

export class CreateRegionComponent implements OnInit {
  createRegionForm!: FormGroup;
  isLoading = false;
  regionListFilerForm!: FormGroup;
  dataSource!: any;
  roleList: any;
  dropdownSettings: any;
  isEditMode: boolean = false;
  pageSize = 1000;
  currentPage = 0;

  constructor(private notificationsService: NotificationsService, private dialog: MatDialog, private formBuilder: FormBuilder, private constantService: ConstantService, private regionService: RegionService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.createRegionForm = this.formBuilder.group({
      id: [0],
      name: ['', Validators.required],
      description: [''],
      coordinates: ['', Validators.required]
    });
    this.regionListFilerForm = this.formBuilder.group({
      name: [''],
    });
    this.LoadData(this.data.element);
  }

  get f() {
    return this.createRegionForm.controls;
  }

  async saveRegion() {
    this.isLoading = true;
    if (this.createRegionForm.invalid) {
      this.constantService.markFormGroupTouched(this.createRegionForm);
      return;
    }
    let _createRegionForm: any = {};
    _createRegionForm = Object.assign(_createRegionForm, this.createRegionForm.value);

    (await this.regionService.saveRegion(_createRegionForm)).subscribe(
      {
        next: (data: { Status: number; }) => {
          if (data.Status == 200) {
            this.notificationsService.showNotification('Region Saved Successfully!', 'snack-bar-success');
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
      this.constantService.LoadData(element, this.createRegionForm);
    }
    console.log(this.createRegionForm);
  }

  async drawRegion() {

    const pagingData = {
      currentPage: this.currentPage,
      take: this.pageSize
    }
    const coordinatesList: any[] = [];

    let _regionListFilerForm: any = {};
    _regionListFilerForm = Object.assign(_regionListFilerForm, this.regionListFilerForm.value);
    _regionListFilerForm["PagingData"] = pagingData;

    (await this.regionService.getAllRegion(_regionListFilerForm)).subscribe({
      next: (data: any) => {
        data.item1.forEach((item: { coordinates: any; name: string; }) => {
          coordinatesList.push({
            typeId: 3,
            coordinates: item.coordinates,
            name: 'Region-' + item.name
          });
        });

        const element = {
          caption: 'Draw Region',
          fromComponent: 'createRegion',
          drawingPolygon: true,
          drawingMarker: false,
          typeId: 3,
          coordinates: coordinatesList,
          isFocusDrawPolygon: false,
          isShowRegionCaption: true,
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
            this.createRegionForm.get('coordinates')?.setValue(result);
          }
        });

      },
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  async editRegion() {

    const coordinatesList: any[] = [];

    coordinatesList.push({
      typeId: 3,
      coordinates: this.data.element.coordinates,
      name: 'Region-' + this.data.element.name
    });

    const element = {
      caption: 'Draw Region',
      fromComponent: 'createRegion',
      drawingPolygon: false,
      drawingMarker: false,
      typeId: 3,
      coordinates: coordinatesList,
      isFocusDrawPolygon: false,
      isShowRegionCaption: true,
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
        this.createRegionForm.get('coordinates')?.setValue(result);
      }
    });
  }

}