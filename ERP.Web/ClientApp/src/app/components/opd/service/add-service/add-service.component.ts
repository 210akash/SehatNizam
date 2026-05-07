import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { ServiceService } from '../service.service';
import { DepartmentService } from '../../../department/department.service';

@Component({
  selector: 'app-add-service',
  templateUrl: './add-service.component.html',
  styleUrls: ['./add-service.component.css'],
  standalone: false
})
export class AddServiceComponent implements OnInit {
  form!: FormGroup;
  isLoading = false;
  isEdit = false;
  departments: any[] = [];

  constructor(
    private fb: FormBuilder,
    private dialog: MatDialog,
    private service: ServiceService,
    private notifications: NotificationsService,
    private departmentService: DepartmentService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  ngOnInit(): void {
    this.isEdit = this.data?.element?.id != null;
    this.form = this.fb.group({
      id: [this.isEdit ? this.data.element.id : 0],
      code: [this.isEdit ? this.data.element.code : '', Validators.required],
      name: [this.isEdit ? this.data.element.name : '', Validators.required],
      basePrice: [this.isEdit ? this.data.element.basePrice : 0, [Validators.required, Validators.min(0)]],
      departmentId: [this.isEdit ? this.data.element.departmentId : null],
      isActive: [this.isEdit ? this.data.element.isActive : true]
    });
    this.loadDepartments();
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

  save(): void {
    if (this.form.invalid) return;

    this.isLoading = true;
    const command = this.form.value;

    this.service.saveService(command).subscribe({
      next: (res: any) => {
        this.isLoading = false;
        if (res.Status === 200) {
          this.notifications.showNotification(res.Data || 'Service Saved Successfully!', 'snack-bar-success');
          this.dialog.closeAll();
        } else if (res.Status === 409) {
          this.notifications.showNotification('Service with this code already exists!', 'snack-bar-danger');
        } else {
          this.notifications.showNotification(res.Message || 'Error saving service!', 'snack-bar-danger');
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
