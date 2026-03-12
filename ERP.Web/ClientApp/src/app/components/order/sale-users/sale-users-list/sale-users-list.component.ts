import { Component, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { FormBuilder, FormGroup } from '@angular/forms';
import { UserService } from '../../../user-management/user.service';
import { ResetpasswordComponent } from '../../../user-management/user/reset-password/reset-password.component';
import { AddSaleUserComponent } from '../add-sale-user/add-sale-user.component';
import { AuthenticationService } from '../../../../Auth/authentication.service';
import * as XLSX from 'xlsx';
import { saveAs } from 'file-saver';
import { ConstantService } from '../../../../Service/constant.service';
import { EmployeeDesignationService } from '../../../hr/employee-designation/employee-designation.service';
import { RegisterMobileDeviceComponent } from '../../../hr/register-mobile-device/register-mobile-device.component';

@Component({
  selector: 'app-sale-users-list',
  templateUrl: './sale-users-list.component.html',
  styleUrl: './sale-users-list.component.css',
  standalone: false
})

export class SaleUsersListComponent {
  userFilterForm!: FormGroup;
  isLoading = false;
  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  displayedColumns: string[] = ['firstName', 'lastName', 'email', 'company','designation',  'roles', 'isActive', 'createdDate', 'actions'];
  dataSource: any;
  pageSize = 20;
  totalRows = 0;
  employeeDesignationList: any;
  currentUser: any;
  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private userService: UserService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private authenticationService: AuthenticationService,
    private constantService: ConstantService,
    private employeeDesignationService: EmployeeDesignationService,
  ) { }

  async ngOnInit(): Promise<void> {
    this.currentUser = this.authenticationService.currentUserValue;

    this.userFilterForm = this.formBuilder.group({
      fdate: [new Date()],
      tdate: [new Date()],
      name: [''],
      role: [''],
      employeeDesignationId : ['0'],
      employeeWorkSiteTypeId : ['2']
    });
    const currentYear = new Date().getFullYear();
    const startDate = new Date(currentYear, 0, 1);
    const now = new Date();
    const endDate = new Date(now.getFullYear(), now.getMonth(), now.getDate());
    this.userFilterForm.get('fdate')?.patchValue(this.constantService.formatDate(startDate));
    this.userFilterForm.get('tdate')?.patchValue(this.constantService.formatDate(endDate));
    this.getEmployeeDesignationList();
    await this.bindData();
  }

  async bindData(): Promise<void> {
    this.isLoading = true;

    const pagingData = {
      currentPage: this.currentPage,
      take: this.pageSize
    }

 if(this.userFilterForm.get('name')?.value.length > 3 || this.userFilterForm.get('name')?.value == '') {

    let _userFilterForm: any = {};
    _userFilterForm = Object.assign(_userFilterForm, this.userFilterForm.value);
    _userFilterForm["PagingData"] = pagingData;

      let fdate = new Date(_userFilterForm.fdate);
      let tdate = new Date(_userFilterForm.tdate);

      _userFilterForm['fdate'] = fdate.toLocaleDateString();
      _userFilterForm['tdate'] = tdate.toLocaleDateString();

    this.userService.getAllSaleUsers(_userFilterForm).subscribe({
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
}

exportToExcel(): void {
  const nameValue = this.userFilterForm.get('name')?.value;

  if (nameValue?.length > 3 || nameValue === '') {
    let _userFilterForm: any = {};
    _userFilterForm = Object.assign(_userFilterForm, this.userFilterForm.value);

    const pageSize = 10000; // or whatever max you want, must be > 0
_userFilterForm["PagingData"] = {
  currentPage: 0,
  take: pageSize
};


      let fdate = new Date(_userFilterForm.fdate);
      let tdate = new Date(_userFilterForm.tdate);

      _userFilterForm['fdate'] = fdate.toLocaleDateString();
      _userFilterForm['tdate'] = tdate.toLocaleDateString();

    this.userService.getAllSaleUsers(_userFilterForm).subscribe({
      next: (data: any) => {
        const users = data.item1;

    
        if (users.length > 0) {
          // Define columns you want to export by keys
          const selectedColumns = ['firstName', 'lastName', 'email','designation', 'roleName','createdDate', 'isActive',]; // example keys

          // Map original data to only include selected columns
          const filteredData = users.map((user: any) => {
            const filteredUser: any = {};
            selectedColumns.forEach(col => {
              filteredUser[col] = user[col];
            });
            return filteredUser;
          });

          const worksheet: XLSX.WorkSheet = XLSX.utils.json_to_sheet(filteredData);
          const workbook: XLSX.WorkBook = {
            Sheets: { 'Users': worksheet },
            SheetNames: ['Users']
          };
          const excelBuffer: any = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
          const blob: Blob = new Blob([excelBuffer], { type: 'application/octet-stream' });
      const now = new Date();

const year = now.getFullYear();
const month = String(now.getMonth() + 1).padStart(2, '0'); // Months are zero-based
const day = String(now.getDate()).padStart(2, '0');

const hours = String(now.getHours()).padStart(2, '0');
const minutes = String(now.getMinutes()).padStart(2, '0');
const seconds = String(now.getSeconds()).padStart(2, '0');

const dateTimeString = `${year}${month}${day}_${hours}${minutes}${seconds}`;

const fileName = `SalesUsers_${dateTimeString}.xlsx`;
          saveAs(blob, fileName);
        } else {
          console.warn('No data to export.');
        }
      },
      error: (error: any) => {
        console.error('Error exporting data:', error);
      }
    });
  }
}

  pageChanged(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData();
  }

  openUserDialog(element: any) {
    const dialogRef = this.dialog.open(AddSaleUserComponent, {
      panelClass: 'cstm_width_800',
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

  openResetPasswordDialog(element: any): void {
    this.dialog.open(ResetpasswordComponent, {
      data: { element: element },
      disableClose: true,
      maxHeight: '95vh',
    });
  }

    getEmployeeDesignationList(): void {
    let _filterForm = {};
    this.employeeDesignationService.getAllEmployeeDesignations(_filterForm).subscribe(data => {
      this.employeeDesignationList = data.item1;
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


}
