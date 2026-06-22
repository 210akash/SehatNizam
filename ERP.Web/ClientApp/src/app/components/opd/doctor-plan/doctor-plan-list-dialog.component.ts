import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { DoctorPlanDetailData } from './doctor-plan-detail-dialog.component';

export interface DoctorPlanListDialogData {
  type: 'appointment' | 'surgical';
  items: DoctorPlanDetailData[];
}

@Component({
  selector: 'app-doctor-plan-list-dialog',
  templateUrl: './doctor-plan-list-dialog.component.html',
  styleUrls: ['./doctor-plan-list-dialog.component.css'],
  standalone: false
})
export class DoctorPlanListDialogComponent {
  constructor(
    private dialogRef: MatDialogRef<DoctorPlanListDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DoctorPlanListDialogData
  ) { }

  get title(): string {
    return this.data.type === 'appointment' ? 'Appointments' : 'Surgical Orders';
  }

  select(item: DoctorPlanDetailData): void {
    this.dialogRef.close(item);
  }

  close(): void {
    this.dialogRef.close();
  }
}
