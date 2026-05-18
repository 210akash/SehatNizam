import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ServiceTypeService } from '../service-type.service';
import { NotificationsService } from '../../../../Service/notification.service';

@Component({
  selector: 'app-delete-service-type',
  templateUrl: './delete-service-type.component.html',
  styleUrls: ['./delete-service-type.component.css'],
  standalone: false
})
export class DeleteServiceTypeComponent {
  constructor(
    private dialog: MatDialog,
    private serviceType: ServiceTypeService,
    private notifications: NotificationsService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  delete(): void {
    this.serviceType.deleteServiceType(this.data.element.id).subscribe({
      next: (res: any) => {
        if (res.Status === 200) {
          this.notifications.showNotification('ServiceType Deleted Successfully!', 'snack-bar-success');
          this.dialog.closeAll();
        } else {
          this.notifications.showNotification(res.Message || 'Error deleting serviceType!', 'snack-bar-danger');
        }
      },
      error: () => {
        this.notifications.showNotification('An error occurred', 'snack-bar-danger');
      }
    });
  }
}
