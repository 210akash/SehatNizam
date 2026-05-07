import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ConstantService } from '../../../../Service/constant.service';
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
  dataSource: any = [];
  form!: FormGroup;
  displayedColumns: string[] = ['name', 'serviceName', 'isActive', 'actions'];
  isLoading = false;
  services: any[] = [];

  constructor(
    private dialog: MatDialog,
    private fb: FormBuilder,
    private constantService: ConstantService,
    private radiologyTypeService: RadiologyTypeService,
    private serviceService: ServiceService
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      searchText: [''],
      serviceId: ['']
    });
    this.loadServices();
    this.bindData();
  }

  loadServices(): void {
    this.serviceService.getAllServices({}).subscribe({
      next: (res: any) => {
        this.services = res?.Data || [];
      },
      error: () => {
        this.services = [];
      }
    });
  }

  bindData(): void {
    this.isLoading = true;
    const filter: any = {};
    const serviceId = this.form.value.serviceId;
    if (serviceId) {
      filter.serviceId = +serviceId;
    }
    this.radiologyTypeService.getAllRadiologyTypes(filter).subscribe({
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
    this.dialog.open(AddRadiologyTypeComponent, { 
      data: { element: {}, services: this.services }, 
      width: '40%', 
      disableClose: true 
    }).afterClosed().subscribe(() => this.bindData());
  }

  openEdit(element: any): void {
    this.dialog.open(AddRadiologyTypeComponent, { 
      data: { element, services: this.services }, 
      width: '40%', 
      disableClose: true 
    }).afterClosed().subscribe(() => this.bindData());
  }

  openView(element: any): void {
    this.dialog.open(ViewRadiologyTypeComponent, { 
      data: { element }, 
      width: '30%', 
      disableClose: true 
    });
  }

  openDelete(element: any): void {
    this.dialog.open(DeleteRadiologyTypeComponent, { 
      data: { element }, 
      width: '30%', 
      disableClose: true 
    }).afterClosed().subscribe(() => this.bindData());
  }
}
