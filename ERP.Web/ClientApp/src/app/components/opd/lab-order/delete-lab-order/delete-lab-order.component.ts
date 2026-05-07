import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { LabOrderService } from '../lab-order.service';

@Component({
  selector: 'app-delete-lab-order',
  templateUrl: './delete-lab-order.component.html',
  styleUrls: ['./delete-lab-order.component.css'],
  standalone: false
})
export class DeleteLabOrderComponent {
  constructor(
    private dialog: MatDialog,
    private service: LabOrderService,
    private notifications: NotificationsService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }
  async delete(): Promise<void> {
    (await this.service.deleteLabOrder(this.data.element.id)).subscribe((res: any) => {
      if (res?.Status === 200 || res === true) {
        this.notifications.showNotification('Lab Order Deleted Successfully!', 'snack-bar-success');
        this.dialog.closeAll();
      }
    });
  }
}
