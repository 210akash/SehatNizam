import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
// import { ConstantService } from 'app/Service/constant.service';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { UserTerritoryService } from '../user-territory.service';
import { CreateUserTerritoryComponent } from '../create-user-territory/create-user-territory.component';
import { ViewUserTerritoryComponent } from '../view-user-territory/view-user-territory.component';
import { DeleteUserTerritoryComponent } from '../delete-user-territory/delete-user-territory.component';
import { ConstantService } from '../../../../Service/constant.service';
import { AreaService } from '../../area/area.service';
import { ZoneService } from '../../zone/zone.service';
import { TerritoryService } from '../../territory/territory.service';
import { RegionService } from '../../region/region.service';
// import { RegionService } from 'app/components/region/region.service';
// import { ZoneService } from 'app/components/zone/zone.service';
// import { AreaService } from 'app/components/area/area.service';
// import { TerritoryService } from 'app/components/territory/territory.service';

@Component({
  selector: 'app-user-territory-list',
  templateUrl: './user-territory-list.component.html',
  styleUrls: ['./user-territory-list.component.css'],standalone: false
})

export class UserTerritoryListComponent implements OnInit {
  dataSource: any;
  userTerritoryListFilerForm!: FormGroup;
  isEditMode: boolean = false;
  displayedColumns: string[] = ['region', 'zone', 'area', 'territory', 'user', 'createdDate', 'actions'];
  isLoading = false;
  element: any;

  currentPage = 0;
  pageSize = 0;
  totalRows = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];

  regionList: any[] = [];
  zoneList: any[] = [];
  areaList: any[] = [];
  territoryList: any[] = [];

  constructor(private constantService: ConstantService, private dialog: MatDialog, private userTerritoryService: UserTerritoryService, private formBuilder: FormBuilder, private regionService: RegionService,
    private zoneService: ZoneService, private areaService: AreaService, private territoryService: TerritoryService) { }

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngOnInit(): void {
    this.pageSize = this.constantService.defaultItemPerPage;

    this.userTerritoryListFilerForm = this.formBuilder.group({
      regionId: [0],
      zoneId: [0],
      areaId: [0],
      territoryId: [0],
    });

    this.bindData();
    this.getRegions();
  }

  openUserTerritoryDialog(element: any): void {
    const dialogRef = this.dialog.open(CreateUserTerritoryComponent, {
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

  openViewUserTerritoryDialog(enterAnimationDuration: string, exitAnimationDuration: string, element: any): void {
    this.dialog.open(ViewUserTerritoryComponent, {
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

    let _userTerritoryListFilerForm: any = {};
    _userTerritoryListFilerForm = Object.assign(_userTerritoryListFilerForm, this.userTerritoryListFilerForm.value);
    _userTerritoryListFilerForm["PagingData"] = pagingData;

    (await this.userTerritoryService.getAllUserTerritory(_userTerritoryListFilerForm)).subscribe({
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
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData();
  }

  openDeleteDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteUserTerritoryComponent, {
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

  async getTerritoryByAreaId() {

    this.territoryList = [];

    this.userTerritoryListFilerForm.get('territoryId')?.patchValue(0);
    (await this.territoryService.getTerritoryByAreaId(this.userTerritoryListFilerForm.get('areaId')?.value)).subscribe(
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

  async getZoneByRegionId() {

    this.zoneList = [];
    this.areaList = [];
    this.territoryList = [];

    (await this.zoneService.getZoneByRegionId(this.userTerritoryListFilerForm.get('regionId')?.value)).subscribe({
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

    (await this.areaService.getAreaByZoneId(this.userTerritoryListFilerForm.get('zoneId')?.value)).subscribe({
      next: (data) => {
        this.areaList = data;
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  filterData() {
    this.bindData();
  }

  onReset() {
    this.userTerritoryListFilerForm.patchValue({
      regionId: 0,
      zoneId: 0,
      areaId: 0,
      territoryId: 0,
    });
    this.bindData();
  }


}
