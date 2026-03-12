import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort, Sort } from '@angular/material/sort'; // Import MatSort and Sort
import { FormBuilder, FormGroup } from '@angular/forms';
import { AddManageEmployeeLeaveComponent } from '../add-manage-employee-leave/add-manage-employee-leave.component';
import { ViewManageEmployeeLeaveComponent } from '../view-manage-employee-leave/view-manage-employee-leave.component';
import { DeleteManageEmployeeLeaveComponent } from '../delete-manage-employee-leave/delete-manage-employee-leave.component';
import { ProcessManageEmployeeLeaveComponent } from '../process-manage-employee-leave/process-manage-employee-leave.component';
import { EmployeeLeaveService } from '../../employee-leave/employee-leave.service';
import { ConstantService } from '../../../../Service/constant.service';
import { DepartmentService } from '../../../department/department.service';


@Component({
  selector: 'app-manage-employee-leave-list',
  templateUrl: './manage-employee-leave-list.component.html',
  styleUrls: ['./manage-employee-leave-list.component.css'],
  standalone: false
})

export class ManageEmployeeLeaveListComponent {
  EmployeeLeaveFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSize = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = ['leaveType', 'startDate', 'endDate', 'employeename', 'employeedesignation', 'department', 'day', 'status', 'reason', 'comments', 'actions'];
  dataSource: any;
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
  this.pageSize = this.constantService.defaultItemPerPage;
    this.EmployeeLeaveFilterForm = this.formBuilder.group({
      fdate: [new Date()],
      tdate: [new Date()],
      statusId: [3],
      employeeId: [null],
      departmentId: ['']
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
       take: this.pageSize
    };

    // Clone the form value and add paging data
    const _EmployeeLeaveFilterForm = {
      ...this.EmployeeLeaveFilterForm.value,
      PagingData: pagingData
    };

    // Call the service method and subscribe with the observer
    this.employeeLeaveService.getAllEmployeeLeaves(_EmployeeLeaveFilterForm).subscribe({
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
    const dialogRef = this.dialog.open(AddManageEmployeeLeaveComponent, {
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
    this.dialog.open(ViewManageEmployeeLeaveComponent, {
      data: { element: element },
      panelClass: 'cstm_width_700',
      height: 'auto',
      disableClose: true
    });
  }

  deleteEmployeeLeaveDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteManageEmployeeLeaveComponent, {
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
    const dialogRef = this.dialog.open(ProcessManageEmployeeLeaveComponent, {
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