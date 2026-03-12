import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { ConstantService } from '../../../../Service/constant.service';
import { DrawMapComponent } from '../../gmap/draw-map/draw-map.component';

@Component({
  selector: 'app-view-shop',
  templateUrl: './view-shop.component.html',
  styleUrls: ['./view-shop.component.css'], standalone: false
})

export class ViewShopComponent implements OnInit {
  viewShopForm!: FormGroup;
  isLoading = false;
  dialogRef: any;

  documents: any[] = [];
  urlSafe: SafeResourceUrl | undefined;

  constructor(private sanitizer: DomSanitizer, private dialog: MatDialog, private formBuilder: FormBuilder, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.viewShopForm = this.formBuilder.group({
      id: [0],
      name: [''],
      distributor: [''],
      phoneNo: [''],
      secondaryPhoneNo: [''],
      address: [''],
      landmark: [''],
      pinLocation: [''],
      region: [''],
      zone: [''],
      area: [''],
      territory: [''],
      scheduler: [''],

      ownerName: [''],
      openingTime: [''],
      closingTime: [''],
      isVerified: [''],
      isTagFromMob: [''],

      createdDate: [''],
      createdBy: [''],
      verifiedDate: [''],
      verifiedBy: [''],
      pepsiFridge: [0],
      cokeFridge: [0],
      nestleFridge: [0],
      nesfrutaFridge: [0],
      othersFridge: [0]
    });

    this.LoadData(this.data.element);
    this.viewShopForm.get('region')?.patchValue(this.data.element.territory?.area?.zone?.region?.name);
    this.viewShopForm.get('zone')?.patchValue(this.data.element.territory?.area?.zone?.name);
    this.viewShopForm.get('area')?.patchValue(this.data.element.territory?.area?.name);
    this.viewShopForm.get('territory')?.patchValue(this.data.element.territory?.name);
    this.viewShopForm.get('scheduler')?.patchValue(this.data.element.scheduler?.name);
  }

  async LoadData(element: any) {
    this.constantService.LoadData(element, this.viewShopForm);
    this.viewShopForm.get('openingTime')?.patchValue(await this.convertToAmPmFormat(element.openingTime));
    this.viewShopForm.get('closingTime')?.patchValue(await this.convertToAmPmFormat(element.closingTime));
    this.viewShopForm.get('isVerified')?.patchValue(element.isVerified == true ? 'Yes' : 'No');
    this.viewShopForm.get('isTagFromMob')?.patchValue(element.isTagFromMob == true ? 'Mobile' : 'Web');
    this.documents = element?.attachments.filter((x: { isActive: boolean; }) => x.isActive == true);

    this.viewShopForm.get('createdDate')?.patchValue(this.constantService.formatDate(element.createdDate));
    this.viewShopForm.get('createdBy')?.patchValue(element.createdBy?.firstName + ' ' + element.createdBy?.lastName);
    this.viewShopForm.get('verifiedDate')?.patchValue(element.verifiedDate == null ? '' : this.constantService.formatDate(element.verifiedDate));
    this.viewShopForm.get('verifiedBy')?.patchValue(element.verifiedBy == null ? '' : element.verifiedBy?.firstName + ' ' + element.verifiedBy?.lastName);

    this.getFirstActiveDealership(this.data.element);
  }

  viewPinLocation(): void {

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
      pinLocation: this.data.element.pinLocation,
      name: 'Shop-' + this.data.element.name,
      address: this.data.element.address,
      phoneNo: this.data.element.phoneNo
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

  GetDocument(event: any, index: number, template: any) {
    this.urlSafe = this.sanitizer.bypassSecurityTrustResourceUrl(this.documents[index].fileSource + '#toolbar=0');
    this.dialogRef = this.dialog.open(template, {
      width: '50%',
      height: '70%',
      disableClose: true,
    });
  }

  // Method to convert TimeSpan string to AM/PM format
  async convertToAmPmFormat(timeString: string): Promise<string> {
    const parts = timeString.split(':');
    const date = new Date();
    date.setHours(parseInt(parts[0], 10));
    date.setMinutes(parseInt(parts[1], 10));
    date.setSeconds(parseInt(parts[2], 10));

    return date.toLocaleTimeString([], {
      hour: '2-digit',
      minute: '2-digit',
      hour12: true
    });
  }

  getFirstActiveDealership(element: any): void {
    const active = element.territory?.dealership?.find((d: any) => d.isActive);
    this.viewShopForm.get('distributor')?.patchValue(active?.name ?? '');
  }


}