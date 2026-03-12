import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { VehicleService } from '../vehicle.service';
import { CreateVehicleComponent } from '../create-vehicle/create-vehicle.component';
import { DeleteVehicleComponent } from '../delete-vehicle/delete-vehicle.component';
import { ViewVehicleComponent } from '../view-vehicle/view-vehicle.component';
import { ConstantService } from '../../../../Service/constant.service';
import { AreaService } from '../../area/area.service';
import { TerritoryService } from '../../territory/territory.service';
import { ZoneService } from '../../zone/zone.service';
import { RegionService } from '../../region/region.service';
import { DealershipService } from '../../dealership/dealership.service';

@Component({
  selector: 'app-vehicle-list',
  templateUrl: './vehicle-list.component.html',
  styleUrls: ['./vehicle-list.component.css'],standalone: false
})

export class VehicleListComponent implements OnInit {
  dataSource: any;
  vehicleListFilerForm!: FormGroup;
  isEditMode: boolean = false;
  displayedColumns: string[] = ['vehicleName', 'driverName', 'registrationNumber','loadCapacity', 'isHeadOfficeVehicle','logisticPartner', 'distributor', 'createdDate', 'actions'];
  isLoading = false;

  currentPage = 0;
  pageSize = 0;
  totalRows = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];

  regionList: any[] = [];
  zoneList: any[] = [];
  areaList: any[] = [];
  territoryList: any[] = [];
  dealershipList: any[] = [];

  constructor(private constantService: ConstantService, private dialog: MatDialog, private vehicleService: VehicleService, private formBuilder: FormBuilder, private regionService: RegionService,
    private zoneService: ZoneService, private areaService: AreaService, private territoryService: TerritoryService, private dealershipService: DealershipService) { }

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngOnInit(): void {
    this.pageSize = this.constantService.defaultItemPerPage;

    this.vehicleListFilerForm = this.formBuilder.group({
      regionId: [0],
      zoneId: [0],
      areaId: [0],
      territoryId: [0],
      dealershipId: [0],
    });

    this.getRegions();
    this.bindData();
  }

  openVehicleDialog(element: any): void {
    const dialogRef = this.dialog.open(CreateVehicleComponent, {
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

  openViewVehicleDialog(enterAnimationDuration: string, exitAnimationDuration: string, element: any): void {
    this.dialog.open(ViewVehicleComponent, {
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

    let _vehicleListFilerForm: any = {};
    _vehicleListFilerForm = Object.assign(_vehicleListFilerForm, this.vehicleListFilerForm.value);
    _vehicleListFilerForm["PagingData"] = pagingData;

    (await this.vehicleService.getAllVehicle(_vehicleListFilerForm)).subscribe({
      next: (data : any) => {
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
    const dialogRef = this.dialog.open(DeleteVehicleComponent, {
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

  async getZoneByRegionId() {

    this.zoneList = [];
    this.areaList = [];
    this.territoryList = [];

    (await this.zoneService.getZoneByRegionId(this.vehicleListFilerForm.get('regionId')?.value)).subscribe({
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

    (await this.areaService.getAreaByZoneId(this.vehicleListFilerForm.get('zoneId')?.value)).subscribe({
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

    this.vehicleListFilerForm.get('territoryId')?.patchValue(0);
    (await this.territoryService.getTerritoryByAreaId(this.vehicleListFilerForm.get('areaId')?.value)).subscribe(
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

  async getDealershipsByTerritoryId() {
    let territoryId = this.vehicleListFilerForm.get('territoryId')?.value;
    (await this.dealershipService.getDealershipByTerritoryId(territoryId)).subscribe({
      next: (data) => {
        if (data && Array.isArray(data)) {
          this.dealershipList = data;
        } else {
          console.error('Expected array but got:', data);
          this.dealershipList = [];
        }

        this.filterData();
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
    this.vehicleListFilerForm.patchValue({
      regionId: 0,
      zoneId: 0,
      areaId: 0,
      territoryId: 0,
      dealershipId: 0,
    });
    this.bindData();
  }


}
