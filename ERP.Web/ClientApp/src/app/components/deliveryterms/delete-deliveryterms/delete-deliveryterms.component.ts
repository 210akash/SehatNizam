import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { DeliveryTermsService } from '../deliveryterms.service';

@Component({
    selector: 'app-delete-deliveryterms',
    templateUrl: './delete-deliveryterms.component.html',
    styleUrl: './delete-deliveryterms.component.css',
    standalone: false
})

export class DeleteDeliveryTermsComponent {
  isLoading = false;

  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private deliverytermsService: DeliveryTermsService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
  }

  async delete() {
    (await this.deliverytermsService.deleteDeliveryTerms(this.data.element.id)).subscribe({
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
