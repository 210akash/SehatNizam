import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { PaymentModeService } from '../paymentmode.service';

@Component({
    selector: 'app-delete-paymentmode',
    templateUrl: './delete-paymentmode.component.html',
    styleUrl: './delete-paymentmode.component.css',
    standalone: false
})

export class DeletePaymentModeComponent {
  isLoading = false;
  isEditMode: boolean = false;

  constructor(private dialog: MatDialog,  private notificationsService: NotificationsService, private paymentmodeService: PaymentModeService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
  }

  async delete() {
    (await this.paymentmodeService.deletePaymentMode(this.data.element.id)).subscribe({
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
