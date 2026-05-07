import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NotificationsService } from '../../../Service/notification.service';
import { LabOrderTypeService } from './lab-order-type.service';

@Component({
  selector: 'app-lab-order-type',
  templateUrl: './lab-order-type.component.html',
  styleUrls: ['./lab-order-type.component.css'],
  standalone: false
})
export class LabOrderTypeComponent implements OnInit {
  isLoading = false;
  items: any[] = [];

  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private service: LabOrderTypeService,
    private notifications: NotificationsService
  ) {
    this.form = this.fb.group({
      id: [0],
      name: [''],
      description: [''],
      serviceId: [0],
      customFieldsSchema: ['[]']
    });
  }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.isLoading = true;
    const filter = { name: '', pagingData: { currentPage: 0, take: 100 } };
    this.service.getAllLabOrderTypes(filter).subscribe({
      next: (res: any) => {
        this.items = res?.item1 || [];
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
      }
    });
  }

  edit(item: any): void {
    this.form.patchValue({
      id: item.id,
      name: item.name,
      description: item.description,
      serviceId: item.serviceId || 0,
      customFieldsSchema: item.customFieldsSchema || '[]'
    });
  }

  resetForm(): void {
    this.form.reset({ id: 0, name: '', description: '', serviceId: 0, customFieldsSchema: '[]' });
  }

  save(): void {
    const payload = this.form.value;
    this.service.saveLabOrderType(payload).subscribe({
      next: (res: any) => {
        if (res?.Status === 200) {
          this.notifications.showNotification(res?.Message || 'Lab order type saved.', 'snack-bar-success');
          this.resetForm();
          this.load();
        } else {
          this.notifications.showNotification(res?.Message || 'Unable to save lab order type.', 'snack-bar-danger');
        }
      },
      error: () => this.notifications.showNotification('Unable to save lab order type.', 'snack-bar-danger')
    });
  }

  remove(id: number): void {
    this.service.deleteLabOrderType(id).subscribe({
      next: () => {
        this.notifications.showNotification('Lab order type deleted.', 'snack-bar-success');
        this.load();
      },
      error: () => this.notifications.showNotification('Unable to delete lab order type.', 'snack-bar-danger')
    });
  }
}
