import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { NotificationService } from '../notification.service';
import { DepartmentService } from '../../../department/department.service';

@Component({
    selector: 'app-add-notification',
    templateUrl: './add-notification.component.html',
    styleUrls: ['./add-notification.component.css'],
    standalone: false
})

export class AddNotificationComponent implements OnInit {
    notificationForm!: FormGroup;
    isLoading = false;
    isEditMode: boolean = false;
    departmentList: any[] = [];
    minDate = new Date();

    constructor(
        private dialog: MatDialog,
        private notificationsService: NotificationsService,
        private formBuilder: FormBuilder,
        private notificationService: NotificationService,
        private constantService: ConstantService,
        private departmentService: DepartmentService,
        @Inject(MAT_DIALOG_DATA) public data: { element: any }
    ) { }

    async ngOnInit(): Promise<void> {
        this.notificationForm = this.formBuilder.group({
            id: [0],
            departmentId: [''],
            title: ['', [Validators.required, Validators.maxLength(200)]],
            message: ['', [Validators.required, Validators.maxLength(2000)]],
            expireDate: ['', Validators.required]
        });

        await this.getDepartmentList();
        this.LoadData(this.data?.element);
    }

  getDepartmentList(): void {
    this.departmentService.getDepartmentByCompany('2').subscribe(data => {
      this.departmentList = data;
    });
  }

    LoadData(element: any) {
        if (element != null) {
            this.isEditMode = true;
            this.constantService.LoadData(element, this.notificationForm);
        }
    }

    validateExpireDate(): boolean {
        const expireDate = this.notificationForm.get('expireDate')?.value;
        if (!expireDate) return false;

        const selectedDate = new Date(expireDate);
        const today = new Date();
        today.setHours(0, 0, 0, 0);

        if (selectedDate < today) {
            this.notificationsService.showNotification('Expire date cannot be in the past', 'snack-bar-danger');
            return false;
        }
        return true;
    }

    SaveData() {
        if (this.notificationForm.invalid) {
            this.constantService.markFormGroupTouched(this.notificationForm);
            this.notificationsService.showNotification('Please fill all required fields', 'snack-bar-danger');
            return;
        }

        if (!this.validateExpireDate()) {
            return;
        }

        this.isLoading = true;
        let _notificationForm: any = {};
        _notificationForm = Object.assign(_notificationForm, this.notificationForm.value);

        // Format date
        let expireDate = new Date(this.notificationForm.get('expireDate')?.value);
        _notificationForm['expireDate'] = expireDate.toISOString();

        this.notificationService.saveNotification(_notificationForm).subscribe({
            next: (data) => {
                if (data.Status == 200) {
                    this.notificationsService.showNotification(data.Data, 'snack-bar-success');
                    this.dialog.closeAll();
                } else {
                    this.notificationsService.showNotification(data.Data || data.Error || 'Error saving notification', 'snack-bar-danger');
                }
                this.isLoading = false;
            },
            error: (error) => {
                this.notificationsService.showNotification(error?.error?.Error || 'Error saving notification', 'snack-bar-danger');
                console.error(error);
                this.isLoading = false;
            }
        });
    }

    close() {
        this.dialog.closeAll();
    }
}
