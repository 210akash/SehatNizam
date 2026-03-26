import { Component, EventEmitter, ViewChild, Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { AppointmentService } from '../appointment.service';
import { ConstantService } from '../../../../Service/constant.service';
import { AddAppointmentComponent } from '../add-appointment/add-appointment.component';

@Component({
  selector: 'app-appointment-list',
  templateUrl: './appointment-list.component.html',
  styleUrls: ['./appointment-list.component.css'],
  standalone: false
})

export class AppointmentListComponent {
  [x: string]: any;
  @Output() getAppointmentCount: EventEmitter<void> = new EventEmitter<void>();
  AppointmentFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = [];
  dataSource: any;
  take = 50;
  pageSize = 0;
  totalRows = 0;
  subcategoryList: any;
  currentUser: any;
  currenttab: any;
  History: any;
  roleList: string | undefined;
  dialogRef: any;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort

  constructor(
    private appointmentService: AppointmentService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private constantService: ConstantService
  ) { }

  async ngOnInit(): Promise<void> {
    this.pageSize = this.constantService.defaultItemPerPage;
    this.AppointmentFilterForm = this.formBuilder.group({
      code: [''],
      fdate: [],
      tdate: []
    });
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase().split(',').map((role: string) => role.trim().toLowerCase());
  }

  async bindData(appointmentFilterForm: any, currenttab: number, isFromParent: boolean): Promise<void> {

    if (isFromParent == true) {
      this.currentPage = 0;
    }

    this.currenttab = currenttab;
    if (currenttab == 0) {
      this.displayedColumns = ['code', 'createdDate', 'requiredDate', 'store', 'createdBy', 'status', 'actions'];
    }
    else if (currenttab == 1) {
      this.displayedColumns = ['code', 'processedDate', 'requiredDate', 'store', 'processedBy', 'status', 'actions'];
    }
    else if (currenttab == 2) {
      this.displayedColumns = ['code', 'approvedDate', 'requiredDate', 'store', 'approvedBy', 'status', 'actions'];
    }
    else if (currenttab == 3) {
      this.displayedColumns = ['code', 'issuedDate', 'requiredDate', 'store', 'issuedBy', 'status', 'actions'];
    }

    return new Promise<void>(async (resolve, reject) => {
      // Set loading indicator
      this.isLoading = true;
      this.AppointmentFilterForm = appointmentFilterForm;

      const pagingData = {
        currentPage: this.currentPage,
        take: this.pageSize
      };

      appointmentFilterForm["PagingData"] = pagingData;
      let fdate = new Date(appointmentFilterForm.fdate);
      let tdate = new Date(appointmentFilterForm.tdate);

      appointmentFilterForm['fdate'] = fdate.toLocaleDateString();
      appointmentFilterForm['tdate'] = tdate.toLocaleDateString();

      // Call the service method and subscribe with the observer

      (await this.appointmentService.getAllAppointments(appointmentFilterForm)).subscribe({
        next: (data: any) => {
          // Update data source for MatTable
          this.dataSource = new MatTableDataSource(data.item1);
          //this.totalRows = data.item2; // Update totalRows

          // Set up sorting
          this.dataSource.sort = this.sort;

          // If there is data, adjust paginator settings after a short delay
          if (data.item1.length > 0) {
            setTimeout(() => {
              this.paginator.pageIndex = this.currentPage;
              this.paginator.length = data.item2;
            });
          }

          // Reset loading indicator
          this.isLoading = false;
        },
        error: (error: any) => {
          // Handle errors
          console.error('Error fetching data:', error);
          this.isLoading = false;
        }
      });
    });
  }

  pageChanged(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData(this.AppointmentFilterForm, this.currenttab, false); // Re-fetch data on page change
  }

  openAppointmentDialog(element: any) {
    const dialogRef = this.dialog.open(AddAppointmentComponent, {
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData(this.AppointmentFilterForm, this.currenttab, false);
      this.getAppointmentCount.emit();
    });
  }

  filterData() {
    this.bindData(this.AppointmentFilterForm, this.currenttab, false);
  }
}