import { Component, EventEmitter, ViewChild, Output } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { AppointmentService } from '../appointment.service';
import { ConstantService } from '../../../../Service/constant.service';
import { Router } from '@angular/router';

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
  displayedColumns: string[] = [
    'appointmentDate',
    'patient',
    'tokenNumber',
    'doctor',
    'department',
    'priority',
    'appointmentType',
    'visitType',
    'reason',
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
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort

  constructor(
    private appointmentService: AppointmentService,
    private formBuilder: FormBuilder,
    private constantService: ConstantService,
    private router: Router
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
    this.bindData();
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
    this.bindData(); // Re-fetch data on page change
  }

  openAppointmentDialog(element: any) {
    // Open the appointment form as a full page instead of a dialog.
    const navigationExtras = element ? { state: { element } } : undefined;
    this.router.navigate(['/newappointment'], navigationExtras);
  }

  filterData() {
    this.bindData();
  }

}
