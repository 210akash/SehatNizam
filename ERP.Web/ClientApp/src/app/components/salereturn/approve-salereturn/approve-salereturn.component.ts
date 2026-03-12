import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { SaleReturnService } from '../salereturn.service';

@Component({
    selector: 'app-approve-salereturn',
    templateUrl: './approve-salereturn.component.html',
    styleUrl: './approve-salereturn.component.css',
    standalone: false
})

export class ApproveSaleReturnComponent {
  isLoading = false;
   grandTotals : any;
  qtyTotals : any;
  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private salereturnService: SaleReturnService,  @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.updateTotals();
  }

  updateTotals(): void {
  const details = this.data?.element?.saleReturnDetail || [];
  let totalQty = 0;
  let totalAmount = 0;

  for (const detail of details) {
    const price = detail.dispatchDetail?.orderItem?.retailPrice || 0;
    const qty = detail.quantity || 0;
    totalQty += qty;
    totalAmount += price * qty;
  }

  this.qtyTotals = totalQty;
  this.grandTotals = totalAmount;
}
  async approve() {
    (await this.salereturnService.approveSaleReturn(this.data.element.id)).subscribe({
       next: (data) => {
          if (data.item1 === 200) {
            this.isLoading = false;
            this.notificationsService.showNotification(data.item2, 'snack-bar-success');
            this.dialog.closeAll();
          }
          else if (data.item1 === 501) {
            this.isLoading = false;
            this.notificationsService.showNotification(data.item2, 'snack-bar-danger');
          }
          else if (data.item1 === 502) {
            this.isLoading = false;
            this.notificationsService.showNotification(data.item2, 'snack-bar-danger');
          }
          else if (data.item1 === 503) {
            this.isLoading = false;
            this.notificationsService.showNotification(data.item2, 'snack-bar-danger');
          }
          else {
            this.isLoading = false;
            this.notificationsService.showNotification('Error Approving, Please contact system admin!', 'snack-bar-danger');
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
