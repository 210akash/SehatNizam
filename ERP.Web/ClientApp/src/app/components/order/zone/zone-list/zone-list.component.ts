import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { ZoneService } from '../zone.service';
import { DeleteZoneComponent } from '../delete-zone/delete-zone.component';
import { ViewZoneComponent } from '../view-zone/view-zone.component';
import { CreateZoneComponent } from '../create-zone/create-zone.component';
import { ConstantService } from '../../../../Service/constant.service';
import { DrawMapComponent } from '../../gmap/draw-map/draw-map.component';
import { RegionService } from '../../region/region.service';

@Component({
  selector: 'app-zone-list',
  templateUrl: './zone-list.component.html',
  styleUrls: ['./zone-list.component.css'],standalone: false
})

export class ZoneListComponent implements OnInit {
  dataSource: any;
  zoneListFilerForm!: FormGroup;
  isEditMode: boolean = false;
  displayedColumns: string[] = ['code', 'region', 'name', 'description',  'createdDate', 'actions'];
  isLoading = false;
  element: any;
  blob: any;

  currentPage = 0;
  pageSize = 0;
  totalRows = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];

  regionList: any[] = [];

  constructor(private constantService: ConstantService, private dialog: MatDialog, private zoneService: ZoneService, private formBuilder: FormBuilder, private regionService: RegionService) { }
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngOnInit(): void {
    this.pageSize = this.constantService.defaultItemPerPage;

    this.zoneListFilerForm = this.formBuilder.group({
      regionId: [0],
    });

    this.getRegions();
    this.bindData();
  }

  openZoneDialog(element: any): void {
    const dialogRef = this.dialog.open(CreateZoneComponent, {
      data: { element: element },
      width: '30%',
      autoFocus: true,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
  }

  openViewZoneDialog(enterAnimationDuration: string, exitAnimationDuration: string, element: any): void {
    this.dialog.open(ViewZoneComponent, {
      data: { element: element },
      width: '30%',
      autoFocus: true,
      disableClose: true
    }),
    {
      enterAnimationDuration,
      exitAnimationDuration,
    };
  }

  async bindData() {
    this.isLoading = true;
    const pagingData = {
      currentPage: this.currentPage,
      take: this.pageSize
    }

    let _zoneListFilerForm: any = {};
    _zoneListFilerForm = Object.assign(_zoneListFilerForm, this.zoneListFilerForm.value);
    _zoneListFilerForm["PagingData"] = pagingData;

    (await this.zoneService.getAllZone(_zoneListFilerForm)).subscribe({
      next: (data) => {
        this.dataSource = new MatTableDataSource(data.item1);
        if (data.item1.length > 0) {
          setTimeout(() => {
            this.paginator.pageIndex = this.currentPage;
            this.paginator.length = data.item2;
          });
        }
        console.log(this.dataSource);
        this.isLoading = false;
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  pageChanged(event: PageEvent) {
    console.log({ event });
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData();
  }

  openDeleteDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteZoneComponent, {
      data: { element: element },
      width: '30%',
      autoFocus: true,
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
  }

  viewZone(element: any): void {
    const coordinatesList: any[] = [];

    coordinatesList.push({
      typeId: 3,
      coordinates: element.region.coordinates,
      name: 'Region-' + element.region.name,
    });

    coordinatesList.push({
      typeId: 1,
      coordinates: element.coordinates,
      name: 'Zone-' + element.name,
    });

    const elementToSend = {
      caption: 'View Zone ( ' + element.name + ')',
      fromComponent: 'viewZone',
      drawingPolygon: false,
      drawingMarker: false,
      coordinates: coordinatesList,
      zoneDescription: element.description,
      isFocusDrawPolygon: true,
      isShowZoneCaption: true,
      isShowInfoBox: true
    };

    const dialogRef = this.dialog.open(DrawMapComponent, {
      width: '95%',
      height: '88vh',
      data: {
        element: elementToSend,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {

    });
  }

  filterData() {
    this.bindData();
  }

  onReset() {
    this.zoneListFilerForm.patchValue({
      regionId: 0,
    });
    this.bindData();
  }

  async getRegions() {
    let _regionFilterForm = {};
    (await this.regionService.getAllRegion(_regionFilterForm)).subscribe({
      next: (data: { item1: any[]; }) => {
        this.regionList = data.item1;
      },
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }


}