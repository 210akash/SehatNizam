import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { RouteService } from '../route.service';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { DrawMapComponent } from '../../gmap/draw-map/draw-map.component';

@Component({
  selector: 'app-view-route',
  templateUrl: './view-route.component.html',
  styleUrls: ['./view-route.component.css'],standalone: false
})

export class ViewRouteComponent implements OnInit {
  viewRouteForm!: FormGroup;
  isLoading = false;
  dataSource!: any;
  displayedColumns: string[] = ['sequenceNo', 'name', 'address', 'actions'];
  roleList: any;
  dialogRef: any;

  gRouteShopObj: any;

  savedData: any;
  daysOfWeek: string[] = ['monday', 'tuesday', 'wednesday', 'thursday', 'friday', 'saturday', 'sunday'];
  storeSchedules: { [key: number]: { [key: string]: boolean } } = {};

  gRoute: any;

  constructor(private notificationsService: NotificationsService, private routeService: RouteService, private dialog: MatDialog, private formBuilder: FormBuilder, private constantService: ConstantService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  async ngOnInit(): Promise<void> {
    this.viewRouteForm = this.formBuilder.group({
      id: [0],
      name: [''],
      visitDay: [''],
      zone: [''],
      territory: [''],
      totalShops: [0]
    });

    this.getRouteById(this.data.element?.id);
  }

  async LoadData(element: any) {
    this.constantService.LoadData(element, this.viewRouteForm);
    this.dataSource = element?.routeShop?.filter((x: { isActive: boolean; }) => x.isActive == true);

    this.initializeSchedules(element);

    this.viewRouteForm.get('zone')?.patchValue(element.territory?.area?.zone.name);
    this.viewRouteForm.get('territory')?.patchValue(element.territory?.name);
    this.viewRouteForm.get('totalShops')?.patchValue(element.shopRouteFrequency?.filter((x: { isActive: boolean; }) => x.isActive === true)?.length);
  }

  viewPinLocation(element: any): void {

    const markerPinsList: any[] = [];
    const coordinatesList: any[] = [];

    coordinatesList.push({
      typeId: 1,
      coordinates: element.territory.zone.coordinates,
      name: 'Zone-' + element.territory.zone.name
    });

    coordinatesList.push({
      typeId: 2,
      coordinates: element.territory.coordinates,
      name: 'Territory-' + element.territory.name
    });

    markerPinsList.push({
      typeId: 2,
      pinLocation: element.shop.pinLocation,
      name: 'Shop-' + element.shop.name,
      address: element.shop.address,
      phoneNo: element.shop.phoneNo
    });

    const lObjelement = {
      caption: 'Zone: ' + element.territory.zone.name + ' - Territory: ' + element.territory.name + ' - Shop: ' + element.shop.name,
      fromComponent: 'viewZone',
      drawingPolygon: false,
      drawingMarker: false,
      coordinates: coordinatesList,
      markerPins: markerPinsList,
      isShowInfoBox: true
    };

    const dialogRef = this.dialog.open(DrawMapComponent, {
      width: '70%',
      height: 'auto',
      data: {
        element: lObjelement,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {

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

  // async selectRouteShops() {
  //   const markerPinsList: any[] = [];
  //   const coordinatesList: any[] = [];
  //   const selectedRouteShopList: any[] = [];

  //   coordinatesList.push({
  //     typeId: 1,
  //     coordinates: this.data.element.territory.zone.coordinates,
  //   });

  //   coordinatesList.push({
  //     typeId: 2,
  //     coordinates: this.data.element.territory.coordinates,
  //   });
  //   var a = this.data.element;

  //   this.data.element.territory.shop.forEach(row => {
  //     if (row.isActive == true) {
  //       markerPinsList.push(row);
  //     }
  //   });

  //   this.data.element.routeShop.forEach(row => {
  //     if (row.isActive == true) {
  //       selectedRouteShopList.push(row.shop);
  //     }
  //   });

  //   // markerPinsList.push({
  //   //   typeId: 2,
  //   //   pinLocation: element.shop.pinLocation,
  //   // });

  //   const lObjelement = {
  //     caption: 'Zone: ' + this.data.element.territory.zone.name + ' - Territory: ' + this.data.element.territory.name + ' - Route: ' + this.data.element.name,
  //     fromComponent: 'viewZone',
  //     drawingPolygon: false,
  //     drawingMarker: false,
  //     coordinates: coordinatesList,
  //     markerPins: markerPinsList,
  //     routeId: this.data.element.id,
  //     selectedMarkers: selectedRouteShopList,
  //     isViewOnly: true
  //   };

  //   const dialogRef = this.dialog.open(DrawRouteShopsComponent, {
  //     width: '70%',
  //     height: 'auto',
  //     data: {
  //       element: lObjelement,
  //     },
  //     disableClose: true
  //   });

  //   dialogRef.afterClosed().subscribe(result => {
  //     const a = result;
  //   });
  // }

  async initializeSchedules(element: any) {
    element.territory?.shop.forEach((store: any) => {
      this.storeSchedules[store.id] = this.daysOfWeek.reduce((acc, day) => {
        acc[day] = false; // Default to false (unchecked)
        return acc;
      }, {} as { [key: string]: boolean });
    });

    await this.getShopRouteFrequencyByTerritoryId();
  }

  async getShopRouteFrequencyByTerritoryId() {
    (await this.routeService.getShopRouteFrequencyByTerritoryId(this.gRoute?.territoryId)).subscribe(
      {
        next: (data: any) => {
          this.savedData = data;
          this.loadSavedDataIntoStoreSchedules();
          // this.LoadData(this.data.element);
        },
        error: (error: any) => {
          console.log(error);
          this.isLoading = false;
        }
      });
  }

  loadSavedDataIntoStoreSchedules() {
    // Loop through the saved data and populate storeSchedules
    this.savedData.forEach((item: { [x: string]: boolean; shopId: any; }) => {
      const shopId = item.shopId;

      // For each shop, set the values for each day of the week
      if (this.storeSchedules[shopId]) {
        // Dynamically assign the saved values from the API to the storeSchedules object
        this.daysOfWeek.forEach(day => {
          this.storeSchedules[shopId][day] = item[day.toLowerCase()] || false; // Use lowercase keys like 'monday'
        });
      }
    });

    // If you want to log the updated storeSchedules for debugging
    console.log('Updated Store Schedules:', this.storeSchedules);
  }

  async getRouteById(routeId: any) {
    (await this.routeService.getRouteById(routeId)).subscribe({
      next: (data: any) => {
        this.gRoute = data;
        this.LoadData(data);
      },
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }


}
