import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { MatSort } from '@angular/material/sort';
import { ConstantService } from '../../../../Service/constant.service';
import { DoctorService } from '../doctor.service';
import { AddDoctorProfileComponent } from '../add-doctor-profile/add-doctor-profile.component';
import { ViewDoctorProfileComponent } from '../view-doctor-profile/view-doctor-profile.component';
import { MatDialog } from '@angular/material/dialog';
import { DepartmentService } from '../../../department/department.service';

@Component({
  selector: 'app-doctor-list',
  templateUrl: './doctor-list.component.html',
  styleUrls: ['./doctor-list.component.css'],
  standalone: false
})
export class DoctorListComponent implements OnInit {
  doctorFilterForm!: FormGroup;
  dataSource: any;
  displayedColumns: string[] = [
    'name',
    'department',
    'designation',
    'phoneNumber',
    'email',
    'status',
    'actions'
  ];
  isLoading = false;

  currentPage = 0;
  pageSize = 0;
  totalRows = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
 departmentList : any;
  constructor(
    private formBuilder: FormBuilder,
        private dialog: MatDialog,
    private doctorService: DoctorService,
    private constantService: ConstantService,
      private departmentService: DepartmentService,
  ) {}

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  ngOnInit(): void {
    this.pageSize = this.constantService.defaultItemPerPage;
    this.doctorFilterForm = this.formBuilder.group({
      name: [''],
      departmentId: [null],
      employeeDesignationId: [null],
    });
    this.bindData();
     this.getDepartmentList();
  }

  getDepartmentList(): void {
    this.departmentService.getClinicalDepartment().subscribe(data => {
      this.departmentList = data;
    });
  }

  async bindData(): Promise<void> {
    this.isLoading = true;
    const pagingData = {
      currentPage: this.currentPage,
      take: this.pageSize
    };

    const filterForm = {
      ...this.doctorFilterForm.value,
      PagingData: pagingData
    };

    (await this.doctorService.getAllDoctors(filterForm)).subscribe({
      next: (data: any) => {
        this.dataSource = new MatTableDataSource(data.item1);
        this.totalRows = data.item2 ?? 0;
        if (this.sort) {
          this.dataSource.sort = this.sort;
        }
        if (data.item1?.length > 0) {
          setTimeout(() => {
            this.paginator.pageIndex = this.currentPage;
            this.paginator.length = data.item2;
          });
        }
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  pageChanged(event: PageEvent) {
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData();
  }

  adddoctorProfileDialog(element: any): void {
    const dialogRef = this.dialog.open(AddDoctorProfileComponent, {
      data: { element: element },
      panelClass: 'cstm_width_800',
      maxHeight: '90vh',
      disableClose: true
    });
     dialogRef.afterClosed().subscribe(result => {
      this.bindData();
    });
  }

   viewdoctorProfileDialog(element: any): void {
    this.dialog.open(ViewDoctorProfileComponent, {
      data: { element: element },
      panelClass: 'cstm_width_800',
      maxHeight: '90vh',
      disableClose: true
    });
  }
}
