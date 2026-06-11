import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DATE_LOCALE } from '@angular/material/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { BloodGroupService } from '../../blood-group/blood-group.service';
import { ComponentTypeService } from '../../component-type/component-type.service';
import { AdmissionService } from '../../../IPD/admission/admission.service';
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
    isLoading = false;
    isEditMode = false;
    isViewMode = false;
    bloodGroupList: any[] = [];
    componentTypeList: any[] = [];
    admissionList: any[] = [];
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
        private admissionService: AdmissionService,
        private constantService: ConstantService,
        @Inject(MAT_DIALOG_DATA) public data: { element: any; isViewMode?: boolean }
    ) { }

    ngOnInit(): void {
        this.isViewMode = this.data.isViewMode === true;
        this.form = this.formBuilder.group({
            id: [0],
            admissionId: [null],
            patientName: ['', bloodBankNameValidators()],
            patientCNIC: ['', bloodBankCnicValidators()],
            bloodGroupMasterId: ['', Validators.required],
            bloodComponentTypeId: ['', Validators.required],
            quantity: ['', [Validators.required, Validators.min(1)]],
            requestDate: [new Date(), Validators.required],
            status: [1, Validators.required],
            remarks: ['']
        });

        this.loadBloodGroups();
        this.loadComponentTypes();
        this.loadAdmissions();
        this.loadData(this.data.element);
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

    loadAdmissions() {
        this.admissionService.getAllAdmissions({ PagingData: { currentPage: 0, take: 200 } }).subscribe((data: any) => {
            this.admissionList = data.item1 || [];
        });
    }

    onAdmissionSelected(admissionId: number) {
        const admission = this.admissionList.find(a => a.id === admissionId);
        if (!admission) return;
        const master = admission?.appointment?.patient?.patientMaster;
        this.form.patchValue({
            patientName: master?.name || admission?.appointment?.patient?.name || '',
            patientCNIC: master?.cnic || ''
        });
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

                if (this.isViewMode || !this.isRequestEditable) {
                    this.form.disable();
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

    private formatDateForSave(value: any): string | null {
        if (!value) return null;
        const date = value instanceof Date ? value : new Date(value);
        if (isNaN(date.getTime())) return null;
        return date.toISOString();
    }
}
