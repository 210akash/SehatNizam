import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { ConstantService } from '../../../../Service/constant.service';
import { BloodFridgeService } from '../blood-fridge.service';

@Component({
    selector: 'app-add-blood-fridge',
    templateUrl: './add-blood-fridge.component.html',
    styleUrl: './add-blood-fridge.component.css',
    standalone: false
})
export class AddBloodFridgeComponent {
    form!: FormGroup;
    isLoading = false;
    isEditMode = false;
    isViewMode = false;

    constructor(
        private dialog: MatDialog,
        private notificationsService: NotificationsService,
        private formBuilder: FormBuilder,
        private service: BloodFridgeService,
        private constantService: ConstantService,
        @Inject(MAT_DIALOG_DATA) public data: { element: any; isViewMode?: boolean }
    ) { }

    ngOnInit(): void {
        this.isViewMode = this.data.isViewMode === true;
        this.form = this.formBuilder.group({
            id: [0],
            name: ['', [Validators.required, Validators.maxLength(50)]],
            description: ['']
        });
        this.loadData(this.data.element);
    }

    get dialogTitle(): string {
        if (this.isViewMode) return 'View Blood Fridge';
        return this.isEditMode ? 'Edit Blood Fridge' : 'Add Blood Fridge';
    }

    loadData(element: any) {
        if (element != null) {
            this.isEditMode = !this.isViewMode;
            this.form.patchValue(element);
            if (this.isViewMode) this.form.disable();
        }
    }

    saveData() {
        if (this.isViewMode) return;
        if (this.form.invalid) {
            this.constantService.markFormGroupTouched(this.form);
            return;
        }
        this.isLoading = true;
        this.service.save(this.form.getRawValue()).subscribe({
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
}
