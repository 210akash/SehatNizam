import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';

export interface DoctorPlanDetailData {
  type: 'appointment' | 'surgical';
  title: string;
  start: Date;
  end: Date;
  patientName: string;
  bookingNumber: string;
  serviceOrDept: string;
  status: string;
  notes: string;
}

@Component({
  selector: 'app-doctor-plan-detail-dialog',
  templateUrl: './doctor-plan-detail-dialog.component.html',
  styleUrls: ['./doctor-plan-detail-dialog.component.css'],
  standalone: false
})
export class DoctorPlanDetailDialogComponent {
  constructor(
    private dialogRef: MatDialogRef<DoctorPlanDetailDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DoctorPlanDetailData
  ) { }

  close(): void {
    this.dialogRef.close();
  }

  get typeLabel(): string {
    return this.data.type === 'appointment' ? 'Appointment' : 'Surgical Order';
  }

  get deptLabel(): string {
    return this.data.type === 'appointment' ? 'Department' : 'Service';
  }
}
