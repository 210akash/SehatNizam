import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { ConstantService } from '../../../../Service/constant.service';
import { BloodFridgeService } from '../../blood-fridge/blood-fridge.service';
import { BloodRackService } from '../blood-rack.service';

@Component({
    selector: 'app-add-blood-rack',
    templateUrl: './add-blood-rack.component.html',
    styleUrl: './add-blood-rack.component.css',
    standalone: false
})
export class AddBloodRackComponent {
    form!: FormGroup;
    isLoading = false;
    isEditMode = false;
    isViewMode = false;
    fridgeList: any[] = [];

    constructor(
        private dialog: MatDialog,
        private notificationsService: NotificationsService,
        private formBuilder: FormBuilder,
        private service: BloodRackService,
        private constantService: ConstantService, private bloodFridgeService: BloodFridgeService,
        @Inject(MAT_DIALOG_DATA) public data: { element: any; isViewMode?: boolean }
    ) { }

    ngOnInit(): void {
        this.isViewMode = this.data.isViewMode === true;
        this.form = this.formBuilder.group({
            id: [0],
            name: ['', [Validators.required, Validators.maxLength(50)]],
            bloodFridgeId: ['', Validators.required]
        });
        this.loadFridges();
        this.loadData(this.data.element);
    }

    get dialogTitle(): string {
        if (this.isViewMode) return 'View Blood Rack';
        return this.isEditMode ? 'Edit Blood Rack' : 'Add Blood Rack';
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
    loadFridges() {
        this.bloodFridgeService.getAll({ PagingData: { currentPage: 0, take: 1000 } }).subscribe((data: any) => {
            this.fridgeList = data.item1 || [];
        });
    }

    getFridgeName(element: any): string {
        return element?.bloodFridge?.name || '';
    }
}
