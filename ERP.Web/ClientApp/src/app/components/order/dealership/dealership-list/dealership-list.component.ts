import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { DealershipService } from '../dealership.service';
import { DeleteDealershipComponent } from '../delete-dealership/delete-dealership.component';
import { ViewDealershipComponent } from '../view-dealership/view-dealership.component';
import { CreateDealershipComponent } from '../create-dealership/create-dealership.component';
import { ConstantService } from '../../../../Service/constant.service';
import { AreaService } from '../../area/area.service';
import { DrawMapComponent } from '../../gmap/draw-map/draw-map.component';
import { RegionService } from '../../region/region.service';
import { TerritoryService } from '../../territory/territory.service';
import { ZoneService } from '../../zone/zone.service';
import { AddDealershipUserComponent } from '../add-user/add-dealershipuser.component';
import { environment } from '../../../../../environments/environment';
import { AuthenticationService } from '../../../../Auth/authentication.service';

@Component({
  selector: 'app-dealership-list',
  templateUrl: './dealership-list.component.html',
  styleUrls: ['./dealership-list.component.css'],standalone: false
})

export class DealershipListComponent implements OnInit {
  dataSource: any;
  dealershipListFilerForm!: FormGroup;
  isEditMode: boolean = false;
  displayedColumns: string[] = ['code', 'type', 'region', 'zone', 'area','territory', 'name', 'description', 'createdDate','isActive', 'actions'];
  isLoading = false;
  element: any;
  blob: any;
  reportsUrl: any;
  isActive: boolean = false;
  currentPage = 0;
  pageSize = 0;
  totalRows = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  currentUser: any;
  regionList: any[] = [];
  zoneList: any[] = [];
  areaList: any[] = [];
  territoryList: any[] = [];
  distributorTypeList: any;

  constructor(private constantService: ConstantService, private dialog: MatDialog, private dealershipService: DealershipService, private formBuilder: FormBuilder, private zoneService: ZoneService,
    private territoryService: TerritoryService, 
    private authenticationService: AuthenticationService,
    private areaService: AreaService, private regionService: RegionService
  ) {    this.reportsUrl = environment.reports_uri;}
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngOnInit(): void {
       this.currentUser = this.authenticationService.currentUserValue;
    this.getAllDistributorTypes();
    this.pageSize = this.constantService.defaultItemPerPage;

    this.dealershipListFilerForm = this.formBuilder.group({
      regionId: [0],
      zoneId: [0],
      areaId: [0],
      territoryId: [0],
      dealershipTypeId: [1],
      name: [''],
      isActive: [null],
    });

    this.bindData();
    this.getRegions();
  }

  filterData1(event: any): void {
    // The `event.checked` will be `true` if the toggle is on, otherwise `false`
    this.isActive = event.checked;

    // Call any function or perform the action you need, passing the `isActive` value
    this.bindData();
  }
  openDealershipDialog(element: any): void {
    const dialogRef = this.dialog.open(CreateDealershipComponent, {
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

  openViewDealershipDialog(enterAnimationDuration: string, exitAnimationDuration: string, element: any): void {
    this.dialog.open(ViewDealershipComponent, {
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

    let _dealershipListFilerForm: any = {};
    _dealershipListFilerForm = Object.assign(_dealershipListFilerForm, this.dealershipListFilerForm.value);

    _dealershipListFilerForm["PagingData"] = pagingData;
    // _dealershipListFilerForm['dealershipTypeId'] = 1;

    (await this.dealershipService.getAllDealershipList(_dealershipListFilerForm)).subscribe({
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

  async getAllDistributorTypes()
  {
    (await this.dealershipService.getAllDistributorType()).subscribe({
      next: (data: { item1: any; }) => {
        this.distributorTypeList = data;
      },
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  openDeleteDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteDealershipComponent, {
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

    openRegisterUserDialog(element: any) {
    const dialogRef = this.dialog.open(AddDealershipUserComponent, {
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
    this.dealershipListFilerForm.patchValue({
      regionId: 0,
      zoneId: 0,
      areaId: 0,
      territoryId: 0,
      isActive: null,
    });
    this.bindData();
  }

  filterData() {
    this.bindData();
  }

  // onZoneChange() {
  //   this.dealershipListFilerForm.get('territoryId')?.patchValue(0);
  //   let zoneId = this.dealershipListFilerForm.get('zoneId')?.value;

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

  viewPinLocation(element: any): void {

    const markerPinsList: any[] = [];
    const coordinatesList: any[] = [];

    coordinatesList.push({
      typeId: 1,
      coordinates: element.territory?.area?.zone?.coordinates,
      name: 'Zone-' + element.territory?.area?.zone?.name
    });

    coordinatesList.push({
      typeId: 2,
      coordinates: element.territory?.coordinates,
      name: 'Territory-' + element.territory?.name,
    });

    markerPinsList.push({
      typeId: 1,
      pinLocation: element.pinLocation,
      name: 'Dealer-' + element.name
    });

    const elementToSend = {
      caption: 'Zone: ' + element.territory?.area?.zone?.name + ' - Territory: ' + element.territory?.name + ' - Distributor: ' + element.name,
      fromComponent: 'viewZone',
      drawingPolygon: false,
      drawingMarker: false,
      coordinates: coordinatesList,
      markerPins: markerPinsList,
      isShowInfoBox: true,
      isFocusDrawMarker: true
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

  async getTerritoryByAreaId() {

    this.territoryList = [];

    this.dealershipListFilerForm.get('territoryId')?.patchValue(0);
    (await this.territoryService.getTerritoryByAreaId(this.dealershipListFilerForm.get('areaId')?.value)).subscribe(
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

    (await this.zoneService.getZoneByRegionId(this.dealershipListFilerForm.get('regionId')?.value)).subscribe({
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

  async getAreaByZoneId() {

    this.areaList = [];
    this.territoryList = [];

    (await this.areaService.getAreaByZoneId(this.dealershipListFilerForm.get('zoneId')?.value)).subscribe({
      next: (data: any[]) => {
        this.areaList = data;
      },
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  redirectToshopdetail(element : any) {
    const url = '' + this.reportsUrl + 'ReportServer/Pages/ReportViewer.aspx?%2FERPReports%2FItemLedgerDistributor&rs%3AClearSession=true&rc%3AView=af7578f5-cebd-4b18-ac9f-43c11e11f867&CompanyId=' + this.currentUser.department.companyId+ '&DistributorId='+element.id;
    window.open(url, '_blank');  // Opens the URL in a new tab
  }


}
