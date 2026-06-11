import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { BloodFridgeService } from '../../blood-fridge/blood-fridge.service';
import { BloodRackService } from '../../blood-rack/blood-rack.service';
import { BloodUnitService } from '../blood-unit.service';

@Component({
    selector: 'app-add-blood-unit',
    templateUrl: './add-blood-unit.component.html',
    styleUrl: './add-blood-unit.component.css',
    standalone: false
})
export class AddBloodUnitComponent {
    form!: FormGroup;
    isLoading = false;
    isEditMode = false;
    isViewMode = false;
    fridgeList: any[] = [];
    rackList: any[] = [];
    statusOptions = [
        { value: 1, name: 'Available' },
        { value: 2, name: 'Reserved' },
        { value: 3, name: 'Issued' },
        { value: 4, name: 'Discarded' },
        { value: 5, name: 'Expired' }
    ];

    constructor(
        private dialog: MatDialog,
        private notificationsService: NotificationsService,
        private formBuilder: FormBuilder,
        private service: BloodUnitService,
        private bloodFridgeService: BloodFridgeService,
        private bloodRackService: BloodRackService,
        private constantService: ConstantService,
        @Inject(MAT_DIALOG_DATA) public data: { element: any; isViewMode?: boolean }
    ) { }

    ngOnInit(): void {
        this.isViewMode = this.data.isViewMode === true;
        this.form = this.formBuilder.group({
            id: [0],
            bloodFridgeId: ['', Validators.required],
            bloodRackId: ['', Validators.required],
            slotNo: ['', Validators.required],
            status: [1, Validators.required]
        });

        this.loadFridges();
        this.loadRacks();
        this.loadData(this.data.element);
    }

    get dialogTitle(): string {
        if (this.isViewMode) return 'View Blood Unit';
        return this.isEditMode ? 'Assign Blood Unit Storage' : 'Add Blood Unit Storage';
    }

    loadFridges() {
        this.bloodFridgeService.getAll({ PagingData: { currentPage: 0, take: 1000 } }).subscribe((data: any) => {
            this.fridgeList = data.item1 || [];
        });
    }

    loadRacks() {
        this.bloodRackService.getAll({ PagingData: { currentPage: 0, take: 1000 } }).subscribe((data: any) => {
            this.rackList = data.item1 || [];
        });
    }

    loadData(element: any) {
        if (element == null) return;

        this.isEditMode = !this.isViewMode;
        this.isLoading = true;

        this.service.getById(element.id).subscribe({
            next: (response: any) => {
                const unit = response || element;
                this.form.patchValue({
                    id: unit.id || 0,
                    bloodFridgeId: unit.bloodFridgeId,
                    bloodRackId: unit.bloodRackId,
                    slotNo: unit.slotNo,
                    status: unit.status || 1
                });

                if (this.isViewMode) {
                    this.form.disable();
                }

                this.isLoading = false;
            },
            error: () => {
                this.notificationsService.showNotification('Failed to load blood unit details', 'snack-bar-danger');
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
