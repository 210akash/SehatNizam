import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { ServiceService } from '../service.service';

@Component({
  selector: 'app-delete-service',
  templateUrl: './delete-service.component.html',
  styleUrls: ['./delete-service.component.css'],
  standalone: false
})
export class DeleteServiceComponent {
  constructor(
    private dialog: MatDialog,
    private service: ServiceService,
    private notifications: NotificationsService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  delete(): void {
    this.service.deleteService(this.data.element.id).subscribe({
      next: (res: any) => {
        if (res.Status === 200) {
          this.notifications.showNotification('Service Deleted Successfully!', 'snack-bar-success');
          this.dialog.closeAll();
        } else {
          this.notifications.showNotification(res.Message || 'Error deleting service!', 'snack-bar-danger');
        }
      },
      error: () => {
        this.notifications.showNotification('An error occurred', 'snack-bar-danger');
      }
    });
  }
}
