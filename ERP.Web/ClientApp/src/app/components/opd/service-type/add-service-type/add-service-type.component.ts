import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { ServiceTypeService } from '../service-type.service';

@Component({
  selector: 'app-add-serviceType',
  templateUrl: './add-service-type.component.html',
  styleUrls: ['./add-service-type.component.css'],
  standalone: false
})
export class AddServiceTypeComponent implements OnInit {
  form!: FormGroup;
  isLoading = false;
  isEdit = false;
  departments: any[] = [];

  constructor(
    private fb: FormBuilder,
    private dialog: MatDialog,
    private serviceType: ServiceTypeService,
    private notifications: NotificationsService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  ngOnInit(): void {
    this.isEdit = this.data?.element?.id != null;
    this.form = this.fb.group({
      id: [this.isEdit ? this.data.element.id : 0],
      name: [this.isEdit ? this.data.element.name : '', Validators.required],
      isActive: [this.isEdit ? this.data.element.isActive : true]
    });
  }

  save(): void {
    if (this.form.invalid) return;

    this.isLoading = true;
    const command = this.form.value;

    this.serviceType.saveServiceType(command).subscribe({
      next: (res: any) => {
        this.isLoading = false;
        if (res.Status === 200) {
          this.notifications.showNotification(res.Data || 'ServiceType Saved Successfully!', 'snack-bar-success');
          this.dialog.closeAll();
        } else if (res.Status === 409) {
          this.notifications.showNotification('ServiceType with this code already exists!', 'snack-bar-danger');
        } else {
          this.notifications.showNotification(res.Message || 'Error saving serviceType!', 'snack-bar-danger');
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
