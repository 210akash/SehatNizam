import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ConstantService } from '../../../../Service/constant.service';
import { ServiceTypeService } from '../service-type.service';
import { AddServiceTypeComponent } from '../add-service-type/add-service-type.component';
import { ViewServiceTypeComponent } from '../view-service-type/view-service-type.component';
import { DeleteServiceTypeComponent } from '../delete-service-type/delete-service-type.component';


@Component({
  selector: 'app-service-type-list',
  templateUrl: './service-type-list.component.html',
  styleUrls: ['./service-type-list.component.css'],
  standalone: false
})
export class ServiceTypeListComponent implements OnInit {
  dataSource: any = [];
  form!: FormGroup;
  displayedColumns: string[] = ['name', 'isActive', 'actions'];
  isLoading = false;
  departments: any[] = [];

  constructor(
    private dialog: MatDialog,
    private fb: FormBuilder,
    private constantServiceType: ConstantService,
    private serviceType: ServiceTypeService,
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      searchText: ['']
    });
    this.bindData();
  }

  bindData(): void {
    this.isLoading = true;
    const filter: any = {};
    this.serviceType.getAllServiceTypes(filter).subscribe({
      next: (data: any) => {
        this.dataSource = data.item1 || [];
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
    this.dialog.open(AddServiceTypeComponent, { data: { element: {} }, width: '50%', disableClose: true })
      .afterClosed().subscribe(() => this.bindData());
  }

  openEdit(element: any): void {
    this.dialog.open(AddServiceTypeComponent, { data: { element }, width: '50%', disableClose: true })
      .afterClosed().subscribe(() => this.bindData());
  }

  openView(element: any): void {
    this.dialog.open(ViewServiceTypeComponent, { data: { element }, width: '40%', disableClose: true });
  }

  openDelete(element: any): void {
    this.dialog.open(DeleteServiceTypeComponent, { data: { element }, width: '30%', disableClose: true })
      .afterClosed().subscribe(() => this.bindData());
  }
}
