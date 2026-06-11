import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { BloodGroupService } from '../blood-group.service';

@Component({
    selector: 'app-delete-blood-group',
    templateUrl: './delete-blood-group.component.html',
    styleUrl: './delete-blood-group.component.css',
    standalone: false
})
export class DeleteBloodGroupComponent {
    isLoading = false;

    constructor(
        private dialog: MatDialog,
        private notificationsService: NotificationsService,
        private service: BloodGroupService,
        @Inject(MAT_DIALOG_DATA) public data: { element: any }
    ) { }

    delete() {
        this.isLoading = true;
        this.service.deleteItem(this.data.element.id).subscribe({
            next: (data) => {
                if (data === true) {
                    this.isLoading = false;
                    this.notificationsService.showNotification('Successfully Deleted!', 'snack-bar-success');
                    this.dialog.closeAll();
                }
            },
            error: (error) => {
                console.log(error);
                this.notificationsService.showNotification(error, 'snack-bar-danger');
                this.isLoading = false;
            }
        });
    }
}
