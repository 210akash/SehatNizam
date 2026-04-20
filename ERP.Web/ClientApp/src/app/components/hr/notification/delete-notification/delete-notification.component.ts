import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { NotificationService } from '../notification.service';

@Component({
    selector: 'app-delete-notification',
    templateUrl: './delete-notification.component.html',
    styleUrls: ['./delete-notification.component.css'],
    standalone: false
})

export class DeleteNotificationComponent {
    notificationForm!: FormGroup;
    isLoading = false;

    constructor(
        private dialog: MatDialog,
        private formBuilder: FormBuilder,
        private notificationsService: NotificationsService,
        private notificationService: NotificationService,
        private constantService: ConstantService,
        @Inject(MAT_DIALOG_DATA) public data: { element: any }
    ) { }

    ngOnInit(): void {
        this.notificationForm = this.formBuilder.group({
            id: [0],
            title: [''],
            departmentName: [''],
            message: [''],
            expireDate: ['']
        });
        this.LoadData(this.data.element);
    }

    LoadData(element: any) {
        if (this.data.element?.id != null) {
            this.constantService.LoadData(element, this.notificationForm);
        }
    }

    async delete() {
        this.isLoading = true;
        (await this.notificationService.deleteNotification(this.data.element.id)).subscribe({
            next: (data) => {
                if (data.Status === 200) {
                    this.isLoading = false;
                    this.notificationsService.showNotification(data.Data, 'snack-bar-success');
                    this.dialog.closeAll();
                } else {
                    this.isLoading = false;
                    this.notificationsService.showNotification(data.Error || 'Error deleting notification', 'snack-bar-danger');
                }
            },
            error: (error) => {
                console.log(error);
                this.notificationsService.showNotification(error?.error?.Error || 'Error deleting notification', 'snack-bar-danger');
                this.isLoading = false;
            }
        });
    }

    close() {
        this.dialog.closeAll();
    }
}
