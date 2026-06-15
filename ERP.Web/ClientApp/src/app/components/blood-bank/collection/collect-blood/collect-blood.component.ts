import { Component, Inject } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DATE_LOCALE } from '@angular/material/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { Observable, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, finalize, map, startWith, switchMap } from 'rxjs/operators';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { AppointmentService } from '../../../opd/appointment/appointment.service';
import { BloodFridgeService } from '../../blood-fridge/blood-fridge.service';
import { BloodRackService } from '../../blood-rack/blood-rack.service';
import { BloodUnitService } from '../../blood-unit/blood-unit.service';
import { ComponentTypeService } from '../../component-type/component-type.service';
import { DonorService } from '../../donor/donor.service';
import { DonationService } from '../../donation/donation.service';
import { AddDonorComponent } from '../../donor/add-donor/add-donor.component';

@Component({
    selector: 'app-collect-blood',
    templateUrl: './collect-blood.component.html',
    styleUrl: './collect-blood.component.css',
    standalone: false,
    providers: [
        { provide: MAT_DATE_LOCALE, useValue: 'en-GB' }
    ]
})
export class CollectBloodComponent {
    form!: FormGroup;
    donorSearchCtrl = new FormControl<string | any>('');
    appointmentSearchCtrl = new FormControl<string | any>('');
    filteredDonors$!: Observable<any[]>;
    filteredAppointments$!: Observable<any[]>;
    selectedDonor: any = null;
    selectedAppointment: any = null;
    appointmentLoading = false;
    donorLocked = false;
    isLoading = false;
    isEditMode = false;
    isViewMode = false;
    donorList: any[] = [];
    componentTypeList: any[] = [];
    fridgeList: any[] = [];
    rackList: any[] = [];
    allRacks: any[] = [];
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
        private donationService: DonationService,
        private donorService: DonorService,
        private bloodUnitService: BloodUnitService,
        private componentTypeService: ComponentTypeService,
        private bloodFridgeService: BloodFridgeService,
        private bloodRackService: BloodRackService,
        private appointmentService: AppointmentService,
        private constantService: ConstantService,
        @Inject(MAT_DIALOG_DATA) public data: {
            donor?: any;
            element?: any;
            isViewMode?: boolean;
            appointment?: any;
        }
    ) { }

    ngOnInit(): void {
        this.isViewMode = this.data.isViewMode === true;
        this.donorLocked = !!this.data?.donor && !this.data?.element;

        this.form = this.formBuilder.group({
            id: [0],
            appointmentId: [null],
            bloodDonorId: ['', Validators.required],
            bloodComponentTypeId: ['', Validators.required],
            bloodGroupMasterId: ['', Validators.required],
            donationDate: [new Date(), Validators.required],
            volume: ['', Validators.required],
            screeningStatus: [1, Validators.required],
            remarks: [''],
            bloodUnitId: [0],
            bloodFridgeId: [null],
            bloodRackId: [null],
            slotNo: ['']
        });

        this.form.get('screeningStatus')?.valueChanges.subscribe(() => this.updateStorageValidators());
        this.form.get('bloodFridgeId')?.valueChanges.subscribe((fridgeId) => this.onFridgeChanged(fridgeId));

        this.setupDonorAutocomplete();
        this.setupAppointmentAutocomplete();
        this.loadComponentTypes();
        this.loadFridges();
        this.loadRacks(() => {
            this.loadDonors(() => {
                this.prefillAppointment(this.data?.appointment);
                if (this.data?.donor) {
                    this.selectedDonor = this.data.donor;
                    this.form.patchValue({
                        bloodDonorId: this.data.donor.id,
                        bloodGroupMasterId: this.data.donor.bloodGroupMasterId || ''
                    });
                    this.onDonorSelected(this.data.donor);
                }
                this.loadData(this.data?.element);
            });
        });

        if (this.isViewMode) {
            this.donorSearchCtrl.disable();
            this.appointmentSearchCtrl.disable();
        }

        if (this.donorLocked) {
            this.donorSearchCtrl.disable();
        }
    }

    get dialogTitle(): string {
        if (this.isViewMode) return 'View Blood Collection';
        if (this.isEditMode) return 'Edit Blood Collection';
        return this.donorLocked && this.selectedDonor
            ? `Collect Blood — ${this.selectedDonor.name}`
            : 'Collect Blood';
    }

    get showStorageSection(): boolean {
        return this.form.get('screeningStatus')?.value === 2;
    }

    displayDonor = (donor: any): string => {
        if (!donor || typeof donor === 'string') {
            return typeof donor === 'string' ? donor : '';
        }
        return this.getDonorLabel(donor);
    };

    displayAppointment = (appointment: any): string => {
        if (!appointment) return '';
        if (typeof appointment === 'string') return appointment;
        const token = appointment.tokenNumber ? `Token # ${appointment.tokenNumber}` : `Booking # ${appointment.id}`;
        const patientName = this.getAppointmentPatientName(appointment);
        return patientName ? `${token} - ${patientName}` : token;
    };

    loadData(element: any) {
        if (!element?.id) return;

        this.isEditMode = !this.isViewMode;
        this.isLoading = true;

        this.donationService.getById(element.id).subscribe({
            next: (response: any) => {
                const donation = response || element;
                this.form.patchValue({
                    ...donation,
                    donationDate: donation?.donationDate ? new Date(donation.donationDate) : new Date(),
                    bloodUnitId: donation?.bloodUnit?.id || 0,
                    bloodFridgeId: donation?.bloodUnit?.bloodFridgeId || null,
                    bloodRackId: donation?.bloodUnit?.bloodRackId || null,
                    slotNo: donation?.bloodUnit?.slotNo || ''
                });

                this.syncDonorFromForm();
                if (donation?.appointment) {
                    this.onAppointmentSelected(donation.appointment);
                } else {
                    this.syncAppointmentFromForm();
                }

                this.filterRacksByFridge(this.form.get('bloodFridgeId')?.value);
                this.isScreeningStatusLocked = this.hasStorageAssigned(donation?.bloodUnit);
                this.updateStorageValidators();

                if (this.isViewMode) {
                    this.form.disable();
                    this.donorSearchCtrl.disable();
                    this.appointmentSearchCtrl.disable();
                }

                this.isLoading = false;
            },
            error: () => {
                this.notificationsService.showNotification('Failed to load collection details', 'snack-bar-danger');
                this.isLoading = false;
            }
        });
    }

    onDonorSelected(donor: any): void {
        if (!donor?.id) return;

        this.donorService.getById(donor.id).subscribe({
            next: (response: any) => this.applySelectedDonor(response || donor),
            error: () => this.applySelectedDonor(donor)
        });
    }

    private applySelectedDonor(donor: any): void {
        this.selectedDonor = donor;
        this.form.patchValue({
            bloodDonorId: donor.id,
            bloodGroupMasterId: donor.bloodGroupMasterId || ''
        });
        this.donorSearchCtrl.setValue(donor, { emitEvent: false });
    }

    openAddDonor(): void {
        if (this.isViewMode || this.donorLocked) return;

        const searchTerm = this.getDonorSearchTerm();
        const prefill = this.getDonorPrefillFromSearch(searchTerm);

        this.dialog.open(AddDonorComponent, {
            panelClass: 'cstm_width_700',
            height: 'auto',
            data: { element: prefill },
            disableClose: true
        }).afterClosed().subscribe(() => {
            this.loadDonors(() => this.selectDonorAfterAdd(searchTerm));
        });
    }

    private getDonorSearchTerm(): string {
        const value = this.donorSearchCtrl.value;
        if (!value) return '';
        if (typeof value === 'string') return value.trim();
        return this.getDonorSearchText(value).trim();
    }

    private getDonorPrefillFromSearch(term: string): any | null {
        if (!term) return null;

        const cnicDigits = term.replace(/\D/g, '');
        if (cnicDigits.length >= 5) {
            const formatted = this.formatCnicFromDigits(cnicDigits);
            return { cnic: formatted };
        }

        return { name: term };
    }

    private formatCnicFromDigits(digits: string): string {
        if (digits.length <= 5) return digits;
        if (digits.length <= 12) return `${digits.slice(0, 5)}-${digits.slice(5)}`;
        return `${digits.slice(0, 5)}-${digits.slice(5, 12)}-${digits.slice(12, 13)}`;
    }

    private selectDonorAfterAdd(searchTerm: string): void {
        if (!searchTerm) return;

        const term = searchTerm.toLowerCase();
        const match = this.donorList.find((donor) => {
            const name = (donor.name || '').toLowerCase();
            const cnic = (donor.cnic || '').toLowerCase();
            const code = (donor.donorCode || '').toLowerCase();
            return name.includes(term) || cnic.includes(term) || code.includes(term);
        });

        if (match) {
            this.onDonorSelected(match);
        }
    }

    onAppointmentSelected(appointment: any): void {
        if (!appointment?.id) return;
        this.applySelectedAppointment(appointment);
    }

    private applySelectedAppointment(appointment: any): void {
        this.selectedAppointment = appointment;
        this.appointmentSearchCtrl.setValue(appointment, { emitEvent: false });
        this.form.patchValue({ appointmentId: appointment.id });
    }

    onAppointmentInputCleared(event: Event): void {
        const value = (event.target as HTMLInputElement)?.value?.trim() ?? '';
        if (value.length > 0) return;
        this.resetAppointmentSelection();
    }

    clearAppointment(): void {
        this.resetAppointmentSelection(true);
    }

    getAppointmentPatientName(appointment: any = this.selectedAppointment): string {
        return appointment?.patient?.patientMaster?.name
            || appointment?.patient?.name
            || '-';
    }

    getAppointmentPatientCnic(appointment: any = this.selectedAppointment): string {
        return appointment?.patient?.patientMaster?.cnic
            || appointment?.patient?.cnic
            || '-';
    }

    getAppointmentToken(appointment: any = this.selectedAppointment): string {
        if (!appointment) return '-';
        return appointment.tokenNumber
            ? `Token # ${appointment.tokenNumber}`
            : `Booking # ${appointment.id}`;
    }

    getAppointmentDate(appointment: any = this.selectedAppointment): string {
        if (!appointment?.appointmentDate) return '-';
        return new Date(appointment.appointmentDate).toLocaleDateString('en-GB');
    }

    getAppointmentPatientMrn(appointment: any = this.selectedAppointment): string {
        return appointment?.patient?.mrn
            || appointment?.patient?.patientMaster?.mrn
            || '-';
    }

    getAppointmentPatientAgeGender(appointment: any = this.selectedAppointment): string {
        const patient = appointment?.patient?.patientMaster;
        if (!patient) return '-';

        const age = patient.age ?? this.calculateAge(patient.dateOfBirth) ?? '-';
        const gender = patient.gender || '-';
        return `${age} / ${gender}`;
    }

    getAppointmentPatientPhone(appointment: any = this.selectedAppointment): string {
        return appointment?.patient?.patientMaster?.phoneNo
            || appointment?.patient?.phoneNo
            || '-';
    }

    getAppointmentDoctorName(appointment: any = this.selectedAppointment): string {
        const doctor = appointment?.doctor;
        if (!doctor) return '-';
        return `${doctor.firstName || ''} ${doctor.lastName || ''}`.trim() || doctor.name || '-';
    }

    getAppointmentDepartmentName(appointment: any = this.selectedAppointment): string {
        return appointment?.department?.name || '-';
    }

    getAppointmentDoctorDepartment(appointment: any = this.selectedAppointment): string {
        const doctor = this.getAppointmentDoctorName(appointment);
        const department = this.getAppointmentDepartmentName(appointment);
        if (doctor === '-' && department === '-') return '-';
        if (doctor === '-') return department;
        if (department === '-') return doctor;
        return `${doctor} — ${department}`;
    }

    getDonorCode(donor: any = this.selectedDonor): string {
        return donor?.donorCode || '-';
    }

    getDonorMobile(donor: any = this.selectedDonor): string {
        return donor?.mobile || '-';
    }

    getDonorGender(donor: any = this.selectedDonor): string {
        return donor?.gender || '-';
    }

    getDonorLastDonationDate(donor: any = this.selectedDonor): string {
        if (!donor?.lastDonationDate) return '-';
        return new Date(donor.lastDonationDate).toLocaleDateString('en-GB');
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

        this.updateStorageValidators();
        if (this.form.invalid) {
            this.constantService.markFormGroupTouched(this.form);
            return;
        }

        this.isLoading = true;
        const raw = this.form.getRawValue();
        const donationPayload = {
            id: raw.id,
            appointmentId: raw.appointmentId || null,
            bloodDonorId: raw.bloodDonorId,
            bloodComponentTypeId: raw.bloodComponentTypeId,
            bloodGroupMasterId: raw.bloodGroupMasterId,
            donationDate: this.formatDateForSave(raw.donationDate),
            volume: raw.volume,
            screeningStatus: raw.screeningStatus,
            remarks: raw.remarks
        };

        this.donationService.save(donationPayload).subscribe({
            next: (data: { Status: number; Data: string }) => {
                if (data.Status !== 200) {
                    this.notificationsService.showNotification(data.Data, 'snack-bar-danger');
                    this.isLoading = false;
                    return;
                }

                if (this.shouldSaveStorage()) {
                    this.saveStorageAfterDonation(raw);
                    return;
                }

                this.notificationsService.showNotification(data.Data || 'Blood collection saved!', 'snack-bar-success');
                this.dialog.closeAll();
                this.isLoading = false;
            },
            error: (error: string) => {
                this.notificationsService.showNotification(error, 'snack-bar-danger');
                this.isLoading = false;
            }
        });
    }

    private shouldSaveStorage(): boolean {
        if (this.form.get('screeningStatus')?.value !== 2) return false;
        const fridgeId = this.form.get('bloodFridgeId')?.value;
        const rackId = this.form.get('bloodRackId')?.value;
        const slotNo = (this.form.get('slotNo')?.value || '').trim();
        return !!(fridgeId && rackId && slotNo);
    }

    private saveStorageAfterDonation(raw: any) {
        const assignStorage = (unitId: number) => {
            if (!unitId) {
                    this.notificationsService.showNotification('Donation saved but blood unit was not found for storage assignment.', 'snack-bar-danger');
                this.dialog.closeAll();
                this.isLoading = false;
                return;
            }

            this.bloodUnitService.save({
                id: unitId,
                bloodFridgeId: raw.bloodFridgeId,
                bloodRackId: raw.bloodRackId,
                slotNo: raw.slotNo,
                status: 1
            }).subscribe({
                next: (unitData: { Status: number; Data: string }) => {
                    if (unitData.Status === 200) {
                        this.notificationsService.showNotification('Blood collection and storage saved!', 'snack-bar-success');
                        this.dialog.closeAll();
                    } else {
                        this.notificationsService.showNotification(unitData.Data, 'snack-bar-danger');
                    }
                    this.isLoading = false;
                },
                error: (error: string) => {
                    this.notificationsService.showNotification(error, 'snack-bar-danger');
                    this.isLoading = false;
                }
            });
        };

        const existingUnitId = raw.bloodUnitId;
        if (existingUnitId > 0) {
            assignStorage(existingUnitId);
            return;
        }

        this.donationService.getAll({
            bloodDonorId: raw.bloodDonorId,
            PagingData: { currentPage: 0, take: 1 }
        }).subscribe({
            next: (listData: any) => {
                const latest = listData?.item1?.[0];
                if (!latest?.id) {
                    this.notificationsService.showNotification('Donation saved but could not locate blood unit.', 'snack-bar-danger');
                    this.dialog.closeAll();
                    this.isLoading = false;
                    return;
                }

                this.donationService.getById(latest.id).subscribe({
                    next: (donation: any) => assignStorage(donation?.bloodUnit?.id),
                    error: () => {
                        this.notificationsService.showNotification('Donation saved but could not load blood unit.', 'snack-bar-danger');
                        this.dialog.closeAll();
                        this.isLoading = false;
                    }
                });
            },
            error: () => {
                this.notificationsService.showNotification('Donation saved but storage assignment failed.', 'snack-bar-danger');
                this.dialog.closeAll();
                this.isLoading = false;
            }
        });
    }

    private loadDonors(onLoaded?: () => void) {
        this.donorService.getAll({ PagingData: { currentPage: 0, take: 1000 } }).subscribe((data: any) => {
            this.donorList = (data.item1 || []).filter((donor: any) => !donor.isDeferred);
            onLoaded?.();
        });
    }

    private loadComponentTypes() {
        this.componentTypeService.getAll({ PagingData: { currentPage: 0, take: 1000 } }).subscribe((data: any) => {
            this.componentTypeList = data.item1 || [];
        });
    }

    private loadFridges() {
        this.bloodFridgeService.getAll({ PagingData: { currentPage: 0, take: 1000 } }).subscribe((data: any) => {
            this.fridgeList = data.item1 || [];
        });
    }

    private loadRacks(onLoaded?: () => void) {
        this.bloodRackService.getAll({ PagingData: { currentPage: 0, take: 1000 } }).subscribe((data: any) => {
            this.allRacks = data.item1 || [];
            this.rackList = [...this.allRacks];
            onLoaded?.();
        });
    }

    private onFridgeChanged(fridgeId: number | null) {
        this.filterRacksByFridge(fridgeId);
        const rackId = this.form.get('bloodRackId')?.value;
        if (rackId && !this.rackList.some((rack) => rack.id === rackId)) {
            this.form.patchValue({ bloodRackId: null });
        }
    }

    private filterRacksByFridge(fridgeId: number | null) {
        if (!fridgeId) {
            this.rackList = [...this.allRacks];
            return;
        }
        this.rackList = this.allRacks.filter((rack) => rack.bloodFridgeId === fridgeId);
    }

    private updateStorageValidators() {
        const pass = this.form.get('screeningStatus')?.value === 2;
        const fridgeCtrl = this.form.get('bloodFridgeId');
        const rackCtrl = this.form.get('bloodRackId');
        const slotCtrl = this.form.get('slotNo');
        const hasAny = !!(fridgeCtrl?.value || rackCtrl?.value || (slotCtrl?.value || '').trim());

        if (pass && hasAny) {
            fridgeCtrl?.setValidators(Validators.required);
            rackCtrl?.setValidators(Validators.required);
            slotCtrl?.setValidators(Validators.required);
        } else {
            fridgeCtrl?.clearValidators();
            rackCtrl?.clearValidators();
            slotCtrl?.clearValidators();
        }

        fridgeCtrl?.updateValueAndValidity({ emitEvent: false });
        rackCtrl?.updateValueAndValidity({ emitEvent: false });
        slotCtrl?.updateValueAndValidity({ emitEvent: false });
    }

    private syncAppointmentFromForm() {
        const appointmentId = this.form.get('appointmentId')?.value;
        if (!appointmentId) {
            this.resetAppointmentSelection(false);
            return;
        }

        const appointment = this.data?.element?.appointment;
        if (appointment?.id === appointmentId) {
            this.applySelectedAppointment(appointment);
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

    private prefillAppointment(appointment: any) {
        if (appointment?.id) {
            this.applySelectedAppointment(appointment);
        }
    }

    private resetAppointmentSelection(resetControl = true) {
        this.selectedAppointment = null;
        this.form.patchValue({ appointmentId: null });
        if (resetControl) {
            this.appointmentSearchCtrl.setValue('', { emitEvent: false });
        }
    }

    private setupAppointmentAutocomplete() {
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

    private setupDonorAutocomplete() {
        this.filteredDonors$ = this.donorSearchCtrl.valueChanges.pipe(
            startWith(''),
            debounceTime(200),
            distinctUntilChanged(),
            map((value) => this.filterDonors(value))
        );

        this.donorSearchCtrl.valueChanges.subscribe((value) => {
            if (this.donorLocked) return;
            if (typeof value === 'string' && value.trim() === '') {
                this.selectedDonor = null;
                this.form.patchValue({ bloodDonorId: '', bloodGroupMasterId: '' });
            }
        });
    }

    private filterDonors(value: string | any): any[] {
        const term = (typeof value === 'string' ? value : this.getDonorSearchText(value)).toLowerCase().trim();
        if (!term) return this.donorList.slice(0, 50);

        return this.donorList.filter((donor) => {
            const name = (donor.name || '').toLowerCase();
            const cnic = (donor.cnic || '').toLowerCase();
            const bloodGroup = this.getDonorBloodGroupName(donor).toLowerCase();
            const code = (donor.donorCode || '').toLowerCase();
            return name.includes(term) || cnic.includes(term) || bloodGroup.includes(term) || code.includes(term);
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

    private syncDonorFromForm() {
        const donorId = this.form.get('bloodDonorId')?.value;
        if (!donorId) return;

        const donor = this.donorList.find((item) => item.id === donorId) || this.data?.donor;
        if (!donor) return;

        this.onDonorSelected(donor);
        this.donorSearchCtrl.setValue(donor, { emitEvent: false });
    }

    private formatDateForSave(value: any): string | null {
        if (!value) return null;
        const date = value instanceof Date ? value : new Date(value);
        if (isNaN(date.getTime())) return null;
        return date.toISOString();
    }

    private calculateAge(dob: string | Date | null): number | null {
        if (!dob) return null;
        const birthDate = new Date(dob);
        if (isNaN(birthDate.getTime())) return null;
        const diff = Date.now() - birthDate.getTime();
        const ageDate = new Date(diff);
        return Math.abs(ageDate.getUTCFullYear() - 1970);
    }
}
