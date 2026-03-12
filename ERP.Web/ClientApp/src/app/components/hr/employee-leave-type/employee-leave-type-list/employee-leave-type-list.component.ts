import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { ViewEmployeeLeaveTypeComponent } from '../view-employee-leave-type/view-employee-leave-type.component';
import { EmployeeLeaveTypeService } from '../employee-leave-type.service';
import { AddEmployeeLeaveTypeComponent } from '../add-employee-leave-type/add-employee-leave-type.component';
import { DeleteEmployeeLeaveTypeComponent } from '../delete-employee-leave-type/delete-employee-leave-type.component';

@Component({
    selector: 'app-employee-leave-type-list',
    templateUrl: './employee-leave-type-list.component.html',
    styleUrls: ['./employee-leave-type-list.component.css'],
    standalone: false
})

export class EmployeeLeaveTypeListComponent {
  EmployeeLeaveTypeFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSize = 10;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = ['code','name', 'createdBy', 'actions'];
  dataSource: any;
  take = 0;
  totalRows = 0;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort

  constructor(
    private employeeLeaveTypeService: EmployeeLeaveTypeService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder
  ) { }

  async ngOnInit(): Promise<void> {
    this.EmployeeLeaveTypeFilterForm = this.formBuilder.group({});
    await this.bindData(); // Await bindData if it's async
  }

  async bindData(): Promise<void> {
    // Set loading indicator
    this.isLoading = true;

    // Prepare paging data
    const pagingData = {
      currentPage: this.currentPage,
      take: this.pageSize
    };

    // Clone the form value and add paging data
    const _EmployeeLeaveTypeFilterForm = {
      ...this.EmployeeLeaveTypeFilterForm.value,
      PagingData: pagingData
    };

    // Call the service method and subscribe with the observer
    this.employeeLeaveTypeService.getAllEmployeeLeaveTypes(_EmployeeLeaveTypeFilterForm).subscribe({
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
  }

  pageChanged(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData(); // Re-fetch data on page change
  }

  openEmployeeLeaveTypeDialog(element: any) {
    const dialogRef = this.dialog.open(AddEmployeeLeaveTypeComponent, {
      panelClass: 'cstm_width_500',
      height: 'auto',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
    });
  }

  viewEmployeeLeaveTypeDialog(element: any): void {
    this.dialog.open(ViewEmployeeLeaveTypeComponent, {
      data: { element: element },
     panelClass: 'cstm_width_500',
     height: 'auto',
      disableClose: true
    });
  }

  deleteEmployeeLeaveTypeDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteEmployeeLeaveTypeComponent, {
      panelClass: 'cstm_width_500',
      height: 'auto',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
    });
  }


}
