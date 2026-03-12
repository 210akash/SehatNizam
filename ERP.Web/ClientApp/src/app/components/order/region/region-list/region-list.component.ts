import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { RegionService } from '../region.service';
import { DeleteRegionComponent } from '../delete-region/delete-region.component';
import { ViewRegionComponent } from '../view-region/view-region.component';
import { CreateRegionComponent } from '../create-region/create-region.component';
import { ConstantService } from '../../../../Service/constant.service';
import { DrawMapComponent } from '../../gmap/draw-map/draw-map.component';

@Component({
  selector: 'app-region-list',
  templateUrl: './region-list.component.html',
  styleUrls: ['./region-list.component.css'],standalone: false
})

export class RegionListComponent implements OnInit {
  dataSource: any;
  regionListFilerForm!: FormGroup;
  isEditMode: boolean = false;
  displayedColumns: string[] = ['code','name', 'description', 'createdDate', 'actions'];
  isLoading = false;
  element: any;
  blob: any;

  currentPage = 0;
  pageSize = 0;
  totalRows = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];

  constructor(private constantService: ConstantService, private dialog: MatDialog, private regionService: RegionService, private formBuilder: FormBuilder) { }
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngOnInit(): void {
    this.pageSize = this.constantService.defaultItemPerPage;

    this.regionListFilerForm = this.formBuilder.group({
      name: [''],
    });

    this.bindData();
  }

  openRegionDialog(element: any): void {
    const dialogRef = this.dialog.open(CreateRegionComponent, {
      data: { element: element },
      width: '30%',
      autoFocus: true,
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
  }

  openViewRegionDialog(enterAnimationDuration: string, exitAnimationDuration: string, element: any): void {
    this.dialog.open(ViewRegionComponent, {
      data: { element: element },
      width: '30%',
      autoFocus: true,
      disableClose: true
    }),
    {
      enterAnimationDuration,
      exitAnimationDuration,
    };
  }

  async bindData() {
    this.isLoading = true;
    const pagingData = {
      currentPage: this.currentPage,
      take: this.pageSize
    }

    let _regionListFilerForm: any = {};
    _regionListFilerForm = Object.assign(_regionListFilerForm, this.regionListFilerForm.value);
    _regionListFilerForm["PagingData"] = pagingData;

    (await this.regionService.getAllRegion(_regionListFilerForm)).subscribe({
      next: (data: any) => {
        this.dataSource = new MatTableDataSource(data.item1);
        if (data.item1.length > 0) {
          setTimeout(() => {
            this.paginator.pageIndex = this.currentPage;
            this.paginator.length = data.item2;
          });
        }
        console.log(this.dataSource);
        this.isLoading = false;
      },
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  pageChanged(event: PageEvent) {
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData();
  }

  openDeleteDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteRegionComponent, {
      data: { element: element },
      width: '30%',
      autoFocus: true,
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
  }

  viewRegion(element: any): void {
    const coordinatesList: any[] = [];
    coordinatesList.push({
      typeId: 3,
      coordinates: element.coordinates,
      name: 'Region-' + element.name,
    });

    const elementToSend = {
      caption: 'View Region ( ' + element.name + ')',
      fromComponent: 'viewRegion',
      drawingPolygon: false,
      drawingMarker: false,
      coordinates: coordinatesList,
      regionDescription: element.description,
      isFocusDrawPolygon: true,
      isShowZoneCaption: true,
      isShowInfoBox: true
    };

    const dialogRef = this.dialog.open(DrawMapComponent, {
      width: '95%',
      height: '88vh',
      data: {
        element: elementToSend,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {

    });
  }

  filterData() {
    this.bindData();
  }

  onReset() {
    this.regionListFilerForm.patchValue({
      name: ''
    });
    this.bindData();
  }


}
