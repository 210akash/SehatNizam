import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { SalesTargetService } from '../sales-target.service';
import { ViewSalesTargetComponent } from '../view-sales-target/view-sales-target.component';
import { DeleteSalesTargetComponent } from '../delete-sales-target/delete-sales-target.component';
import { CreateTerritoryTargetComponent } from '../create-territory-target/create-territory-target.component';
import { CreateDSFTargetComponent } from '../create-dsf-target/create-dsf-target.component';
import { CreateSalesTargetComponent } from '../create-sales-target/create-sales-target.component';
import { ConstantService } from '../../../../Service/constant.service';
import { AreaService } from '../../area/area.service';
import { RegionService } from '../../region/region.service';
import { TerritoryService } from '../../territory/territory.service';
import { ZoneService } from '../../zone/zone.service';

@Component({
  selector: 'app-sales-target-list',
  templateUrl: './sales-target-list.component.html',
  styleUrls: ['./sales-target-list.component.css'], standalone: false
})

export class SalesTargetListComponent implements OnInit {
  dataSource: any;
  salesTargetListFilerForm!: FormGroup;
  isEditMode: boolean = false;
  displayedColumns: string[] = ['region', 'zone', 'area', 'territory', 'user', 'target', 'targetMonth', 'createdDate', 'actions'];
  isLoading = false;
  element: any;

  blob: any;

  currentPage = 0;
  pageSize = 0;
  totalRows = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];

  regionList: any[] = [];
  zoneList: any[] = [];
  areaList: any[] = [];
  territoryList: any[] = [];

  constructor(private constantService: ConstantService, private dialog: MatDialog, private salesTargetService: SalesTargetService, private formBuilder: FormBuilder, private zoneService: ZoneService,
    private areaService: AreaService, private regionService: RegionService, private territoryService: TerritoryService,
  ) { }

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngOnInit(): void {
    this.pageSize = this.constantService.defaultItemPerPage;

    this.salesTargetListFilerForm = this.formBuilder.group({
      regionId: [0],
      zoneId: [0],
      areaId: [0],
      territoryId: [0],
      distributor: ['']
    });

    this.bindData();
    this.getRegions();
  }

  openSalesTargetDialog(element: any): void {
    const dialogRef = this.dialog.open(CreateSalesTargetComponent, {
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

  openViewSalesTargetDialog(enterAnimationDuration: string, exitAnimationDuration: string, element: any): void {
    this.dialog.open(ViewSalesTargetComponent, {
      data: { element: element },
      width: '90%',
      maxHeight: '95vh',
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

    let _salesTargetListFilerForm: any = {};
    _salesTargetListFilerForm = Object.assign(_salesTargetListFilerForm, this.salesTargetListFilerForm.value);
    _salesTargetListFilerForm["PagingData"] = pagingData;

    (await this.salesTargetService.getAllSalesTarget(_salesTargetListFilerForm)).subscribe({
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
    const dialogRef = this.dialog.open(DeleteSalesTargetComponent, {
      data: { element: element },
      width: '30%',
      maxHeight: '95vh',
      autoFocus: true,
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
  }

  openTerritoryTargetDialog(element: any): void {
    const dialogRef = this.dialog.open(CreateTerritoryTargetComponent, {
      data: { element: element },
      width: '30%',
      maxHeight: '95vh',
      autoFocus: true,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
  }

  openDSFTargetDialog(element: any): void {
    const dialogRef = this.dialog.open(CreateDSFTargetComponent, {
      data: { element: element },
      width: '30%',
      maxHeight: '95vh',
      autoFocus: true,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
  }

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

  onReset() {
    this.salesTargetListFilerForm.patchValue({
      regionId: 0,
      zoneId: 0,
      areaId: 0,
      territoryId: 0,
      distributor: ''
    });

    this.bindData();
  }

  filterData() {
    this.bindData();
  }

  // onZoneChange() {
  //   this.filterData();
  // }

  async getZoneByRegionId() {

    this.zoneList = [];
    this.areaList = [];
    this.territoryList = [];

    (await this.zoneService.getZoneByRegionId(this.salesTargetListFilerForm.get('regionId')?.value)).subscribe({
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

    (await this.areaService.getAreaByZoneId(this.salesTargetListFilerForm.get('zoneId')?.value)).subscribe({
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

    this.salesTargetListFilerForm.get('territoryId')?.patchValue(0);
    (await this.territoryService.getTerritoryByAreaId(this.salesTargetListFilerForm.get('areaId')?.value)).subscribe(
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

  territoryChange() {
    let territory = this.territoryList?.find(territory => territory.id === this.salesTargetListFilerForm.get('territoryId')?.value);
    if (territory?.dealership?.length > 0) {
      let filterDistributor = territory?.dealership?.find((y: { isActive: boolean; }) => y.isActive == true)
      this.salesTargetListFilerForm.get('distributor')?.patchValue(filterDistributor.name + ' (' + filterDistributor.address + ')');
    }
  }


}
