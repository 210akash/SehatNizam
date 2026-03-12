import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { RouteService } from '../route.service';
import { DeleteRouteComponent } from '../delete-route/delete-route.component';
import { ViewRouteComponent } from '../view-route/view-route.component';
import { CreateRouteComponent } from '../create-route/create-route.component';
import { AddShopsRouteFrequencyComponent } from '../add-shops-route-frequency/add-shops-route-frequency.component';
import { ConstantService } from '../../../../Service/constant.service';
import { AreaService } from '../../area/area.service';
import { RegionService } from '../../region/region.service';
import { TerritoryService } from '../../territory/territory.service';
import { ZoneService } from '../../zone/zone.service';

@Component({
  selector: 'app-route-list',
  templateUrl: './route-list.component.html',
  styleUrls: ['./route-list.component.css'],standalone: false
})

export class RouteListComponent implements OnInit {
  dataSource: any;
  routeListFilerForm!: FormGroup;
  isEditMode: boolean = false;
  displayedColumns: string[] = ['territory', 'name',  'createdDate', 'actions'];
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

  constructor(private constantService: ConstantService, private dialog: MatDialog, private routeService: RouteService, private formBuilder: FormBuilder, private territoryService: TerritoryService,
    private zoneService: ZoneService, private areaService: AreaService, private regionService: RegionService) { }
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngOnInit(): void {
    this.pageSize = this.constantService.defaultItemPerPage;

    this.routeListFilerForm = this.formBuilder.group({
      regionId: [0],
      zoneId: [0],
      areaId: [0],
      territoryId: [0],
    });

    this.bindData();
    this.getRegions();
  }

  openRouteDialog(element: any): void {
    const dialogRef = this.dialog.open(CreateRouteComponent, {
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

  openViewRouteDialog(enterAnimationDuration: string, exitAnimationDuration: string, element: any): void {
    this.dialog.open(ViewRouteComponent, {
      data: { element: element },
      width: '50%',
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

    let _routeListFilerForm: any = {};
    _routeListFilerForm = Object.assign(_routeListFilerForm, this.routeListFilerForm.value);
    _routeListFilerForm["PagingData"] = pagingData;

    (await this.routeService.getAllRoute(_routeListFilerForm)).subscribe({
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
    const dialogRef = this.dialog.open(DeleteRouteComponent, {
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

  openAddShopsDialog(element: any) {
    const dialogRef = this.dialog.open(AddShopsRouteFrequencyComponent, {
      data: { element: element },
      width: '70%',
      autoFocus: true,
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
  }

  countTotalShops(element: any) {
    var result = element?.shopRouteFrequency?.filter((x: { isActive: boolean; }) => x.isActive == true)?.length; // We are assigning only one route for now
    return result;
  }

  onReset() {
    this.routeListFilerForm.patchValue({
      regionId: 0,
      zoneId: 0,
      areaId: 0,
      territoryId: 0,
    });
    this.bindData();
  }

  filterData() {
    this.bindData();
  }

  // onZoneChange() {
  //   this.routeListFilerForm.get('territoryId')?.patchValue(0);
  //   let zoneId = this.routeListFilerForm.get('zoneId')?.value;

  //   this.getTerritoryByAreaId(zoneId);
  //   this.filterData();
  // }

  // async getTerritoryByAreaId(zoneId: any) {
  //   (await this.territoryService.getTerritoryByAreaId(zoneId)).subscribe({
  //     next: (data) => {
  //       if (data && Array.isArray(data)) {
  //         this.territoryList = data;
  //       } else {
  //         console.error('Expected array but got:', data);
  //         this.territoryList = [];
  //       }
  //     },
  //     error: (error) => {
  //       console.log(error);
  //       this.isLoading = false;
  //     }
  //   });
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

  async getTerritoryByAreaId() {

    this.territoryList = [];

    this.routeListFilerForm.get('territoryId')?.patchValue(0);
    (await this.territoryService.getTerritoryByAreaId(this.routeListFilerForm.get('areaId')?.value)).subscribe(
      {
        next: (data: any[]) => {
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

    (await this.zoneService.getZoneByRegionId(this.routeListFilerForm.get('regionId')?.value)).subscribe({
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

    (await this.areaService.getAreaByZoneId(this.routeListFilerForm.get('zoneId')?.value)).subscribe({
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
