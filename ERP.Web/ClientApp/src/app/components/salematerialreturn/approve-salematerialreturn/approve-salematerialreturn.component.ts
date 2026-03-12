import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { SaleMaterialReturnService } from '../salematerialreturn.service';

@Component({
    selector: 'app-approve-salematerialreturn',
    templateUrl: './approve-salematerialreturn.component.html',
    styleUrl: './approve-salematerialreturn.component.css',
    standalone: false
})

export class ApproveSaleMaterialReturnComponent {
  isLoading = false;
   grandTotals : any;
  qtyTotals : any;
  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private salematerialreturnService: SaleMaterialReturnService,  @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.updateTotals();
  }

  updateTotals(): void {
  const details = this.data?.element?.saleMaterialReturnDetail || [];
  let totalQty = 0;

  for (const detail of details) {
    const qty = detail.quantity || 0;
    totalQty += qty;
  }

  this.qtyTotals = totalQty;
}

  async approve() {
    (await this.salematerialreturnService.approveSaleMaterialReturn(this.data.element.id)).subscribe({
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
