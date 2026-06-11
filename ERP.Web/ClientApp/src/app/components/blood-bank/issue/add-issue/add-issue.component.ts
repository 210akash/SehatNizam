import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DATE_LOCALE } from '@angular/material/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { IssueService } from '../issue.service';

type IssueDialogMode = 'issue' | 'view' | 'edit';

@Component({
    selector: 'app-add-issue',
    templateUrl: './add-issue.component.html',
    styleUrl: './add-issue.component.css',
    standalone: false,
    providers: [
        { provide: MAT_DATE_LOCALE, useValue: 'en-GB' }
    ]
})
export class AddIssueComponent {
    form!: FormGroup;
    isLoading = false;
    mode: IssueDialogMode = 'issue';
    selectedRequest: any = null;
    selectedUnit: any = null;
    crossMatchDate: Date | null = null;

    constructor(
        private dialog: MatDialog,
        private notificationsService: NotificationsService,
        private formBuilder: FormBuilder,
        private service: IssueService,
        private constantService: ConstantService,
        @Inject(MAT_DIALOG_DATA) public data: { worklistRow?: any; element?: any; mode?: IssueDialogMode; isViewMode?: boolean }
    ) { }

    ngOnInit(): void {
        this.mode = this.data.mode || (this.data.isViewMode ? 'view' : (this.data.element ? 'edit' : 'issue'));
        this.form = this.formBuilder.group({
            id: [0],
            bloodRequestId: ['', Validators.required],
            bloodUnitId: ['', Validators.required],
            bloodCrossMatchId: ['', Validators.required],
            issueDate: [new Date(), Validators.required],
            issuedTo: ['', Validators.required],
            remarks: ['']
        });

        if (this.data.worklistRow) {
            this.initFromWorklist(this.data.worklistRow);
        } else if (this.data.element) {
            this.initFromHistory(this.data.element);
        }

        if (this.mode === 'view') {
            this.form.disable();
        }
    }

    get dialogTitle(): string {
        if (this.mode === 'view') return 'View Blood Issue';
        if (this.mode === 'edit') return 'Edit Blood Issue';
        return 'Issue Blood';
    }

    get isReadonly(): boolean {
        return this.mode === 'view' || this.mode === 'edit';
    }

    initFromWorklist(row: any): void {
        this.selectedRequest = row.bloodRequest;
        this.selectedUnit = row.bloodUnit;
        this.crossMatchDate = row.crossMatchDate ? new Date(row.crossMatchDate) : null;
        this.form.patchValue({
            bloodRequestId: row.bloodRequestId,
            bloodUnitId: row.bloodUnitId,
            bloodCrossMatchId: row.bloodCrossMatchId,
            issueDate: new Date(),
            issuedTo: row.bloodRequest?.patientName || ''
        });
    }

    initFromHistory(element: any): void {
        this.selectedRequest = element.bloodRequest;
        this.selectedUnit = element.bloodUnit;
        this.crossMatchDate = element.bloodCrossMatch?.crossMatchDate
            ? new Date(element.bloodCrossMatch.crossMatchDate)
            : null;
        this.form.patchValue({
            ...element,
            issueDate: element.issueDate ? new Date(element.issueDate) : new Date()
        });
    }

    getRequestLabel(): string {
        const req = this.selectedRequest;
        if (!req) return '';
        const code = req.code || '';
        const name = req.patientName || '';
        const cnic = req.patientCNIC || '';
        const date = this.formatDisplayDate(req.requestDate);
        return `${code} — ${name} | ${cnic} | ${date}`;
    }

    getRequestDetailsLabel(): string {
        const req = this.selectedRequest;
        if (!req) return '';
        const bloodGroup = req.bloodGroupMaster?.name || req.bloodGroupMaster?.code || '';
        const component = req.bloodComponentType?.name || req.bloodComponentType?.code || '';
        const quantity = req.quantity != null ? `Qty ${req.quantity}` : '';
        return [bloodGroup, component, quantity].filter(Boolean).join(' | ');
    }

    getUnitLabel(): string {
        const unit = this.selectedUnit;
        if (!unit) return '';
        const unitNo = unit.unitNo || '';
        const component = unit.bloodComponentType?.name || unit.bloodComponentType?.code || '';
        const bloodGroup = unit.bloodGroupMaster?.name || unit.bloodGroupMaster?.code || '';
        const storage = this.getStorageText(unit);
        const expiryDays = this.getExpiryDays(unit);
        const storagePart = storage || 'No storage';
        return `${unitNo} — ${bloodGroup} | ${component} | ${storagePart} | Exp: ${expiryDays} days`;
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

        if (this.form.invalid) {
            this.constantService.markFormGroupTouched(this.form);
            return;
        }

        this.isLoading = true;
        const raw = this.form.getRawValue();
        const payload = {
            ...raw,
            issueDate: this.formatDateForSave(raw.issueDate)
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
