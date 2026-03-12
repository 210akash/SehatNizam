import { Component, Inject, TemplateRef, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { WarehouseTransferService } from '../warehousetransfer.service';

@Component({
  selector: 'app-approve-warehousetransfer',
  templateUrl: './approve-warehousetransfer.component.html',
  styleUrl: './approve-warehousetransfer.component.css',
  standalone: false,
})
export class ApproveWarehouseTransferComponent {
  isLoading = false;
    TMaterialCost! : number;
  Quantity ! : number;

  @ViewChild('confirmationDialog') confirmationDialog!: TemplateRef<any>;
  constructor(
    private dialog: MatDialog,
    private notificationsService: NotificationsService,
    private warehousetransferService: WarehouseTransferService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) {}

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
  async approve() {
    // Open the confirmation dialog using the template reference
    const dialogRef = this.dialog.open(this.confirmationDialog);

    // Wait for the dialog to be closed and get the result
    const confirmed = await dialogRef.afterClosed().toPromise();

    if (confirmed) {
      // Proceed with approval if user confirmed
      this.isLoading = true;

      (
        await this.warehousetransferService.approveWarehouseTransfer(this.data.element.id)
      ).subscribe({
        next: (data: { item1: number; item2: string; }) => {
          if (data.item1 === 200) {
            this.isLoading = false;
            this.notificationsService.showNotification(
              data.item2,
              'snack-bar-success'
            );
            this.dialog.closeAll();
          } else if (data.item1 === 501) {
            this.isLoading = false;
            this.notificationsService.showNotification(
              data.item2,
              'snack-bar-danger'
            );
          } else if (data.item1 === 502) {
            this.isLoading = false;
            this.notificationsService.showNotification(
              data.item2,
              'snack-bar-danger'
            );
          } else if (data.item1 === 503) {
            this.isLoading = false;
            this.notificationsService.showNotification(
              data.item2,
              'snack-bar-danger'
            );
          } else {
            this.isLoading = false;
            this.notificationsService.showNotification(
              'Error Approving, Please contact system admin!',
              'snack-bar-danger'
            );
          }
        },
        error: (error: string) => {
          console.log(error);
          this.notificationsService.showNotification(error, 'snack-bar-danger');
          this.isLoading = false;
        },
      });
    } else {
      // User canceled, don't proceed
      console.log('Approval canceled');
    }
  }

   async revoke() {
    (await this.warehousetransferService.revokeWarehouseTransfer(this.data.element.id)).subscribe({
     next: (data: { item1: number; item2: string; }) => {
        if (data.item1 == 200) {
          this.isLoading = false;
          this.notificationsService.showNotification('Warehouse Transfer Revoke', 'snack-bar-success');
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
