import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { DeleteDeviceComponent } from '../delete-device/delete-device.component';
import { ViewDeviceComponent } from '../view-device/view-device.component';
import { CreateDeviceComponent } from '../create-device/create-device.component';
import { DealershipService } from '../../order/dealership/dealership.service';
import { ConstantService } from '../../../Service/constant.service';
import { DeviceService } from '../device.service';

@Component({
  selector: 'app-device-list',
  templateUrl: './device-list.component.html',
  styleUrls: ['./device-list.component.css'],
  standalone: false,
})
export class DeviceListComponent implements OnInit {
  dataSource: any;
  deviceListFilerForm!: FormGroup;
  isEditMode: boolean = false;
  displayedColumns: string[] = [
    'name',
    'ipAddress',
    'port',
    'createdBy',
    'isActive',
    'actions',
  ];
  isLoading = false;
  element: any;
  blob: any;

  isActive: boolean = false;
  currentPage = 0;
  pageSize = 0;
  totalRows = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];

  regionList: any[] = [];
  zoneList: any[] = [];
  areaList: any[] = [];
  territoryList: any[] = [];

  constructor(
    private constantService: ConstantService,
    private dialog: MatDialog,
    private deviceService: DeviceService,
    private formBuilder: FormBuilder  ) {}
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngOnInit(): void {
    this.pageSize = this.constantService.defaultItemPerPage;

    this.deviceListFilerForm = this.formBuilder.group({
      name: [''],
      isActive: [null],
    });

    this.bindData();
  }

  filterData1(event: any): void {
    // The `event.checked` will be `true` if the toggle is on, otherwise `false`
    this.isActive = event.checked;

    // Call any function or perform the action you need, passing the `isActive` value
    this.bindData();
  }

  openDeviceDialog(element: any): void {
    const dialogRef = this.dialog.open(CreateDeviceComponent, {
      data: { element: element },
      width: '60%',
      maxHeight: '95vh',
      autoFocus: true,
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe((result) => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
  }

  openViewDeviceDialog(element: any): void {
    const dialogRef = this.dialog.open(ViewDeviceComponent, {
      data: { element: element },
      width: '60%',
      maxHeight: '95vh',
      autoFocus: true,
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe((result) => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
  }

  async bindData() {
    this.isLoading = true;
    const pagingData = {
      currentPage: this.currentPage,
      take: this.pageSize,
    };

    let _deviceListFilerForm: any = {};
    _deviceListFilerForm = Object.assign(
      _deviceListFilerForm,
      this.deviceListFilerForm.value
    );

    _deviceListFilerForm['PagingData'] = pagingData;

    (
      await this.deviceService.getAllDevices(_deviceListFilerForm)
    ).subscribe({
      next: (data) => {
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
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      },
    });
  }

  pageChanged(event: PageEvent) {
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData();
  }

  openDeleteDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteDeviceComponent, {
      data: { element: element },
      width: '60%',
      autoFocus: true,
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe((result) => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
  }

    syncAttendance(element: any) {
    const dialogRef = this.dialog.open(DeleteDeviceComponent, {
      data: { element: element },
      width: '60%',
      autoFocus: true,
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe((result) => {
      this.bindData();
      console.log(`Dialog result: ${result}`);
    });
  }


  onReset() {
    this.deviceListFilerForm.patchValue({
      name: '',
      isActive: null,
    });
    this.bindData();
  }

  filterData() {
    this.bindData();
  }
}
