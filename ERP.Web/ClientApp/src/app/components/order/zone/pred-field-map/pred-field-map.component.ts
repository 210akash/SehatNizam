import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ZoneService } from '../zone.service';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { AreaService } from '../../area/area.service';
import { RegionService } from '../../region/region.service';
import { TerritoryService } from '../../territory/territory.service';
import { DealershipService } from '../../dealership/dealership.service';
import { ShopService } from '../../shop/shop.service';

@Component({
  selector: 'app-pred-field-map',
  templateUrl: './pred-field-map.component.html',
  styleUrls: ['./pred-field-map.component.css'],standalone: false
})

export class PredFieldMapComponent implements OnInit {
  fieldMapForm!: FormGroup;
  isLoading = false;

  dropRegionList: any[] = [];
  dropZoneList: any[] = [];
  dropAreaList: any[] = [];
  dropTerritoryList: any[] = [];
  dropDealershipList: any[] = [];
  dropShopList: any[] = [];

  dataRegionList: any[] = [];
  dataZoneList: any[] = [];
  dataAreaList: any[] = [];
  dataTerritoryList: any[] = [];
  dataDealershipList: any[] = [];
  dataShopList: any[] = [];

  map: any;
  drawnPolygons: google.maps.Polygon[] = [];
  markers: google.maps.Marker[] = [];

  infoWindow: any; // InfoWindow instance

  constructor(private notificationsService: NotificationsService, private constantService: ConstantService, private territoryService: TerritoryService, private dealershipService: DealershipService,
    private shopService: ShopService, private formBuilder: FormBuilder, private zoneService: ZoneService, private regionService: RegionService, private areaService: AreaService) { }

  ngOnInit(): void {
    this.fieldMapForm = this.formBuilder.group({
      zoneId: [{ value: 0, disabled: true }],
      territoryId: [{ value: 0, disabled: true }],   // Initially disabled
      dealershipId: [{ value: 0, disabled: true }],  // Initially disabled
      shopId: [{ value: 0, disabled: true }], // Initially disabled
      regionId: [0],
      areaId: [{ value: 0, disabled: true }], // Initially disabled

      zoneFilterEnabled: [false],
      areaFilterEnabled: [false],
      territoryFilterEnabled: [false],
      dealershipFilterEnabled: [false],
      shopFilterEnabled: [false],
    });

    this.getRegions();

    this.fieldMapForm.get('zoneFilterEnabled')?.valueChanges.subscribe(enabled => {
      if (enabled) {
        this.fieldMapForm.get('zoneId')?.enable();
      } else {
        this.fieldMapForm.get('zoneId')?.disable();
      }
    });

    this.fieldMapForm.get('areaFilterEnabled')?.valueChanges.subscribe(enabled => {
      if (enabled) {
        this.fieldMapForm.get('areaId')?.enable();
      } else {
        this.fieldMapForm.get('areaId')?.disable();
      }
    });

    this.fieldMapForm.get('territoryFilterEnabled')?.valueChanges.subscribe(enabled => {
      if (enabled) {
        this.fieldMapForm.get('territoryId')?.enable();
      } else {
        this.fieldMapForm.get('territoryId')?.disable();
      }
    });

    this.fieldMapForm.get('dealershipFilterEnabled')?.valueChanges.subscribe(enabled => {
      if (enabled) {
        this.fieldMapForm.get('dealershipId')?.enable();
      } else {
        this.fieldMapForm.get('dealershipId')?.disable();
      }
    });

    this.fieldMapForm.get('shopFilterEnabled')?.valueChanges.subscribe(enabled => {
      if (enabled) {
        this.fieldMapForm.get('shopId')?.enable();
      } else {
        this.fieldMapForm.get('shopId')?.disable();
      }
    });

    this.initMap();
  }

  initMap(): void {
    this.map = new google.maps.Map(document.getElementById("fieldMap")!, {
      center: { lat: 30.51, lng: 70.36 }, // Initial map center Set Lahore
      zoom: 5.8, // Initial zoom level
      mapTypeControl: false
    });

    // Initialize the InfoWindow
    this.infoWindow = new google.maps.InfoWindow();
    this.filter();
    
  }

  drawPolygon(data: any[], typeId: number, preText: any): void {

    if (data.length == 0) {
      return;
    }

    // Iterate over each set of coordinates in the list
    data.forEach((coordinateSet: any) => {

      // Parse the JSON string into an array of coordinate objects
      const coordinates = JSON.parse(coordinateSet.coordinates);

      // Convert the array into a format suitable for Google Maps
      const path = coordinates.map((coord: any) => new google.maps.LatLng(coord.lat, coord.lng));

      var polygonDrawProperty = this.constantService.getPolygonDrawProperty(typeId)

      // Create the polygon with border only
      const polygon = new google.maps.Polygon({
        paths: path,
        strokeColor: polygonDrawProperty.borderColor, // Border color
        strokeOpacity: polygonDrawProperty.borderOpacity, // Border opacity
        strokeWeight: polygonDrawProperty.borderWidth, // Border width
        fillColor: polygonDrawProperty.fillColor, // Fill color (not visible due to fillOpacity being 0)
        fillOpacity: polygonDrawProperty.fillOpacity // Make the fill transparent
      });

      // Add event listeners for polygon hover
      google.maps.event.addListener(polygon, 'mouseover', (event: any) => {
        this.infoWindow.setContent(preText + coordinateSet.name); // Set the content to the polygon's name
        this.infoWindow.setPosition(event.latLng); // Position the InfoWindow near the cursor
        this.infoWindow.open(this.map);
      });

      google.maps.event.addListener(polygon, 'mouseout', () => {
        this.infoWindow.close(); // Close the InfoWindow when the mouse leaves the polygon
      });

      // Add the polygon to the map
      polygon.setMap(this.map);

      // Store the polygon in the list
      this.drawnPolygons.push(polygon);
      if (data.length == 1) {
        // Create a LatLngBounds object
        const bounds = new google.maps.LatLngBounds();

        // Extend the bounds with each coordinate
        path.forEach((latLng: google.maps.LatLng | google.maps.LatLngLiteral) => bounds.extend(latLng));

        // Adjust the map's viewport to fit the polygon
        this.map.fitBounds(bounds);

      }
    });
  }

  markPin(data: any[], typeId: number, preText: any) {
    const bounds = new google.maps.LatLngBounds();

    data.forEach((markerPinsSet: any) => {
      var pinLocationDrawProperty = this.constantService.getPinLocationDrawProperty(typeId);

      const icon = {
        url: pinLocationDrawProperty.iconFilePath,
        scaledSize: new google.maps.Size(50, 50),
        anchor: new google.maps.Point(22, 50),
      };

      const markerPin = JSON.parse(markerPinsSet.pinLocation);
      const marker = new google.maps.Marker({
        position: new google.maps.LatLng(markerPin.lat, markerPin.lng),
        map: this.map,
        icon: typeId == 1 ? "https://maps.gstatic.com/mapfiles/ms2/micons/green-dot.png" : "https://maps.gstatic.com/mapfiles/ms2/micons/red-dot.png",
      });

      this.markers.push(marker); // Store the marker

      google.maps.event.addListener(marker, 'mouseover', (event: any) => {
        this.infoWindow.setContent(preText + markerPinsSet.name); // Set the content to the marker's name
        this.infoWindow.setPosition(marker.getPosition()); // Position the InfoWindow at the marker's position
        this.infoWindow.open(this.map);
      });

      bounds.extend(marker.getPosition()!);
      const listener = google.maps.event.addListenerOnce(this.map, 'bounds_changed', () => {
        const currentZoom = this.map.getZoom();
        this.map.setZoom(currentZoom); // Zoom out by 2 levels
      });
    });
  }

  private clearPolygons(): void {
    // Remove each polygon from the map
    this.drawnPolygons.forEach((polygon) => polygon.setMap(null));
    this.markers.forEach(marker => marker.setMap(null));

    // Clear the list of polygons
    this.drawnPolygons = [];
    this.markers = [];
  }

  // onZoneChange() {
  //   this.fieldMapForm.get('territoryId')?.patchValue(0);
  //   let zoneId = this.fieldMapForm.get('zoneId')?.value;

  //   this.getTerritoryByAreaId();

  //   this.dropDealershipList = [];
  //   this.dropShopList = [];
  //   this.fieldMapForm.get('dealershipId')?.patchValue(0);
  //   this.fieldMapForm.get('shopId')?.patchValue(0);
  // }

  onTerritoryChange() {
    this.fieldMapForm.get('dealershipId')?.patchValue(0);
    this.fieldMapForm.get('shopId')?.patchValue(0);

    let territoryId = this.fieldMapForm.get('territoryId')?.value;

    this.getDealershipByTerritoryId(territoryId);
    this.getShopsByTerritoryId(territoryId);
  }

  async filter() {
    var checkZone = this.fieldMapForm.get('zoneFilterEnabled')?.value;
    var checkArea = this.fieldMapForm.get('areaFilterEnabled')?.value;
    var checkTerritory = this.fieldMapForm.get('territoryFilterEnabled')?.value;
    var checkDealership = this.fieldMapForm.get('dealershipFilterEnabled')?.value;
    var checkShop = this.fieldMapForm.get('shopFilterEnabled')?.value;

    if (checkDealership === true || checkShop === true) {
      if (checkTerritory === false) {
        this.notificationsService.showNotification('Please Select Territory Filter!', 'snack-bar-danger');
        return;
      }
    }

    let _filterForm: any = {};
    _filterForm = Object.assign(_filterForm, this.fieldMapForm.value);

    (await this.zoneService.getFieldMapFilter(_filterForm)).subscribe({
      next: (data) => {
        console.log(data);
        this.clearPolygons();
        if (data.regionList.length > 0) {
          this.drawPolygon(data.regionList, 3, 'Region - ');
        }
        if (data.zoneList.length > 0) {
          this.drawPolygon(data.zoneList, 1, 'Zone - ');
        }
        if (data.areaList.length > 0) {
          this.drawPolygon(data.areaList, 4, 'Area - ');
        }
        if (data.territoryList.length > 0) {
          this.drawPolygon(data.territoryList, 2, 'Territory - ');
        }
        if (data.dealershipList.length > 0) {
          this.markPin(data.dealershipList, 1, 'Distributor - ');
        }
        if (data.shopList.length > 0) {
          this.markPin(data.shopList, 2, 'Shop - ');
        }
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  // // ------------------Region Area Start------------------
  async getRegions() {
    let _regionFilterForm = {};
    (await this.regionService.getAllRegion(_regionFilterForm)).subscribe({
      next: (data) => {
        if (data && Array.isArray(data.item1)) {
          this.dropRegionList = data.item1;
        } else {
          console.error('Expected array but got:', data.item1);
          this.dropRegionList = [];
        }
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }
  // // ------------------Region Area End------------------


  // // ------------------Zone Area Start------------------
  async getZoneByRegionId() {
    (await this.zoneService.getZoneByRegionId(this.fieldMapForm.get('regionId')?.value)).subscribe({
      next: (data) => {
        if (data && Array.isArray(data)) {
          this.dropZoneList = data;
        } else {
          console.error('Expected array but got:', data);
          this.dropZoneList = [];
        }
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }
  // // ------------------Zone Area End------------------


  // // ------------------Area Start------------------
  async getAreaByZoneId() {
    (await this.areaService.getAreaByZoneId(this.fieldMapForm.get('zoneId')?.value)).subscribe({
      next: (data) => {
        if (data && Array.isArray(data)) {
          this.dropAreaList = data;
        } else {
          console.error('Expected array but got:', data);
          this.dropAreaList = [];
        }
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }
  // // ------------------Area End------------------


  // // ------------------Territory Area Start------------------
  // async getTerritories() {
  //   let _territoryFilterForm = {};
  //   (await this.territoryService.getAllTerritory(_territoryFilterForm)).subscribe({
  //     next: (data) => {
  //       if (data && Array.isArray(data.item1)) {
  //         this.territoryList = data.item1;
  //       } else {
  //         console.error('Expected array but got:', data.item1);
  //         this.territoryList = [];
  //       }
  //     },
  //     error: (error) => {
  //       console.log(error);
  //       this.isLoading = false;
  //     }
  //   });
  // }

  // async getTerritoryById(territoryId: any) {
  //   (await this.territoryService.getTerritoryById(territoryId)).subscribe({
  //     next: (data) => {
  //       if (data) {
  //         this.territoryList = data;
  //       } else {
  //         console.error('Expected array but got:', data);
  //         this.territoryList = null;
  //       }
  //     },
  //     error: (error) => {
  //       console.log(error);
  //       this.isLoading = false;
  //     }
  //   });
  // }

  async getTerritoryByAreaId() {
    (await this.territoryService.getTerritoryByAreaId(this.fieldMapForm.get('areaId')?.value)).subscribe({
      next: (data) => {
        if (data && Array.isArray(data)) {
          this.dropTerritoryList = data;
        } else {
          console.error('Expected array but got:', data);
          this.dropTerritoryList = [];
        }
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }
  // // ------------------Territory Area End------------------


  // // ------------------Distributor Area Start------------------
  // async getDealerships() {
  //   let _dealershipFilterForm = {};
  //   (await this.dealershipService.getAllDealership(_dealershipFilterForm)).subscribe({
  //     next: (data) => {
  //       if (data && Array.isArray(data.item1)) {
  //         this.dealershipList = data.item1;
  //       } else {
  //         console.error('Expected array but got:', data.item1);
  //         this.dealershipList = [];
  //       }
  //     },
  //     error: (error) => {
  //       console.log(error);
  //       this.isLoading = false;
  //     }
  //   });
  // }

  // async getDealershipById(dealershipId: any) {
  //   (await this.dealershipService.getDealershipById(dealershipId)).subscribe({
  //     next: (data) => {
  //       if (data) {
  //         this.dealershipList = data;
  //       } else {
  //         console.error('Expected array but got:', data);
  //         this.dealershipList = null;
  //       }
  //     },
  //     error: (error) => {
  //       console.log(error);
  //       this.isLoading = false;
  //     }
  //   });
  // }

  async getDealershipByTerritoryId(territoryId: any) {
    (await this.dealershipService.getDealershipByTerritoryId(territoryId)).subscribe({
      next: (data) => {
        if (data && Array.isArray(data)) {
          this.dropDealershipList = data;
        } else {
          console.error('Expected array but got:', data);
          this.dropDealershipList = [];
        }
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }
  // // ------------------Distributor Area End------------------


  // // ------------------Shop Area Start------------------
  // async getShops() {
  //   let _shopFilterForm = {};
  //   (await this.shopService.getAllShop(_shopFilterForm)).subscribe({
  //     next: (data) => {
  //       if (data && Array.isArray(data.item1)) {
  //         this.shopList = data.item1;
  //       } else {
  //         console.error('Expected array but got:', data.item1);
  //         this.shopList = [];
  //       }
  //     },
  //     error: (error) => {
  //       console.log(error);
  //       this.isLoading = false;
  //     }
  //   });
  // }

  // async getShopById(shopId: any) {
  //   (await this.shopService.getShopById(shopId)).subscribe({
  //     next: (data) => {
  //       if (data) {
  //         this.shopList = data;
  //       } else {
  //         console.error('Expected array but got:', data);
  //         this.shopList = null;
  //       }
  //     },
  //     error: (error) => {
  //       console.log(error);
  //       this.isLoading = false;
  //     }
  //   });
  // }

  async getShopsByTerritoryId(territoryId: any) {
    (await this.shopService.getShopsByTerritoryId(territoryId)).subscribe({
      next: (data) => {
    
        if (data && Array.isArray(data)) {
          this.dropShopList = data;
        } else {
          console.error('Expected array but got:', data);
          this.dropShopList = [];
        }
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }
  // // ------------------Shop Area End------------------


}