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

@Component({
  selector: 'app-service-list',
  templateUrl: './service-list.component.html',
  styleUrls: ['./service-list.component.css'],
  standalone: false
})
export class ServiceListComponent implements OnInit {
  dataSource!: MatTableDataSource<any>;
  form!: FormGroup;
  displayedColumns: string[] = ['code', 'name', 'basePrice', 'departmentName', 'isActive', 'actions'];
  isLoading = false;
  departments: any[] = [];
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
    private departmentService: DepartmentService
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      searchText: [''],
      departmentId: ['']
    });
    this.loadDepartments();
    this.bindData();
  }

  loadDepartments(): void {
    this.departmentService.getAllDepartments({}).subscribe({
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
    const filter: any = {
      currentPage: this.currentPage,
      take: this.pageSize
    };
    const deptId = this.form.value.departmentId;
    if (deptId) {
      filter.departmentId = +deptId;
    }
    this.service.getAllServices(filter).subscribe({
      next: (data: any) => {
        this.dataSource = new MatTableDataSource(data.item1 || []);
        this.totalRows = data.item2 || 0;
        this.dataSource.sort = this.sort;
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

  openAdd(): void {
    this.dialog.open(AddServiceComponent, { data: { element: {} }, width: '50%', disableClose: true })
      .afterClosed().subscribe(() => this.bindData());
  }

  openEdit(element: any): void {
    this.dialog.open(AddServiceComponent, { data: { element }, width: '50%', disableClose: true })
      .afterClosed().subscribe(() => this.bindData());
  }

  openView(element: any): void {
    this.dialog.open(ViewServiceComponent, { data: { element }, width: '40%', disableClose: true });
  }

  openDelete(element: any): void {
    this.dialog.open(DeleteServiceComponent, { data: { element }, width: '30%', disableClose: true })
      .afterClosed().subscribe(() => this.bindData());
  }
}
