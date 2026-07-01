import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { AppointmentService } from '../appointment.service';
import { ConstantService } from '../../../../Service/constant.service';
import { AuthenticationService } from '../../../../Auth/authentication.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PaymentModeService } from '../../../paymentmode/paymentmode.service';
import { PrintReceiptAppoinmentComponent } from '../print-receipt-appoinment/print-receipt-appoinment.component';

@Component({
  selector: 'app-confirm-appointment',
  templateUrl: './confirm-appointment.component.html',
  styleUrls: ['./confirm-appointment.component.css'],
  standalone: false
})
export class ConfirmAppointmentComponent {
  currentUser: any;
  isSubmitting = false;
  form!: FormGroup;
  paymentModesList: any;
  constructor(
    private fb: FormBuilder,
    private appointmentService: AppointmentService,
    private constantService: ConstantService,
    private authenticationService: AuthenticationService,
    private notifications: NotificationsService,
    private paymentModeService: PaymentModeService,
    private dialogRef: MatDialogRef<ConfirmAppointmentComponent>,
    private dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  ngOnInit(): void {
    this.currentUser = this.authenticationService.currentUserValue;
    this.form = this.fb.group({
      id: [this.data.element.id],
      basePrice: [this.data.element.appointmentPayments[0]?.visitFee, Validators.required],
      paymentModeId: [5, Validators.required],
      discount: [0, Validators.required],
      payable: [0, Validators.required],
    });

    this.getAllPaymentModes();
    this.calculateTotalPayable();
  }

  getAllPaymentModes() {
    this.paymentModeService.getAllPaymentModes({})
      .subscribe((d: any) => this.paymentModesList = d?.item1 ?? []);
  }

  calculateTotalPayable() {
    const fee = Number(this.form.get('basePrice')?.value) || 0;
    const discount = Number(this.form.get('discount')?.value) || 0;
    if (discount > fee) {
      this.form.get('discount')?.setValue(0, { emitEvent: false });
      this.form.get('payable')?.setValue(fee, { emitEvent: false });
      this.notifications.showNotification('Discount can be greater than rate.', 'snack-bar-danger');
    }
    else {
      const total = fee - discount;
      var payable = total < 0 ? 0 : Number(total.toFixed(2));
      this.form.get('payable')?.setValue(payable, { emitEvent: false });
    }
  }

  async onConfirm(): Promise<void> {
    if (this.isSubmitting) return;
    this.isSubmitting = true;

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.notifications.showNotification('Please fill all required fields.', 'snack-bar-danger');
      return;
    }
    const command = this.form.value;
    (await this.appointmentService.confirmAppointment(command)).subscribe({
      next: (data: any) => {
        this.isSubmitting = false;
        if (data.Status === 200) {
          this.notifications.showNotification('appoinment confirmed successfully!',
            'snack-bar-success'
          );
          this.dialogRef.close(true);
    const camelCaseData = this.toCamelCaseObject(data.Data);
            this.printrecreiptAppoinmnetDialog(camelCaseData);
        } else {
          this.notifications.showNotification('Failed to confirm appoinment.',
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
    return this.data?.element?.patient?.patientMaster?.name || '-';
  }

  getPatientPhone(): string {
    return this.data?.element?.patient?.patientMaster?.phoneNo || '-';
  }

  getAgeGender(): string {
    const patient = this.data?.element?.patient?.patientMaster;
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

     printrecreiptAppoinmnetDialog(element: any) {
      const dialogRef = this.dialog.open(PrintReceiptAppoinmentComponent, {
        panelClass: 'cstm_width_400',
        maxHeight: '90vh',
        data: {
          element: element,
        },
        disableClose: true
      });
    }

      private toCamelCaseObject(obj: any): any {
        if (Array.isArray(obj)) {
          return obj.map(item => this.toCamelCaseObject(item));
        }
    
        if (obj !== null && typeof obj === 'object') {
          return Object.keys(obj).reduce((result: any, key) => {
            const camelKey = key.charAt(0).toLowerCase() + key.slice(1);
    
            result[camelKey] = this.toCamelCaseObject(obj[key]);
    
            return result;
          }, {});
        }
    
        return obj;
      }
}
