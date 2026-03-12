import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { TerritoryService } from '../territory.service';
import { DeleteTerritoryComponent } from '../delete-territory/delete-territory.component';
import { ViewTerritoryComponent } from '../view-territory/view-territory.component';
import { CreateTerritoryComponent } from '../create-territory/create-territory.component';
import { ConstantService } from '../../../../Service/constant.service';
import { AreaService } from '../../area/area.service';
import { DrawMapComponent } from '../../gmap/draw-map/draw-map.component';
import { RegionService } from '../../region/region.service';
import { ZoneService } from '../../zone/zone.service';

@Component({
  selector: 'app-territory-list',
  templateUrl: './territory-list.component.html',
  styleUrls: ['./territory-list.component.css'],standalone: false
})

export class TerritoryListComponent implements OnInit {
  dataSource: any;
  territoryListFilerForm!: FormGroup;
  isEditMode: boolean = false;
  displayedColumns: string[] = ['code', 'region', 'zone', 'area', 'name', 'description', 'saleModel', 'createdDate', 'actions'];
  isLoading = false;

  currentPage = 0;
  pageSize = 0;
  totalRows = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];

  regionList: any[] = [];
  zoneList: any[] = [];
  areaList: any[] = [];

  constructor(private constantService: ConstantService, private dialog: MatDialog, private territoryService: TerritoryService, private formBuilder: FormBuilder,
    private regionService: RegionService, private zoneService: ZoneService, private areaService: AreaService) { }

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngOnInit(): void {
    this.pageSize = this.constantService.defaultItemPerPage;

    this.territoryListFilerForm = this.formBuilder.group({
      regionId: [0],
      zoneId: [0],
      areaId: [0],
    });

    this.bindData();
    this.getRegions();
  }

  openTerritoryDialog(element: any): void {
    const dialogRef = this.dialog.open(CreateTerritoryComponent, {
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

  openViewTerritoryDialog(enterAnimationDuration: string, exitAnimationDuration: string, element: any): void {
    this.dialog.open(ViewTerritoryComponent, {
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

    let _territoryListFilerForm: any = {};
    _territoryListFilerForm = Object.assign(_territoryListFilerForm, this.territoryListFilerForm.value);
    _territoryListFilerForm["PagingData"] = pagingData;

    (await this.territoryService.getAllTerritory(_territoryListFilerForm)).subscribe({
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
    console.log({ event });
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData();
  }

  openDeleteDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteTerritoryComponent, {
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

  onReset() {
    this.territoryListFilerForm.patchValue({
      regionId: 0,
      zoneId: 0,
      areaId: 0,
    });
    this.bindData();
  }

  filterData() {
    this.bindData();
  }

  // onZoneChange() {
  //   this.filterData();
  // }

  // async getZones() {
  //   let _zoneFilterForm = {};
  //   (await this.zoneService.getAllZone(_zoneFilterForm)).subscribe({
  //     next: (data) => {
  //       if (data && Array.isArray(data.item1)) {
  //         this.zoneList = data.item1;
  //       } else {
  //         console.error('Expected array but got:', data.item1);
  //         this.zoneList = [];
  //       }
  //     },
  //     error: (error) => {
  //       console.log(error);
  //       this.isLoading = false;
  //     }
  //   });
  // }

  viewTerritory(element: any): void {

    const coordinatesList: any[] = [];

    coordinatesList.push({
      typeId: 3,
      coordinates: element.area.zone.region.coordinates,
      name: 'Region-' + element.area.zone.region.name,
    });

    coordinatesList.push({
      typeId: 1,
      coordinates: element.area.zone.coordinates,
      name: 'Zone-' + element.area.zone.name,
    });

    coordinatesList.push({
      typeId: 4,
      coordinates: element.area.coordinates,
      name: 'Area-' + element.area.name,
    });

    coordinatesList.push({
      typeId: 2,
      coordinates: element.coordinates,
      name: 'Territory-' + element.name,
    });

    const elementToSend = {
      caption: 'Territory: ' + element.name + ' - Zone: ' + element.area?.zone?.name,
      fromComponent: 'viewZone',
      drawingPolygon: false,
      drawingMarker: false,
      coordinates: coordinatesList,
      isFocusDrawPolygon: true,
      isShowInfoBox: true,
      isShowZoneCaption: true
    };

    const dialogRef = this.dialog.open(DrawMapComponent, {
      width: '70%',
      height: 'auto',
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
    this.areaList = [];

    (await this.zoneService.getZoneByRegionId(this.territoryListFilerForm.get('regionId')?.value)).subscribe({
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

    (await this.areaService.getAreaByZoneId(this.territoryListFilerForm.get('zoneId')?.value)).subscribe({
      next: (data) => {
        this.areaList = data;
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }


}