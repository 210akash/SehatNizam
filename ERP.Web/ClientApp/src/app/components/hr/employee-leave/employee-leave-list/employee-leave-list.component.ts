import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { ViewEmployeeLeaveComponent } from '../view-employee-leave/view-employee-leave.component';
import { EmployeeLeaveService } from '../employee-leave.service';
import { AddEmployeeLeaveComponent } from '../add-employee-leave/add-employee-leave.component';
import { DeleteEmployeeLeaveComponent } from '../delete-employee-leave/delete-employee-leave.component';
import { ProcessEmployeeLeaveComponent } from '../process-employee-leave/process-employee-leave.component';
import { ConstantService } from '../../../../Service/constant.service';

@Component({
  selector: 'app-employee-leave-list',
  templateUrl: './employee-leave-list.component.html',
  styleUrls: ['./employee-leave-list.component.css'],
  standalone: false
})

export class EmployeeLeaveListComponent {
  employeeLeaveFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSize = 0;
  pageSizeOptions: number[] = [5, 10, 25, 50, 100];
  displayedColumns: string[] = ['leaveType', 'startDate', 'endDate', 'day', 'status', 'reason', 'comments', 'createdDate', 'actions'];
  dataSource: any;
  take = 50;
  totalRows = 0;
  leaveStatusList: any;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort

  constructor(
    private employeeLeaveService: EmployeeLeaveService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private constantService: ConstantService
  ) { }

  async ngOnInit(): Promise<void> {
    this.employeeLeaveFilterForm = this.formBuilder.group({
      fdate: [new Date()],
      tdate: [new Date()],
      statusId: [0],
    });

    this.employeeLeaveFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(new Date().setDate(new Date().getDate() - 30)));
    this.employeeLeaveFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(new Date()));
    await this.bindData(); // Await bindData if it's async
  }

  async bindData(): Promise<void> {
    // Set loading indicator
    this.isLoading = true;

    // Prepare paging data
    const pagingData = {
      currentPage: this.currentPage,
      take: this.take
    };

    // Clone the form value and add paging data
    const _EmployeeLeaveFilterForm = {
      ...this.employeeLeaveFilterForm.value,
      PagingData: pagingData
    };

    // Call the service method and subscribe with the observer
    this.employeeLeaveService.getSingleEmployeeLeaves(_EmployeeLeaveFilterForm).subscribe({
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

  openEmployeeLeaveDialog(element: any) {
    const dialogRef = this.dialog.open(AddEmployeeLeaveComponent, {
      panelClass: 'cstm_width_700',
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

  viewEmployeeLeaveDialog(element: any): void {
    this.dialog.open(ViewEmployeeLeaveComponent, {
      data: { element: element },
      panelClass: 'cstm_width_700',
      height: 'auto',
      disableClose: true
    });
  }

  deleteEmployeeLeaveDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteEmployeeLeaveComponent, {
      panelClass: 'cstm_width_700',
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

  processLeaveDialog(element: any) {
    const dialogRef = this.dialog.open(ProcessEmployeeLeaveComponent, {
      panelClass: 'cstm_width_700',
      maxHeight: '90vh',
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