import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { ConstantService } from '../../../../Service/constant.service';
import { AppointmentPaymentService } from '../appointment-payment.service';
import { PaymentModeService } from '../../../paymentmode/paymentmode.service';
import { getBillingStatusLabel } from '../appointment-payment.util';

@Component({
    selector: 'app-edit-appointment-payment',
    templateUrl: './edit-appointment-payment.component.html',
    styleUrls: ['./edit-appointment-payment.component.css'],
    standalone: false
})
export class EditAppointmentPaymentComponent implements OnInit {
    form!: FormGroup;
    isLoading = false;
    paymentModesList: any[] = [];

    constructor(
        private formBuilder: FormBuilder,
        private service: AppointmentPaymentService,
        private paymentModeService: PaymentModeService,
        private notificationsService: NotificationsService,
        private constantService: ConstantService,
        private dialogRef: MatDialogRef<EditAppointmentPaymentComponent>,
        @Inject(MAT_DIALOG_DATA) public data: { element: any }
    ) { }

    ngOnInit(): void {
        const element = this.data.element;
        this.form = this.formBuilder.group({
            id: [element.id],
            appointmentId: [element.appointmentId],
            serviceId: [element.serviceId],
            serviceName: [{ value: element.service?.name ?? '', disabled: true }],
            visitFee: [{ value: element.visitFee, disabled: true }],
            discount: [element.discount ?? 0, [Validators.required, Validators.min(0)]],
            totalPayable: [{ value: element.totalPayable, disabled: true }],
            paymentModeId: [element.paymentModeId, Validators.required],
            paymentStatusId: [{ value: element.paymentStatusId, disabled: true }],
            paymentStatusTitle: [{ value: getBillingStatusLabel(element.paymentStatusId), disabled: true }]
        });

        this.paymentModeService.getAllPaymentModes({})
            .subscribe((res: any) => this.paymentModesList = res?.item1 ?? []);
    }

    calculateTotalPayable(): void {
        const fee = Number(this.form.getRawValue().visitFee) || 0;
        let discount = Number(this.form.get('discount')?.value) || 0;

        if (discount > fee) {
            discount = 0;
            this.form.get('discount')?.setValue(0, { emitEvent: false });
            this.notificationsService.showNotification('Discount cannot be greater than visit fee.', 'snack-bar-danger');
        }

        const total = Math.max(fee - discount, 0);
        this.form.get('totalPayable')?.setValue(Number(total.toFixed(2)), { emitEvent: false });
    }

    save(): void {
        if (this.form.invalid) {
            this.constantService.markFormGroupTouched(this.form);
            return;
        }

        this.isLoading = true;
        const raw = this.form.getRawValue();
        const payload = {
            id: raw.id,
            appointmentId: raw.appointmentId,
            serviceId: raw.serviceId,
            visitFee: raw.visitFee,
            discount: raw.discount,
            totalPayable: raw.totalPayable,
            paymentModeId: raw.paymentModeId,
            paymentStatusId: raw.paymentStatusId
        };

        this.service.savePayment(payload).subscribe({
            next: (data: any) => {
                if (data.Status === 200) {
                    this.notificationsService.showNotification(data.Data, 'snack-bar-success');
                    this.dialogRef.close(true);
                } else {
                    this.notificationsService.showNotification(data.Data, 'snack-bar-danger');
                }
                this.isLoading = false;
            },
            error: (error: any) => {
                this.notificationsService.showNotification(error, 'snack-bar-danger');
                this.isLoading = false;
            }
        });
    }
}
