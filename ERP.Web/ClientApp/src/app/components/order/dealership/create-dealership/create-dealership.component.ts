import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { DealershipService } from '../dealership.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { createMask } from '@ngneat/input-mask';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { AreaService } from '../../area/area.service';
import { DrawMapComponent } from '../../gmap/draw-map/draw-map.component';
import { RegionService } from '../../region/region.service';
import { TerritoryService } from '../../territory/territory.service';
import { ZoneService } from '../../zone/zone.service';

@Component({
  selector: 'app-create-dealership',
  templateUrl: './create-dealership.component.html',
  styleUrls: ['./create-dealership.component.css'], standalone: false
})

export class CreateDealershipComponent implements OnInit {
  createDealershipForm!: FormGroup;
  isLoading = false;
  dataSource!: any;
  roleList: any;
  dropdownSettings: any;
  isEditMode: boolean = false;

  zoneList: any;
  territoryList: any;

  zoneId: any;

  filteredZone: any;
  filteredTerritory: any;

  phoneNoInputMask = createMask('0399-9999999');
  cnicInputMask = createMask('99999-9999999-9');
  dialogRef: any;

  documents: any[] = [];
  urlSafe: SafeResourceUrl | undefined;

  areaList: any;
  regionList: any;

  filteredRegion: any;
  filteredArea: any;
  distributorTypeList: any;

  constructor(private sanitizer: DomSanitizer, private notificationsService: NotificationsService, private territoryService: TerritoryService, private zoneService: ZoneService, private dialog: MatDialog,
    private formBuilder: FormBuilder, private constantService: ConstantService, private dealershipService: DealershipService, private regionService: RegionService, private areaService: AreaService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.createDealershipForm = this.formBuilder.group({
      id: [0],
      name: ['', Validators.required],
      phoneNo: ['', Validators.required],
      secondaryPhoneNo: [''],
      cnic: ['', Validators.required],
      ownerName: ['', Validators.required],
      address: ['', Validators.required],
      remarks: [''],
      landmark: [''],
      pinLocation: ['', Validators.required],
      territoryId: ['', Validators.required],
      zoneId: [0],
      regionId: [0],
      areaId: [0],
      isActive: [true, Validators.required],
      dealershipTypeId: [1],
    });

    this.getRegions();

    this.zoneId = this.data?.element?.territory?.area?.zone?.id;
    this.createDealershipForm.get('zoneId')?.patchValue(this.zoneId);

    // this.getTerritoryByAreaId();
    this.LoadData(this.data?.element);
    this.getAllDistributorTypes();
  }

  get f() {
    return this.createDealershipForm.controls;
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
  async saveDealership() {
    this.isLoading = true;
    if (this.createDealershipForm.invalid) {
      this.constantService.markFormGroupTouched(this.createDealershipForm);
      return;
    }
    let _createDealershipForm: any = {};
    _createDealershipForm = Object.assign(_createDealershipForm, this.createDealershipForm.value);

    _createDealershipForm['dealershipImages'] = this.documents;

    (await this.dealershipService.saveDealership(_createDealershipForm)).subscribe(
      {
        next: (data) => {
          if (data.Status == 200) {
            this.notificationsService.showNotification('Distributor Saved Successfully!', 'snack-bar-success');
            this.dialog.closeAll();
            this.isLoading = false;
          }
          else if (data.Status == 409) {
            this.notificationsService.showNotification('Active Distributor alreay exists against territory!', 'snack-bar-danger');
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

  LoadData(element: any) {
    if (this.data.element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.createDealershipForm);

      this.createDealershipForm.get('regionId')?.patchValue(this.data.element.territory?.area?.zone?.regionId);
      this.getZoneByRegionId();

      this.createDealershipForm.get('zoneId')?.patchValue(this.data.element.territory?.area?.zoneId);
      this.getAreaByZoneId();

      this.createDealershipForm.get('areaId')?.patchValue(this.data.element.territory?.areaId);
      this.getTerritoryByAreaId();

      this.createDealershipForm.get('territoryId')?.patchValue(this.data.element.territoryId);

      this.filteredRegion = this.data.element.territory?.area?.zone?.region;
      this.filteredZone = this.data.element.territory?.area?.zone;
      this.filteredArea = this.data.element.territory?.area;
      this.filteredTerritory = this.data.element.territory;

      this.documents = element?.attachments?.filter((x: { isActive: boolean; }) => x.isActive == true);
    }
    console.log(this.createDealershipForm);
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

    // if (zoneId > 0) {
    //   this.filteredZone = this.zoneList?.find(zone => zone.id === zoneId);
    // }
    // else {
    //   this.filteredZone = null;
    // }

    this.territoryList = [];
    this.createDealershipForm.get('territoryId')?.patchValue('');
    (await this.territoryService.getTerritoryByAreaId(this.createDealershipForm.get('areaId')?.value)).subscribe(
      {
        next: (data: any) => {
          this.territoryList = data;
          // this.LoadData(this.data.element);
        },
        error: (error: any) => {
          console.log(error);
          this.isLoading = false;
        }
      });
  }

  selectPinLocation(): void {
    if (this.filteredRegion == null || this.filteredZone == null || this.filteredArea == null || this.filteredTerritory == null) {
  
      this.notificationsService.showNotification('Please Select Region, Zone, Area, Territory First!', 'danger');
      return;
    }

    const coordinatesList: any[] = [];

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

    const element = {
      caption: 'Zone: ' + this.filteredZone.name + '- Territory:' + this.filteredTerritory.name,
      fromComponent: 'createDealership',
      drawingPolygon: false,
      drawingMarker: true,
      coordinates: coordinatesList,
      typeId: 2,
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
      if (result) {
        this.createDealershipForm.get('pinLocation')?.setValue(result);
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
    this.removeDocuments(docIndex);
  }

  removeDocuments(i: number) {
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

    (await this.zoneService.getZoneByRegionId(this.createDealershipForm.get('regionId')?.value)).subscribe({
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
      next: (data: { item1: any; }) => {
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

    (await this.areaService.getAreaByZoneId(this.createDealershipForm.get('zoneId')?.value)).subscribe({
      next: (data: any) => {
        this.areaList = data;
      },
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  getFilteredData() {
    this.filteredRegion = this.regionList?.find((region: { id: any; }) => region.id === this.createDealershipForm.get('regionId')?.value);
    this.filteredZone = this.zoneList?.find((zone: { id: any; }) => zone.id === this.createDealershipForm.get('zoneId')?.value);
    this.filteredArea = this.areaList?.find((area: { id: any; }) => area.id === this.createDealershipForm.get('areaId')?.value);
    this.filteredTerritory = this.territoryList?.find((territory: { id: any; }) => territory.id === this.createDealershipForm.get('territoryId')?.value);
  }


}
