import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { LabOrderTypeService } from '../lab-order-type.service';
import { ServiceService } from '../../service/service.service';

@Component({
  selector: 'app-add-lab-order-type',
  templateUrl: './add-lab-order-type.component.html',
  styleUrls: ['./add-lab-order-type.component.css'],
  standalone: false
})
export class AddLabOrderTypeComponent implements OnInit {
  form!: FormGroup;
  isLoading = false;
  isEdit = false;
  services: any[] = [];

  constructor(
    private fb: FormBuilder,
    private dialog: MatDialog,
    private serviceService: ServiceService,
    private service: LabOrderTypeService,
    private notifications: NotificationsService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any; services: any[] }
  ) { }

  ngOnInit(): void {
    this.isEdit = this.data?.element?.id != null;
    this.services = this.data?.services || [];

    this.form = this.fb.group({
      id: [this.isEdit ? this.data.element.id : 0],
      name: [this.isEdit ? this.data.element.name : '', Validators.required],
      description: [this.isEdit ? this.data.element.description : ''],
      serviceId: [this.isEdit ? this.data.element.serviceId : null, Validators.required],
      customFieldsSchema: [this.isEdit ? this.data.element.customFieldsSchema : '[]']
    });
    this.getservicesList();
  }

  getservicesList() {
    let _CategoryFilter: any = {DepartmentId : 27};
    this.serviceService.getAllServices(_CategoryFilter).subscribe((data: any) => {
      this.services = data.item1;
    });
  }

  save(): void {
    if (this.form.invalid) return;

    this.isLoading = true;
    const command = this.form.value;

    this.service.saveLabOrderType(command).subscribe({
      next: (res: any) => {
        this.isLoading = false;
        if (res.Status === 200) {
          this.notifications.showNotification(res.Data || 'Lab Order Type Saved Successfully!', 'snack-bar-success');
          this.dialog.closeAll();
        } else if (res.Status === 409) {
          this.notifications.showNotification('Lab Order Type with this name already exists!', 'snack-bar-danger');
        } else {
          this.notifications.showNotification(res.Message || 'Error saving lab order type!', 'snack-bar-danger');
        }
      },
      error: (error: any) => {
        this.isLoading = false;
        const message = error?.error?.Message || 'An error occurred';
        this.notifications.showNotification(message, 'snack-bar-danger');
      }
    });
  }
}
