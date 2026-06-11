import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { ComponentTypeService } from '../component-type.service';

@Component({
    selector: 'app-delete-component-type',
    templateUrl: './delete-component-type.component.html',
    styleUrl: './delete-component-type.component.css',
    standalone: false
})
export class DeleteComponentTypeComponent {
    isLoading = false;

    constructor(
        private dialog: MatDialog,
        private notificationsService: NotificationsService,
        private service: ComponentTypeService,
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
