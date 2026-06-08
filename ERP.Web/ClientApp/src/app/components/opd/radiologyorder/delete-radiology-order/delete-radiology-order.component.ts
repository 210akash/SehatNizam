import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { RadiologyOrderService } from '../radiologyorder.service';

@Component({
  selector: 'app-delete-radiology-order',
  templateUrl: './delete-radiology-order.component.html',
  styleUrls: ['./delete-radiology-order.component.css'],
  standalone: false
})
export class DeleteRadiologyOrderComponent {
  constructor(
    private dialog: MatDialog,
    private service: RadiologyOrderService,
    private notifications: NotificationsService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }
  async delete(): Promise<void> {
    (await this.service.deleteRadiologyOrder(this.data.element.id)).subscribe((res: any) => {
      if (res?.Status === 200 || res === true) {
        this.notifications.showNotification('Radiology Order Deleted Successfully!', 'snack-bar-success');
        this.dialog.closeAll();
      }
    });
  }
}
