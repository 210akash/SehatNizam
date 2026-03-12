import { Component, Inject } from '@angular/core';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { AuditReviewService } from '../auditreview.service';

@Component({
    selector: 'app-delete-auditreview',
    templateUrl: './delete-auditreview.component.html',
    styleUrl: './delete-auditreview.component.css',
    standalone: false
})

export class DeleteAuditReviewComponent {
  isLoading = false;
  isEditMode: boolean = false;

  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private iGPService: AuditReviewService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
  }

  async delete() {
    (await this.iGPService.deleteAuditReview(this.data.element.id)).subscribe({
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
