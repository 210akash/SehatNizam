import { Component, EventEmitter, ViewChild, Output } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { ConstantService } from '../../../../Service/constant.service';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { DepartmentService } from '../../../department/department.service';
import { AdmissionService } from '../admission.service';
import { AppointmentService } from '../../../opd/appointment/appointment.service';
import { PrintReceiptAdmissionComponent } from '../print-receipt-admission/print-receipt-admission.component';
import { AddAdmissionBedComponent } from '../../admissionbed/add-admissionbed/add-admissionbed.component';
import { AdmissionBedListComponent } from '../../admissionbed/admissionbed-list/admissionbed-list.component';
import { AdmissionServiceListComponent } from '../../admissionservice/admissionservice-list/admissionservice-list.component';
import { AddDischargeComponent } from '../../discharge/discharge.component';

@Component({
  selector: 'app-admission-list',
  templateUrl: './admission-list.component.html',
  styleUrls: ['./admission-list.component.css'],
  standalone: false
})

export class AdmissionListComponent {
  [x: string]: any;
  @Output() getAppointmentCount: EventEmitter<void> = new EventEmitter<void>();
  AdmissionFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = [
    'admissionDate',
    'patient',
    'mrn',
    'tokenNumber',
    'doctor',
    'department',
    'referrer',
    'bed',
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
    private admissionService: AdmissionService,
    private formBuilder: FormBuilder,
    private constantService: ConstantService,
   private departmentService: DepartmentService,
    private router: Router,
    private dialog: MatDialog,
  ) { }

  async ngOnInit(): Promise<void> {
    this.pageSize = this.constantService.defaultItemPerPage;
    this.AdmissionFilterForm = this.formBuilder.group({
      id:[null],
      bookingFormType : [5],
      tokenNo: [''],
      mRN: [''],
      patientName: [''],
      fDate: [new Date()],
      tDate: [new Date()],
      departmentId : [null],
      statusId : [null]
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
      ...this.AdmissionFilterForm.value
    };
      // Set loading indicator
      this.isLoading = true;
      appointmentFilterForm["PagingData"] = pagingData;
      // Call the service method and subscribe with the observer
      appointmentFilterForm['fDate'] = this.constantService.formatDate(appointmentFilterForm['fDate']);
      appointmentFilterForm['tDate'] = this.constantService.formatDate(appointmentFilterForm['tDate']);
      (await this.admissionService.getAllAdmissions(appointmentFilterForm)).subscribe({
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

  openAdmissionDialog(element: any) {
    this.router.navigate(['/addadmission']);
  }

  //  printAppoinmnetDialog(element: any) {
  //   const dialogRef = this.dialog.open(PrintAppoinmentComponent, {
  //     panelClass: 'cstm_width_1100',
  //     maxHeight: '90vh',
  //     data: {
  //       element: element,
  //     },
  //     disableClose: true
  //   });
  // }
  
   printrecreiptAdmissionDialog(element: any) {
    const dialogRef = this.dialog.open(PrintReceiptAdmissionComponent, {
      panelClass: 'cstm_width_400',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });
  }


 openAddDischargeServiceDialog(element: any) {
    const dialogRef = this.dialog.open(AddDischargeComponent, {
      id: 'message-Insurance',
      width: '50%',
      maxHeight: '800px',
      height: 'auto',
      data: {
        element: element
      },
      disableClose: true
    });
    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
    });
  }


 openAddAdmissionServiceDialog(element: any) {
    const dialogRef = this.dialog.open(AdmissionServiceListComponent, {
      id: 'message-Insurance',
      width: '50%',
      maxHeight: '800px',
      height: 'auto',
      data: {
        element: element
      },
      disableClose: true
    });
    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
    });
  }

  openAddAdmissionBedDialog(element: any) {
    const dialogRef = this.dialog.open(AdmissionBedListComponent, {
      id: 'message-Insurance',
      width: '50%',
      maxHeight: '800px',
      height: 'auto',
      data: {
        element: element
      },
      disableClose: true
    });
    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
    });
  }

  // openConfirmDialog(element: any) {
  //   const dialogRef = this.dialog.open(ConfirmAppointmentComponent, {
  //     maxWidth: '560px',
  //     disableClose: true,
  //     data: {
  //       element: element,
  //     }
  //   });

  //   dialogRef.afterClosed().subscribe((confirmed: boolean) => {
  //     if (confirmed) {
  //       this.bindData();
  //     }
  //   });
  // }

  filterData() {
    this.bindData();
  }

  formatBed(element: any): string {
  if (!element.admissionBeds || element.admissionBeds.length === 0) {
    return 'N/A';
  }

  const bedInfo = element.admissionBeds[0]?.bed;
  const ward = bedInfo?.room?.ward?.name;
  const room = bedInfo?.room?.name;
  const bedNo = bedInfo?.bedNo;

  const parts = [];
  if (ward) parts.push(ward);
  if (room) parts.push(room);
  if (bedNo) parts.push(bedNo);

  return parts.length > 0 ? parts.join(' > ') : 'N/A';
}

}
