import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { CrossMatchService } from '../cross-match.service';

@Component({
    selector: 'app-delete-cross-match',
    templateUrl: './delete-cross-match.component.html',
    styleUrl: './delete-cross-match.component.css',
    standalone: false
})
export class DeleteCrossMatchComponent {
    isLoading = false;

    constructor(
        private dialog: MatDialog,
        private notificationsService: NotificationsService,
        private service: CrossMatchService,
        @Inject(MAT_DIALOG_DATA) public data: { element: any }
    ) { }

    delete() {
        const id = this.data.element.crossMatchId || this.data.element.id;
        this.isLoading = true;
        this.service.deleteItem(id).subscribe({
            next: (data: any) => {
                if (data === true || data?.Status === 200) {
                    this.notificationsService.showNotification('Successfully Deleted!', 'snack-bar-success');
                    this.dialog.closeAll();
                } else {
                    this.notificationsService.showNotification(data?.Data || 'Delete failed', 'snack-bar-danger');
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
