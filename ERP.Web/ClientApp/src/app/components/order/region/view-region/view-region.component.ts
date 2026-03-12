import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';
import { DrawMapComponent } from '../../gmap/draw-map/draw-map.component';

@Component({
  selector: 'app-view-region',
  templateUrl: './view-region.component.html',
  styleUrls: ['./view-region.component.css'],standalone: false
})

export class ViewRegionComponent implements OnInit {
  viewRegionForm!: FormGroup;
  isLoading = false;
  dataSource!: any;
  roleList: any;

  constructor(private dialog: MatDialog, private formBuilder: FormBuilder, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.viewRegionForm = this.formBuilder.group({
      id: [0],
      name: [''],
      description: [''],
      coordinates: ['']
    });

    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    this.constantService.LoadData(element, this.viewRegionForm);
  }

  viewRegion(): void {

    const coordinatesList: any[] = [];

    coordinatesList.push({
      typeId: 3,
      coordinates: 'Region-' + this.data.element.coordinates
    });

    const element = {
      caption: 'View Region ( ' + this.data.element.name + ')',
      fromComponent: 'viewRegion',
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
