import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';
import { DrawMapComponent } from '../../gmap/draw-map/draw-map.component';

@Component({
  selector: 'app-view-area',
  templateUrl: './view-area.component.html',
  styleUrls: ['./view-area.component.css'],standalone: false
})

export class ViewAreaComponent implements OnInit {
  viewAreaForm!: FormGroup;
  isLoading = false;
  dataSource!: any;
  roleList: any;
  constructor(private dialog: MatDialog,private formBuilder: FormBuilder, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.viewAreaForm = this.formBuilder.group({
      id: [0],
      name: [''],
      description: [''],
      coordinates: ['']
    });

    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    this.constantService.LoadData(element, this.viewAreaForm);
  }

  viewArea(): void {

    const coordinatesList: any[] = [];

    coordinatesList.push({
      typeId: 4,
      coordinates: 'Area-' + this.data.element.coordinates
    });

    const element = {
      caption: 'View Area ( ' + this.data.element.name + ')',
      fromComponent: 'viewArea',
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
