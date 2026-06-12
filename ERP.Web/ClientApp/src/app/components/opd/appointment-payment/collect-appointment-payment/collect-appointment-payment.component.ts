import { Component, Inject, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { ConstantService } from '../../../../Service/constant.service';
import { AppointmentPaymentService } from '../appointment-payment.service';
import { PaymentModeService } from '../../../paymentmode/paymentmode.service';
import { getBillingStatusLabel } from '../appointment-payment.util';

@Component({
    selector: 'app-collect-appointment-payment',
    templateUrl: './collect-appointment-payment.component.html',
    styleUrls: ['./collect-appointment-payment.component.css'],
    standalone: false
})
export class CollectAppointmentPaymentComponent implements OnInit {
    form!: FormGroup;
    isLoading = false;
    isSaving = false;
    paymentModesList: any[] = [];
    isSinglePayment = false;

    constructor(
        private formBuilder: FormBuilder,
        private service: AppointmentPaymentService,
        private paymentModeService: PaymentModeService,
        private notificationsService: NotificationsService,
        private constantService: ConstantService,
        private dialogRef: MatDialogRef<CollectAppointmentPaymentComponent>,
        @Inject(MAT_DIALOG_DATA) public data: {
            appointment: any;
            appointmentId: number;
            paymentId?: number;
        }
    ) { }

    ngOnInit(): void {
        this.isSinglePayment = !!this.data.paymentId;
        this.form = this.formBuilder.group({
            paymentModeId: [1, Validators.required],
            lines: this.formBuilder.array([])
        });

        this.paymentModeService.getAllPaymentModes({})
            .subscribe((res: any) => {
                this.paymentModesList = res?.item1 ?? [];
                const hasCash = this.paymentModesList.some((m: any) => m.id === 1);
                if (!hasCash && this.paymentModesList.length > 0) {
                    this.form.get('paymentModeId')?.setValue(this.paymentModesList[0].id);
                }
            });

        this.loadPendingPayments();
    }

    get lines(): FormArray {
        return this.form.get('lines') as FormArray;
    }

    get patientName(): string {
        return this.data.appointment?.patient?.patientMaster?.name ?? '';
    }

    get mrn(): string {
        return this.data.appointment?.patient?.mrn ?? '';
    }

    get tokenNumber(): string {
        return this.data.appointment?.tokenNumber ?? '';
    }

    get departmentName(): string {
        return this.data.appointment?.department?.name ?? '';
    }

    get dialogTitle(): string {
        return this.isSinglePayment ? 'Collect Payment' : 'Collect Payments';
    }

    get totalVisitFee(): number {
        return this.lines.controls.reduce((sum, ctrl) => sum + (Number(ctrl.get('visitFee')?.value) || 0), 0);
    }

    get totalDiscount(): number {
        return this.lines.controls.reduce((sum, ctrl) => sum + (Number(ctrl.get('discount')?.value) || 0), 0);
    }

    get grandTotal(): number {
        return this.lines.controls.reduce((sum, ctrl) => sum + this.getLinePayableFromGroup(ctrl as FormGroup), 0);
    }

    getStatusLabel(statusId: number): string {
        return getBillingStatusLabel(statusId);
    }

    loadPendingPayments(): void {
        this.isLoading = true;
        const startDate = new Date();
        startDate.setFullYear(startDate.getFullYear() - 5);

        const request = {
            appointmentId: this.data.appointmentId,
            fDate: this.constantService.formatDate(startDate),
            tDate: this.constantService.formatDate(new Date()),
            PagingData: { currentPage: 0, take: 500 }
        };

        this.service.getAll(request).subscribe({
            next: (res: any) => {
                let rows = (res?.item1 ?? []).filter((x: any) => x.paymentStatusId === 1 || x.paymentStatusId === 2);
                if (this.data.paymentId) {
                    rows = rows.filter((x: any) => x.id === this.data.paymentId);
                }

                this.lines.clear();
                rows.forEach((row: any) => this.lines.push(this.createLineGroup(row)));
                this.isLoading = false;
            },
            error: () => this.isLoading = false
        });
    }

    createLineGroup(row: any): FormGroup {
        const group = this.formBuilder.group({
            id: [row.id],
            serviceName: [row.service?.name ?? ''],
            visitFee: [row.visitFee ?? 0],
            discount: [row.discount ?? 0, [Validators.required, Validators.min(0)]],
            totalPayable: [row.totalPayable ?? 0],
            paymentStatusId: [row.paymentStatusId]
        });

        group.get('discount')?.valueChanges.subscribe(() => {
            this.recalculateLine(group);
        });

        return group;
    }

    getLinePayable(index: number): number {
        return this.getLinePayableFromGroup(this.lines.at(index) as FormGroup);
    }

    private getLinePayableFromGroup(line: FormGroup): number {
        const visitFee = Number(line.get('visitFee')?.value) || 0;
        const discount = Number(line.get('discount')?.value) || 0;
        return Number(Math.max(visitFee - discount, 0).toFixed(2));
    }

    recalculateLine(line: FormGroup): void {
        const visitFee = Number(line.get('visitFee')?.value) || 0;
        let discount = Number(line.get('discount')?.value) || 0;

        if (discount < 0) {
            discount = 0;
            line.get('discount')?.setValue(0, { emitEvent: false });
            this.notificationsService.showNotification('Discount cannot be negative.', 'snack-bar-danger');
        } else if (discount > visitFee) {
            discount = 0;
            line.get('discount')?.setValue(0, { emitEvent: false });
            this.notificationsService.showNotification('Discount cannot be greater than visit fee.', 'snack-bar-danger');
        }

        const total = Number(Math.max(visitFee - discount, 0).toFixed(2));
        line.get('totalPayable')?.setValue(total);
    }

    selectPaymentMode(modeId: number): void {
        this.form.get('paymentModeId')?.setValue(modeId);
    }

    isPaymentModeSelected(modeId: number): boolean {
        return this.form.get('paymentModeId')?.value === modeId;
    }

    approvePayments(): void {
        if (this.lines.length === 0) {
            return;
        }

        if (this.form.invalid) {
            this.constantService.markFormGroupTouched(this.form);
            return;
        }

        for (let i = 0; i < this.lines.length; i++) {
            this.recalculateLine(this.lines.at(i) as FormGroup);
        }

        this.isSaving = true;
        const payload = {
            appointmentId: this.data.appointmentId,
            paymentModeId: this.form.value.paymentModeId,
            payments: this.lines.getRawValue().map((line: any) => ({
                id: line.id,
                discount: Number(line.discount) || 0
            }))
        };

        this.service.approvePayments(payload).subscribe({
            next: (data: any) => {
                if (data.Status === 200) {
                    this.notificationsService.showNotification(data.Data, 'snack-bar-success');
                    this.dialogRef.close(true);
                } else {
                    this.notificationsService.showNotification(data.Data, 'snack-bar-danger');
                }
                this.isSaving = false;
            },
            error: (error: any) => {
                this.notificationsService.showNotification(error, 'snack-bar-danger');
                this.isSaving = false;
            }
        });
    }

    close(): void {
        this.dialogRef.close(false);
    }
}
