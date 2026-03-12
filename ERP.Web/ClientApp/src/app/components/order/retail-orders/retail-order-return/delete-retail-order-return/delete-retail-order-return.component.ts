import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ConstantService } from '../../../../../Service/constant.service';
import { NotificationsService } from '../../../../../Service/notification.service';
import { RetailOrderReturnService } from '../retail-order-return.service';

@Component({
  selector: 'app-delete-retail-order-return',
  templateUrl: './delete-retail-order-return.component.html',
  styleUrl: './delete-retail-order-return.component.css',
  standalone: false
})

export class DeleteRetailOrderReturnComponent {
  isLoading = false;
  isEditMode: boolean = false;
  grandTotals: any;
  qtyTotals: any;
  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private retailOrderReturnService: RetailOrderReturnService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.updateTotals();
  }

  updateTotals(): void {
    const details = this.data?.element?.retailOrderReturnDetail || [];
    let totalQty = 0;
    let totalAmount = 0;

    for (const detail of details) {
      const price = detail.dispatchDetail?.retailOrderItem?.retailPrice || 0;
      const qty = detail.quantity || 0;
      totalQty += qty;
      totalAmount += price * qty;
    }

    this.qtyTotals = totalQty;
    this.grandTotals = totalAmount;
  }

  async delete() {
    (await this.retailOrderReturnService.deleteRetailOrderReturn(this.data.element.id)).subscribe({
      next: (data) => {
        if (data == true) {
          this.isLoading = false;
          this.notificationsService.showNotification('Delete Successfully', 'snack-bar-success');
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
