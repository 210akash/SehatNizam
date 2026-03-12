import { Component, Inject } from '@angular/core';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { RejectReasonService } from '../rejectreason.service';

@Component({
    selector: 'app-delete-rejectreason',
    templateUrl: './delete-rejectreason.component.html',
    styleUrl: './delete-rejectreason.component.css',
    standalone: false
})

export class DeleteRejectReasonComponent {
  isLoading = false;
  isEditMode: boolean = false;

  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private rejectreasonService: RejectReasonService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
  }

  async delete() {
    (await this.rejectreasonService.deleteRejectReason(this.data.element.id)).subscribe({
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
