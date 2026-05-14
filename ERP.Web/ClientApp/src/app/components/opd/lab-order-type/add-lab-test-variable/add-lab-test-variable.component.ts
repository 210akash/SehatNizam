import { Component, Inject, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { LabOrderTypeService } from '../lab-order-type.service';
import { NotificationsService } from '../../../../Service/notification.service';

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
    private dialogRef: MatDialogRef<any>,
    private service: LabOrderTypeService,
    private notify: NotificationsService,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {}

  ngOnInit(): void {

    this.labOrderTypeId = this.data?.element?.id ?? 0;

    this.form = this.fb.group({
      labOrderTypeId: [this.labOrderTypeId, Validators.required],
      variables: this.fb.array([])
    });

    this.loadVariables();
  }

  // -----------------------------
  // GET VARIABLES ARRAY
  // -----------------------------
  get variables(): FormArray {
    return this.form.get('variables') as FormArray;
  }

  // -----------------------------
  // CREATE VARIABLE
  // -----------------------------
  createVariableGroup(v: any = {}): FormGroup {
    return this.fb.group({
      id: [v.id ?? 0],
      name: [v.name ?? '', Validators.required],
      unit: [v.unit ?? ''],

      resultType: [v.resultType ?? 1, Validators.required],
      displayOrder: [v.displayOrder ?? 0],

      maleMin: [v.maleMin ?? null],
      maleMax: [v.maleMax ?? null],
      femaleMin: [v.femaleMin ?? null],
      femaleMax: [v.femaleMax ?? null],

      hasGenderRange: [v.hasGenderRange ?? false],

      options: this.fb.array(
        (v.options ?? v.labTestVariableOptions ?? []).map((o: any) =>
          this.createOption(o)
        )
      )
    });
  }

  // -----------------------------
  // CREATE OPTION
  // -----------------------------
  createOption(o: any = {}): FormGroup {
    return this.fb.group({
      id: [o.id ?? 0],
      name: [o.name ?? '', Validators.required],
      displayOrder: [o.displayOrder ?? 0]
    });
  }

  // -----------------------------
  // LOAD DATA
  // -----------------------------
  loadVariables(): void {

    const raw = this.data?.element?.variables ?? [];

    if (raw.length > 0) {
      raw.forEach((v: any) => this.variables.push(this.createVariableGroup(v)));
    } else {
      this.variables.push(this.createVariableGroup());
    }
  }

  // -----------------------------
  // OPTIONS HELPERS
  // -----------------------------
  getOptions(index: number): FormArray {
    return this.variables.at(index).get('options') as FormArray;
  }

  addOption(index: number): void {
    this.getOptions(index).push(this.createOption());
  }

  removeOption(variableIndex: number, optionIndex: number): void {
    const options = this.getOptions(variableIndex);
    if (options.length > 1) {
      options.removeAt(optionIndex);
    } else {
      options.at(0).patchValue({ id: 0, name: '', displayOrder: 0 });
    }
  }

  closeDialog(): void {
    this.dialogRef.close(false);
  }

  isOptionType(index: number): boolean {
    const value = this.variables.at(index)?.get('resultType')?.value;
    return Number(value) === 3;
  }

  // -----------------------------
  // VARIABLE ROWS
  // -----------------------------
  addVariable(index: number): void {
    this.variables.insert(index + 1, this.createVariableGroup());
  }

  removeVariable(index: number): void {
    if (this.variables.length > 1) {
      this.variables.removeAt(index);
    }
  }

  // -----------------------------
  // SAVE
  // -----------------------------
  saveLabTestVariables(): void {

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.notify.showNotification('Please fill required fields', 'danger');
      return;
    }

    this.isLoading = true;

    const command = {
      labOrderTypeId: this.labOrderTypeId,
      variables: this.variables.value
    };

    this.service.saveLabTestVariables(command).subscribe({
      next: (res: any) => {
        this.isLoading = false;

        if (res.Status === 200) {
          this.notify.showNotification('Saved successfully', 'success');
          this.dialogRef.close(true);
        } else {
          this.notify.showNotification(res.Message || 'Error', 'danger');
        }
      },
      error: () => {
        this.isLoading = false;
        this.notify.showNotification('Server error', 'danger');
      }
    });
  }
}
