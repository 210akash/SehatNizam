import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { PurchaseDemandService } from '../purchasedemand.service';

@Component({
    selector: 'app-approve-purchasedemand',
    templateUrl: './approve-purchasedemand.component.html',
    styleUrl: './approve-purchasedemand.component.css',
    standalone: false
})

export class ApprovePurchaseDemandComponent {
  isLoading = false;
  isEditMode: boolean = false;

  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private purchasedemandService: PurchaseDemandService,  @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
  }

  async approve() {
    (await this.purchasedemandService.approvePurchaseDemand(this.data.element.id)).subscribe({
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
