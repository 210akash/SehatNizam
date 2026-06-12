import { Component, Injectable, OnInit, ViewChild } from '@angular/core';
import { DateAdapter, MAT_DATE_FORMATS, NativeDateAdapter } from '@angular/material/core';
import { FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ConstantService } from '../../../../Service/constant.service';
import { AppointmentPaymentService } from '../appointment-payment.service';
import { ServiceService } from '../../service/service.service';
import { ManageAppointmentBillingComponent } from '../manage-appointment-billing/manage-appointment-billing.component';
import { CollectAppointmentPaymentComponent } from '../collect-appointment-payment/collect-appointment-payment.component';
import { getBillingStatusLabel, isPaidStatus, isUnPaidStatus } from '../appointment-payment.util';

type PaymentViewMode = 'appointment' | 'records';

@Injectable()
class AppointmentPaymentDateAdapter extends NativeDateAdapter {
    override format(date: Date): string {
        if (!this.isValid(date)) {
            return '';
        }

        const day = date.getDate().toString().padStart(2, '0');
        const month = date.toLocaleString('en-US', { month: 'short' });
        const year = date.getFullYear();
        return `${day}-${month}-${year}`;
    }
}

const APPOINTMENT_PAYMENT_DATE_FORMATS = {
    parse: { dateInput: 'input' },
    display: {
        dateInput: 'input',
        monthYearLabel: 'MMM YYYY',
        dateA11yLabel: 'LL',
        monthYearA11yLabel: 'MMMM YYYY',
    },
};

@Component({
    selector: 'app-appointment-payment-list',
    templateUrl: './appointment-payment-list.component.html',
    styleUrls: ['./appointment-payment-list.component.css'],
    standalone: false,
    providers: [
        { provide: DateAdapter, useClass: AppointmentPaymentDateAdapter },
        { provide: MAT_DATE_FORMATS, useValue: APPOINTMENT_PAYMENT_DATE_FORMATS },
    ]
})
export class AppointmentPaymentListComponent implements OnInit {
    filterForm!: FormGroup;
    isLoading = false;
    viewMode: PaymentViewMode = 'appointment';
    currentPage = 0;
    pageSizeOptions: number[] = [10, 25, 50, 100];

    recordColumns: string[] = [
        'tokenNumber',
        'mrn',
        'patientName',
        'department',
        'service',
        'visitFee',
        'discount',
        'totalPayable',
        'status',
        'actions'
    ];

    appointmentColumns: string[] = [
        'tokenNumber',
        'mrn',
        'patientName',
        'department',
        'paymentCounts',
        'lastCreatedDate',
        'actions'
    ];

    recordDataSource = new MatTableDataSource<any>([]);
    appointmentDataSource = new MatTableDataSource<any>([]);
    take = 10;
    totalRows = 0;

    services: any[] = [];
    serviceSearchControl = new FormControl('');
    private serviceIdsBeforeClose: number[] = [];
    paymentStatusOptions = [
        { value: null, name: 'All' },
        { value: -1, name: 'UnPaid' },
        { value: 3, name: 'Paid' }
    ];

    @ViewChild(MatPaginator) paginator!: MatPaginator;
    @ViewChild(MatSort) sort!: MatSort;

    constructor(
        private service: AppointmentPaymentService,
        private serviceService: ServiceService,
        private constantService: ConstantService,
        private formBuilder: FormBuilder,
        private dialog: MatDialog
    ) { }

    ngOnInit(): void {
        this.take = this.constantService.defaultItemPerPage;
        const { fDate, tDate } = this.getDefaultDateRange();
        this.filterForm = this.formBuilder.group({
            fDate: [fDate],
            tDate: [tDate],
            tokenNo: [''],
            mRN: [''],
            patientName: [''],
            paymentStatusId: [null],
            serviceIds: [[]]
        });

        this.setupDebouncedFilters();
        this.loadLookups();
        this.bindData();
    }

    get activeColumns(): string[] {
        return this.viewMode === 'appointment' ? this.appointmentColumns : this.recordColumns;
    }

    get activeDataSource(): MatTableDataSource<any> {
        return this.viewMode === 'appointment' ? this.appointmentDataSource : this.recordDataSource;
    }

    setupDebouncedFilters(): void {
        ['tokenNo', 'mRN', 'patientName'].forEach(field => {
            this.filterForm.get(field)?.valueChanges.pipe(
                debounceTime(400),
                distinctUntilChanged()
            ).subscribe(() => {
                this.currentPage = 0;
                this.bindData();
            });
        });
    }

    get filteredServices(): any[] {
        const term = (this.serviceSearchControl.value ?? '').toString().toLowerCase().trim();
        if (!term) {
            return this.services;
        }

        return this.services.filter((item: any) =>
            (item?.name ?? '').toLowerCase().includes(term));
    }

    loadLookups(): void {
        this.serviceService.getAllServices({})
            .subscribe((res: any) => this.services = res?.item1 ?? res ?? []);
    }

    onServiceFilterClosed(): void {
        const selected: number[] = this.filterForm.get('serviceIds')?.value ?? [];
        const prev = this.serviceIdsBeforeClose ?? [];
        const changed = selected.length !== prev.length
            || selected.some((id, index) => id !== prev[index]);

        this.serviceSearchControl.setValue('');
        if (changed) {
            this.filterData();
        }
    }

    onServiceFilterOpened(): void {
        this.serviceIdsBeforeClose = [...(this.filterForm.get('serviceIds')?.value ?? [])];
    }

    onViewModeChange(mode: PaymentViewMode): void {
        if (this.viewMode === mode) {
            return;
        }

        this.viewMode = mode;
        this.currentPage = 0;
        this.bindData();
    }

    bindData(): void {
        this.isLoading = true;
        const formValue = this.filterForm.value;
        const serviceIds: number[] = formValue.serviceIds ?? [];

        const request = {
            fDate: this.constantService.formatDate(formValue.fDate),
            tDate: this.constantService.formatDate(formValue.tDate),
            tokenNo: formValue.tokenNo,
            mRN: formValue.mRN,
            patientName: formValue.patientName,
            paymentStatusId: formValue.paymentStatusId,
            serviceIds: serviceIds.length > 0 ? serviceIds : null,
            PagingData: { currentPage: this.currentPage, take: this.take }
        };

        const request$ = this.viewMode === 'appointment'
            ? this.service.getGroups(request)
            : this.service.getAll(request);

        request$.subscribe({
            next: (data: any) => {
                const rows = data?.item1 ?? [];
                if (this.viewMode === 'appointment') {
                    this.appointmentDataSource = new MatTableDataSource(rows);
                    this.appointmentDataSource.sort = this.sort;
                } else {
                    this.recordDataSource = new MatTableDataSource(rows);
                    this.recordDataSource.sort = this.sort;
                }

                this.totalRows = data?.item2 ?? 0;

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

    pageChanged(event: PageEvent): void {
        this.take = event.pageSize;
        this.currentPage = event.pageIndex;
        this.bindData();
    }

    filterData(): void {
        this.currentPage = 0;
        this.bindData();
    }

    private getDefaultDateRange(): { fDate: Date; tDate: Date } {
        const tDate = new Date();
        const fDate = new Date();
        fDate.setDate(tDate.getDate() - 7);
        return { fDate, tDate };
    }

    resetFilters(): void {
        const { fDate, tDate } = this.getDefaultDateRange();
        this.filterForm.reset({
            fDate,
            tDate,
            tokenNo: '',
            mRN: '',
            patientName: '',
            paymentStatusId: null,
            serviceIds: []
        });
        this.serviceSearchControl.setValue('');
        this.currentPage = 0;
        this.bindData();
    }

    getPatientName(element: any): string {
        const appointment = element?.appointment ?? element;
        return appointment?.patient?.patientMaster?.name
            ?? appointment?.patient?.name
            ?? '';
    }

    getMrn(element: any): string {
        const appointment = element?.appointment ?? element;
        return appointment?.patient?.mrn ?? '';
    }

    getTokenNumber(element: any): string {
        const appointment = element?.appointment ?? element;
        return appointment?.tokenNumber ?? '';
    }

    getDepartmentName(element: any): string {
        const appointment = element?.appointment ?? element;
        return appointment?.department?.name ?? '';
    }

    getServiceName(element: any): string {
        return element?.service?.name ?? '';
    }

    getPaymentModeName(element: any): string {
        return element?.paymentMode?.name ?? '';
    }

    getPaymentStatus(element: any): string {
        return getBillingStatusLabel(element?.paymentStatusId);
    }

    isPendingStatus(statusId: number): boolean {
        return isUnPaidStatus(statusId);
    }

    isApprovedStatus(statusId: number): boolean {
        return isPaidStatus(statusId);
    }

    canEdit(element: any): boolean {
        return isUnPaidStatus(element?.paymentStatusId);
    }

    isPaid(element: any): boolean {
        return isPaidStatus(element?.paymentStatusId);
    }

    hasPendingPayments(element: any): boolean {
        return (element?.pendingPaymentCount ?? 0) > 0;
    }

    openCollectPayments(element: any, payment?: any): void {
        const appointment = payment?.appointment ?? element.appointment ?? element;
        const appointmentId = payment?.appointmentId ?? element.appointmentId ?? appointment?.id;

        this.dialog.open(CollectAppointmentPaymentComponent, {
            panelClass: 'cstm_width_800',
            maxHeight: '90vh',
            height: 'auto',
            data: {
                appointment,
                appointmentId,
                paymentId: payment?.id
            },
            disableClose: true
        }).afterClosed().subscribe((saved: boolean) => {
            if (saved) {
                this.bindData();
            }
        });
    }

    openManageBilling(element: any): void {
        const appointment = element.appointment ?? element;
        const appointmentId = element.appointmentId ?? appointment?.id;

        this.dialog.open(ManageAppointmentBillingComponent, {
            panelClass: 'cstm_width_900',
            maxHeight: '90vh',
            height: 'auto',
            data: { appointment, appointmentId },
            disableClose: true
        }).afterClosed().subscribe(() => this.bindData());
    }

}
