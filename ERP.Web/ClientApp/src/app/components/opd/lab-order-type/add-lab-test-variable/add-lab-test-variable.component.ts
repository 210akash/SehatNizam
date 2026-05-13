import { Component, Inject, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { LabOrderTypeService } from '../lab-order-type.service';

@Component({
  selector: 'app-add-lab-test-variable',
  templateUrl: './add-lab-test-variable.component.html',
  styleUrls: ['./add-lab-test-variable.component.css'],
  standalone: false
})
export class AddLabTestVariableComponent implements OnInit {
  form!: FormGroup;
  isLoading = false;
  labOrderTypeId = 0;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<AddLabTestVariableComponent>,
    private service: LabOrderTypeService,
    private notifications: NotificationsService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  ngOnInit(): void {
    this.labOrderTypeId = this.data?.element?.id ?? 0;

    this.form = this.fb.group({
      labOrderTypeId: [this.labOrderTypeId, Validators.required],
      variables: this.fb.array([])
    });

    this.loadVariables();
  }

  get variables(): FormArray {
    return this.form.get('variables') as FormArray;
  }

  createVariableGroup(variable?: any): FormGroup {
    return this.fb.group({
      id: [variable?.id ?? 0],
      name: [variable?.name ?? '', Validators.required],
      unit: [variable?.unit ?? ''],
      maleMin: [variable?.maleMin ?? null],
      maleMax: [variable?.maleMax ?? null],
      femaleMin: [variable?.femaleMin ?? null],
      femaleMax: [variable?.femaleMax ?? null],
      hasGenderRange: [variable?.hasGenderRange ?? false]
    });
  }

  loadVariables(): void {
    const rawVariables =
      this.data?.element?.variables ??
      this.data?.element?.labTestVariables ??
      [];

    const existingVariables = Array.isArray(rawVariables)
      ? rawVariables
      : rawVariables
        ? [rawVariables]
        : [];

    if (existingVariables.length > 0) {
      existingVariables.forEach((variable: any) => {
        this.variables.push(this.createVariableGroup(variable));
      });
      return;
    }

    this.variables.push(this.createVariableGroup());
  }

  addVariable(index: number): void {
    this.variables.insert(index + 1, this.createVariableGroup());
  }

  removeVariable(index: number): void {
    if (this.variables.length > 1) {
      this.variables.removeAt(index);
    } else {
      this.notifications.showNotification(
        'At least one variable is required.',
        'snack-bar-danger'
      );
    }
  }

  private toNullableDecimal(value: any): number | null {
    if (value === '' || value === null || value === undefined) {
      return null;
    }

    const parsedValue = Number(value);
    return Number.isNaN(parsedValue) ? null : parsedValue;
  }

  saveLabTestVariables(): void {
    if (this.isLoading) {
      return;
    }

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.notifications.showNotification('Please fill all required fields.', 'snack-bar-danger');
      return;
    }

    this.isLoading = true;

    const command = {
      labOrderTypeId: this.form.get('labOrderTypeId')?.value,
      variables: this.variables.controls.map((control) => {
        const value = control.value;
        return {
          id: value.id ?? 0,
          name: value.name,
          unit: value.unit,
          maleMin: this.toNullableDecimal(value.maleMin),
          maleMax: this.toNullableDecimal(value.maleMax),
          femaleMin: this.toNullableDecimal(value.femaleMin),
          femaleMax: this.toNullableDecimal(value.femaleMax),
          hasGenderRange: !!value.hasGenderRange
        };
      })
    };

    this.service.saveLabTestVariables(command).subscribe({
      next: (res: any) => {
        this.isLoading = false;
        if (res.Status === 200) {
          this.notifications.showNotification(res.Data || 'Lab test variables saved successfully!', 'snack-bar-success');
          this.dialogRef.close(true);
        } else {
          this.notifications.showNotification(res.Message || res.Data || 'Error saving lab test variables!', 'snack-bar-danger');
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
