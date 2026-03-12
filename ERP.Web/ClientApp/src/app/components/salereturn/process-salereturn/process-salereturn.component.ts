import { Component, Inject } from '@angular/core';
import { FormGroup} from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { SaleReturnService } from '../salereturn.service';

@Component({
    selector: 'app-process-salereturn',
    templateUrl: './process-salereturn.component.html',
    styleUrl: './process-salereturn.component.css',
    standalone: false
})

export class ProcessSaleReturnComponent {
  saleReturnForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  grandTotals : any;
  qtyTotals : any;
  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private saleReturnService: SaleReturnService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

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

  async process() {
    (await this.saleReturnService.processSaleReturn(this.data.element.id)).subscribe({
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
