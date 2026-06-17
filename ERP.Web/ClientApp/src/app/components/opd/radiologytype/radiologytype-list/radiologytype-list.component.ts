import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { RadiologyTypeService } from '../radiologytype.service';
import { AddRadiologyTypeComponent } from '../add-radiologytype/add-radiologytype.component';
import { ViewRadiologyTypeComponent } from '../view-radiologytype/view-radiologytype.component';
import { DeleteRadiologyTypeComponent } from '../delete-radiologytype/delete-radiologytype.component';
import { ServiceService } from '../../service/service.service';

@Component({
  selector: 'app-radiologytype-list',
  templateUrl: './radiologytype-list.component.html',
  styleUrls: ['./radiologytype-list.component.css'],
  standalone: false
})
export class RadiologyTypeListComponent implements OnInit {
  dataSource = new MatTableDataSource<any>([]);
  form!: FormGroup;
  displayedColumns: string[] = ['name', 'serviceName', 'isActive', 'actions'];
  services: any[] = [];
  currentPage = 0;
  pageSize = 10;
  totalRecords = 0;

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(
    private dialog: MatDialog,
    private fb: FormBuilder,
    private constantService: ConstantService,
    private radiologyTypeService: RadiologyTypeService,
    private serviceService: ServiceService,
    private notifications: NotificationsService
  ) { }

  ngOnInit(): void {
    this.pageSize = this.constantService.defaultItemPerPage;
    this.buildForm();
    this.setupFilters();
    this.loadServices();
    this.bindData();
  }

  buildForm(): void {
    this.form = this.fb.group({
      name: [''],
      serviceId: [null]
    });
  }

  setupFilters(): void {
    this.form.get('name')?.valueChanges.pipe(
      debounceTime(400),
      distinctUntilChanged()
    ).subscribe(() => {
      this.currentPage = 0;
      this.bindData();
    });
  }

  loadServices(): void {
    this.serviceService.getAllServices({ departmentId: 28 }).subscribe({
      next: (res: any) => {
        this.services = res?.item1 || [];
      },
      error: () => {
        this.services = [];
      }
    });
  }

  bindData(): void {
    const { name, serviceId } = this.form.value;
    const filter: any = {
      name: name?.trim() || null,
      serviceId: serviceId || null,
      pagingData: {
        currentPage: this.currentPage,
        take: this.pageSize
      }
    };

    this.radiologyTypeService.getAllRadiologyTypes(filter).subscribe({
      next: (data: any) => {
        this.dataSource.data = data?.item1 ?? [];
        this.totalRecords = data?.item2 ?? this.dataSource.data.length;
      },
      error: () => {
        this.dataSource.data = [];
        this.totalRecords = 0;
        this.notifications.showNotification('Error loading radiology types', 'snack-bar-danger');
      }
    });
  }

  filterData(): void {
    this.currentPage = 0;
    this.bindData();
  }

  clearFilters(): void {
    this.form.reset({
      name: '',
      serviceId: null
    });
    this.currentPage = 0;
    this.bindData();
  }

  pageChanged(event: PageEvent): void {
    this.pageSize = event.pageSize;
    this.currentPage = event.pageIndex;
    this.bindData();
  }

  getServiceName(element: any): string {
    return element?.service?.name || '-';
  }

  openAdd(): void {
    this.dialog.open(AddRadiologyTypeComponent, {
      data: { element: {}, services: this.services },
      panelClass: 'cstm_width_700',
      disableClose: true
    }).afterClosed().subscribe(() => this.bindData());
  }

  openEdit(element: any): void {
    this.dialog.open(AddRadiologyTypeComponent, {
      data: { element, services: this.services },
      panelClass: 'cstm_width_700',
      disableClose: true
    }).afterClosed().subscribe(() => this.bindData());
  }

  openView(element: any): void {
    this.dialog.open(ViewRadiologyTypeComponent, {
      data: { element },
      panelClass: 'cstm_width_500',
      disableClose: true
    });
  }

  openDelete(element: any): void {
    this.dialog.open(DeleteRadiologyTypeComponent, {
      data: { element },
      panelClass: 'cstm_width_500',
      disableClose: true
    }).afterClosed().subscribe(() => this.bindData());
  }
}
