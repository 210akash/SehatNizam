import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PaymentModeService } from '../../../paymentmode/paymentmode.service';
import { RadiologyOrderService } from '../radiologyorder.service';

@Component({
  selector: 'app-confirm-radiology-order',
  templateUrl: './confirm-radiology-order.component.html',
  styleUrls: ['./confirm-radiology-order.component.css'],
  standalone: false
})
export class ConfirmRadiologyOrderComponent implements OnInit {
  isSubmitting = false;
  form!: FormGroup;
  paymentModesList: Array<{ id: number; name: string }> = [];

  constructor(
    private fb: FormBuilder,
    private radiologyOrderService: RadiologyOrderService,
    private notifications: NotificationsService,
    private paymentModeService: PaymentModeService,
    private dialogRef: MatDialogRef<ConfirmRadiologyOrderComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) {
    const basePrice = this.getBasePrice();

    this.form = this.fb.group({
      id: [this.data?.element?.id ?? 0],
      basePrice: [basePrice, Validators.required],
      paymentModeId: [5, Validators.required],
      discount: [0, Validators.required],
      payable: [basePrice, Validators.required],
    });
  }

  ngOnInit(): void {
    this.getAllPaymentModes();
    this.calculateTotalPayable();
  }

  get patient(): any {
    const patient = this.data?.element?.appointment?.patient;
    return patient?.patientMaster ?? patient ?? {};
  }

  get radiologyTypeName(): string {
    return this.data?.element?.radiologyType?.name
      || this.data?.element?.radiologyOrderType?.name
      || '-';
  }

  private getBasePrice(): number {
    const radiologyType = this.data?.element?.radiologyType ?? this.data?.element?.radiologyOrderType;
    return Number(radiologyType?.service?.basePrice ?? 0);
  }

  getAllPaymentModes(): void {
    this.paymentModeService.getAllPaymentModes({})
      .subscribe((d: any) => this.paymentModesList = d?.item1 ?? []);
  }

  onConfirm(): void {
    if (this.isSubmitting) return;

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.notifications.showNotification('Please fill all required fields.', 'snack-bar-danger');
      return;
    }

    this.isSubmitting = true;
    const command = this.form.value;

    this.radiologyOrderService.confirmRadiologyOrder(command).subscribe({
      next: (data: any) => {
        this.isSubmitting = false;
        if (data.Status === 200) {
          this.notifications.showNotification(
            data.Message || 'Radiology order confirmed successfully!',
            'snack-bar-success'
          );
          this.dialogRef.close(true);
        } else {
          this.notifications.showNotification(
            data.Message || data.Data || 'Failed to confirm radiology order.',
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

  calculateTotalPayable(): void {
    if (!this.form) return;

    const fee = Number(this.form.get('basePrice')?.value) || 0;
    const discount = Number(this.form.get('discount')?.value) || 0;

    if (discount > fee) {
      this.form.get('discount')?.setValue(0, { emitEvent: false });
      this.form.get('payable')?.setValue(fee, { emitEvent: false });
      this.notifications.showNotification('Discount cannot be greater than rate.', 'snack-bar-danger');
      return;
    }

    const payable = Math.max(0, Number((fee - discount).toFixed(2)));
    this.form.get('payable')?.setValue(payable, { emitEvent: false });
  }

  closeDialog(): void {
    this.dialogRef.close(false);
  }
}
