import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';
import { AuthenticationService } from '../../../../Auth/authentication.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { LabOrderService } from '../lab-order.service';

@Component({
  selector: 'app-confirm-lab-order',
  templateUrl: './confirm-lab-order.component.html',
  styleUrls: ['./confirm-lab-order.component.css'],
  standalone: false
})
export class ConfirmLabOrderComponent {
  currentUser: any;
  isSubmitting = false;

  constructor(
    private labOrderService: LabOrderService,
    private constantService: ConstantService,
    private authenticationService: AuthenticationService,
    private notifications: NotificationsService,
    private dialogRef: MatDialogRef<ConfirmLabOrderComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authenticationService.currentUserValue;
  }

  onConfirm(): void {
    if (this.isSubmitting) return;
    this.isSubmitting = true;

    this.labOrderService.confirmLabOrder(this.data.element.id).subscribe({
      next: (data: any) => {
        this.isSubmitting = false;
        if (data.item1 === 200) {
          this.notifications.showNotification(
            data.Data || 'Lab order confirmed successfully!',
            'snack-bar-success'
          );
          this.dialogRef.close(true);
        } else {
          this.notifications.showNotification(
            data.Message || data.Data || 'Failed to confirm lab-order.',
            'snack-bar-danger'
          );
        }
      },
      error: (error: any) => {
        this.isSubmitting = false;
        const message =
          error?.error?.Message ||
          error?.error?.Data ||
          error.statusText ||
          'An unexpected error occurred.';
        this.notifications.showNotification(message, 'snack-bar-danger');
      }
    });
  }

  onCancel(): void {
    if (this.isSubmitting) return;
    this.isSubmitting = true;

    this.labOrderService.cancelLabOrder(this.data.element.id).subscribe({
      next: (data: any) => {
        this.isSubmitting = false;
        if (data.item1 === 200) {
          this.notifications.showNotification(
            data.Data || 'Appointment cancel successfully!',
            'snack-bar-success'
          );
          this.dialogRef.close(true);
        } else {
          this.notifications.showNotification(
            data.Message || data.Data || 'Failed to confirm lab-order.',
            'snack-bar-danger'
          );
        }
      },
      error: (error: any) => {
        this.isSubmitting = false;
        const message =
          error?.error?.Message ||
          error?.error?.Data ||
          error.statusText ||
          'An unexpected error occurred.';
        this.notifications.showNotification(message, 'snack-bar-danger');
      }
    });
  }

  closeDialog(): void {
    this.dialogRef.close(false);
  }

  getPatientName(): string {
    return this.data?.element?.patient?.name || '-';
  }

  getPatientPhone(): string {
    return this.data?.element?.patient?.phoneNo || '-';
  }

  getAgeGender(): string {
    const patient = this.data?.element?.patient;
    if (!patient) return '-';
    const age = patient.age ?? '-';
    const gender = patient.gender || '-';
    return `${age} / ${gender}`;
  }

  getDoctorName(): string {
    const doctor = this.data?.element?.doctor;
    if (!doctor) return '-';
    return `${doctor.firstName || ''} ${doctor.lastName || ''}`.trim() || '-';
  }

  getDepartmentName(): string {
    return this.data?.element?.department?.name || '-';
  }

  getAppointmentDateTime(): string {
    const d = new Date(this.data?.element?.appointmentDate);
    if (Number.isNaN(d.getTime())) return '-';
    return d.toLocaleString('en-US', {
      day: '2-digit',
      month: 'short',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      hour12: true
    });
  }

  getVisitType(): string {
    return this.data?.element?.visitType?.name || '-';
  }

  getReason(): string {
    return this.data?.element?.reason || '-';
  }
}
