import { Component, Inject, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { HttpClient } from '@angular/common/http';
import { LabOrderService } from '../lab-order.service';
import { NotificationsService } from '../../../../Service/notification.service';

@Component({
  selector: 'app-save-lab-result',
  templateUrl: './save-lab-result.component.html',
  styleUrls: ['./save-lab-result.component.css'],
  standalone: false
})

export class SaveLabResultComponent implements OnInit {

  form: FormGroup;
  isSaving = false;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private labOrderService: LabOrderService,
    private notifications: NotificationsService,
    private dialogRef: MatDialogRef<SaveLabResultComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.form = this.fb.group({
      labOrderId: [0, Validators.required],
      results: this.fb.array([])
    });
  }

 ngOnInit(): void {

  const order = this.data.order;

  this.form.patchValue({
    labOrderId: order.id
  });

  // 🔥 Patient Gender
  const gender = order.appointment?.patient?.gender?.toLowerCase();

  // 🔥 Variables
  const variables = order.labOrderType?.variables || [];

  variables.forEach((x: any) => {
    this.resultsFormArray.push(
      this.createResultRow(x, gender)
    );

  });

}

  get resultsFormArray(): FormArray {
    return this.form.get('results') as FormArray;
  }

createResultRow(item: any, gender: string): FormGroup {

  let referenceRange = '';
  // 🔥 Gender based range
  if (item.hasGenderRange) {

    if (gender === 'male') {

      referenceRange =
        `${item.maleMin} - ${item.maleMax}`;

    }
    else {

      referenceRange =
        `${item.femaleMin} - ${item.femaleMax}`;
    }

  }
  else {

    referenceRange =
      `${item.maleMin} - ${item.maleMax}`;
  }

  return this.fb.group({

    labTestVariableId: [item.id],

    variableName: [item.name],

    unit: [item.unit],

    referenceRange: [referenceRange],

    resultValue: [null, Validators.required]

  });
}

  getReferenceRange(item: any): string {
    if (item.hasGenderRange) {
      return `Male: ${item.maleMin}-${item.maleMax} | Female: ${item.femaleMin}-${item.femaleMax}`;
    }

    return `${item.maleMin}-${item.maleMax}`;
  }

  async save(): Promise<void> {
   if (this.form.invalid) {
  console.log('Form invalid', this.form);

  Object.keys(this.form.controls).forEach(key => {
    const control = this.form.get(key);

    // Type guard to ensure control exists
    if (control && control.invalid) {
      console.log(`${key} is invalid`, control.errors);
    }
  });

  // Handle FormArray separately
  if (this.resultsFormArray && this.resultsFormArray.controls) {
    this.resultsFormArray.controls.forEach((group, i) => {
      // Check that it's a FormGroup
      if (group instanceof FormGroup && group.invalid) {
        console.log(`results[${i}] is invalid`, group.errors, group.value);

        Object.keys(group.controls).forEach(key => {
          const childControl = group.get(key);
          if (childControl && childControl.invalid) {
            console.log(` - ${key} invalid`, childControl.errors);
          }
        });
      }
    });
  }

  this.form.markAllAsTouched();
  return;
}

    this.isSaving = true;

    const payload = {
      labOrderId: this.form.value.labOrderId,
      results: this.resultsFormArray.value.map((x: any) => ({
        labTestVariableId: x.labTestVariableId,
        resultValue: x.resultValue
      }))
    };

 (await this.labOrderService.saveLabResult(payload)).subscribe({
      next: (res: any) => {
        this.isSaving = false;
        if (res?.Status === 200) {
          this.notifications.showNotification('Lab Result Saved Successfully!', 'snack-bar-success');
        this.dialogRef.close(true);
        } else {
          this.notifications.showNotification(res?.Message || 'Unable to save lab order.', 'snack-bar-danger');
        }
      },
      error: (error: any) => {
        this.isSaving = false;
        const msg = error?.error?.Message || 'An unexpected error occurred.';
        this.notifications.showNotification(msg, 'snack-bar-danger');
      }
    });
  }
}