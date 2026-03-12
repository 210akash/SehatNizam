import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { createMask } from '@ngneat/input-mask';
import { ConstantService } from '../../../../Service/constant.service';
import { DrawMapComponent } from '../../gmap/draw-map/draw-map.component';

@Component({
  selector: 'app-view-DSF',
  templateUrl: './view-DSF.component.html',
  styleUrls: ['./view-DSF.component.css'], standalone: false
})

export class ViewDSFComponent implements OnInit {
  viewDSFForm!: FormGroup;
  isLoading = false;
  dataSource!: any;
  roleList: any;

  displayedColumns: string[] = ['zone', 'territory', 'name'];

  emailMask = createMask({ alias: 'email' });
  phoneNoInputMask = createMask('0399-9999999');
  cnicInputMask = createMask('99999-9999999-9');

  imageSrc: any;

  constructor(private dialog: MatDialog, private formBuilder: FormBuilder, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.viewDSFForm = this.formBuilder.group({
      id: [0],
      email: ['', Validators.required],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      phoneNumber: ['', Validators.required],
      dateOfBirth: [null, Validators.required],
      emergencyPhoneNo: ['', Validators.required],
      bloodGroup: ['', Validators.required],
      cnic: ['', Validators.required],
      shiftTimeStart: [null, Validators.required],
      shiftTimeEnd: [null, Validators.required],
      address: ['', Validators.required],
      imageName: [''],
      fileSource: ['', Validators.required],
      extension: ['']
    });

    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    //this.dataSource = element.dsfRoute.filter((x: { isActive: boolean; }) => x.isActive == true);
    this.constantService.LoadData(element, this.viewDSFForm);
    this.imageSrc = this.data.element.attachments[0]?.imageName;
    this.viewDSFForm.get("fileSource")?.patchValue(this.data.element.attachments[0]?.imageName);

    let convertDate = this.constantService.formatDate(this.data.element.dateOfBirth);
    this.viewDSFForm.get("dateOfBirth")?.patchValue(convertDate);
  }

  viewDSF(): void {

    const coordinatesList: any[] = [];

    coordinatesList.push({
      typeId: 1,
      coordinates: 'DSF-' + this.data.element.coordinates
    });

    const element = {
      caption: 'View DSF ( ' + this.data.element.name + ')',
      fromComponent: 'viewDSF',
      drawingPolygon: false,
      drawingMarker: false,
      coordinates: coordinatesList,
      isFocusDrawPolygon: true,
      isShowInfoBox: true
    };

    const dialogRef = this.dialog.open(DrawMapComponent, {
      width: '70%',
      height: 'auto',
      minHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {

    });
  }


}