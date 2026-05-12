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
  variables: any[] = [];
  selectedLabOrderTypeId: number | null = null;

  form: FormGroup;
  variableForm: FormGroup;

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

    this.variableForm = this.fb.group({
      name: [''],
      unit: [''],
      maleMin: [null],
      maleMax: [null],
      femaleMin: [null],
      femaleMax: [null],
      hasGenderRange: [false]
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
    this.selectedLabOrderTypeId = item.id;
    this.loadVariables(item.id);
  }

  resetForm(): void {
    this.form.reset({ id: 0, name: '', description: '', serviceId: 0, customFieldsSchema: '[]' });
    this.selectedLabOrderTypeId = null;
    this.variables = [];
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

  // Lab Test Variables Management
  loadVariables(labOrderTypeId: number): void {
    // For now, we'll just show empty list. In real app, you'd have a GET endpoint.
    this.variables = [];
  }

  addVariable(): void {
    if (this.variableForm.invalid || !this.selectedLabOrderTypeId) return;

    const variable = this.variableForm.value;
    this.variables.push({ ...variable, id: 0 });
    this.variableForm.reset();
  }

  removeVariable(index: number): void {
    this.variables.splice(index, 1);
  }

  saveVariables(): void {
    if (!this.selectedLabOrderTypeId || this.variables.length === 0) {
      this.notifications.showNotification('Please select a lab order type and add at least one variable.', 'snack-bar-danger');
      return;
    }

    const command = {
      labOrderTypeId: this.selectedLabOrderTypeId,
      variables: this.variables.map(v => ({
        id: v.id || 0,
        name: v.name,
        unit: v.unit,
        maleMin: v.maleMin,
        maleMax: v.maleMax,
        femaleMin: v.femaleMin,
        femaleMax: v.femaleMax,
        hasGenderRange: v.hasGenderRange
      }))
    };

    this.service.saveLabTestVariables(command).subscribe({
      next: (res: any) => {
        if (res?.Status === 200) {
          this.notifications.showNotification('Lab test variables saved successfully!', 'snack-bar-success');
        } else {
          this.notifications.showNotification(res?.Message || 'Error saving variables.', 'snack-bar-danger');
        }
      },
      error: () => this.notifications.showNotification('Error saving variables.', 'snack-bar-danger')
    });
  }
}
