import { Component, EventEmitter, ViewChild, Output } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { SafeHtml } from '@angular/platform-browser';
import { ConstantService } from '../../../../Service/constant.service';
import { Router } from '@angular/router';
import { RosterService } from '../../roster/roster.service';
import { ViewRosterComponent } from '../../roster/view-roster/view-roster.component';
import { DeleteRosterComponent } from '../../roster/delete-roster/delete-roster.component';
import { PrintRosterComponent } from '../../roster/print-roster/print-roster.component';
import { ProcessRosterComponent } from '../../roster/process-roster/process-roster.component';
@Component({
  selector: 'app-roster-department-list',
  templateUrl: './roster-department-list.component.html',
  styleUrls: ['./roster-department-list.component.css'],
  standalone: false
})

export class RosterDepartmentListComponent {
  [x: string]: any;
  @Output() getRosterCount: EventEmitter<void> = new EventEmitter<void>();
  RosterFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns = ['month', 'year','department','createdDate',  'createdBy', 'actions'];
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
   years: number[] = [];
  months = [
    { value: 1, label: 'January' },
    { value: 2, label: 'February' },
    { value: 3, label: 'March' },
    { value: 4, label: 'April' },
    { value: 5, label: 'May' },
    { value: 6, label: 'June' },
    { value: 7, label: 'July' },
    { value: 8, label: 'August' },
    { value: 9, label: 'September' },
    { value: 10, label: 'October' },
    { value: 11, label: 'November' },
    { value: 12, label: 'December' },
  ];
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort

  constructor(
    private router: Router, 
    private rosterService: RosterService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private constantService: ConstantService
  ) { }

  async ngOnInit(): Promise<void> {
    this.pageSize = this.constantService.defaultItemPerPage;
    this.RosterFilterForm = this.formBuilder.group({
       year: [2026],
      month: [5],
      statusId: [1]
    });
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase().split(',').map((role: string) => role.trim().toLowerCase());
    this.buildYears();
    this.bindData();
  }


  buildYears(): void {
    const current = new Date().getFullYear();
    for (let y = current; y <= current + 1; y++) {
      this.years.push(y);
    }
  }


  async bindData(): Promise<void> {

    return new Promise<void>(async (resolve, reject) => {
      this.isLoading = true;

    // Prepare paging data
    const pagingData = {
      currentPage: this.currentPage,
      take: this.take
    };

    // Clone the form value and add paging data
    const _RosterFilterForm = {
      ...this.RosterFilterForm.value,
      PagingData: pagingData
    };


      // Call the service method and subscribe with the observer

      (await this.rosterService.getAllRostersByManager(_RosterFilterForm)).subscribe({
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

  resetForm() {
    this.RosterFilterForm.reset({
      // code: "",
      // fdate: new Date(),
      // tdate: new Date(),
    });

    this.filterData();
  }

  pageChanged(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData(); // Re-fetch data on page change
  }

    openRosterDialog(element: any) {
      // Open the appointment form as a full page instead of a dialog.
      const navigationExtras = element ? { state: { element } } : undefined;
      this.router.navigate(['/adddepartmentroster'], navigationExtras);
    }

  viewRosterDialog(element: any): void {
    this.dialog.open(ViewRosterComponent, {
      data: { element: element },
      panelClass: 'cstm_width_1200',
      maxHeight: '90vh',
      disableClose: true
    });
  }

  deleteRosterDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteRosterComponent, {
      panelClass: 'cstm_width_1200',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
      this.getRosterCount.emit();
    });
  }

  processRosterDialog(element: any) {
    const dialogRef = this.dialog.open(ProcessRosterComponent, {
      panelClass: 'cstm_width_1200',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
      this.getRosterCount.emit();
    });
  }

  printRosterDialog(element: any) {
    const dialogRef = this.dialog.open(PrintRosterComponent, {
      panelClass: 'cstm_width_1200',
      maxHeight: '90vh',
      data: {
        element: element,
      },
      disableClose: true
    });
  }

  filterData() {
    this.bindData();
  }

getMonthLabel(value: number): string {
  return this.months.find(m => m.value === value)?.label || '';
}


}