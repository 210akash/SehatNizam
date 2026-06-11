import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';
import { bloodBankCnicValidators, bloodBankNameValidators } from '../../shared/blood-bank-input.utils';
import { NotificationsService } from '../../../../Service/notification.service';
import { BloodGroupService } from '../../blood-group/blood-group.service';
import { DonorService } from '../donor.service';

@Component({
    selector: 'app-add-donor',
    templateUrl: './add-donor.component.html',
    styleUrl: './add-donor.component.css',
    standalone: false
})
export class AddDonorComponent {
    form!: FormGroup;
    isLoading = false;
    isEditMode = false;
    isViewMode = false;
    bloodGroups: any[] = [];
    genders = ['Male', 'Female', 'Other'];
    maxDate = new Date();

    constructor(
        private dialog: MatDialog,
        private notificationsService: NotificationsService,
        private formBuilder: FormBuilder,
        private service: DonorService,
        private bloodGroupService: BloodGroupService,
        private constantService: ConstantService,
        @Inject(MAT_DIALOG_DATA) public data: { element: any; isViewMode?: boolean }
    ) { }

    ngOnInit(): void {
        this.isViewMode = this.data.isViewMode === true;
        this.form = this.formBuilder.group({
            id: [0],
            donorCode: [{ value: '', disabled: true }],
            name: ['', bloodBankNameValidators()],
            cnic: ['', bloodBankCnicValidators(true)],
            mobile: ['', [Validators.required, Validators.pattern(/^\d{1,15}$/)]],
            gender: ['', Validators.required],
            dateOfBirth: [null, Validators.required],
            bloodGroupMasterId: ['', Validators.required],
            patientMasterId: [null],
            isDeferred: [false],
            deferralReason: ['']
        });

        this.form.get('isDeferred')?.valueChanges.subscribe((isDeferred) => {
            const reasonCtrl = this.form.get('deferralReason');
            if (isDeferred) {
                reasonCtrl?.setValidators(Validators.required);
            } else {
                reasonCtrl?.clearValidators();
                reasonCtrl?.setValue('');
            }
            reasonCtrl?.updateValueAndValidity();
        });

        this.loadBloodGroups();
        this.loadData(this.data.element);
    }

    get dialogTitle(): string {
        if (this.isViewMode) return 'View Blood Donor';
        return this.isEditMode ? 'Edit Blood Donor' : 'Add Blood Donor';
    }

    loadBloodGroups() {
        this.bloodGroupService.getAll({ PagingData: { currentPage: 0, take: 1000 } }).subscribe((data: any) => {
            this.bloodGroups = data.item1 || [];
        });
    }

    loadData(element: any) {
        if (element == null) return;

        this.isEditMode = !this.isViewMode;
        this.isLoading = true;

        this.service.getById(element.id).subscribe({
            next: (response: any) => {
                const donor = response || element;
                this.form.patchValue({
                    ...donor,
                    dateOfBirth: donor?.dateOfBirth ? new Date(donor.dateOfBirth) : null
                });

                if (this.isViewMode) {
                    this.form.disable();
                }

                this.isLoading = false;
            },
            error: () => {
                this.notificationsService.showNotification('Failed to load donor details', 'snack-bar-danger');
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
            dateOfBirth: this.formatDateForSave(this.form.get('dateOfBirth')?.value)
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

    allowDigitsOnly(event: KeyboardEvent): boolean {
        const key = event.key;
        if (key === 'Backspace' || key === 'Delete' || key === 'Tab' || key === 'ArrowLeft' || key === 'ArrowRight') {
            return true;
        }
        return /^\d$/.test(key);
    }

    onMobileInput(event: Event): void {
        const input = event.target as HTMLInputElement;
        const digitsOnly = input.value.replace(/\D/g, '').slice(0, 15);
        if (input.value !== digitsOnly) {
            this.form.get('mobile')?.setValue(digitsOnly);
        }
    }

    private formatDateForSave(value: any): string | null {
        if (!value) return null;
        const date = value instanceof Date ? value : new Date(value);
        if (isNaN(date.getTime())) return null;
        return date.toISOString();
    }
}
