import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { ShipmentModeService } from '../shipmentmode.service';

@Component({
    selector: 'app-delete-shipmentmode',
    templateUrl: './delete-shipmentmode.component.html',
    styleUrl: './delete-shipmentmode.component.css',
    standalone: false
})

export class DeleteShipmentModeComponent {
  isLoading = false;

  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private shipmentmodeService: ShipmentModeService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
  }

  async delete() {
    (await this.shipmentmodeService.deleteShipmentMode(this.data.element.id)).subscribe({
      next: (data) => {
        if (data == true) {
          this.isLoading = false;
          this.notificationsService.showNotification(data.Data, 'snack-bar-success');
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
