import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
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
  dataSource: any = [];
  form!: FormGroup;
  displayedColumns: string[] = ['code', 'name', 'basePrice', 'departmentName', 'isActive', 'actions'];
  isLoading = false;
  departments: any[] = [];

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
    const filter: any = {};
    const deptId = this.form.value.departmentId;
    if (deptId) {
      filter.departmentId = +deptId;
    }
    this.service.getAllServices(filter).subscribe({
      next: (data: any) => {
        this.dataSource = data.Data || [];
        this.isLoading = false;
      },
      error: () => {
        this.dataSource = [];
        this.isLoading = false;
      }
    });
  }

  filterData(): void {
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
