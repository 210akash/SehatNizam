import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { ConstantService } from '../../../../Service/constant.service';
import { ServiceTypeService } from '../service-type.service';
import { AddServiceTypeComponent } from '../add-service-type/add-service-type.component';
import { ViewServiceTypeComponent } from '../view-service-type/view-service-type.component';
import { DeleteServiceTypeComponent } from '../delete-service-type/delete-service-type.component';
import { AddServiceAccountComponent } from '../../serviceaccount/add-serviceaccount/add-serviceaccount.component';

@Component({
  selector: 'app-service-type-list',
  templateUrl: './service-type-list.component.html',
  styleUrls: ['./service-type-list.component.css'],
  standalone: false
})
export class ServiceTypeListComponent implements OnInit {
  dataSource!: MatTableDataSource<any>;
  form!: FormGroup;
  displayedColumns: string[] = ['name',  'actions'];
  isLoading = false;

  currentPage = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];
  pageSize = 10;
  totalRows = 0;

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(
    private dialog: MatDialog,
    private fb: FormBuilder,
    private constantServiceType: ConstantService,
    private serviceType: ServiceTypeService,
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      name: ['']
    });
    this.bindData();
  }

  bindData(): void {
    this.isLoading = true;

    // Prepare paging data
    const pagingData = {
      currentPage: this.currentPage,
          take: this.pageSize
    };

    // Clone the form value and add paging data
    const _FilterForm = {
      ...this.form.value,
      PagingData: pagingData
    };

    this.serviceType.getAllServiceTypes(_FilterForm).subscribe({
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
    const dialogRef = this.dialog.open(AddServiceTypeComponent, {
      data: { element: {} },
      panelClass: 'cstm_width_500',
      height: 'auto',
      disableClose: true
    });
    dialogRef.afterClosed().subscribe(() => this.bindData());
  }

  openEdit(element: any): void {
    const dialogRef = this.dialog.open(AddServiceTypeComponent, {
      data: { element },
      panelClass: 'cstm_width_500',
      height: 'auto',
      disableClose: true
    });
    dialogRef.afterClosed().subscribe(() => this.bindData());
  }



  openView(element: any): void {
    this.dialog.open(ViewServiceTypeComponent, {
      data: { element },
      panelClass: 'cstm_width_500',
      height: 'auto',
      disableClose: true
    });
  }

  openDelete(element: any): void {
    const dialogRef = this.dialog.open(DeleteServiceTypeComponent, {
      data: { element },
      panelClass: 'cstm_width_500',
      height: 'auto',
      disableClose: true
    });
    dialogRef.afterClosed().subscribe(() => this.bindData());
  }

    accounts(element: any): void {
      const dialogRef = this.dialog.open(AddServiceAccountComponent, {
        data: { element },
        panelClass: 'cstm_width_1000',
        height: 'auto',
        disableClose: true
      });
      dialogRef.afterClosed().subscribe(() => this.bindData());
    }
}