import { Component, Inject, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { HttpClient } from '@angular/common/http';
import { NotificationsService } from '../../../../Service/notification.service';
import { RadiologyOrderService } from '../radiologyorder.service';

@Component({
  selector: 'app-save-radiology-result',
  templateUrl: './save-radiology-result.component.html',
  styleUrls: ['./save-radiology-result.component.css'],
  standalone: false
})

export class SaveRadiologyResultComponent implements OnInit {

  form: FormGroup;
  isSaving = false;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    private radiologyOrderService: RadiologyOrderService,
    private notifications: NotificationsService,
    private dialogRef: MatDialogRef<SaveRadiologyResultComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.form = this.fb.group({
      radiologyOrderId: [0, Validators.required],
      results: this.fb.array([])
    });
  }

 ngOnInit(): void {

  const order = this.data.order;

  this.form.patchValue({
    radiologyOrderId: order.id
  });

  // 🔥 Patient Gender
  const gender = order.appointment?.patient?.gender?.toLowerCase();

  // 🔥 Variables
  const variables = order.radiologyOrderType?.variables || [];

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

    radiologyTestVariableId: [item.id],

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

save(): void {
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
      this.notifications.showNotification('Please complete all required fields.', 'snack-bar-danger');
      return;
    }

    this.isSaving = true;

    const payload = {
      radiologyOrderId: this.form.value.radiologyOrderId,
      results: this.resultsFormArray.value.map((x: any) => ({
        radiologyTestVariableId: x.radiologyTestVariableId,
        resultValue: x.resultValue
      }))
    };

    this.radiologyOrderService.saveRadiologyResult(payload).subscribe({
      next: (res: any) => {
        this.isSaving = false;
        if (res?.Status === 200) {
          this.notifications.showNotification('Radiology Result Saved Successfully!', 'snack-bar-success');
          this.dialogRef.close(true);
        } else {
          this.notifications.showNotification(res?.Message || 'Unable to save radiology order.', 'snack-bar-danger');
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