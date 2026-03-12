/// <reference types="@types/google.maps" />
import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ZoneService } from '../zone.service';
import { ConstantService } from '../../../../Service/constant.service';
import { DealershipService } from '../../dealership/dealership.service';
import { ShopService } from '../../shop/shop.service';
import { TerritoryService } from '../../territory/territory.service';

declare const google: any;

@Component({
  selector: 'app-field-map',
  templateUrl: './field-map.component.html',
  styleUrls: ['./field-map.component.css'],standalone: false
})

export class FieldMapComponent implements OnInit {
  drawnPolygons: google.maps.Polygon[] = [];
  map: any;
  fieldMapForm!: FormGroup;
  isLoading = false;

  zoneList: any[] = [];
  territoryList: any[] = [];
  dealershipList: any[] = [];
  shopList: any[] = [];

  LzoneList: any = [];
  LterritoryList: any = [];
  LdealershipList: any = [];
  LshopList: any = [];

  SzoneList: any;
  SterritoryList: any;
  SdealershipList: any;
  SshopList: any;

  constructor(private constantService: ConstantService, private territoryService: TerritoryService, private dealershipService: DealershipService, private shopService: ShopService, private formBuilder: FormBuilder, private zoneService: ZoneService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.fieldMapForm = this.formBuilder.group({
      zoneId: [0],
      territoryId: [{ value: 0, disabled: true }],   // Initially disabled
      dealershipId: [{ value: 0, disabled: true }],  // Initially disabled
      shopId: [{ value: 0, disabled: true }], // Initially disabled
      territoryfilterEnabled: [false],
      dealershipfilterEnabled: [false],
      shopfilterEnabled: [false],
    });

    this.getZones();

    this.fieldMapForm.get('territoryfilterEnabled')?.valueChanges.subscribe(enabled => {
      if (enabled) {
        this.fieldMapForm.get('territoryId')?.enable();
      } else {
        this.fieldMapForm.get('territoryId')?.disable();
      }
    });
    this.initMap();
  }


  initMap(): void {
    this.map = new google.maps.Map(document.getElementById("fieldMap"), {
      center: { lat: 31.51, lng: 74.36 }, // Initial map center Set Lahore
      zoom: 8, // Initial zoom level
      mapTypeControl: false
    });
  }

  onZoneChange() {
    this.fieldMapForm.get('territoryId')?.patchValue(0);
    let zoneId = this.fieldMapForm.get('zoneId')?.value;

    this.getTerritoryByAreaId(zoneId);
  }

  onTerritoryChange() {
    this.fieldMapForm.get('dealershipId')?.patchValue(0);
    this.fieldMapForm.get('shopId')?.patchValue(0);

    let territoryId = this.fieldMapForm.get('territoryId')?.value;

    this.getShopsByTerritoryId(territoryId);
    this.getDealershipByTerritoryId(territoryId);
  }

  async filter() {
    // this.isLoading = true;
    let _fieldMapForm: any = {};
    _fieldMapForm = Object.assign(_fieldMapForm, this.fieldMapForm.value);

    (await this.zoneService.getFieldMapFilter(_fieldMapForm)).subscribe(
      {
        next: (data) => {
          if (data.Status == 200) {
            this.isLoading = false;
          }
        },
        error: (error) => {
          console.log(error);
          this.isLoading = false;
        }
      });






    this.clearPolygons();
    this.LzoneList = [];
    this.LterritoryList = [];
    // this.LdealershipList = [];
    // this.LshopList = [];

    this.SzoneList = null;
    this.SterritoryList = null;
    // this.SdealershipList = null;
    // this.SshopList = null;

    let zoneId = this.fieldMapForm.get('zoneId')?.value;
    let territoryId = this.fieldMapForm.get('territoryId')?.value;
    // let dealershipId = this.fieldMapForm.get('dealershipId')?.value;
    // let shopId = this.fieldMapForm.get('shopId')?.value;

    // console.log(zoneId);
    // console.log(territoryId);
    // console.log(dealershipId);
    // console.log(shopId);

    let territoryFilterEnable = this.fieldMapForm.get('territoryfilterEnabled')?.value;

    if (zoneId === 0) {
      this.getZones();
    }
    else {
      this.getZonesById(zoneId);
    }

    if (territoryFilterEnable == true) {
      if (territoryId === 0) {
        this.getTerritories();
      }
      else {
        this.getTerritoryById(territoryId);
      }
    }

    // if (dealershipId === 0) {
    //   this.getDealerships();
    // }
    // else {
    //   this.getDealershipById(dealershipId);
    // }

    // if (shopId === 0) {
    //   this.getShops();
    // }
    // else {
    //   this.getShopById(shopId);
    // }

    // setTimeout(() => {

    //   console.log(this.SzoneList);
    //   console.log(this.SterritoryList);
    //   console.log(this.SdealershipList);
    //   console.log(this.SshopList);

    //   console.log(this.LzoneList);
    //   console.log(this.LterritoryList);
    //   console.log(this.LdealershipList);
    //   console.log(this.LshopList);
    // }, 2000);

  }

  // ------------------Zones Area Start------------------
  async getZones() {
    let _zoneFilterForm = {};
    (await this.zoneService.getAllZone(_zoneFilterForm)).subscribe({
      next: (data) => {
        if (data && Array.isArray(data.item1)) {
          this.zoneList = data.item1;
          this.LzoneList = data.item1;

          this.drawPolygon(data.item1, 1);
        } else {
          console.error('Expected array but got:', data.item1);
          this.zoneList = [];
          this.LzoneList = [];
        }
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  async getZonesById(zoneId: any) {
    (await this.zoneService.getZoneById(zoneId)).subscribe({
      next: (data) => {
        if (data) {
          this.SzoneList = data;
          const dataList = [data];
          this.drawPolygon(dataList, 1);
        } else {
          console.error('Expected array but got:', data);
          this.SzoneList = null;
        }
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }
  // ------------------Zones Area End------------------


  // ------------------Territory Area Start------------------
  async getTerritories() {
    let _territoryFilterForm = {};
    (await this.territoryService.getAllTerritory(_territoryFilterForm)).subscribe({
      next: (data) => {
        if (data && Array.isArray(data.item1)) {
          this.LterritoryList = data.item1;
          this.drawPolygon(data.item1, 2);
        } else {
          console.error('Expected array but got:', data.item1);
          this.LterritoryList = [];
        }
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  async getTerritoryById(territoryId: any) {
    (await this.territoryService.getTerritoryById(territoryId)).subscribe({
      next: (data) => {
        if (data) {
          this.SterritoryList = data;
          const dataList = [data];
          this.drawPolygon(dataList, 2);
        } else {
          console.error('Expected array but got:', data);
          this.SterritoryList = null;
        }
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  async getTerritoryByAreaId(zoneId: any) {
    (await this.territoryService.getTerritoryByAreaId(zoneId)).subscribe({
      next: (data) => {
        if (data && Array.isArray(data)) {
          this.territoryList = data;
          this.LterritoryList = data;
        } else {
          console.error('Expected array but got:', data);
          this.territoryList = [];
          this.LterritoryList = null;
        }
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }
  // ------------------Territory Area End------------------


  // ------------------Distributor Area Start------------------
  async getDealerships() {
    let _dealershipFilterForm = {
      'dealershipTypeId': 1
    };
    (await this.dealershipService.getAllDealership(_dealershipFilterForm)).subscribe({
      next: (data) => {
        if (data && Array.isArray(data.item1)) {
          this.LdealershipList = data.item1;
        } else {
          console.error('Expected array but got:', data.item1);
          this.LdealershipList = [];
        }
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  async getDealershipById(dealershipId: any) {
    (await this.dealershipService.getDealershipById(dealershipId)).subscribe({
      next: (data) => {
        if (data) {
          this.SdealershipList = data;
        } else {
          console.error('Expected array but got:', data);
          this.SdealershipList = null;
        }
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  async getDealershipByTerritoryId(territoryId: any) {
    (await this.dealershipService.getDealershipByTerritoryId(territoryId)).subscribe({
      next: (data) => {
        if (data && Array.isArray(data)) {
          this.dealershipList = data;
          this.LdealershipList = data;
        } else {
          console.error('Expected array but got:', data);
          this.dealershipList = [];
          this.LdealershipList = [];
        }
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }
  // ------------------Distributor Area End------------------


  // ------------------Shop Area Start------------------
  async getShops() {
    let _shopFilterForm = {};
    (await this.shopService.getAllShop(_shopFilterForm)).subscribe({
      next: (data) => {
        if (data && Array.isArray(data.item1)) {
          this.LshopList = data.item1;
        } else {
          console.error('Expected array but got:', data.item1);
          this.LshopList = [];
        }
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  async getShopById(shopId: any) {
    (await this.shopService.getShopById(shopId)).subscribe({
      next: (data) => {
        if (data) {
          this.SshopList = data;
        } else {
          console.error('Expected array but got:', data);
          this.SshopList = null;
        }
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  async getShopsByTerritoryId(territoryId: any) {
    (await this.shopService.getShopsByTerritoryId(territoryId)).subscribe({
      next: (data) => {
        if (data && Array.isArray(data)) {
          this.shopList = data;
          this.LshopList = data;
        } else {
          console.error('Expected array but got:', data);
          this.shopList = [];
          this.LshopList = [];
        }
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }
  // ------------------Shop Area End------------------




  drawPolygon(data: any[], typeId: number): void {

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

      // Add the polygon to the map
      polygon.setMap(this.map);

      // Store the polygon in the list
      this.drawnPolygons.push(polygon);
      if (data.length == 1) {
        // Create a LatLngBounds object
        const bounds = new google.maps.LatLngBounds();

        // Extend the bounds with each coordinate
        path.forEach((latLng: any) => bounds.extend(latLng));

        // Adjust the map's viewport to fit the polygon
        this.map.fitBounds(bounds);

      }
    });
  }

  private clearPolygons(): void {
    // Remove each polygon from the map
    this.drawnPolygons.forEach((polygon) => polygon.setMap(null));

    // Clear the list of polygons
    this.drawnPolygons = [];
  }
}