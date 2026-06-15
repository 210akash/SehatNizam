import { Component, EventEmitter, ViewChild, Output } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { AppointmentService } from '../appointment.service';
import { ConstantService } from '../../../../Service/constant.service';
import { Router } from '@angular/router';
import { PrintAppoinmentComponent } from '../print-appoinment/print-appoinment.component';
import { ConfirmAppointmentComponent } from '../confirm-appointment/confirm-appointment.component';
import { MatDialog } from '@angular/material/dialog';
import { PrintReceiptAppoinmentComponent } from '../print-receipt-appoinment/print-receipt-appoinment.component';
import { DepartmentService } from '../../../department/department.service';

@Component({
  selector: 'app-book-appointment-list',
  templateUrl: './book-appointment-list.component.html',
  styleUrls: ['./book-appointment-list.component.css'],
  standalone: false
})

export class BookAppointmentListComponent {
  [x: string]: any;
  @Output() getAppointmentCount: EventEmitter<void> = new EventEmitter<void>();
  AppointmentFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = [
    'appointmentDate',
    'patient',
    'bookingNumber',
    'doctor',
    'department',
    'priority',
    'appointmentType',
    'visitType',
    'referrer',
    'status',
    'actions'
  ];
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
  departments : any;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort

  constructor(
    private appointmentService: AppointmentService,
    private formBuilder: FormBuilder,
    private constantService: ConstantService,
        private departmentService: DepartmentService,
    private router: Router,
    private dialog: MatDialog,
  ) { }

  async ngOnInit(): Promise<void> {
    this.pageSize = this.constantService.defaultItemPerPage;
    this.AppointmentFilterForm = this.formBuilder.group({
      id: [null],
      bookingFormType : [1],
      mRN: [''],
      patientName: [''],
      fDate: [new Date()],
      tDate: [new Date()],
      departmentId : [null],
      statusId : [1]
    });
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase().split(',').map((role: string) => role.trim().toLowerCase());
    this.loadDepartments();
    this.bindData();
  }

 private loadDepartments(): void {
    this.departmentService.getClinicalDepartment().subscribe({
      next: (res: any) => {
        this.departments = res?.item1 ?? res ?? [];
      },
      error: () => {
        // Fallback: keep an empty list; UI will show required validation
        this.departments = [];
      }
    });
  }

  async bindData(): Promise<void> {
    return new Promise<void>(async (resolve, reject) => {
        // Clone the form value and add paging data
          const pagingData = {
        currentPage: this.currentPage,
        take: this.pageSize
      };
    const appointmentFilterForm = {
      ...this.AppointmentFilterForm.value
    };
      // Set loading indicator
      this.isLoading = true;
      appointmentFilterForm["PagingData"] = pagingData;
      // let fdate = new Date(appointmentFilterForm.fdate);
      // let tdate = new Date(appointmentFilterForm.tdate);

      // appointmentFilterForm['fdate'] = fdate.toLocaleDateString();
      // appointmentFilterForm['tdate'] = tdate.toLocaleDateString();

      // Call the service method and subscribe with the observer
      appointmentFilterForm['fDate'] = this.constantService.formatDate(appointmentFilterForm['fDate']);
      appointmentFilterForm['tDate'] = this.constantService.formatDate(appointmentFilterForm['tDate']);

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
    this.bindData(); // Re-fetch data on page change
  }

  openAppointmentDialog(element: any) {
    const navigationExtras = {
      queryParams: { appointmentStatusId: 1 },
      state: element ? { element } : {}
    };
    this.router.navigate(['/booknewappointment'], navigationExtras);
  }

   printAppoinmnetDialog(element: any) {
    const dialogRef = this.dialog.open(PrintAppoinmentComponent, {
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });
  }
  
   printrecreiptAppoinmnetDialog(element: any) {
    const dialogRef = this.dialog.open(PrintReceiptAppoinmentComponent, {
      panelClass: 'cstm_width_400',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });
  }

  openConfirmDialog(element: any) {
    const dialogRef = this.dialog.open(ConfirmAppointmentComponent, {
      maxWidth: '560px',
      disableClose: true,
      data: {
        element: element,
      }
    });

    dialogRef.afterClosed().subscribe((confirmed: boolean) => {
      if (confirmed) {
        this.bindData();
      }
    });
  }

  filterData() {
    this.bindData();
  }

}
