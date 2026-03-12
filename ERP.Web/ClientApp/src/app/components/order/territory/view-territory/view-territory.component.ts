import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';
import { DrawMapComponent } from '../../gmap/draw-map/draw-map.component';

@Component({
  selector: 'app-view-territory',
  templateUrl: './view-territory.component.html',
  styleUrls: ['./view-territory.component.css'],standalone: false
})

export class ViewTerritoryComponent implements OnInit {
  viewTerritoryForm!: FormGroup;
  isLoading = false;
  dataSource!: any;
  roleList: any;
  constructor(private dialog: MatDialog, private formBuilder: FormBuilder, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.viewTerritoryForm = this.formBuilder.group({
      id: [0],
      name: [''],
      description: [''],
      coordinates: [''],
      region: [''],
      zone: [''],
      area: ['']
    });

    this.LoadData(this.data.element);
    this.viewTerritoryForm.get('region')?.patchValue(this.data.element.area?.zone?.region?.name);
    this.viewTerritoryForm.get('zone')?.patchValue(this.data.element.area?.zone?.name);
    this.viewTerritoryForm.get('area')?.patchValue(this.data.element.area?.name);
  }

  LoadData(element: any) {
    this.constantService.LoadData(element, this.viewTerritoryForm);
  }

  viewTerritory(): void {

    const coordinatesList: any[] = [];

    coordinatesList.push({
      typeId: 1,
      coordinates: this.data.element.area?.zone?.coordinates,
      name: 'Zone-' + this.data.element.area?.zone?.name,
    });

    coordinatesList.push({
      typeId: 2,
      coordinates: this.data.element.coordinates,
      name: 'Territory-' + this.data.element.name,
    });

    const element = {
      caption: 'Territory: ' + this.data.element.name + ' - Zone: '+ this.data.element.area?.zone?.name ,
      fromComponent: 'viewZone',
      drawingPolygon: false,
      drawingMarker: false,
      coordinates: coordinatesList,
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
}
