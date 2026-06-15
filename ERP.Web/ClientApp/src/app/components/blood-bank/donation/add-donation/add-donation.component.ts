import { Component, Inject } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DATE_LOCALE } from '@angular/material/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { Observable, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, finalize, map, startWith, switchMap } from 'rxjs/operators';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { AppointmentService } from '../../../opd/appointment/appointment.service';
import { ComponentTypeService } from '../../component-type/component-type.service';
import { DonorService } from '../../donor/donor.service';
import { DonationService } from '../donation.service';

@Component({
    selector: 'app-add-donation',
    templateUrl: './add-donation.component.html',
    styleUrl: './add-donation.component.css',
    standalone: false,
    providers: [
        { provide: MAT_DATE_LOCALE, useValue: 'en-GB' }
    ]
})
export class AddDonationComponent {
    form!: FormGroup;
    donorSearchCtrl = new FormControl<string | any>('');
    appointmentSearchCtrl = new FormControl<string | any>('');
    filteredDonors$!: Observable<any[]>;
    filteredAppointments$!: Observable<any[]>;
    selectedDonor: any = null;
    selectedAppointment: any = null;
    appointmentLoading = false;
    isLoading = false;
    isEditMode = false;
    isViewMode = false;
    donorList: any[] = [];
    componentTypeList: any[] = [];
    maxDate = new Date();
    isScreeningStatusLocked = false;
    screeningStatusList = [
        { value: 1, name: 'Pending' },
        { value: 2, name: 'Pass' },
        { value: 3, name: 'Fail' },
        { value: 4, name: 'Deferred' }
    ];

    constructor(
        private dialog: MatDialog,
        private notificationsService: NotificationsService,
        private formBuilder: FormBuilder,
        private service: DonationService,
        private donorService: DonorService,
        private componentTypeService: ComponentTypeService,
        private appointmentService: AppointmentService,
        private constantService: ConstantService,
        @Inject(MAT_DIALOG_DATA) public data: { element: any; isViewMode?: boolean; appointment?: any }
    ) { }

    ngOnInit(): void {
        this.isViewMode = this.data.isViewMode === true;
        this.form = this.formBuilder.group({
            id: [0],
            appointmentId: [null],
            bloodDonorId: ['', Validators.required],
            bloodComponentTypeId: ['', Validators.required],
            bloodGroupMasterId: ['', Validators.required],
            donationDate: [new Date(), Validators.required],
            volume: ['', Validators.required],
            screeningStatus: [1, Validators.required],
            remarks: ['']
        });

        this.setupDonorAutocomplete();
        this.setupAppointmentAutocomplete();
        this.loadComponentTypes();
        this.loadDonors(() => {
            this.prefillAppointment(this.data?.appointment);
            this.loadData(this.data.element);
        });

        if (this.isViewMode) {
            this.donorSearchCtrl.disable();
            this.appointmentSearchCtrl.disable();
        }
    }

    get dialogTitle(): string {
        if (this.isViewMode) return 'View Blood Donation';
        return this.isEditMode ? 'Edit Blood Donation' : 'Add Blood Donation';
    }

    displayDonor = (donor: any): string => {
        if (!donor || typeof donor === 'string') {
            return typeof donor === 'string' ? donor : '';
        }
        return this.getDonorLabel(donor);
    };

    loadData(element: any) {
        if (element == null) return;

        this.isEditMode = !this.isViewMode;
        this.isLoading = true;

        this.service.getById(element.id).subscribe({
            next: (response: any) => {
                const donation = response || element;
                this.form.patchValue({
                    ...donation,
                    donationDate: donation?.donationDate ? new Date(donation.donationDate) : new Date()
                });

                this.syncDonorFromForm();
                if (donation?.appointment) {
                    this.onAppointmentSelected(donation.appointment);
                } else {
                    this.syncAppointmentFromForm();
                }
                this.isScreeningStatusLocked = this.hasStorageAssigned(donation?.bloodUnit);

                if (this.isViewMode) {
                    this.form.disable();
                    this.donorSearchCtrl.disable();
                    this.appointmentSearchCtrl.disable();
                }

                this.isLoading = false;
            },
            error: () => {
                this.notificationsService.showNotification('Failed to load donation details', 'snack-bar-danger');
                this.isLoading = false;
            }
        });
    }

    loadDonors(onLoaded?: () => void) {
        this.donorService.getAll({ PagingData: { currentPage: 0, take: 1000 } }).subscribe((data: any) => {
            this.donorList = (data.item1 || []).filter((donor: any) => !donor.isDeferred);
            onLoaded?.();
        });
    }

    loadComponentTypes() {
        this.componentTypeService.getAll({ PagingData: { currentPage: 0, take: 1000 } }).subscribe((data: any) => {
            this.componentTypeList = data.item1 || [];
        });
    }

    onDonorSelected(donor: any): void {
        if (!donor?.id) return;

        this.selectedDonor = donor;
        this.form.patchValue({
            bloodDonorId: donor.id,
            bloodGroupMasterId: donor.bloodGroupMasterId || ''
        });
    }

    displayAppointment = (appointment: any): string => {
        if (!appointment) {
            return '';
        }

        if (typeof appointment === 'string') {
            return appointment;
        }

        const token = appointment.tokenNumber ? `Token # ${appointment.tokenNumber}` : `Booking # ${appointment.id}`;
        const patientName = this.getAppointmentPatientName(appointment);
        return patientName ? `${token} - ${patientName}` : token;
    };

    onAppointmentSelected(appointment: any): void {
        if (!appointment?.id) {
            return;
        }

        this.selectedAppointment = appointment;
        this.appointmentSearchCtrl.setValue(appointment, { emitEvent: false });
        this.form.patchValue({ appointmentId: appointment.id });
    }

    onAppointmentInputCleared(event: Event): void {
        const value = (event.target as HTMLInputElement)?.value?.trim() ?? '';
        if (value.length > 0) {
            return;
        }

        this.resetAppointmentSelection();
    }

    lookupAppointment(): void {
        const value = this.appointmentSearchCtrl.value;

        if (!value) {
            return;
        }

        if (typeof value === 'object' && value?.id) {
            this.onAppointmentSelected(value);
            return;
        }

        const term = String(value).trim();
        if (!term) {
            return;
        }

        this.appointmentLoading = true;
        this.appointmentService.getAppointmentByToken(term, 0)
            .pipe(finalize(() => (this.appointmentLoading = false)))
            .subscribe({
                next: (data: any) => {
                    const appointments = data?.item1 ?? data ?? [];
                    const appointment = Array.isArray(appointments) ? appointments[0] : appointments;

                    if (!appointment) {
                        this.notificationsService.showNotification('No appointment found for the entered token.', 'snack-bar-danger');
                        return;
                    }

                    this.onAppointmentSelected(appointment);
                },
                error: () => {
                    this.notificationsService.showNotification('Failed to lookup appointment.', 'snack-bar-danger');
                }
            });
    }

    getAppointmentPatientName(appointment: any = this.selectedAppointment): string {
        return appointment?.patient?.patientMaster?.name
            || appointment?.patient?.name
            || '';
    }

    getAppointmentPatientCnic(appointment: any = this.selectedAppointment): string {
        return appointment?.patient?.patientMaster?.cnic
            || appointment?.patient?.cnic
            || '';
    }

    getDonorBloodGroupName(donor: any): string {
        if (!donor) return '';
        return donor.bloodGroupMaster?.name || donor.bloodGroupMaster?.code || '';
    }

    hasStorageAssigned(unit: any): boolean {
        return !!(unit?.bloodFridgeId && unit?.bloodRackId);
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
            appointmentId: this.form.get('appointmentId')?.value || null,
            donationDate: this.formatDateForSave(this.form.get('donationDate')?.value)
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

    private syncAppointmentFromForm(): void {
        const appointmentId = this.form.get('appointmentId')?.value;
        const appointment = this.data?.element?.appointment;

        if (appointment?.id) {
            this.onAppointmentSelected(appointment);
            return;
        }

        if (!appointmentId) {
            this.resetAppointmentSelection(false);
            return;
        }

        this.appointmentService.getAppointmentById(appointmentId).subscribe({
            next: (response: any) => {
                if (response?.id) {
                    this.onAppointmentSelected(response);
                }
            }
        });
    }

    private prefillAppointment(appointment: any): void {
        if (!appointment?.id) {
            return;
        }

        this.onAppointmentSelected(appointment);
    }

    clearAppointment(): void {
        this.resetAppointmentSelection(true);
    }

    private resetAppointmentSelection(resetControl = true): void {
        this.selectedAppointment = null;
        this.form.patchValue({ appointmentId: null });

        if (resetControl) {
            this.appointmentSearchCtrl.setValue('', { emitEvent: false });
        }
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

                if (!term) {
                    return of([]);
                }

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

    private setupDonorAutocomplete(): void {
        this.filteredDonors$ = this.donorSearchCtrl.valueChanges.pipe(
            startWith(''),
            debounceTime(200),
            distinctUntilChanged(),
            map((value) => this.filterDonors(value))
        );

        this.donorSearchCtrl.valueChanges.subscribe((value) => {
            if (typeof value === 'string' && value.trim() === '') {
                this.selectedDonor = null;
                this.form.patchValue({ bloodDonorId: '', bloodGroupMasterId: '' });
            }
        });
    }

    private filterDonors(value: string | any): any[] {
        const term = (typeof value === 'string' ? value : this.getDonorSearchText(value)).toLowerCase().trim();
        if (!term) {
            return this.donorList.slice(0, 50);
        }

        return this.donorList.filter((donor) => {
            const name = (donor.name || '').toLowerCase();
            const cnic = (donor.cnic || '').toLowerCase();
            const bloodGroup = this.getDonorBloodGroupName(donor).toLowerCase();
            const code = (donor.donorCode || '').toLowerCase();
            return name.includes(term)
                || cnic.includes(term)
                || bloodGroup.includes(term)
                || code.includes(term);
        }).slice(0, 50);
    }

    private getDonorLabel(donor: any): string {
        const code = donor.donorCode ? `${donor.donorCode} — ` : '';
        return `${code}${donor.name || ''} | ${donor.cnic || ''}`;
    }

    private getDonorSearchText(donor: any): string {
        if (!donor) return '';
        return `${donor.name || ''} ${donor.cnic || ''} ${this.getDonorBloodGroupName(donor)} ${donor.donorCode || ''}`;
    }

    private syncDonorFromForm(): void {
        const donorId = this.form.get('bloodDonorId')?.value;
        if (!donorId) return;

        const donor = this.donorList.find((item) => item.id === donorId);
        if (!donor) return;

        this.selectedDonor = donor;
        this.donorSearchCtrl.setValue(donor, { emitEvent: false });
    }

    private formatDateForSave(value: any): string | null {
        if (!value) return null;
        const date = value instanceof Date ? value : new Date(value);
        if (isNaN(date.getTime())) return null;
        return date.toISOString();
    }
}
