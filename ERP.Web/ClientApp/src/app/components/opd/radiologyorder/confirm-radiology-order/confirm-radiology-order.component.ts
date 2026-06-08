import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';
import { AuthenticationService } from '../../../../Auth/authentication.service';
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
export class ConfirmRadiologyOrderComponent {
  currentUser: any;
  isSubmitting = false;
  form!: FormGroup;
  paymentModesList: Array<{ id: number; name: string }> = [];

  constructor(
    private fb: FormBuilder,
    private radiologyOrderService: RadiologyOrderService,
    private constantService: ConstantService,
    private authenticationService: AuthenticationService,
    private notifications: NotificationsService,
    private paymentModeService: PaymentModeService,
    private dialogRef: MatDialogRef<ConfirmRadiologyOrderComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  ngOnInit(): void {
    this.form = this.fb.group({
      id: [this.data.element.id],
      basePrice: [this.data.element.radiologyOrderType.service.basePrice, Validators.required],
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

  async onConfirm(): Promise<void> {
    if (this.isSubmitting) return;
    this.isSubmitting = true;

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.notifications.showNotification('Please fill all required fields.', 'snack-bar-danger');
      return;
    }
    const command = this.form.value;
    (await this.radiologyOrderService.confirmRadiologyOrder(command)).subscribe({
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
            data.Message || data.Data || 'Failed to confirm radiology-order.',
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

  // onCancel(): void {
  //   if (this.isSubmitting) return;
  //   this.isSubmitting = true;

  //   this.radiologyOrderService.cancelRadiologyOrder(this.data.element.id).subscribe({
  //     next: (data: any) => {
  //       this.isSubmitting = false;
  //       if (data.item1 === 200) {
  //         this.notifications.showNotification(
  //           data.Data || 'Appointment cancel successfully!',
  //           'snack-bar-success'
  //         );
  //         this.dialogRef.close(true);
  //       } else {
  //         this.notifications.showNotification(
  //           data.Message || data.Data || 'Failed to confirm radiology-order.',
  //           'snack-bar-danger'
  //         );
  //       }
  //     },
  //     error: (error: any) => {
  //       this.isSubmitting = false;
  //       const message =
  //         error?.error?.Message ||
  //         error?.error?.Data ||
  //         error.statusText ||
  //         'An unexpected error occurred.';
  //       this.notifications.showNotification(message, 'snack-bar-danger');
  //     }
  //   });
  // }

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

  closeDialog(): void {
    this.dialogRef.close(false);
  }
}
