import { Component, Inject } from '@angular/core';

import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';

import { MAT_DATE_LOCALE } from '@angular/material/core';

import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';

import { BehaviorSubject, Observable, combineLatest } from 'rxjs';

import { debounceTime, distinctUntilChanged, map, startWith } from 'rxjs/operators';

import { ConstantService } from '../../../../Service/constant.service';

import { NotificationsService } from '../../../../Service/notification.service';

import { BloodUnitService } from '../../blood-unit/blood-unit.service';

import { CrossMatchService } from '../cross-match.service';



type CrossMatchDialogMode = 'assign' | 'updateResult' | 'view';



@Component({

    selector: 'app-add-cross-match',

    templateUrl: './add-cross-match.component.html',

    styleUrl: './add-cross-match.component.css',

    standalone: false,

    providers: [

        { provide: MAT_DATE_LOCALE, useValue: 'en-GB' }

    ]

})

export class AddCrossMatchComponent {

    form!: FormGroup;

    unitSearchCtrl = new FormControl<string | any>('');

    filteredUnits$!: Observable<any[]>;

    selectedRequest: any = null;

    selectedUnit: any = null;

    isLoading = false;

    mode: CrossMatchDialogMode = 'assign';

    unitList: any[] = [];
    unitsLoaded$ = new BehaviorSubject<any[]>([]);
    noMatchingUnits = false;

    resultList = [

        { value: 1, name: 'Compatible' },

        { value: 2, name: 'Incompatible' }

    ];

    readonly inProcessResult = 3;



    constructor(

        private dialog: MatDialog,

        private notificationsService: NotificationsService,

        private formBuilder: FormBuilder,

        private service: CrossMatchService,

        private bloodUnitService: BloodUnitService,

        private constantService: ConstantService,

        @Inject(MAT_DIALOG_DATA) public data: { worklistRow?: any; mode?: CrossMatchDialogMode; isViewMode?: boolean }

    ) { }



    ngOnInit(): void {

        this.mode = this.data.mode || (this.data.isViewMode ? 'view' : 'assign');

        this.form = this.formBuilder.group({

            id: [0],

            bloodRequestId: ['', Validators.required],

            bloodUnitId: ['', this.mode === 'assign' ? Validators.required : []],

            crossMatchDate: [new Date(), Validators.required],

            result: [null as number | null, this.mode === 'updateResult' ? Validators.required : []],

            remarks: ['']

        });



        this.initFromWorklist();
        this.setupUnitAutocomplete();

        if (this.mode === 'assign') {
            this.unitSearchCtrl.setValidators([this.bloodUnitRequiredValidator()]);
            this.loadUnits();
        }



        if (this.mode === 'view') {

            this.unitSearchCtrl.disable();

            this.form.disable();

        } else if (this.mode === 'updateResult') {

            this.unitSearchCtrl.disable();

        }

    }



    get isDateReadonly(): boolean {

        return this.mode === 'view' || this.mode === 'updateResult';

    }



    get dialogTitle(): string {

        if (this.mode === 'view') return 'View Cross Match';

        if (this.mode === 'updateResult') return 'Update Cross Match Result';

        return 'Assign Blood Unit';

    }



    get showUnitPicker(): boolean {

        return this.mode === 'assign';

    }



    get showResultField(): boolean {

        return this.mode === 'updateResult';

    }



    displayUnit = (unit: any): string => {

        if (!unit || typeof unit === 'string') {

            return typeof unit === 'string' ? unit : '';

        }

        return this.getUnitLabel(unit);

    };



    initFromWorklist(): void {

        const row = this.data.worklistRow;

        if (!row) return;



        this.selectedRequest = row.bloodRequest;

        this.form.patchValue({

            id: row.crossMatchId || 0,

            bloodRequestId: row.bloodRequestId,

            bloodUnitId: row.bloodUnitId || '',

            crossMatchDate: row.crossMatchDate ? new Date(row.crossMatchDate) : new Date(),

            result: row.result === this.inProcessResult ? null : row.result,

            remarks: row.remarks || ''

        });



        if (row.bloodUnit) {

            this.selectedUnit = row.bloodUnit;

            this.unitSearchCtrl.setValue(row.bloodUnit, { emitEvent: false });

            this.ensureUnitInList(row.bloodUnit);

        }

    }



    loadUnits() {

        this.bloodUnitService.getAll({

            Status: 1,

            PagingData: { currentPage: 0, take: 1000 }

        }).subscribe((data: any) => {
            const units = data.item1 || data.Item1 || [];
            this.unitList = this.filterUnitsForRequest(units);
            this.noMatchingUnits = this.unitList.length === 0;
            this.unitsLoaded$.next(this.unitList);

            if (this.selectedUnit) {
                this.ensureUnitInList(this.selectedUnit);
            }
        });

    }



    onUnitSelected(unit: any): void {

        if (!unit?.id) return;

        this.selectedUnit = unit;

        this.form.patchValue({ bloodUnitId: unit.id });
        this.unitSearchCtrl.updateValueAndValidity();

    }



    getRequestLabel(request: any): string {

        if (!request) return '';

        const code = request.code || '';

        const name = request.patientName || '';

        const cnic = request.patientCNIC || '';

        const date = this.formatDisplayDate(request.requestDate);

        return `${code} — ${name} | ${cnic} | ${date}`;

    }

    getRequestDetailsLabel(request: any): string {
        if (!request) return '';
        const bloodGroup = request.bloodGroupMaster?.name || request.bloodGroupMaster?.code || '';
        const component = request.bloodComponentType?.name || request.bloodComponentType?.code || '';
        const quantity = request.quantity != null ? `Qty ${request.quantity}` : '';
        return [bloodGroup, component, quantity].filter(Boolean).join(' | ');
    }



    getUnitLabel(unit: any): string {

        if (!unit) return '';

        const unitNo = unit.unitNo || '';

        const component = unit.bloodComponentType?.name || unit.bloodComponentType?.code || '';

        const storage = this.getStorageText(unit);

        const expiryDays = this.getExpiryDays(unit);

        const storagePart = storage ? storage : 'No storage';

        return `${unitNo} — ${component} | ${storagePart} | Exp: ${expiryDays} days`;

    }



    getStorageText(unit: any): string {

        if (!unit?.bloodFridgeId || !unit?.bloodRackId) return '';

        const fridge = unit.bloodFridge?.name || unit.bloodFridge?.code || 'Fridge';

        const rack = unit.bloodRack?.name || unit.bloodRack?.code || 'Rack';

        const slot = unit.slotNo ? `Slot ${unit.slotNo}` : '';

        return [fridge, rack, slot].filter(Boolean).join(' | ');

    }



    getExpiryDays(unit: any): number {

        if (!unit?.expiryDate) return 0;

        const expiry = new Date(unit.expiryDate);

        const today = new Date();

        today.setHours(0, 0, 0, 0);

        expiry.setHours(0, 0, 0, 0);

        return Math.ceil((expiry.getTime() - today.getTime()) / (1000 * 60 * 60 * 24));

    }



    saveData() {

        if (this.mode === 'view') return;



        if (this.mode === 'assign') {
            this.unitSearchCtrl.updateValueAndValidity();
            this.unitSearchCtrl.markAsTouched();
        }

        if (this.form.invalid || this.unitSearchCtrl.invalid) {

            this.constantService.markFormGroupTouched(this.form);

            return;

        }



        this.isLoading = true;

        const raw = this.form.getRawValue();

        const payload = {

            ...raw,

            crossMatchDate: this.formatDateForSave(raw.crossMatchDate),

            result: this.mode === 'assign' ? this.inProcessResult : raw.result

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



    private setupUnitAutocomplete(): void {
        this.filteredUnits$ = combineLatest([
            this.unitSearchCtrl.valueChanges.pipe(startWith('')),
            this.unitsLoaded$
        ]).pipe(
            debounceTime(200),
            map(([value]) => this.filterUnits(value))
        );



        this.unitSearchCtrl.valueChanges.subscribe((value) => {

            if (this.mode !== 'assign') return;

            if (typeof value === 'string' && value.trim() === '') {

                this.selectedUnit = null;

                this.form.patchValue({ bloodUnitId: '' });
                this.unitSearchCtrl.updateValueAndValidity();

            }

        });

    }

    private bloodUnitRequiredValidator(): ValidatorFn {
        return (_control: AbstractControl): ValidationErrors | null => {
            const unitId = this.form?.get('bloodUnitId')?.value;
            return unitId ? null : { required: true };
        };
    }



    private filterUnits(value: string | any): any[] {
        const term = (typeof value === 'string' ? value : (value?.unitNo || '')).toLowerCase().trim();
        if (!term) return this.unitList.slice(0, 50);
        return this.unitList.filter((unit) => (unit.unitNo || '').toLowerCase().includes(term)).slice(0, 50);
    }

    private filterUnitsForRequest(units: any[]): any[] {
        if (!this.selectedRequest) return units;

        const groupId = this.getRequestBloodGroupId();
        const componentId = this.getRequestComponentTypeId();
        if (!groupId || !componentId) return units;

        return units.filter((unit: any) =>
            Number(unit.bloodGroupMasterId) === groupId
            && Number(unit.bloodComponentTypeId) === componentId
            && !this.isUnitExpired(unit)
        );
    }

    private getRequestBloodGroupId(): number {
        return Number(this.selectedRequest?.bloodGroupMasterId || this.selectedRequest?.bloodGroupMaster?.id || 0);
    }

    private getRequestComponentTypeId(): number {
        return Number(this.selectedRequest?.bloodComponentTypeId || this.selectedRequest?.bloodComponentType?.id || 0);
    }

    private isUnitExpired(unit: any): boolean {
        if (!unit?.expiryDate) return false;
        const expiry = new Date(unit.expiryDate);
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        expiry.setHours(0, 0, 0, 0);
        return expiry.getTime() < today.getTime();
    }



    private ensureUnitInList(unit: any): void {

        if (!unit?.id) return;

        if (!this.unitList.some((item) => item.id === unit.id)) {

            this.unitList = [unit, ...this.unitList];

        }

    }



    private formatDisplayDate(value: any): string {

        if (!value) return '';

        const date = new Date(value);

        if (isNaN(date.getTime())) return '';

        return date.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: '2-digit' });

    }



    private formatDateForSave(value: any): string | null {

        if (!value) return null;

        const date = value instanceof Date ? value : new Date(value);

        if (isNaN(date.getTime())) return null;

        return date.toISOString();

    }

}


