import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { UserAttendanceService } from '../user-attendance.service';
import { ConstantService } from '../../../../Service/constant.service';
import { ViewUserattendanceComponent } from '../view-user-attendance/view-user-attendance.component';
import { DrawMapComponent } from '../../gmap/draw-map/draw-map.component';
import { UserService } from '../../../user-management/user.service';

@Component({
  selector: 'app-user-attendance-list',
  templateUrl: './user-attendance-list.component.html',
  styleUrls: ['./user-attendance-list.component.css'],standalone: false
})

export class UserAttendanceListComponent implements OnInit {
  dataSource: any;
  userAttendanceListFilerForm!: FormGroup;
  isEditMode: boolean = false;
  displayedColumns: string[] = ['region', 'zone', 'area' ,'territory', 'distributor', 'user', 'createdDate', 'isPresent','checkIn','checkout','workingTime', 'actions'];
  isLoading = false;
  element: any;

  blob: any;

  currentPage = 0;
  pageSize = 0;
  totalRows = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];

  rolesList: any;
  selectedRolls: any;

  constructor(private constantService: ConstantService, private dialog: MatDialog, private userService: UserService,
    private userAttendanceService: UserAttendanceService, private formBuilder: FormBuilder) { }
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngOnInit(): void {
    this.pageSize = this.constantService.defaultItemPerPage;

    this.userAttendanceListFilerForm = this.formBuilder.group({
      fdate: [new Date()],
      tdate: [new Date()],
      roleId: [''],
    });

    const today = new Date(); // today date
    const lastWeek = new Date();
    lastWeek.setDate(today.getDate() - 7);
    this.userAttendanceListFilerForm.get('fdate')?.patchValue(this.constantService.formatDate(lastWeek));
    this.userAttendanceListFilerForm.get('tdate')?.patchValue(this.constantService.formatDate(today));

    this.getRolesList();
    this.bindData();
  }
  // Function to format dates (helper)
  formatDate(date: Date): string {
    const year = date.getFullYear();
    const month = (date.getMonth() + 1).toString().padStart(2, '0'); // Months are zero-based
    const day = date.getDate().toString().padStart(2, '0');
    return `${year}-${month}-${day}`;
  }
  // Function to trigger download (by making an HTTP request to the URL)
  downloadFile(url: string): void {
    const request = new XMLHttpRequest();
    request.open('GET', url, true);
    request.responseType = 'blob';  // Indicates that the response should be a binary blob
    request.setRequestHeader('Accept', 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'); // Optional, for Excel format

    // On successful download, create a link to trigger the download
    request.onload = () => {
      if (request.status === 200) {
        const blob = request.response;
        const link = document.createElement('a');
        const url = window.URL.createObjectURL(blob);
        link.href = url;
        link.download = 'UserAttendanceReport.xlsx'; // Set the filename for the downloaded file
        link.click();
        window.URL.revokeObjectURL(url); // Clean up the URL object
      } else {
        console.error("Error while downloading the file:", request.statusText);
      }
    };

    // Handle any error
    request.onerror = () => {
      console.error("Error in the network request");
    };

    request.send();  // Send the request
  }
  getWorkingTime(attendanceDate: string | Date, checkOut: string | Date): string | null {
    if (!attendanceDate || !checkOut) return null;
  
    const inTime = new Date(attendanceDate);
    const outTime = new Date(checkOut);
    const diffMs = outTime.getTime() - inTime.getTime();
  
    if (diffMs <= 0) return null;
  
    const diffMinutes = Math.round(diffMs / 60000);
    const hours = Math.floor(diffMinutes / 60);
    const minutes = diffMinutes % 60;
  
    return `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}`;
  }
  
  
  async downloadAttReportExcel() {
    this.isLoading = true;
    let _dashboardFilterForm: any = {};
    _dashboardFilterForm = Object.assign(_dashboardFilterForm, this.userAttendanceListFilerForm.value);

    const formattedFDate = _dashboardFilterForm.fdate;
    const formattedTDate = _dashboardFilterForm.tdate;
    this.userAttendanceService.getReportDemo(formattedFDate,formattedTDate).subscribe(
      (response) => {
        const blob = new Blob([response], { type: 'text/csv' }); // Use text/csv MIME type
        const url = window.URL.createObjectURL(blob);

        // Trigger file download
        const a = document.createElement('a');
        a.href = url;
        a.download = 'UserAttendance.csv'; // Set desired CSV file name
        a.click();

        // Cleanup
        window.URL.revokeObjectURL(url);
      },
      (error) => {
        console.error('Error downloading the report:', error);
      }
    );

  }


  // openUserAttendanceDialog(element: any): void {
  //   const dialogRef = this.dialog.open(CreateUserAttendanceComponent, {
  //     data: { element: element },
  //     width: '30%',
  //     autoFocus: true,
  //     disableClose: true
  //   });

  //   dialogRef.afterClosed().subscribe(result => {
  //     this.bindData();
  //     console.log(`Dialog result: ${result}`);
  //   });
  // }

  filterData() {
    this.isLoading = true;
    let _dashboardFilterForm: any = {};
    _dashboardFilterForm = Object.assign(_dashboardFilterForm, this.userAttendanceListFilerForm.value);
    this.bindData();
    this.isLoading = false;

  }

  openViewUserAttendanceDialog(enterAnimationDuration: string, exitAnimationDuration: string, element: any): void {
    this.dialog.open(ViewUserattendanceComponent, {
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

  viewPinLocation(userAttendanceRowData: any): void {

    const markerPinsList: any[] = [];

    markerPinsList.push({
      typeId: 2,
      pinLocation: userAttendanceRowData.pinLocation
    });

    const createdDate = new Date(userAttendanceRowData.attendanceDate);

    const formattedDate = createdDate.toLocaleString('en-US', {
      day: '2-digit',
      month: 'short',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      hour12: true,
    });

    const element = {
      caption: userAttendanceRowData.user?.firstName + ' ' + userAttendanceRowData.user?.lastName + ' (' + formattedDate + ')',
      fromComponent: 'viewZone',
      drawingPolygon: false,
      drawingMarker: false,
      markerPins: markerPinsList,
      isFocusDrawPolygon: true,
      isShowInfoBox: false
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

  viewCheckOutPinLocation(userAttendanceRowData: any): void {

    const markerPinsList: any[] = [];

    markerPinsList.push({
      typeId: 2,
      pinLocation: userAttendanceRowData.checkOutLocation
    });

    const createdDate = new Date(userAttendanceRowData.attendanceDate);

    const formattedDate = createdDate.toLocaleString('en-US', {
      day: '2-digit',
      month: 'short',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      hour12: true,
    });

    const element = {
      caption: userAttendanceRowData.user?.firstName + ' ' + userAttendanceRowData.user?.lastName + ' (' + formattedDate + ')',
      fromComponent: 'viewZone',
      drawingPolygon: false,
      drawingMarker: false,
      markerPins: markerPinsList,
      isFocusDrawPolygon: true,
      isShowInfoBox: false
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


  async bindData() {
    this.isLoading = true;
    const pagingData = {
      currentPage: this.currentPage,
      take: this.pageSize
    }

    let _userAttendanceListFilerForm: any = {};
    _userAttendanceListFilerForm = Object.assign(_userAttendanceListFilerForm, this.userAttendanceListFilerForm.value);
    _userAttendanceListFilerForm["PagingData"] = pagingData;

    (await this.userAttendanceService.getAllUserAttendance(_userAttendanceListFilerForm)).subscribe({
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
      }
    });
  }

  pageChanged(event: PageEvent) {
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData();
  }

  // openDeleteDialog(element: any) {
  //   const dialogRef = this.dialog.open(DeleteUserAttendanceComponent, {
  //     data: { element: element },
  //     width: '30%',
  //     autoFocus: true,
  //     disableClose: true,
  //   });

  //   dialogRef.afterClosed().subscribe(result => {
  //     this.bindData();
  //     console.log(`Dialog result: ${result}`);
  //   });
  // }

  async getRolesList(): Promise<void> {
    try {
      const data: any = await this.userService.getAllRolesByDepartment(12).toPromise(); // Convert Observable to Promise using toPromise()
      this.rolesList = data;

    } catch (error) {
      console.error('Error fetching roles:', error);
    }
  }


}
