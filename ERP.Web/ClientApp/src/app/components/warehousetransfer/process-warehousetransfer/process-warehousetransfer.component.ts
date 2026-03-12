import { Component, Inject } from '@angular/core';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { WarehouseTransferService } from '../warehousetransfer.service';

@Component({
    selector: 'app-process-warehousetransfer',
    templateUrl: './process-warehousetransfer.component.html',
    styleUrl: './process-warehousetransfer.component.css',
    standalone: false
})

export class ProcessWarehouseTransferComponent {
  isLoading = false;
   TMaterialCost! : number;
  Quantity ! : number;
  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private warehousetransferService: WarehouseTransferService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.calculateTotal();
  }

calculateTotal() {
    this.TMaterialCost = 0;
    this.Quantity = 0;
    const costDetails = this.data.element.warehouseTransferDetail || [];
    costDetails.forEach((item: any) => {
      const amount = item.rate * item.quantity || 0;
      const quantity = item.quantity || 0;
      this.TMaterialCost += amount;
      this.Quantity += quantity;
    });
  }

  async process() {
    (await this.warehousetransferService.processWarehouseTransfer(this.data.element.id)).subscribe({
      next: (data: boolean) => {
        if (data == true) {
          this.isLoading = false;
          this.notificationsService.showNotification('Proceed successfully', 'snack-bar-success');
          this.dialog.closeAll();
        }
      },
      error: (error: string) => {
        console.log(error);
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        this.isLoading = false;
      }
    });
  }
}
