import { Component, Inject } from '@angular/core';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { AccountFlowService } from '../accountflow.service';

@Component({
    selector: 'app-delete-accountflow',
    templateUrl: './delete-accountflow.component.html',
    styleUrl: './delete-accountflow.component.css',
    standalone: false
})

export class DeleteAccountFlowComponent {
  isLoading = false;
  isEditMode: boolean = false;

  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private accountflowService: AccountFlowService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
  }

  async delete() {
    (await this.accountflowService.deleteAccountFlow(this.data.element.id)).subscribe({
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
