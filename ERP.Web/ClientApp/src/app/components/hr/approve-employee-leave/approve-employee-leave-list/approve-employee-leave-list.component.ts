import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { EmployeeLeaveService } from '../../employee-leave/employee-leave.service';
import { ConstantService } from '../../../../Service/constant.service';
import { DepartmentService } from '../../../department/department.service';
import { ViewManageEmployeeLeaveComponent } from '../../manage-employee-leave/view-manage-employee-leave/view-manage-employee-leave.component';
import { ProcessApproveEmployeeLeaveComponent } from '../process-approve-employee-leave/process-approve-employee-leave.component';


@Component({
  selector: 'app-approve-employee-leave-list',
  templateUrl: './approve-employee-leave-list.component.html',
  styleUrls: ['./approve-employee-leave-list.component.css'],
  standalone: false
})

export class ApproveEmployeeLeaveListComponent {
  EmployeeLeaveFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSize = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = ['leaveType', 'startDate', 'endDate', 'employeename', 'employeedesignation', 'department', 'day', 'status', 'reason', 'comments', 'actions'];
  dataSource: any;
  take = 50;
  totalRows = 0;
  departmentList: any;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort; // ViewChild for MatSort

  constructor(
    private employeeLeaveService: EmployeeLeaveService,
    private constantService: ConstantService,
    private departmentService: DepartmentService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder
  ) { }

  async ngOnInit(): Promise<void> {
    this.EmployeeLeaveFilterForm = this.formBuilder.group({
      fdate: [new Date()],
      tdate: [new Date()],
      statusId: [2],
      employeeId: [null]
    });

    this.EmployeeLeaveFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(new Date().setDate(new Date().getDate() - 30)));
    this.EmployeeLeaveFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(new Date()));
    this.getDepartmentList();
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
      ...this.EmployeeLeaveFilterForm.value,
      PagingData: pagingData
    };

    // Call the service method and subscribe with the observer
    this.employeeLeaveService.getAllDepartmentLeaves(_EmployeeLeaveFilterForm).subscribe({
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

  viewEmployeeLeaveDialog(element: any): void {
    this.dialog.open(ViewManageEmployeeLeaveComponent, {
      data: { element: element },
      panelClass: 'cstm_width_700',
      height: 'auto',
      disableClose: true
    });
  }

  processLeaveDialog(element: any) {
    const dialogRef = this.dialog.open(ProcessApproveEmployeeLeaveComponent, {
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

  async filterData() {
    await this.bindData();
  }

  getDepartmentList(): void {
    let _employeeForm: any = {};
    this.departmentService.getAllDepartments(_employeeForm).subscribe(data => {
      this.departmentList = data.item1;
    });
  }
}