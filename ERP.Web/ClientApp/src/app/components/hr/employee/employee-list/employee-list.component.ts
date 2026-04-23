import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { FormBuilder, FormGroup } from '@angular/forms';
import { UserService } from '../../../user-management/user.service';
import { AddEmployeeComponent } from '../add-employee/add-employee.component';
import { ResetpasswordComponent } from '../../../user-management/user/reset-password/reset-password.component';
import { DepartmentService } from '../../../department/department.service';
import { MatSlideToggleChange } from '@angular/material/slide-toggle';
import { ViewEmployeeComponent } from '../view-employee/view-employee.component';
import { EmployeeDeviceComponent } from '../../employee-device/save-employee-device/employee-device.component';
import { ShowUserAttendanceComponent } from '../../../order/user-attendance/show-user-attendance/show-user-attendance.component';
import { EmployeeWorkSiteTypeService } from '../../employee-worksitetype/employee-worksitetype.service';
import { RegisterMobileDeviceComponent } from '../../register-mobile-device/register-mobile-device.component';
import { AddEmployeeSalaryComponent } from '../../payroll/employeesalary/add-employee-salary/add-employee-salary.component';

@Component({
  selector: 'app-employee-list',
  templateUrl: './employee-list.component.html',
  styleUrl: './employee-list.component.css',
  standalone: false
})

export class EmployeeListComponent {
  employeeFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = ['firstName', 'lastName','hrCode', 'email', 'department', 'designation', 'employeeWorkSiteType','isActive', 'actions'];
  dataSource: any;
  pageSize = 20;
  totalRows = 0;
  departmentList: any;
  employeeWorkSiteTypeList : any;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;
  currentUser: any;
  roleList: string | undefined;
  constructor(
    private employeeService: UserService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private departmentService: DepartmentService,
    private employeeWorkSiteTypeService: EmployeeWorkSiteTypeService
  ) { }

  async ngOnInit(): Promise<void> {
    this.employeeFilterForm = this.formBuilder.group({
      name: [''],
      cnic: [''],
      hrCode: [''],
      departmentId: ['0'],
      employeeWorkSiteTypeId : ['0']
    });
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') ?? '{}');
    this.roleList = this.currentUser.role.toLowerCase().split(',').map((role: string) => role.trim().toLowerCase());
    this.getDepartmentList();
    this.getemployeeWorkSiteTypeList();
    await this.bindData();
  }

  async bindData(): Promise<void> {
    this.isLoading = true;

    const pagingData = {
      currentPage: this.currentPage,
      take: this.pageSize
    }

    let _employeeFilterForm: any = {};
    _employeeFilterForm = Object.assign(_employeeFilterForm, this.employeeFilterForm.value);
    _employeeFilterForm["PagingData"] = pagingData;
    this.employeeService.getAllUsers(_employeeFilterForm).subscribe({
      next: (data: any) => {
        this.dataSource = new MatTableDataSource(data.item1);

        if (data.item1.length > 0) {
          setTimeout(() => {
            this.paginator.pageIndex = this.currentPage;
            this.paginator.length = data.item2;
          });
        }

        this.totalRows = data;
        this.dataSource.sort = this.sort;
        this.isLoading = false;
      },
      error: (error: any) => {
        console.error('Error fetching data:', error);
        this.isLoading = false;
      }
    });
  }

  pageChanged(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData();
  }

  openEmployeeDialog(element: any) {
    const dialogRef = this.dialog.open(AddEmployeeComponent, {
      width: '60%',
      height: 'auto',
      maxHeight: '95vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
    });
  }

  viewEmployeeDialog(element: any) {
    const dialogRef = this.dialog.open(ViewEmployeeComponent, {
      width: '60%',
      height: 'auto',
      maxHeight: '95vh',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
    });
  }

     viewAttendanceDialog(element:any) {
      const dialogRef = this.dialog.open(ShowUserAttendanceComponent, {
        width: '60%',
        height: 'auto',
        maxHeight: '95vh',
         data: { element: element },
        disableClose: true
      });
    }
  

  employeeDeviceDialog(element: any): void {
   const dialogRef = this.dialog.open(EmployeeDeviceComponent, {
      data: { element: element },
      panelClass: 'cstm_width_600',
      maxHeight: '90vh',
      disableClose: true
    });
       dialogRef.afterClosed().subscribe(result => {
      this.bindData();
    });
  }

  openResetPasswordDialog(element: any): void {
    this.dialog.open(ResetpasswordComponent, {
      data: { element: element },
      disableClose: true,
      maxHeight: '95vh',
    });
  }

  registerDeviceDialog(element: any): void {
   const dialogRef = this.dialog.open(RegisterMobileDeviceComponent, {
      data: { element: element },
      panelClass: 'cstm_width_600',
      maxHeight: '90vh',
      disableClose: true
    });
       dialogRef.afterClosed().subscribe(result => {
      this.bindData();
    });
  }

  openEmployeeSalaryDialog(element: any): void {
    const dialogRef = this.dialog.open(AddEmployeeSalaryComponent, {
      panelClass: 'cstm_width_1000',
      height: 'auto',
      maxHeight: '90vh',
      data: { element: element },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(() => {
      this.bindData();
    });
  }

  getDepartmentList(): void {
    var companyId = this.currentUser.department?.companyId;
    this.departmentService.getDepartmentByCompany(companyId).subscribe(data => {
      this.departmentList = data;
    });
  }

    getemployeeWorkSiteTypeList(): void {
       let _companyForm: any = {};
    this.employeeWorkSiteTypeService.getAllEmployeeWorkSiteTypes(_companyForm).subscribe(data => {
      this.employeeWorkSiteTypeList = data.item1;
    });
  }

  onToggleChange(event: MatSlideToggleChange): void {
    const isChecked = event.checked; // true if toggle is on
    this.employeeFilterForm.get('isEmployee')?.setValue(isChecked);
    this.bindData();
  }
}
