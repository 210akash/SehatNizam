import { Component, Inject } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { MAT_DATE_LOCALE } from '@angular/material/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { BehaviorSubject, Observable, combineLatest, forkJoin, of, throwError } from 'rxjs';
import { debounceTime, distinctUntilChanged, finalize, map, startWith, switchMap } from 'rxjs/operators';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { AppointmentService } from '../../../opd/appointment/appointment.service';
import { BloodGroupService } from '../../blood-group/blood-group.service';
import { BloodRequestService } from '../../blood-request/blood-request.service';
import { BloodUnitService } from '../../blood-unit/blood-unit.service';
import { ComponentTypeService } from '../../component-type/component-type.service';
import { CrossMatchService } from '../../cross-match/cross-match.service';
import { IssueService } from '../../issue/issue.service';
import { bloodBankCnicValidators, bloodBankNameValidators } from '../../shared/blood-bank-input.utils';

@Component({
    selector: 'app-process-transfusion',
    templateUrl: './process-transfusion.component.html',
    styleUrl: './process-transfusion.component.css',
    standalone: false,
    providers: [{ provide: MAT_DATE_LOCALE, useValue: 'en-GB' }]
})
export class ProcessTransfusionComponent {
    form!: FormGroup;
    appointmentSearchCtrl = new FormControl<string | any>('');
    filteredAppointments$!: Observable<any[]>;
    selectedAppointment: any = null;
    appointmentLoading = false;
    unitSearchCtrl = new FormControl<string | any>('');
    filteredUnits$!: Observable<any[]>;

    isLoading = false;
    isViewMode = false;
    requestId = 0;
    currentStatus = 1;
    crossMatchId = 0;
    crossMatchResult = 0;
    issueId = 0;
    selectedUnit: any = null;
    noMatchingUnits = false;

    bloodGroupList: any[] = [];
    componentTypeList: any[] = [];
    unitList: any[] = [];
    unitsLoaded$ = new BehaviorSubject<any[]>([]);

    readonly inProcessResult = 3;
    statusMap: { [key: number]: string } = {
        1: 'Pending',
        2: 'Cross Matched',
        3: 'Issued',
        4: 'Cancelled'
    };
    resultList = [
        { value: 1, name: 'Compatible' },
        { value: 2, name: 'Incompatible' }
    ];
    resultMap: { [key: number]: string } = {
        0: 'Not Assigned',
        1: 'Compatible',
        2: 'Incompatible',
        3: 'In Process'
    };

    constructor(
        private dialog: MatDialog,
        private notificationsService: NotificationsService,
        private formBuilder: FormBuilder,
        private bloodRequestService: BloodRequestService,
        private crossMatchService: CrossMatchService,
        private issueService: IssueService,
        private bloodGroupService: BloodGroupService,
        private componentTypeService: ComponentTypeService,
        private appointmentService: AppointmentService,
        private bloodUnitService: BloodUnitService,
        private constantService: ConstantService,
        @Inject(MAT_DIALOG_DATA) public data: { element?: any; isViewMode?: boolean }
    ) { }

    ngOnInit(): void {
        this.isViewMode = this.data.isViewMode === true;
        this.form = this.formBuilder.group({
            appointmentId: [null],
            patientName: ['', bloodBankNameValidators()],
            patientCNIC: ['', bloodBankCnicValidators()],
            bloodGroupMasterId: ['', Validators.required],
            bloodComponentTypeId: ['', Validators.required],
            quantity: ['', [Validators.required, Validators.min(1)]],
            requestDate: [new Date(), Validators.required],
            requestRemarks: [''],
            crossMatchDate: [new Date()],
            crossMatchResult: [null as number | null],
            crossMatchRemarks: [''],
            issueDate: [new Date()],
            issuedTo: [''],
            issueRemarks: ['']
        });

        this.setupUnitAutocomplete();
        this.setupAppointmentAutocomplete();
        this.loadLookups();

        if (this.data?.element?.id) {
            this.loadRequest(this.data.element.id);
        } else if (this.isViewMode) {
            this.form.disable();
            this.unitSearchCtrl.disable();
        }
    }

    get dialogTitle(): string {
        if (this.isViewMode) return 'View Transfusion';
        if (!this.requestId) return 'New Blood Transfusion';
        return `Transfusion — ${this.form.get('patientName')?.value || 'Request'}`;
    }

    get statusText(): string {
        return this.statusMap[this.currentStatus] || 'Pending';
    }

    get canEditRequest(): boolean {
        return !this.isViewMode && (!this.requestId || this.currentStatus === 1);
    }

    get showCrossMatchSection(): boolean {
        if (this.requestId > 0) return true;
        if (this.isViewMode) return false;
        return !!this.form?.get('bloodGroupMasterId')?.value
            && !!this.form?.get('bloodComponentTypeId')?.value;
    }

    get canEditCrossMatch(): boolean {
        return !this.isViewMode && this.currentStatus === 1 && !this.crossMatchId;
    }

    get showCrossMatchResultField(): boolean {
        if (this.isViewMode) return this.crossMatchResult > 0;
        return (this.canEditCrossMatch && !!this.selectedUnit?.id) || this.showCrossMatchResultFieldUpdate;
    }

    get showCrossMatchResultFieldUpdate(): boolean {
        return !this.isViewMode && !!this.crossMatchId && this.crossMatchResult === this.inProcessResult;
    }

    get canPickUnit(): boolean {
        return this.canEditCrossMatch
            && !!this.form.get('bloodGroupMasterId')?.value
            && !!this.form.get('bloodComponentTypeId')?.value;
    }

    get showIssueSection(): boolean {
        if (this.issueId || this.currentStatus === 3) return true;
        const pickedResult = this.form.get('crossMatchResult')?.value;
        return this.crossMatchResult === 1 || pickedResult === 1;
    }

    get canEditIssue(): boolean {
        if (this.isViewMode || this.issueId) return false;
        const pickedResult = this.form.get('crossMatchResult')?.value;
        return this.currentStatus === 2 || (this.canPickUnit && pickedResult === 1);
    }

    get crossMatchResultText(): string {
        return this.resultMap[this.crossMatchResult] || 'Not Assigned';
    }

    get requestAppointmentId(): number | null {
        const id = Number(this.form?.get('appointmentId')?.value || this.selectedAppointment?.id || 0);
        return id > 0 ? id : null;
    }

    get appointmentMatchedUnits(): any[] {
        return this.unitList.filter(u => this.isUnitFromSameAppointment(u));
    }

    get hasAppointmentMatchedUnits(): boolean {
        return !!this.requestAppointmentId && this.appointmentMatchedUnits.length > 0;
    }

    get patientFieldsLocked(): boolean {
        return !!this.selectedAppointment?.id && this.canEditRequest;
    }

    displayUnit = (unit: any): string => {
        if (!unit || typeof unit === 'string') return typeof unit === 'string' ? unit : '';
        return this.getUnitLabel(unit, this.isUnitFromSameAppointment(unit));
    };

    loadLookups(): void {
        this.bloodGroupService.getAll({ PagingData: { currentPage: 0, take: 1000 } }).subscribe((data: any) => {
            this.bloodGroupList = data.item1 || [];
        });
        this.componentTypeService.getAll({ PagingData: { currentPage: 0, take: 1000 } }).subscribe((data: any) => {
            this.componentTypeList = data.item1 || [];
        });
    }

    displayAppointment = (appointment: any): string => {
        if (!appointment) return '';
        if (typeof appointment === 'string') return appointment;
        const token = appointment.tokenNumber ? `Token # ${appointment.tokenNumber}` : `Booking # ${appointment.id}`;
        const patientName = appointment?.patient?.patientMaster?.name || appointment?.patient?.name || '';
        return patientName ? `${token} - ${patientName}` : token;
    };

    onAppointmentSelected(appointment: any): void {
        if (!appointment?.id) return;
        this.applySelectedAppointment(appointment);
        this.patchPatientFromAppointment(appointment);
        this.refreshUnitListOrder();
    }

    onAppointmentInputCleared(event: Event): void {
        const value = (event.target as HTMLInputElement)?.value?.trim() ?? '';
        if (value.length > 0) return;
        this.resetAppointmentSelection();
    }

    clearAppointment(): void {
        this.resetAppointmentSelection(true);
    }

    isUnitFromSameAppointment(unit: any): boolean {
        const appointmentId = this.requestAppointmentId;
        if (!appointmentId || !unit) return false;
        const donationAppointmentId = unit.donationAppointmentId ?? unit.bloodDonation?.appointmentId;
        return Number(donationAppointmentId) === Number(appointmentId);
    }

    selectAppointmentUnit(unit: any): void {
        if (!unit?.id || !this.canPickUnit) return;
        this.applySelectedUnit(unit);
    }

    loadRequest(id: number): void {
        this.isLoading = true;
        forkJoin({
            request: this.bloodRequestService.getById(id),
            crossMatches: this.crossMatchService.getAll({ bloodRequestId: id, PagingData: { currentPage: 0, take: 1 } }),
            issues: this.issueService.getAll({ bloodRequestId: id, PagingData: { currentPage: 0, take: 1 } })
        }).subscribe({
            next: ({ request, crossMatches, issues }: any) => {
                const crossMatch = (crossMatches?.item1 || [])[0];
                const issue = (issues?.item1 || [])[0];

                this.requestId = request?.id || id;
                this.currentStatus = request?.status || 1;
                this.crossMatchId = crossMatch?.id || 0;
                this.crossMatchResult = crossMatch?.result ?? 0;
                this.issueId = issue?.id || 0;

                this.form.patchValue({
                    appointmentId: request?.appointmentId || null,
                    patientName: request?.patientName || '',
                    patientCNIC: request?.patientCNIC || '',
                    bloodGroupMasterId: request?.bloodGroupMasterId || '',
                    bloodComponentTypeId: request?.bloodComponentTypeId || '',
                    quantity: request?.quantity || '',
                    requestDate: request?.requestDate ? new Date(request.requestDate) : new Date(),
                    requestRemarks: request?.remarks || '',
                    crossMatchDate: crossMatch?.crossMatchDate ? new Date(crossMatch.crossMatchDate) : new Date(),
                    crossMatchResult: this.crossMatchResult === this.inProcessResult ? null : this.crossMatchResult,
                    crossMatchRemarks: crossMatch?.remarks || '',
                    issueDate: issue?.issueDate ? new Date(issue.issueDate) : new Date(),
                    issuedTo: issue?.issuedTo || request?.patientName || '',
                    issueRemarks: issue?.remarks || ''
                });

                if (crossMatch?.bloodUnit) {
                    this.applySelectedUnit(crossMatch.bloodUnit);
                }

                this.syncAppointmentFromRequest(request);
                this.applyFormLocks();
                if (this.showCrossMatchSection) {
                    this.loadUnits();
                }
                this.isLoading = false;
            },
            error: () => {
                this.notificationsService.showNotification('Failed to load transfusion details', 'snack-bar-danger');
                this.isLoading = false;
            }
        });
    }

    private applySelectedAppointment(appointment: any): void {
        this.selectedAppointment = appointment;
        this.appointmentSearchCtrl.setValue(appointment, { emitEvent: false });
        this.form.patchValue({ appointmentId: appointment.id });
        this.updatePatientFieldLock();
    }

    private resetAppointmentSelection(resetControl = true): void {
        this.selectedAppointment = null;
        this.form.patchValue({ appointmentId: null });
        if (resetControl) {
            this.appointmentSearchCtrl.setValue('', { emitEvent: false });
        }
        this.updatePatientFieldLock();
        if (this.unitList.length) {
            this.refreshUnitListOrder();
        }
    }

    private patchPatientFromAppointment(appointment: any): void {
        const master = appointment?.patient?.patientMaster;
        const name = master?.name || appointment?.patient?.name || '';
        const cnic = master?.cnic || appointment?.patient?.cnic || '';
        this.form.patchValue({
            patientName: name,
            patientCNIC: cnic,
            issuedTo: name
        });
    }

    private syncAppointmentFromRequest(request: any): void {
        const appointmentId = request?.appointmentId;
        if (!appointmentId) {
            this.resetAppointmentSelection(false);
            return;
        }

        if (request?.appointment?.id) {
            this.applySelectedAppointment(request.appointment);
            this.refreshUnitListOrder();
            return;
        }

        this.appointmentService.getAppointmentById(appointmentId).subscribe({
            next: (response: any) => {
                if (response?.id) {
                    this.applySelectedAppointment(response);
                    this.refreshUnitListOrder();
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

    onUnitSelected(unit: any): void {
        this.applySelectedUnit(unit);
    }

    private applySelectedUnit(unit: any): void {
        if (!unit?.id) return;
        this.selectedUnit = unit;
        this.unitSearchCtrl.setValue(unit, { emitEvent: false });
        this.unitSearchCtrl.setErrors(null);
        this.unitSearchCtrl.updateValueAndValidity({ emitEvent: false });
        this.updateCrossMatchResultValidators();
    }

    saveData(): void {
        if (this.isViewMode) return;

        const wantsCrossMatch = (this.canPickUnit && this.selectedUnit?.id) || this.showCrossMatchResultFieldUpdate;
        const wantsIssue = this.canEditIssue;

        if (this.canEditRequest && !this.validateRequest()) return;
        if (this.canPickUnit && this.selectedUnit?.id && !this.validateUnitPick()) return;
        if (!this.validateCrossMatchResult()) return;
        if (wantsIssue && !this.validateIssue()) return;

        this.isLoading = true;
        this.saveRequest()
            .pipe(switchMap(() => wantsCrossMatch ? this.saveCrossMatch() : of(void 0)))
            .pipe(switchMap(() => wantsIssue ? this.saveIssue() : of(void 0)))
            .subscribe({
                next: () => {
                    this.notificationsService.showNotification('Transfusion saved successfully', 'snack-bar-success');
                    this.dialog.closeAll();
                    this.isLoading = false;
                },
                error: (msg: string) => {
                    this.notificationsService.showNotification(msg || 'Save failed', 'snack-bar-danger');
                    this.isLoading = false;
                }
            });
    }

    private saveRequest(): Observable<void> {
        if (!this.canEditRequest) return of(void 0);
        if (!this.validateRequest()) return throwError(() => 'Please complete request details');

        const raw = this.form.getRawValue();
        const payload = {
            id: this.requestId,
            appointmentId: raw.appointmentId,
            patientName: raw.patientName,
            patientCNIC: raw.patientCNIC,
            bloodGroupMasterId: raw.bloodGroupMasterId,
            bloodComponentTypeId: raw.bloodComponentTypeId,
            quantity: raw.quantity,
            status: 1,
            remarks: raw.requestRemarks,
            requestDate: this.formatDateForSave(raw.requestDate)
        };

        return this.bloodRequestService.save(payload).pipe(
            switchMap((data: any) => {
                if (data.Status !== 200) return throwError(() => data.Data);
                if (this.requestId) return of(void 0);
                return this.bloodRequestService.getAll({
                    patientCNIC: raw.patientCNIC,
                    status: 1,
                    PagingData: { currentPage: 0, take: 1 }
                }).pipe(switchMap((list: any) => {
                    const item = list?.item1?.[0];
                    if (!item?.id) return throwError(() => 'Could not locate saved request');
                    this.requestId = item.id;
                    this.currentStatus = item.status || 1;
                    this.loadUnits();
                    return of(void 0);
                }));
            })
        );
    }

    private saveCrossMatch(): Observable<void> {
        const raw = this.form.getRawValue();
        const selectedResult = raw.crossMatchResult;

        if (this.canPickUnit && this.selectedUnit?.id) {
            const assignPayload = {
                id: 0,
                bloodRequestId: this.requestId,
                bloodUnitId: this.selectedUnit.id,
                crossMatchDate: this.formatDateForSave(raw.crossMatchDate),
                result: this.inProcessResult,
                remarks: raw.crossMatchRemarks
            };

            return this.crossMatchService.save(assignPayload).pipe(
                switchMap((data: any) => {
                    if (data.Status !== 200) return throwError(() => data.Data);
                    return this.crossMatchService.getAll({
                        bloodRequestId: this.requestId,
                        PagingData: { currentPage: 0, take: 1 }
                    });
                }),
                switchMap((list: any) => {
                    const cm = list?.item1?.[0];
                    if (!cm?.id) return of(void 0);
                    this.crossMatchId = cm.id;
                    this.crossMatchResult = cm.result;
                    this.selectedUnit = cm.bloodUnit || this.selectedUnit;

                    if (selectedResult === 1 || selectedResult === 2) {
                        return this.crossMatchService.save({
                            id: cm.id,
                            bloodRequestId: this.requestId,
                            bloodUnitId: cm.bloodUnitId || this.selectedUnit?.id,
                            crossMatchDate: this.formatDateForSave(raw.crossMatchDate),
                            result: selectedResult,
                            remarks: raw.crossMatchRemarks
                        }).pipe(switchMap((res: any) => {
                            if (res.Status !== 200) return throwError(() => res.Data);
                            this.crossMatchResult = selectedResult;
                            this.currentStatus = selectedResult === 1 ? 2 : 1;
                            return of(void 0);
                        }));
                    }
                    return of(void 0);
                })
            );
        }

        if (this.showCrossMatchResultFieldUpdate && selectedResult) {
            return this.crossMatchService.save({
                id: this.crossMatchId,
                bloodRequestId: this.requestId,
                bloodUnitId: this.selectedUnit?.id,
                crossMatchDate: this.formatDateForSave(raw.crossMatchDate),
                result: selectedResult,
                remarks: raw.crossMatchRemarks
            }).pipe(switchMap((data: any) => {
                if (data.Status !== 200) return throwError(() => data.Data);
                this.crossMatchResult = selectedResult;
                if (selectedResult === 1) this.currentStatus = 2;
                return of(void 0);
            }));
        }

        return of(void 0);
    }

    private saveIssue(): Observable<void> {
        if (!this.canEditIssue) return of(void 0);

        const raw = this.form.getRawValue();
        const payload = {
            id: 0,
            bloodRequestId: this.requestId,
            bloodUnitId: this.selectedUnit?.id,
            bloodCrossMatchId: this.crossMatchId,
            issueDate: this.formatDateForSave(raw.issueDate),
            issuedTo: raw.issuedTo,
            remarks: raw.issueRemarks
        };

        return this.issueService.save(payload).pipe(switchMap((data: any) => {
            if (data.Status !== 200) return throwError(() => data.Data);
            return of(void 0);
        }));
    }

    private validateRequest(): boolean {
        const fields = ['patientName', 'bloodGroupMasterId', 'bloodComponentTypeId', 'quantity', 'requestDate'];
        let valid = true;
        fields.forEach(f => {
            const ctrl = this.form.get(f);
            if (ctrl?.invalid) {
                ctrl.markAsTouched();
                valid = false;
            }
        });
        return valid;
    }

    private validateUnitPick(): boolean {
        if (!this.canPickUnit) return true;
        this.unitSearchCtrl.updateValueAndValidity({ emitEvent: false });
        if (this.unitSearchCtrl.invalid) {
            this.unitSearchCtrl.markAsTouched();
            return false;
        }
        return true;
    }

    private validateCrossMatchResult(): boolean {
        if (!this.showCrossMatchResultField) return true;

        this.updateCrossMatchResultValidators();
        const ctrl = this.form.get('crossMatchResult');
        if (!ctrl) return true;

        if (!ctrl.value) {
            ctrl.setErrors({ required: true });
            ctrl.markAsTouched();
            this.notificationsService.showNotification('Cross match result is required when a blood unit is selected', 'snack-bar-danger');
            return false;
        }

        return true;
    }

    private updateCrossMatchResultValidators(): void {
        const ctrl = this.form.get('crossMatchResult');
        if (!ctrl || this.isViewMode) return;

        if (this.showCrossMatchResultField) {
            ctrl.setValidators(Validators.required);
        } else {
            ctrl.clearValidators();
        }
        ctrl.updateValueAndValidity({ emitEvent: false });
    }

    private validateIssue(): boolean {
        const issuedTo = this.form.get('issuedTo');
        const issueDate = this.form.get('issueDate');
        let valid = true;
        if (issuedTo?.invalid) { issuedTo.markAsTouched(); valid = false; }
        if (issueDate?.invalid) { issueDate.markAsTouched(); valid = false; }
        if (valid && !issuedTo?.value?.trim()) {
            issuedTo?.setErrors({ required: true });
            issuedTo?.markAsTouched();
            valid = false;
        }
        return valid;
    }

    private applyFormLocks(): void {
        if (this.isViewMode) {
            this.form.disable();
            this.unitSearchCtrl.disable();
            this.appointmentSearchCtrl.disable();
            return;
        }
        if (!this.canEditRequest) {
            ['patientName', 'patientCNIC', 'bloodGroupMasterId', 'bloodComponentTypeId', 'quantity', 'requestDate', 'requestRemarks']
                .forEach(f => this.form.get(f)?.disable());
            this.appointmentSearchCtrl.disable();
        }
        if (!this.canEditCrossMatch && !this.showCrossMatchResultFieldUpdate) {
            ['crossMatchDate', 'crossMatchResult', 'crossMatchRemarks'].forEach(f => this.form.get(f)?.disable());
            if (!this.canPickUnit) this.unitSearchCtrl.disable();
        }
        if (!this.canEditIssue) {
            ['issueDate', 'issuedTo', 'issueRemarks'].forEach(f => this.form.get(f)?.disable());
        }
        if (this.canEditIssue) {
            this.form.get('issuedTo')?.setValidators(Validators.required);
            this.form.get('issueDate')?.setValidators(Validators.required);
        }
        this.updatePatientFieldLock();
        this.updateCrossMatchResultValidators();
    }

    private updatePatientFieldLock(): void {
        if (this.isViewMode) return;

        const lock = this.patientFieldsLocked;
        ['patientName', 'patientCNIC'].forEach((field) => {
            const control = this.form.get(field);
            if (!control) return;
            if (lock) {
                control.disable({ emitEvent: false });
            } else if (this.canEditRequest) {
                control.enable({ emitEvent: false });
            }
        });
    }

    private loadUnits(): void {
        this.bloodUnitService.getAll({ Status: 1, PagingData: { currentPage: 0, take: 1000 } }).subscribe((data: any) => {
            const units = data.item1 || data.Item1 || [];
            this.unitList = this.sortUnitsWithAppointmentFirst(this.filterUnitsForRequest(units));
            this.noMatchingUnits = this.unitList.length === 0;
            this.unitsLoaded$.next(this.unitList);
        });
    }

    private refreshUnitListOrder(): void {
        this.unitList = this.sortUnitsWithAppointmentFirst(this.unitList);
        this.unitsLoaded$.next(this.unitList);
    }

    private setupUnitAutocomplete(): void {
        this.unitSearchCtrl.setValidators([this.bloodUnitRequiredValidator()]);
        this.filteredUnits$ = combineLatest([
            this.unitSearchCtrl.valueChanges.pipe(startWith('')),
            this.unitsLoaded$
        ]).pipe(
            debounceTime(200),
            map(([value]) => this.filterUnits(value))
        );

        this.unitSearchCtrl.valueChanges.subscribe((value) => {
            if (!this.canPickUnit) return;
            if (typeof value === 'string') {
                if (value.trim() === '') {
                    this.selectedUnit = null;
                    this.form.patchValue({ crossMatchResult: null });
                } else if (!this.selectedUnit?.unitNo || !value.includes(this.selectedUnit.unitNo)) {
                    this.selectedUnit = null;
                    this.form.patchValue({ crossMatchResult: null });
                }
                this.unitSearchCtrl.updateValueAndValidity({ emitEvent: false });
            } else if (value && typeof value === 'object' && (value as any).id) {
                this.selectedUnit = value;
                this.unitSearchCtrl.setErrors(null);
                this.unitSearchCtrl.updateValueAndValidity({ emitEvent: false });
            }
            this.updateCrossMatchResultValidators();
        });

        this.form.get('bloodGroupMasterId')?.valueChanges.subscribe(() => this.loadUnits());
        this.form.get('bloodComponentTypeId')?.valueChanges.subscribe(() => this.loadUnits());
        this.loadUnits();
    }

    private bloodUnitRequiredValidator(): ValidatorFn {
        return (control: AbstractControl): ValidationErrors | null => {
            if (!this.canPickUnit) return null;
            const unit = this.resolveSelectedUnit(control.value);
            return unit?.id ? null : { required: true };
        };
    }

    private resolveSelectedUnit(controlValue: unknown): any {
        if (this.selectedUnit?.id) return this.selectedUnit;
        if (controlValue && typeof controlValue === 'object' && (controlValue as any).id) {
            return controlValue;
        }
        return null;
    }

    private filterUnits(value: string | any): any[] {
        const term = (typeof value === 'string' ? value : (value?.unitNo || '')).toLowerCase().trim();
        const filtered = !term
            ? this.unitList
            : this.unitList.filter(u => (u.unitNo || '').toLowerCase().includes(term));
        return this.sortUnitsWithAppointmentFirst(filtered).slice(0, 50);
    }

    private sortUnitsWithAppointmentFirst(units: any[]): any[] {
        if (!this.requestAppointmentId) return units;
        const matched = units.filter(u => this.isUnitFromSameAppointment(u));
        const others = units.filter(u => !this.isUnitFromSameAppointment(u));
        return [...matched, ...others];
    }

    private filterUnitsForRequest(units: any[]): any[] {
        const groupId = Number(this.form.get('bloodGroupMasterId')?.value || 0);
        const componentId = Number(this.form.get('bloodComponentTypeId')?.value || 0);
        if (!groupId || !componentId) return [];
        return units.filter(u =>
            Number(u.bloodGroupMasterId) === groupId
            && Number(u.bloodComponentTypeId) === componentId
            && !this.isUnitExpired(u)
        );
    }

    private isUnitExpired(unit: any): boolean {
        const expiry = this.resolveUnitExpiryDate(unit);
        if (!expiry) return false;
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        expiry.setHours(0, 0, 0, 0);
        return expiry.getTime() < today.getTime();
    }

    getUnitLabel(unit: any, includeAppointmentBadge = false): string {
        return this.getUnitPrimaryLabel(unit, includeAppointmentBadge);
    }

    getUnitPrimaryLabel(unit: any, includeAppointmentBadge = false): string {
        if (!unit) return '';
        const unitNo = unit.unitNo || '';
        const component = unit.bloodComponentType?.name || unit.bloodComponentType?.code || '';
        const bloodGroup = unit.bloodGroupMaster?.name || unit.bloodGroupMaster?.code || '';
        const label = `${unitNo} — ${bloodGroup} | ${component}`;
        if (includeAppointmentBadge && this.isUnitFromSameAppointment(unit)) {
            return `${label} (Same appointment)`;
        }
        return label;
    }

    getUnitDetailLabel(unit: any): string {
        if (!unit) return '';
        const storage = this.getStorageText(unit) || 'No storage assigned';
        const donor = unit.bloodDonation?.bloodDonor;
        const donorName = donor?.name ? `Donor: ${donor.name}` : 'Donor: —';
        const donorCnic = donor?.cnic ? `CNIC ${donor.cnic}` : '';
        const donorMobile = donor?.mobile ? `Ph ${donor.mobile}` : '';
        const expiry = this.getExpiryDaysText(unit);
        return [storage, donorName, donorCnic, donorMobile, expiry].filter(Boolean).join(' · ');
    }

    private getStorageText(unit: any): string {
        if (!unit?.bloodFridgeId || !unit?.bloodRackId) return '';
        const fridge = unit.bloodFridge?.name || unit.bloodFridge?.code || 'Fridge';
        const rack = unit.bloodRack?.name || unit.bloodRack?.code || 'Rack';
        const slot = unit.slotNo ? `Slot ${unit.slotNo}` : '';
        return [fridge, rack, slot].filter(Boolean).join(' | ');
    }

    getExpiryDays(unit: any): number {
        const expiry = this.resolveUnitExpiryDate(unit);
        if (!expiry) return 0;
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        expiry.setHours(0, 0, 0, 0);
        return Math.ceil((expiry.getTime() - today.getTime()) / (1000 * 60 * 60 * 24));
    }

    getExpiryDaysText(unit: any): string {
        const days = this.getExpiryDays(unit);
        if (days < 0) return `Expired ${Math.abs(days)}d ago`;
        if (days === 0) return 'Expires today';
        return `${days} days left`;
    }

    private resolveUnitExpiryDate(unit: any): Date | null {
        if (unit?.expiryDate) {
            const date = new Date(unit.expiryDate);
            return isNaN(date.getTime()) ? null : date;
        }

        const collectionDate = unit?.collectionDate;
        const shelfLifeDays = unit?.bloodComponentType?.shelfLifeDays;
        if (!collectionDate || shelfLifeDays == null) return null;

        const base = new Date(collectionDate);
        if (isNaN(base.getTime())) return null;
        base.setHours(0, 0, 0, 0);
        base.setDate(base.getDate() + Number(shelfLifeDays));
        return base;
    }

    blockInvalidQuantity(event: KeyboardEvent): boolean {
        const key = event.key;
        if (['Backspace', 'Delete', 'Tab', 'ArrowLeft', 'ArrowRight'].includes(key)) return true;
        return /^\d$/.test(key);
    }

    private formatDateForSave(value: any): string | null {
        if (!value) return null;
        const date = value instanceof Date ? value : new Date(value);
        if (isNaN(date.getTime())) return null;
        return date.toISOString();
    }
}
