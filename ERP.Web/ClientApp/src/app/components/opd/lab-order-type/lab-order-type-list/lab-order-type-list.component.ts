import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ConstantService } from '../../../../Service/constant.service';
import { LabOrderTypeService } from '../lab-order-type.service';
import { AddLabOrderTypeComponent } from '../add-lab-order-type/add-lab-order-type.component';
import { ViewLabOrderTypeComponent } from '../view-lab-order-type/view-lab-order-type.component';
import { DeleteLabOrderTypeComponent } from '../delete-lab-order-type/delete-lab-order-type.component';
import { ServiceService } from '../../service/service.service';
import { AddLabTestVariableComponent } from '../add-lab-test-variable/add-lab-test-variable.component';

@Component({
  selector: 'app-lab-order-type-list',
  templateUrl: './lab-order-type-list.component.html',
  styleUrls: ['./lab-order-type-list.component.css'],
  standalone: false
})
export class LabOrderTypeListComponent implements OnInit {
  dataSource: any = [];
  form!: FormGroup;
  displayedColumns: string[] = ['name', 'service', 'actions'];
  isLoading = false;
  services: any[] = [];

  constructor(
    private dialog: MatDialog,
    private fb: FormBuilder,
    private constantService: ConstantService,
    private labOrderTypeService: LabOrderTypeService,
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
    this.serviceService.getAllServices({ departmentId: 27 }).subscribe({
      next: (data: any) => {
        this.services = data.item1 || [];
      },
      error: () => {
        this.services = [];
      }
    });
  }

  bindData(): void {
    this.isLoading = true;
    const filter: any = {};
    const searchText = this.form.value.searchText;
    const serviceId = this.form.value.serviceId;
    if (searchText) {
      filter.name = searchText;
    }
    if (serviceId) {
      filter.serviceId = +serviceId;
    }
    this.labOrderTypeService.getAllLabOrderTypes(filter).subscribe({
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

openAdd() {
    const dialogRef = this.dialog.open(AddLabOrderTypeComponent, {
      panelClass: 'cstm_width_500',
      height: 'auto',
     data: { element: {}, services: this.services },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
    });
  }

  openEdit(element: any): void {
    this.dialog.open(AddLabOrderTypeComponent, {
      data: { element, services: this.services },
      width: '40%',
      disableClose: true
    }).afterClosed().subscribe(() => this.bindData());
  }

  openView(element: any): void {
    this.dialog.open(ViewLabOrderTypeComponent, {
      data: { element },
      width: '30%',
      disableClose: true
    });
  }

  openDelete(element: any): void {
    this.dialog.open(DeleteLabOrderTypeComponent, {
      data: { element },
      width: '30%',
      disableClose: true
    }).afterClosed().subscribe(() => this.bindData());
  }

  openTestVariable(element: any) {
    const dialogRef = this.dialog.open(AddLabTestVariableComponent, {
      panelClass: 'cstm_width_1400',
      height: 'auto',
      data: {
        element: element,
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      this.bindData();
    });
  }
}
