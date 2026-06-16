import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
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
    private dialogRef: MatDialogRef<DeleteRadiologyOrderComponent>,
    private service: RadiologyOrderService,
    private notifications: NotificationsService,
    @Inject(MAT_DIALOG_DATA) public data: { radiologyOrderId: number, variables: any }
  ) { }

  delete(): void {
    this.service.deleteRadiologyOrder(this.data.radiologyOrderId).subscribe((res: any) => {
      if (res?.Status === 200 || res === true) {
        this.notifications.showNotification('Radiology Order Deleted Successfully!', 'snack-bar-success');
        this.dialogRef.close();
      }
    });
  }
}
