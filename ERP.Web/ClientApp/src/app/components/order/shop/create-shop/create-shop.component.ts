import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ShopService } from '../shop.service';
import { createMask } from '@ngneat/input-mask';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { AreaService } from '../../area/area.service';
import { DrawMapComponent } from '../../gmap/draw-map/draw-map.component';
import { RegionService } from '../../region/region.service';
import { TerritoryService } from '../../territory/territory.service';
import { ZoneService } from '../../zone/zone.service';
import { ShopTypeService } from '../../shop-type/shop-type.service';

@Component({
  selector: 'app-create-shop',
  templateUrl: './create-shop.component.html',
  styleUrls: ['./create-shop.component.css'],standalone: false
})

export class CreateShopComponent implements OnInit {
  createShopForm!: FormGroup;
  isLoading = false;
  dataSource!: any;
  roleList: any;
  dropdownSettings: any;
  isEditMode: boolean = false;

  zoneList: any;
  territoryList: any;

  zoneId: any;

  filteredRegion: any;
  filteredZone: any;
  filteredArea: any;
  filteredTerritory: any;

  scheduleList: any;

  phoneNoInputMask = createMask('0399-9999999');

  dialogRef: any;

  documents: any[] = [];
  urlSafe: SafeResourceUrl | undefined;

  regionList: any;
  areaList: any;
  shopTypeList: any;

  constructor(private sanitizer: DomSanitizer, private notificationsService: NotificationsService, private territoryService: TerritoryService,
    private zoneService: ZoneService, private dialog: MatDialog, private formBuilder: FormBuilder, private constantService: ConstantService, private regionService: RegionService,
    private shopService: ShopService, private areaService: AreaService, private shopTypeService: ShopTypeService, @Inject(MAT_DIALOG_DATA) public data: { element: any, shops: any }) { }

  ngOnInit(): void {
    this.createShopForm = this.formBuilder.group({
      id: [0],
      name: ['', Validators.required],
      ownerName: ['', Validators.required],
      phoneNo: ['', Validators.required],
      openingTime: ['', Validators.required],
      closingTime: ['', Validators.required],
      address: ['', Validators.required],
      pinLocation: ['', Validators.required],
      territoryId: [0, Validators.required],
      // schedulerId: [0, Validators.required],
      zoneId: [0],
      regionId: [0],
      areaId: [0],
      shopTypeId: [0],

      secondaryPhoneNo: [''],
      landmark: [''],
      pepsiFridge: [0],
      cokeFridge: [0],
      nestleFridge: [0],
      nesfrutaFridge: [0],
      othersFridge: [0]
    });

    this.getRegions();
    this.getAllShopTypes();
    // this.getSchedulers();

    this.zoneId = this.data.element?.territory?.area?.zone.id;
    this.createShopForm.get('zoneId')?.patchValue(this.zoneId);

    // if (this.data.element != null) {
    //   this.getTerritoryByAreaId(this.data.element?.territory?.areaId);
    // }

    this.LoadData(this.data.element);
  }

  get f() {
    return this.createShopForm.controls;
  }

  async saveShop() {
    this.isLoading = true;
    if (this.createShopForm.invalid) {
      this.constantService.markFormGroupTouched(this.createShopForm);
      return;
    }
    let _createShopForm: any = {};
    _createShopForm = Object.assign(_createShopForm, this.createShopForm.value);

    _createShopForm['shopImages'] = this.documents;

    (await this.shopService.saveShop(_createShopForm)).subscribe(
      {
        next: (data) => {
          if (data.Status == 200) {
            this.notificationsService.showNotification('Shop Saved Successfully', 'snack-bar-success');
            this.dialog.closeAll();
            this.isLoading = false;
          }
          else if (data.Status == 409) {
            this.notificationsService.showNotification('Name already exist!', 'snack-bar-danger');
            this.isLoading = false;
          }
          else if (data.Status == 412) {
            this.notificationsService.showNotification('Phone No already exist!', 'snack-bar-danger');
            this.isLoading = false;
          }
        },
        error: (error) => {
          this.notificationsService.showNotification('Please Fill the required fields!', 'snack-bar-danger');
          console.log(error);
          this.isLoading = false;
        }
      });
  }
  timeStringToTimeSpan(timeString: string): { hours: number, minutes: number } {
    const parts = timeString.split(':');
    const hours = parseInt(parts[0], 10);
    const minutes = parseInt(parts[1], 10);

    return { hours, minutes };
  }

  LoadData(element: any) {
    if (this.data.element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.createShopForm);

      this.createShopForm.get('regionId')?.patchValue(this.data.element.territory?.area?.zone?.regionId);
      this.getZoneByRegionId();

      this.createShopForm.get('zoneId')?.patchValue(this.data.element.territory?.area?.zoneId);
      this.getAreaByZoneId();

      this.createShopForm.get('areaId')?.patchValue(this.data.element.territory?.areaId);
      this.getTerritoryByAreaId();

      this.createShopForm.get('territoryId')?.patchValue(this.data.element.territoryId);

      this.filteredRegion = this.data.element.territory?.area?.zone?.region;
      this.filteredZone = this.data.element.territory?.area?.zone;
      this.filteredArea = this.data.element.territory?.area;
      this.filteredTerritory = this.data.element.territory;

      this.documents = element?.attachments?.filter((x: { isActive: boolean; }) => x.isActive == true);
    }
    console.log(this.createShopForm);
  }

  async getZones() {
    let _zoneFilterForm = {};
    (await this.zoneService.getAllZone(_zoneFilterForm)).subscribe(
      {
        next: (data) => {
          this.zoneList = data.item1;
        },
        error: (error) => {
          console.log(error);
          this.isLoading = false;
        }
      });
  }

  async getTerritoryByAreaId() {

    this.territoryList = [];
    
    this.createShopForm.get('territoryId')?.patchValue('');
    (await this.territoryService.getTerritoryByAreaId(this.createShopForm.get('areaId')?.value)).subscribe(
      {
        next: (data: any) => {
          this.territoryList = data;
        },
        error: (error: any) => {
          console.log(error);
          this.isLoading = false;
        }
      });
  }

  selectPinLocation(): void {

    if (this.filteredRegion == null || this.filteredZone == null || this.filteredArea == null || this.filteredTerritory == null) {
  
      this.notificationsService.showNotification('Please Select Region, Zone, Area, Territory First!', 'snack-bar-danger');
      return;
    }

    const coordinatesList: any[] = [];
    const markerPinsList: any[] = [];

    coordinatesList.push({
      typeId: 1,
      coordinates: this.filteredZone.coordinates,
      name: 'Zone-' + this.filteredZone.name
    });

    coordinatesList.push({
      typeId: 2,
      coordinates: this.filteredTerritory.coordinates,
      name: 'Territory-' + this.filteredTerritory.name
    });

    if (this.isEditMode == true) {
      this.data.shops.filter((x: { territoryId: any; }) => x.territoryId == this.data.element.territoryId).forEach((item: { isActive: boolean; pinLocation: any; name: string; address: any; phoneNo: any; }) => {
        if (item.isActive == true) {
          markerPinsList.push({
            typeId: 1,
            pinLocation: item.pinLocation,
            name: 'Shop-' + item.name,
            address: item.address,
            phoneNo: item.phoneNo
          });
        }
      });
    }
    else {
      this.filteredTerritory.shop.forEach((item: { isActive: boolean; pinLocation: any; name: string; address: any; phoneNo: any; }) => {
        if (item.isActive == true) {
          markerPinsList.push({
            typeId: 1,
            pinLocation: item.pinLocation,
            name: 'Shop-' + item.name,
            address: item.address,
            phoneNo: item.phoneNo
          });
        }
      });
    }

    const element = {
      caption: 'Zone: ' + this.filteredZone.name + '- Territory:' + this.filteredTerritory.name,
      fromComponent: 'createDealership',
      drawingPolygon: false,
      drawingMarker: true,
      coordinates: coordinatesList,
      typeId: 2,
      isShowInfoBox: true,
      markerPins: markerPinsList,
      isFocusDrawPolygon: true
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
      if (result) {
        this.createShopForm.get('pinLocation')?.setValue(result);
      }
    });
  }

  // onTerritoryChange(selectedTerritoryId: number): void {
  //   if (selectedTerritoryId > 0) {
  //     this.filteredTerritory = this.territoryList?.find(zone => zone.id === selectedTerritoryId);
  //   }
  //   else {
  //     this.filteredTerritory = null;
  //   }
  //   this.getFilteredData();
  // }

  // async getSchedulers() {
  //   let _schedulerFilterForm = {};
  //   (await this.schedulerService.getAllScheduler(_schedulerFilterForm)).subscribe(
  //     {
  //       next: (data) => {
  //         this.scheduleList = data.item1;
  //       },
  //       error: (error) => {
  //         console.log(error);
  //         this.isLoading = false;
  //       }
  //     });
  // }

  onDocumentSourceChange(event: any) {
    if (event.target.files.length > 0) {
      const selectedFiles = event.target.files;
      for (let file of selectedFiles) {
        let fileName = file.name;
        let fileExtension = fileName.split('.').pop().toLowerCase();
        let reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = (event) => {
          let fileSource = event.target?.result;

          let documentObj = {
            'id': 0,
            'fileSource': fileSource,
            'imageName': fileName,
            'extension': fileExtension
          }

          this.documents.push(documentObj);
        };
      }

      console.log(this.documents);
    }
  }


  onDocumentSourceRemove(event: any, docIndex: number) {
    this.removeDraftsmanDesignDocuments(docIndex);
  }

  removeDraftsmanDesignDocuments(i: number) {
    this.documents.splice(i, 1);
  }

  GetDocument(event: any, index: number, template: any) {
    this.urlSafe = this.sanitizer.bypassSecurityTrustResourceUrl(this.documents[index].fileSource + '#toolbar=0');
    this.dialogRef = this.dialog.open(template, {
      width: '50%',
      height: '70%',
      disableClose: true,
    });
  }

  async getZoneByRegionId() {

    this.zoneList = [];
    this.areaList = [];
    this.territoryList = [];

    (await this.zoneService.getZoneByRegionId(this.createShopForm.get('regionId')?.value)).subscribe({
      next: (data) => {
        this.zoneList = data;
        // if (this.data.element != null) {
        //   this.filteredZone = this.zoneList.find(zone => zone.id === this.data.element.area?.zoneId);
        // }
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

    (await this.areaService.getAreaByZoneId(this.createShopForm.get('zoneId')?.value)).subscribe({
      next: (data) => {
        this.areaList = data;
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  async getAllShopTypes() {
    let _shopTypeFilter = {};
    (await this.shopTypeService.getAllShopType(_shopTypeFilter)).subscribe({
      next: (data: { item1: any; }) => {
        this.shopTypeList = data.item1;
      },
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  getFilteredData() {
    this.filteredRegion = this.regionList?.find((region: { id: any; }) => region.id === this.createShopForm.get('regionId')?.value);
    this.filteredZone = this.zoneList?.find((zone: { id: any; }) => zone.id === this.createShopForm.get('zoneId')?.value);
    this.filteredArea = this.areaList?.find((area: { id: any; }) => area.id === this.createShopForm.get('areaId')?.value);
    this.filteredTerritory = this.territoryList?.find((territory: { id: any; }) => territory.id === this.createShopForm.get('territoryId')?.value);
  }


}