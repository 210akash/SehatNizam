import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { RouteService } from '../route.service';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { DrawMapComponent } from '../../gmap/draw-map/draw-map.component';
import { DrawRouteShopsComponent } from '../../gmap/draw-route-shops/draw-route-shops.component';
import { ShopService } from '../../shop/shop.service';

@Component({
  selector: 'app-add-shops-route',
  templateUrl: './add-shops-route.component.html',
  styleUrls: ['./add-shops-route.component.css'],standalone: false
})

export class AddShopsRouteComponent implements OnInit {
  isLoading: any;
  addShopsRouteForm!: FormGroup;
  dataSource: any;
  // displayedColumns: string[] = ['srNo', 'name', 'address'];
  displayedColumns: string[] = ['sequenceNo', 'name', 'address', 'actions'];

  selection = new Set<any>();
  isAllSelected: boolean = false;
  selectMultipleButtonsShow: boolean = false;

  finalRows: any = [];
  selectedRowsCount: any;

  selectedShops: Set<number> = new Set();

  gRouteShopObj: any;
  dialogRef: any;

  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private routeService: RouteService, private formBuilder: FormBuilder, private constantService: ConstantService, private shopService: ShopService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.addShopsRouteForm = this.formBuilder.group({
      id: [0],
      name: [''],
      visitDay: [''],
      zone: [''],
      territory: [''],
      totalShops: [0]
    });

    this.LoadData(this.data.element);
    this.addShopsRouteForm.get('zone')?.patchValue(this.data.element.territory.zone.name);
    this.addShopsRouteForm.get('territory')?.patchValue(this.data.element.territory.name);
    this.addShopsRouteForm.get('totalShops')?.patchValue(this.data.element.routeShop?.filter((x: { isActive: boolean; }) => x.isActive === true)?.length);

    // this.getShopsByTerritoryId(this.data.element.territory.id);
  }

  LoadData(element: any) {
    this.constantService.LoadData(element, this.addShopsRouteForm);
    this.dataSource = element.routeShop.filter((x: { isActive: boolean; }) => x.isActive == true);
  }

  // async getShopsByTerritoryId(territoryId: any) {
  //   this.dataSource = [];
  //   (await this.shopService.getShopsByTerritoryId(territoryId)).subscribe(
  //     {
  //       next: (data) => {
  //         this.dataSource = data;
  //         this.updateCheckedStatus();
  //       },
  //       error: (error) => {
  //         console.log(error);
  //         this.isLoading = false;
  //       }
  //     });
  // }

  // updateCheckedStatus() {
  //   const shopIdsToCheck = this.data.element.routeShop.filter(shop => shop.isActive).map(shop => shop.shopId);
  //   this.dataSource.forEach(shop => {
  //     if (shopIdsToCheck.includes(shop.id)) {
  //       this.selectedShops.add(shop.id);
  //     }
  //   });

  //   const shopIdsToCheckSet = new Set(shopIdsToCheck);

  //   this.dataSource.forEach(item => {
  //     if (item.id && shopIdsToCheckSet.has(item.id)) {
  //       this.selection.add(item);       // Add item to selection if its ID is in the Set
  //     }
  //   });
  // }

  async saveAddShops() {
    let shopsToAdd = {
      'route': this.data.element,
      'shopsToAdd': this.finalRows
    };

    (await this.routeService.addShopsRoute(shopsToAdd)).subscribe(
      {
        next: (data: { Status: number; }) => {
          if (data.Status == 200) {
            this.notificationsService.showNotification('Routed Saved Successfully', 'snack-bar-success');
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

  async selectRouteShops() {
    const markerPinsList: any[] = [];
    const coordinatesList: any[] = [];
    const selectedRouteShopList: any[] = [];

    coordinatesList.push({
      typeId: 1,
      coordinates: this.data.element.territory.zone.coordinates,
    });

    coordinatesList.push({
      typeId: 2,
      coordinates: this.data.element.territory.coordinates,
    });
    var a = this.data.element;

    this.data.element.territory.shop.forEach((row: { isActive: boolean; }) => {
      if (row.isActive == true) {
        markerPinsList.push(row);
      }
    });

    this.data.element.routeShop.forEach((row: { isActive: boolean; shop: any; }) => {
      if (row.isActive == true) {
        selectedRouteShopList.push(row.shop);
      }
    });

    // markerPinsList.push({
    //   typeId: 2,
    //   pinLocation: element.shop.pinLocation,
    // });

    const lObjelement = {
      caption: 'Zone: ' + this.data.element.territory.zone.name + ' - Territory: ' + this.data.element.territory.name + ' - Route: ' + this.data.element.name,
      fromComponent: 'viewZone',
      drawingPolygon: false,
      drawingMarker: false,
      coordinates: coordinatesList,
      markerPins: markerPinsList,
      routeId: this.data.element.id,
      selectedMarkers: selectedRouteShopList,
      isViewOnly: false
    };

    const dialogRef = this.dialog.open(DrawRouteShopsComponent, {
      width: '70%',
      height: 'auto',
      data: {
        element: lObjelement,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.finalRows = result;
      result === false ? 0 : this.addShopsRouteForm.get('totalShops')?.patchValue(this.finalRows.length);
    });
  }

  openDeleteRouteShopPopup(element: any, template: any) {
    this.gRouteShopObj = element;
    this.dialogRef = this.dialog.open(template, {
      width: '30%',
      height: 'auto%',
      disableClose: true,
    });
  }

  async deleteRouteShop() {
    (await this.routeService.deleteRouteShop(this.gRouteShopObj.id)).subscribe({
      next: (data: { Status: number; Message: string; }) => {
        if (data.Status == 200) {
          this.isLoading = false;
          this.notificationsService.showNotification(data.Message, 'snack-bar-success');
          this.dialog.closeAll();
        }
        else {
          this.isLoading = false;
          this.notificationsService.showNotification(data.Message, 'snack-bar-danger');
        }
      },
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  viewPinLocation(shopElement: any): void {
    const markerPinsList: any[] = [];
    const coordinatesList: any[] = [];

    coordinatesList.push({
      typeId: 1,
      coordinates: this.data.element.territory.zone.coordinates,
      name: 'Zone-' + this.data.element.territory.zone.name
    });

    coordinatesList.push({
      typeId: 2,
      coordinates: this.data.element.territory.coordinates,
      name: 'Territory-' + this.data.element.territory.name
    });

    markerPinsList.push({
      typeId: 2,
      pinLocation: shopElement.shop.pinLocation,
      name: 'Shop-' + shopElement.shop.name,
      address: shopElement.shop.address,
      phoneNo: shopElement.shop.phoneNo
    });

    const element = {
      caption: 'Zone: ' + this.data.element.territory.zone.name + ' - Territory: ' + this.data.element.territory.name + ' - Shop: ' + this.data.element.name,
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
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {

    });
  }

  // toggleCheckbox(row: any) {
  //   this.selectedShops = null;
  //   if (this.selection.has(row)) {
  //     this.selection.delete(row);
  //   } else {
  //     this.selection.add(row);
  //   }
  //   this.isAllSelected = this.isAllSelectedCheckbox();
  //   this.checkLength();
  // }

  // selectAll(event: any) {
  //   this.selectedShops = null;
  //   this.isAllSelected = event.checked;
  //   this.dataSource.forEach(row => {
  //     if (this.isAllSelected) {
  //       this.selection.add(row);
  //       this.selectMultipleButtonsShow = true;
  //     } else {
  //       this.selection.delete(row);
  //       this.selectMultipleButtonsShow = false;
  //     }
  //   });
  //   this.checkLength();
  // }

  // isAllSelectedCheckbox(): boolean {
  //   const numSelected = this.selection.size;
  //   if (numSelected > 0) {
  //     this.selectMultipleButtonsShow = true;
  //   }
  //   else {
  //     this.selectMultipleButtonsShow = false;
  //   }
  //   const numRows = this.dataSource.length;
  //   return numSelected === numRows;
  // }

  // checkLength() {
  //   // Get the selected rows
  //   const selectedRows = Array.from(this.selection);
  //   this.finalRows = selectedRows;
  //   // Log the selected rows to the console
  //   console.log("Selected Rows:", selectedRows);
  //   this.selectedRowsCount = selectedRows.length;
  // }


}
