import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { AreaService } from '../area.service';
import { DeleteAreaComponent } from '../delete-area/delete-area.component';
import { ViewAreaComponent } from '../view-area/view-area.component';
import { CreateAreaComponent } from '../create-area/create-area.component';
import { ConstantService } from '../../../../Service/constant.service';
import { DrawMapComponent } from '../../gmap/draw-map/draw-map.component';
import { RegionService } from '../../region/region.service';
import { ZoneService } from '../../zone/zone.service';

@Component({
  selector: 'app-area-list',
  templateUrl: './area-list.component.html',
  styleUrls: ['./area-list.component.css'],standalone: false
})

export class AreaListComponent implements OnInit {
  dataSource: any;
  areaListFilerForm!: FormGroup;
  isEditMode: boolean = false;
  displayedColumns: string[] = ['code', 'region', 'zone', 'name', 'description', 'createdDate', 'actions'];
  isLoading = false;

  currentPage = 0;
  pageSize = 0;
  totalRows = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];

  regionList: any[] = [];
  zoneList: any[] = [];

  constructor(private constantService: ConstantService, private dialog: MatDialog, private areaService: AreaService, private formBuilder: FormBuilder, private regionService: RegionService,
    private zoneService: ZoneService) { }

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngOnInit(): void {
    this.pageSize = this.constantService.defaultItemPerPage;

    this.areaListFilerForm = this.formBuilder.group({
      regionId: [0],
      zoneId: [0],
    });

    this.getRegions();
    this.bindData();
  }

  openAreaDialog(element: any): void {
    const dialogRef = this.dialog.open(CreateAreaComponent, {
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

  openViewAreaDialog(enterAnimationDuration: string, exitAnimationDuration: string, element: any): void {
    this.dialog.open(ViewAreaComponent, {
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

    let _areaListFilerForm: any = {};
    _areaListFilerForm = Object.assign(_areaListFilerForm, this.areaListFilerForm.value);
    _areaListFilerForm["PagingData"] = pagingData;

    (await this.areaService.getAllArea(_areaListFilerForm)).subscribe({
      next: (data: any) => {
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
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  pageChanged(event: PageEvent) {
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData();
  }

  openDeleteDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteAreaComponent, {
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

  viewArea(element: any): void {
    const coordinatesList: any[] = [];

    coordinatesList.push({
      typeId: 3,
      coordinates: element.zone.region.coordinates,
      name: 'Region-' + element.zone.region.name,
    });

    coordinatesList.push({
      typeId: 1,
      coordinates: element.zone.coordinates,
      name: 'Zone-' + element.zone.name,
    });

    coordinatesList.push({
      typeId: 4,
      coordinates: element.coordinates,
      name: 'Area-' + element.name,
    });

    const elementToSend = {
      caption: 'View Area ( ' + element.name + ')',
      fromComponent: 'viewArea',
      drawingPolygon: false,
      drawingMarker: false,
      coordinates: coordinatesList,
      areaDescription: element.description,
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

  async getZoneByRegionId() {

    this.zoneList = [];

    (await this.zoneService.getZoneByRegionId(this.areaListFilerForm.get('regionId')?.value)).subscribe({
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
      next: (data: { item1: any[]; }) => {
        this.regionList = data.item1;
      },
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  filterData() {
    this.bindData();
  }

  onReset() {
    this.areaListFilerForm.patchValue({
      regionId: 0,
      zoneId: 0,
    });
    this.bindData();
  }


}
