import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ReferrerService } from '../referrer.service';
import { NotificationsService } from '../../../../Service/notification.service';

@Component({
  selector: 'app-delete-referrer',
  templateUrl: './delete-referrer.component.html',
  styleUrls: ['./delete-referrer.component.css'],
  standalone: false
})
export class DeleteReferrerComponent {
  constructor(
    private dialog: MatDialog,
    private Referrer: ReferrerService,
    private notifications: NotificationsService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  delete(): void {
    this.Referrer.deleteReferrer(this.data.element.id).subscribe({
      next: (res: any) => {
        if (res.Status === 200) {
          this.notifications.showNotification('Referrer Deleted Successfully!', 'snack-bar-success');
          this.dialog.closeAll();
        } else {
          this.notifications.showNotification(res.Message || 'Error deleting Referrer!', 'snack-bar-danger');
        }
      },
      error: () => {
        this.notifications.showNotification('An error occurred', 'snack-bar-danger');
      }
    });
  }
}
