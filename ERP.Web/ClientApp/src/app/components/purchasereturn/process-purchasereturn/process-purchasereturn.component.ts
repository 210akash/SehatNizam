import { Component, Inject } from '@angular/core';
import { FormGroup} from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { PurchaseReturnService } from '../purchasereturn.service';

@Component({
    selector: 'app-process-purchasereturn',
    templateUrl: './process-purchasereturn.component.html',
    styleUrl: './process-purchasereturn.component.css',
    standalone: false
})

export class ProcessPurchaseReturnComponent {
  purchaseReturnForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  qtyTotals : any;
  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private purchaseReturnService: PurchaseReturnService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.updateTotals();
  }
 updateTotals(): void {
  const details = this.data?.element?.purchaseReturnDetail || [];
  let totalQty = 0;

  for (const detail of details) {
    const qty = detail.quantity || 0;
    totalQty += qty;
  }

  this.qtyTotals = totalQty;
}

  async process() {
    (await this.purchaseReturnService.processPurchaseReturn(this.data.element.id)).subscribe({
      next: (data) => {
        if (data == true) {
          this.isLoading = false;
          this.notificationsService.showNotification('Process Successfully', 'snack-bar-success');
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
