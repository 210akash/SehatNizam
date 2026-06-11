import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { CrossMatchService } from '../../cross-match/cross-match.service';

@Component({
    selector: 'app-return-to-cross-match',
    templateUrl: './return-to-cross-match.component.html',
    styleUrl: './return-to-cross-match.component.css',
    standalone: false
})
export class ReturnToCrossMatchComponent {
    isLoading = false;

    constructor(
        private dialog: MatDialog,
        private notificationsService: NotificationsService,
        private crossMatchService: CrossMatchService,
        @Inject(MAT_DIALOG_DATA) public data: { element: any }
    ) { }

    confirm() {
        const crossMatchId = this.data.element?.bloodCrossMatchId;
        if (!crossMatchId) {
            this.notificationsService.showNotification('No cross match found for this request', 'snack-bar-danger');
            return;
        }

        this.isLoading = true;
        this.crossMatchService.deleteItem(crossMatchId).subscribe({
            next: (data: any) => {
                if (data === true || data?.Status === 200) {
                    this.notificationsService.showNotification('Request returned to Cross Match queue', 'snack-bar-success');
                    this.dialog.closeAll();
                } else {
                    this.notificationsService.showNotification(data?.Message || 'Action failed', 'snack-bar-danger');
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
