import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { ShopService } from '../shop.service';
import { DeleteShopComponent } from '../delete-shop/delete-shop.component';
import { ViewShopComponent } from '../view-shop/view-shop.component';
import { CreateShopComponent } from '../create-shop/create-shop.component';
import { ConstantService } from '../../../../Service/constant.service';
import { AreaService } from '../../area/area.service';
import { DrawMapComponent } from '../../gmap/draw-map/draw-map.component';
import { RegionService } from '../../region/region.service';
import { TerritoryService } from '../../territory/territory.service';
import { ZoneService } from '../../zone/zone.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { ApproveShopComponent } from '../approve-shop/approve-shop.component';

@Component({
  selector: 'app-shop-list',
  templateUrl: './shop-list.component.html',
  styleUrls: ['./shop-list.component.css'], standalone: false
})

export class ShopListComponent implements OnInit {
  dataSource: any;
  shopListFilerForm!: FormGroup;
  isEditMode: boolean = false;
  displayedColumns: string[] = ['id','region', 'territory', 'name', 'distributor', 'ownerName', 'phoneNo', 'description', 'createdDate','createdBy', 'isverified','status', 'actions'];
  isLoading = false;
  element: any;
  blob: any;

  currentPage = 0;
  pageSize = 0;
  totalRows = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];

  allShops: any;

  regionList: any[] = [];
  zoneList: any[] = [];
  areaList: any[] = [];
  territoryList: any[] = [];

  constructor(private notificationsService: NotificationsService, private constantService: ConstantService, private dialog: MatDialog, private shopService: ShopService, private formBuilder: FormBuilder, private territoryService: TerritoryService,
    private zoneService: ZoneService, private regionService: RegionService, private areaService: AreaService) { }
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngOnInit(): void {
    this.pageSize = this.constantService.defaultItemPerPage;

    this.shopListFilerForm = this.formBuilder.group({
      fdate: [new Date()],
      tdate: [new Date()],
      regionId: [0],
      zoneId: [0],
      areaId: [0],
      territoryId: [0],
      statusId: [2],
      createdBy: [''],
      name: ['']
    });
    const startDate = new Date(new Date().getFullYear(), new Date().getMonth(), 1);
    const endDate = new Date();
    this.shopListFilerForm.get('fdate')?.patchValue(this.constantService.formatDate(startDate));
    this.shopListFilerForm.get('tdate')?.patchValue(this.constantService.formatDate(endDate));
    this.bindData();
    this.getRegions();
  }

  async verifiedShop(element: any) {
    (await this.shopService.verifyShopById(element.id)).subscribe({
      next: (data) => {
        if (data.Status == 200) {
          this.isLoading = false;
          this.notificationsService.showNotification(data.Message, 'snack-bar-success');
          this.bindData();
        }
        else {
          this.isLoading = false;
          this.notificationsService.showNotification(data.Message, 'snack-bar-danger');
        }
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  openShopDialog(element: any): void {
    const dialogRef = this.dialog.open(CreateShopComponent, {
      data: { element: element, shops: this.allShops },
      width: '50%',
      maxHeight: '90vh',
      autoFocus: true,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
  }

  openViewShopDialog(enterAnimationDuration: string, exitAnimationDuration: string, element: any): void {

    this.dialog.open(ViewShopComponent, {
      data: { element: element },
      width: '40%',
      maxHeight: '90vh',
      autoFocus: true,
      disableClose: true
    }),
    {
      enterAnimationDuration,
      exitAnimationDuration,
    };
  }

  approveShopDialog(element: any): void {
    const dialogRef = this.dialog.open(ApproveShopComponent, {
      data: { element: element},
      width: '50%',
      maxHeight: '90vh',
      autoFocus: true,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
  }

  async bindData() {
    this.isLoading = true;
    const pagingData = {
      currentPage: this.currentPage,
      take: this.pageSize
    }

    let _shopListFilerForm: any = {};
    _shopListFilerForm = Object.assign(_shopListFilerForm, this.shopListFilerForm.value);
    _shopListFilerForm["PagingData"] = pagingData;

     let fdate = new Date(_shopListFilerForm.fdate);
    let tdate = new Date(_shopListFilerForm.tdate);

      _shopListFilerForm['fdate'] = fdate.toLocaleDateString();
      _shopListFilerForm['tdate'] = tdate.toLocaleDateString();

    (await this.shopService.getAllShop(_shopListFilerForm)).subscribe({
      next: (data) => {
        this.allShops = data.item1;
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
    const dialogRef = this.dialog.open(DeleteShopComponent, {
      data: { element: element },
      width: '30%',
      maxHeight: '90vh',
      autoFocus: true,
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
  }

  onReset() {
    this.shopListFilerForm.patchValue({
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
  //   this.shopListFilerForm.get('territoryId')?.patchValue(0);
  //   let zoneId = this.shopListFilerForm.get('zoneId')?.value;

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
      name: 'Territory-' + element.territory?.name
    });

    markerPinsList.push({
      typeId: 2,
      pinLocation: element.pinLocation,
      name: 'Shop-' + element.name,
      address: element.address,
      phoneNo: element.phoneNo
    });

    const elementToSend = {
      caption: 'Zone: ' + element.territory?.area?.zone?.name + ' - Territory: ' + element.territory?.name + ' - Shop: ' + element.name,
      fromComponent: 'viewZone',
      drawingPolygon: false,
      drawingMarker: false,
      coordinates: coordinatesList,
      markerPins: markerPinsList,
      isFocusDrawPolygon: true,
      isShowInfoBox: true
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

    this.shopListFilerForm.get('territoryId')?.patchValue(0);
    (await this.territoryService.getTerritoryByAreaId(this.shopListFilerForm.get('areaId')?.value)).subscribe(
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

    (await this.zoneService.getZoneByRegionId(this.shopListFilerForm.get('regionId')?.value)).subscribe({
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

    (await this.areaService.getAreaByZoneId(this.shopListFilerForm.get('zoneId')?.value)).subscribe({
      next: (data) => {
        this.areaList = data;
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  getFirstActiveDealership(element: any): string {
    const active = element.territory?.dealership?.find((d: any) => d.isActive);
    return active?.name ?? '';
  }


}