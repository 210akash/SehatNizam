import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { AppointmentService } from '../appointment.service';
import { ConstantService } from '../../../../Service/constant.service';
import { AuthenticationService } from '../../../../Auth/authentication.service';
import { NotificationsService } from '../../../../Service/notification.service';

@Component({
  selector: 'app-confirm-appointment',
  templateUrl: './confirm-appointment.component.html',
  styleUrls: ['./confirm-appointment.component.css'],
  standalone: false
})
export class ConfirmAppointmentComponent {
  currentUser: any;
  isSubmitting = false;

  constructor(
    private appointmentService: AppointmentService,
    private constantService: ConstantService,
    private authenticationService: AuthenticationService,
    private notifications: NotificationsService,
    private dialogRef: MatDialogRef<ConfirmAppointmentComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authenticationService.currentUserValue;
  }

  onConfirm(): void {
    if (this.isSubmitting) return;
    this.isSubmitting = true;

    this.appointmentService.confirmAppointment(this.data.element.id).subscribe({
      next: (data: any) => {
        this.isSubmitting = false;
        if (data.item1 === 200) {
          this.notifications.showNotification(
            data.Data || 'Appointment confirmed successfully!',
            'snack-bar-success'
          );
          this.dialogRef.close(true);
        } else {
          this.notifications.showNotification(
            data.Message || data.Data || 'Failed to confirm appointment.',
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

    this.appointmentService.cancelAppoinment(this.data.element.id).subscribe({
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
            data.Message || data.Data || 'Failed to confirm appointment.',
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
