import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { LabOrderTypeService } from '../lab-order-type.service';

@Component({
  selector: 'app-delete-lab-order-type',
  templateUrl: './delete-lab-order-type.component.html',
  styleUrls: ['./delete-lab-order-type.component.css'],
  standalone: false
})
export class DeleteLabOrderTypeComponent {
  constructor(
    private dialog: MatDialog,
    private service: LabOrderTypeService,
    private notifications: NotificationsService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  delete(): void {
    this.service.deleteLabOrderType(this.data.element.id).subscribe({
      next: (res: any) => {
        if (res.Status === 200) {
          this.notifications.showNotification('Lab Order Type Deleted Successfully!', 'snack-bar-success');
          this.dialog.closeAll();
        } else {
          this.notifications.showNotification(res.Message || 'Error deleting lab order type!', 'snack-bar-danger');
        }
      },
      error: () => {
        this.notifications.showNotification('An error occurred', 'snack-bar-danger');
      }
    });
  }
}
