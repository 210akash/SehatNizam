import { Component, Inject } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DATE_LOCALE } from '@angular/material/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { Observable, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, finalize, map, startWith, switchMap } from 'rxjs/operators';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { AppointmentService } from '../../../opd/appointment/appointment.service';
import { BloodGroupService } from '../../blood-group/blood-group.service';
import { ComponentTypeService } from '../../component-type/component-type.service';
import { BloodRequestService } from '../blood-request.service';
import { bloodBankCnicValidators, bloodBankNameValidators } from '../../shared/blood-bank-input.utils';

@Component({
    selector: 'app-add-blood-request',
    templateUrl: './add-blood-request.component.html',
    styleUrl: './add-blood-request.component.css',
    standalone: false,
    providers: [
        { provide: MAT_DATE_LOCALE, useValue: 'en-GB' }
    ]
})
export class AddBloodRequestComponent {
    form!: FormGroup;
    appointmentSearchCtrl = new FormControl<string | any>('');
    filteredAppointments$!: Observable<any[]>;
    selectedAppointment: any = null;
    appointmentLoading = false;
    isLoading = false;
    isEditMode = false;
    isViewMode = false;
    bloodGroupList: any[] = [];
    componentTypeList: any[] = [];
    currentStatus = 1;
    statusMap: { [key: number]: string } = {
        1: 'Pending',
        2: 'Cross Matched',
        3: 'Issued',
        4: 'Cancelled'
    };

    constructor(
        private dialog: MatDialog,
        private notificationsService: NotificationsService,
        private formBuilder: FormBuilder,
        private service: BloodRequestService,
        private bloodGroupService: BloodGroupService,
        private componentTypeService: ComponentTypeService,
        private appointmentService: AppointmentService,
        private constantService: ConstantService,
        @Inject(MAT_DIALOG_DATA) public data: { element: any; isViewMode?: boolean }
    ) { }

    ngOnInit(): void {
        this.isViewMode = this.data.isViewMode === true;
        this.form = this.formBuilder.group({
            id: [0],
            appointmentId: [null],
            patientName: ['', bloodBankNameValidators()],
            patientCNIC: ['', bloodBankCnicValidators()],
            bloodGroupMasterId: ['', Validators.required],
            bloodComponentTypeId: ['', Validators.required],
            quantity: ['', [Validators.required, Validators.min(1)]],
            requestDate: [new Date(), Validators.required],
            status: [1, Validators.required],
            remarks: ['']
        });

        this.setupAppointmentAutocomplete();
        this.loadBloodGroups();
        this.loadComponentTypes();
        this.loadData(this.data.element);

        if (this.isViewMode) {
            this.appointmentSearchCtrl.disable();
        }
    }

    get dialogTitle(): string {
        if (this.isViewMode) return 'View Blood Request';
        return this.isEditMode ? 'Edit Blood Request' : 'Add Blood Request';
    }

    get statusText(): string {
        return this.statusMap[this.currentStatus] || 'Pending';
    }

    get showStatusField(): boolean {
        return this.isEditMode || this.isViewMode;
    }

    get isRequestEditable(): boolean {
        if (this.isViewMode) return false;
        if (!this.isEditMode) return true;
        return this.currentStatus === 1;
    }

    displayAppointment = (appointment: any): string => {
        if (!appointment) return '';
        if (typeof appointment === 'string') return appointment;
        const token = appointment.tokenNumber ? `Token # ${appointment.tokenNumber}` : `Booking # ${appointment.id}`;
        const patientName = appointment?.patient?.patientMaster?.name || appointment?.patient?.name || '';
        return patientName ? `${token} - ${patientName}` : token;
    };

    loadBloodGroups() {
        this.bloodGroupService.getAll({ PagingData: { currentPage: 0, take: 1000 } }).subscribe((data: any) => {
            this.bloodGroupList = data.item1 || [];
        });
    }

    loadComponentTypes() {
        this.componentTypeService.getAll({ PagingData: { currentPage: 0, take: 1000 } }).subscribe((data: any) => {
            this.componentTypeList = data.item1 || [];
        });
    }

    onAppointmentSelected(appointment: any) {
        if (!appointment?.id) return;
        this.applySelectedAppointment(appointment);
        const master = appointment?.patient?.patientMaster;
        this.form.patchValue({
            patientName: master?.name || appointment?.patient?.name || '',
            patientCNIC: master?.cnic || appointment?.patient?.cnic || ''
        });
    }

    onAppointmentInputCleared(event: Event): void {
        const value = (event.target as HTMLInputElement)?.value?.trim() ?? '';
        if (value.length > 0) return;
        this.resetAppointmentSelection();
    }

    clearAppointment(): void {
        this.resetAppointmentSelection(true);
    }

    loadData(element: any) {
        if (element == null) return;

        this.isEditMode = !this.isViewMode;
        this.isLoading = true;

        this.service.getById(element.id).subscribe({
            next: (response: any) => {
                const request = response || element;
                this.currentStatus = request?.status || 1;
                this.form.patchValue({
                    ...request,
                    status: this.currentStatus,
                    requestDate: request?.requestDate ? new Date(request.requestDate) : new Date()
                });

                this.syncAppointmentFromRequest(request);

                if (this.isViewMode || !this.isRequestEditable) {
                    this.form.disable();
                    this.appointmentSearchCtrl.disable();
                }

                this.isLoading = false;
            },
            error: () => {
                this.notificationsService.showNotification('Failed to load blood request details', 'snack-bar-danger');
                this.isLoading = false;
            }
        });
    }

    saveData() {
        if (this.isViewMode) return;

        if (this.form.invalid) {
            this.constantService.markFormGroupTouched(this.form);
            return;
        }

        this.isLoading = true;
        const payload = {
            ...this.form.getRawValue(),
            status: 1,
            requestDate: this.formatDateForSave(this.form.get('requestDate')?.value)
        };
        this.service.save(payload).subscribe({
            next: (data: { Status: number; Data: string }) => {
                if (data.Status == 200) {
                    this.notificationsService.showNotification(data.Data, 'snack-bar-success');
                    this.dialog.closeAll();
                } else {
                    this.notificationsService.showNotification(data.Data, 'snack-bar-danger');
                }
                this.isLoading = false;
            },
            error: (error: string) => {
                this.notificationsService.showNotification(error, 'snack-bar-danger');
                this.isLoading = false;
            }
        });
    }

    blockInvalidQuantity(event: KeyboardEvent): boolean {
        const key = event.key;
        if (key === 'Backspace' || key === 'Delete' || key === 'Tab' || key === 'ArrowLeft' || key === 'ArrowRight') {
            return true;
        }
        return /^\d$/.test(key);
    }

    onQuantityInput(event: Event): void {
        const input = event.target as HTMLInputElement;
        const value = Number(input.value);
        if (!input.value || value < 1) {
            this.form.get('quantity')?.setValue(null);
        }
    }

    private applySelectedAppointment(appointment: any): void {
        this.selectedAppointment = appointment;
        this.appointmentSearchCtrl.setValue(appointment, { emitEvent: false });
        this.form.patchValue({ appointmentId: appointment.id });
    }

    private resetAppointmentSelection(resetControl = true): void {
        this.selectedAppointment = null;
        this.form.patchValue({ appointmentId: null });
        if (resetControl) {
            this.appointmentSearchCtrl.setValue('', { emitEvent: false });
        }
    }

    private syncAppointmentFromRequest(request: any): void {
        const appointmentId = request?.appointmentId;
        if (!appointmentId) {
            this.resetAppointmentSelection(false);
            return;
        }

        if (request?.appointment?.id) {
            this.applySelectedAppointment(request.appointment);
            return;
        }

        this.appointmentService.getAppointmentById(appointmentId).subscribe({
            next: (response: any) => {
                if (response?.id) {
                    this.applySelectedAppointment(response);
                }
            }
        });
    }

    private setupAppointmentAutocomplete(): void {
        this.filteredAppointments$ = this.appointmentSearchCtrl.valueChanges.pipe(
            startWith(''),
            debounceTime(300),
            distinctUntilChanged(),
            switchMap((value: string | any) => {
                const term = typeof value === 'string'
                    ? value.trim()
                    : String(value?.tokenNumber ?? value?.id ?? '').trim();
                if (!term) return of([]);
                this.appointmentLoading = true;
                return this.appointmentService.getAppointmentByToken(term, 0).pipe(
                    map((data: any) => data?.item1 ?? data ?? []),
                    finalize(() => (this.appointmentLoading = false))
                );
            })
        );

        this.appointmentSearchCtrl.valueChanges.subscribe((value) => {
            if (typeof value === 'string' && value.trim() === '') {
                this.resetAppointmentSelection(false);
            }
        });
    }

    private formatDateForSave(value: any): string | null {
        if (!value) return null;
        const date = value instanceof Date ? value : new Date(value);
        if (isNaN(date.getTime())) return null;
        return date.toISOString().split('T')[0];
    }
}
