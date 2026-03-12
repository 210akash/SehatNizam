import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { ConstantService } from '../../../../Service/constant.service';
import { DrawMapComponent } from '../../gmap/draw-map/draw-map.component';

@Component({
  selector: 'app-view-dealership',
  templateUrl: './view-dealership.component.html',
  styleUrls: ['./view-dealership.component.css'],standalone: false
})

export class ViewDealershipComponent implements OnInit {
  viewDealershipForm!: FormGroup;
  isLoading = false;
  dialogRef: any;

  documents: any[] = [];
  urlSafe: SafeResourceUrl | undefined;

  constructor(private sanitizer: DomSanitizer,private dialog: MatDialog, private formBuilder: FormBuilder, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.viewDealershipForm = this.formBuilder.group({
      id: [0],
      name: [''],
      phoneNo: [''],
      address: [''],
      pinLocation: [''],
      zone: [''],
      territory: ['']
    });

    this.LoadData(this.data.element);
    this.viewDealershipForm.get('zone')?.patchValue(this.data.element.territory?.area?.zone.name);
    this.viewDealershipForm.get('territory')?.patchValue(this.data.element.territory?.name);
  }

  LoadData(element: any) {
    this.constantService.LoadData(element, this.viewDealershipForm);
    this.documents = element?.attachments.filter((x: { isActive: boolean; }) => x.isActive == true);;
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
      name: 'Territory-' + this.data.element.territory.name,
    });

    markerPinsList.push({
      typeId: 1,
      pinLocation: this.data.element.pinLocation,
      name: 'Dealer-'+this.data.element.name
    });

    const element = {
      caption: 'Zone: ' + this.data.element.territory.zone.name + ' - Territory: ' + this.data.element.territory.name + ' - Distributor: ' + this.data.element.name,
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


}
