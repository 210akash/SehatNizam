import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { ViewDSFComponent } from '../view-DSF/view-DSF.component';
import { AddDSFRouteComponent } from '../add-DSF-route/add-DSF-route.component';
import { ConstantService } from '../../../../Service/constant.service';
import { AreaService } from '../../area/area.service';
import { RegionService } from '../../region/region.service';
import { TerritoryService } from '../../territory/territory.service';
import { ZoneService } from '../../zone/zone.service';
import { DSFService } from '../DSF.service';

@Component({
  selector: 'app-DSF-list',
  templateUrl: './DSF-list.component.html',
  styleUrls: ['./DSF-list.component.css'], standalone: false
})

export class DSFListComponent implements OnInit {
  dataSource: any;
  dSFListFilerForm!: FormGroup;
  isEditMode: boolean = false;
  displayedColumns: string[] = ['name', 'phoneNo', 'email', 'dob', 'role', 'shiftTimeStart', 'shiftTimeEnd', 'createdDate', 'actions'];
  isLoading = false;
  element: any;
  blob: any;

  currentPage = 0;
  pageSize = 0;
  totalRows = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];

  regionList: any[] = [];
  zoneList: any[] = [];
  areaList: any[] = [];
  territoryList: any[] = [];

  constructor(private constantService: ConstantService, private dialog: MatDialog, private dSFService: DSFService, private formBuilder: FormBuilder, private zoneService: ZoneService,
    private territoryService: TerritoryService, private regionService: RegionService, private areaService: AreaService) { }
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngOnInit(): void {
    this.pageSize = this.constantService.defaultItemPerPage;

    this.dSFListFilerForm = this.formBuilder.group({
      regionId: [0],
      zoneId: [0],
      areaId: [0],
      territoryId: [0],
    });

    this.bindData();
    this.getRegions();
  }

  openViewDSFDialog(enterAnimationDuration: string, exitAnimationDuration: string, element: any): void {
    this.dialog.open(ViewDSFComponent, {
      data: { element: element },
      width: '40%',
      maxHeight: '90vh',
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

    let _dSFListFilerForm: any = {};
    _dSFListFilerForm = Object.assign(_dSFListFilerForm, this.dSFListFilerForm.value);
    _dSFListFilerForm["PagingData"] = pagingData;

    (await this.dSFService.getAll(_dSFListFilerForm)).subscribe({
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

  openAddDSFRouteDialog(element: any) {
    const dialogRef = this.dialog.open(AddDSFRouteComponent, {
      data: { element: element },
      width: '50%',
      maxHeight: '95vh',
      autoFocus: true,
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
  }

  onReset() {
    this.dSFListFilerForm.patchValue({
      regionId: 0,
      zoneId: 0,
      areaId: 0,
      territoryId: 0,
    });
    this.bindData();
  }

  filterData() {
    this.bindData();
  }

  // onZoneChange() {
  //   this.dSFListFilerForm.get('territoryId')?.patchValue(0);
  //   let zoneId = this.dSFListFilerForm.get('zoneId').value;

  //   this.getTerritoryByAreaId(zoneId);
  //   this.filterData();
  // }

  // async getTerritoryByAreaId(zoneId: any) {
  //   (await this.territoryService.getTerritoryByAreaId(zoneId)).subscribe({
  //     next: (data) => {
  //       if (data && Array.isArray(data)) {
  //         this.territoryList = data;
  //       } else {
  //         console.error('Expected array but got:', data);
  //         this.territoryList = [];
  //       }
  //     },
  //     error: (error) => {
  //       console.log(error);
  //       this.isLoading = false;
  //     }
  //   });
  // }

  // async getZones() {
  //   let _zoneFilterForm = {};
  //   (await this.zoneService.getAllZone(_zoneFilterForm)).subscribe({
  //     next: (data) => {
  //       if (data && Array.isArray(data.item1)) {
  //         this.zoneList = data.item1;
  //       } else {
  //         console.error('Expected array but got:', data.item1);
  //         this.zoneList = [];
  //       }
  //     },
  //     error: (error) => {
  //       console.log(error);
  //       this.isLoading = false;
  //     }
  //   });
  // }

  async getTerritoryByAreaId() {

    this.territoryList = [];

    this.dSFListFilerForm.get('territoryId')?.patchValue(0);
    (await this.territoryService.getTerritoryByAreaId(this.dSFListFilerForm.get('areaId')?.value)).subscribe(
      {
        next: (data) => {
          this.territoryList = data;
        },
        error: (error) => {
          console.log(error);
          this.isLoading = false;
        }
      });
  }

  async getZoneByRegionId() {

    this.zoneList = [];
    this.areaList = [];
    this.territoryList = [];

    (await this.zoneService.getZoneByRegionId(this.dSFListFilerForm.get('regionId')?.value)).subscribe({
      next: (data) => {
        this.zoneList = data;
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

    (await this.areaService.getAreaByZoneId(this.dSFListFilerForm.get('zoneId')?.value)).subscribe({
      next: (data) => {
        this.areaList = data;
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }


}
