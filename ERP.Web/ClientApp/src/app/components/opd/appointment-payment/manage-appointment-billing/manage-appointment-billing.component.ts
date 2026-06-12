import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog, MatDialogRef } from '@angular/material/dialog';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { AppointmentPaymentService } from '../appointment-payment.service';
import { CollectAppointmentPaymentComponent } from '../collect-appointment-payment/collect-appointment-payment.component';
import { ConstantService } from '../../../../Service/constant.service';
import { getBillingStatusLabel, isPaidStatus, isUnPaidStatus } from '../appointment-payment.util';

@Component({
    selector: 'app-manage-appointment-billing',
    templateUrl: './manage-appointment-billing.component.html',
    styleUrls: ['./manage-appointment-billing.component.css'],
    standalone: false
})
export class ManageAppointmentBillingComponent implements OnInit {
    isLoading = false;
    currentPage = 0;
    take = 10;
    totalRows = 0;
    pageSizeOptions: number[] = [10, 25, 50, 100];
    displayedColumns: string[] = ['service', 'visitFee', 'discount', 'totalPayable', 'status', 'actions'];
    dataSource = new MatTableDataSource<any>([]);
    allRows: any[] = [];

    pendingCount = 0;
    approvedCount = 0;
    pendingPayable = 0;
    approvedPayable = 0;
    totalVisitFee = 0;
    totalDiscount = 0;
    grandPayable = 0;

    @ViewChild(MatPaginator) paginator!: MatPaginator;

    constructor(
        private service: AppointmentPaymentService,
        private constantService: ConstantService,
        private dialog: MatDialog,
        private dialogRef: MatDialogRef<ManageAppointmentBillingComponent>,
        @Inject(MAT_DIALOG_DATA) public data: { appointment: any; appointmentId: number }
    ) { }

    ngOnInit(): void {
        this.take = this.constantService.defaultItemPerPage;
        this.bindData();
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

    bindData(): void {
        this.isLoading = true;
        const startDate = new Date();
        startDate.setFullYear(startDate.getFullYear() - 5);

        const request = {
            appointmentId: this.data.appointmentId,
            fDate: this.constantService.formatDate(startDate),
            tDate: this.constantService.formatDate(new Date()),
            PagingData: { currentPage: 0, take: 1000 }
        };

        this.service.getAll(request).subscribe({
            next: (res: any) => {
                this.allRows = res?.item1 ?? [];
                this.totalRows = this.allRows.length;
                this.computeSummary(this.allRows);
                this.applyPage();

                setTimeout(() => {
                    if (this.paginator) {
                        this.paginator.pageIndex = this.currentPage;
                        this.paginator.length = this.totalRows;
                    }
                });

                this.isLoading = false;
            },
            error: () => this.isLoading = false
        });
    }

    private computeSummary(rows: any[]): void {
        const pending = rows.filter((x: any) => isUnPaidStatus(x.paymentStatusId));
        const approved = rows.filter((x: any) => isPaidStatus(x.paymentStatusId));

        this.pendingCount = pending.length;
        this.approvedCount = approved.length;
        this.pendingPayable = pending.reduce((sum, row) => sum + (row.totalPayable ?? 0), 0);
        this.approvedPayable = approved.reduce((sum, row) => sum + (row.totalPayable ?? 0), 0);
        this.totalVisitFee = rows.reduce((sum, row) => sum + (row.visitFee ?? 0), 0);
        this.totalDiscount = rows.reduce((sum, row) => sum + (row.discount ?? 0), 0);
        this.grandPayable = rows.reduce((sum, row) => sum + (row.totalPayable ?? 0), 0);
    }

    private applyPage(): void {
        const start = this.currentPage * this.take;
        const pageRows = this.allRows.slice(start, start + this.take);
        this.dataSource = new MatTableDataSource(pageRows);
    }

    pageChanged(event: PageEvent): void {
        this.take = event.pageSize;
        this.currentPage = event.pageIndex;
        this.applyPage();
    }

    getStatusLabel(statusId: number): string {
        return getBillingStatusLabel(statusId);
    }

    getPaymentModeName(element: any): string {
        return element?.paymentMode?.name ?? '';
    }

    canCollect(element: any): boolean {
        return isUnPaidStatus(element.paymentStatusId);
    }

    isApproved(element: any): boolean {
        return isPaidStatus(element.paymentStatusId);
    }

    openCollect(element: any): void {
        this.dialog.open(CollectAppointmentPaymentComponent, {
            panelClass: 'cstm_width_800',
            maxHeight: '90vh',
            height: 'auto',
            data: {
                appointment: this.data.appointment,
                appointmentId: this.data.appointmentId,
                paymentId: element.id
            },
            disableClose: true
        }).afterClosed().subscribe((saved: boolean) => {
            if (saved) {
                this.bindData();
                this.dialogRef.close(true);
            }
        });
    }

    openCollectAll(): void {
        this.dialog.open(CollectAppointmentPaymentComponent, {
            panelClass: 'cstm_width_800',
            maxHeight: '90vh',
            height: 'auto',
            data: {
                appointment: this.data.appointment,
                appointmentId: this.data.appointmentId
            },
            disableClose: true
        }).afterClosed().subscribe((saved: boolean) => {
            if (saved) {
                this.bindData();
                this.dialogRef.close(true);
            }
        });
    }

    close(): void {
        this.dialogRef.close(false);
    }
}
