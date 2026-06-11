import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ConstantService } from '../../../../Service/constant.service';
import { ServiceService } from '../service.service';
import { AddServiceComponent } from '../add-service/add-service.component';
import { ViewServiceComponent } from '../view-service/view-service.component';
import { DeleteServiceComponent } from '../delete-service/delete-service.component';
import { DepartmentService } from '../../../department/department.service';
import { ServiceTypeService } from '../../service-type/service-type.service';

@Component({
  selector: 'app-service-list',
  templateUrl: './service-list.component.html',
  styleUrls: ['./service-list.component.css'],
  standalone: false
})
export class ServiceListComponent implements OnInit {
  dataSource!: MatTableDataSource<any>;
  form!: FormGroup;
  displayedColumns: string[] = ['code', 'name', 'basePrice', 'departmentName','serviceType', 'isActive', 'actions'];
  isLoading = false;
  departments: any[] = [];
  serviceType : any[] = [];
  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  pageSize = 10;
  totalRows = 0;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private dialog: MatDialog,
    private fb: FormBuilder,
    private constantService: ConstantService,
    private service: ServiceService,
    private departmentService: DepartmentService,
    private serviceTypeService: ServiceTypeService,
    
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      name: [''],
      departmentId: [''],
      serviceTypeId: ['']
    });
    this.loadDepartments();
    this.loadServiceType();
    this.bindData();
  }

  loadServiceType(): void {
    this.serviceTypeService.getAllServiceTypes({}).subscribe({
      next: (res: any) => {
        this.serviceType = res?.item1 ?? res ?? [];
      },
      error: () => {
        this.serviceType = [];
      }
    });
  }

  loadDepartments(): void {
    this.departmentService.getClinicalDepartment().subscribe({
      next: (res: any) => {
        this.departments = res?.item1 ?? res ?? [];
      },
      error: () => {
        this.departments = [];
      }
    });
  }

  bindData(): void {
    this.isLoading = true;

    const pagingData = {
      currentPage: this.currentPage,
      take: this.pageSize
    };

    const _FilterForm = {
      ...this.form.value,
      PagingData: pagingData
    };

    this.service.getAllServices(_FilterForm).subscribe({
      next: (data: any) => {
        this.dataSource = new MatTableDataSource(data.item1 || []);
        this.totalRows = data.item2 || 0;
        this.dataSource.sort = this.sort;
        setTimeout(() => {
          if (this.paginator) {
            this.paginator.pageIndex = this.currentPage;
            this.paginator.length = this.totalRows;
          }
        });
        this.isLoading = false;
      },
      error: () => {
        this.totalRows = 0;
        this.isLoading = false;
      }
    });
  }

  pageChanged(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData();
  }

  filterData(): void {
    this.currentPage = 0;
    this.bindData();
  }

  openService(element: any) {
    const dialogRef = this.dialog.open(AddServiceComponent, {
      panelClass: 'cstm_width_500',
      height: 'auto',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.currentPage = 0;
      this.bindData();
    });
  }

  viewServiceDialog(element: any): void {
    this.dialog.open(ViewServiceComponent, {
      data: { element: element },
      panelClass: 'cstm_width_500',
      height: 'auto',
      disableClose: true
    });
  }

  deleteServiceDialog(element: any) {
    const dialogRef = this.dialog.open(DeleteServiceComponent, {
      panelClass: 'cstm_width_500',
      height: 'auto',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.currentPage = 0;
      this.bindData();
    });
  }
}
