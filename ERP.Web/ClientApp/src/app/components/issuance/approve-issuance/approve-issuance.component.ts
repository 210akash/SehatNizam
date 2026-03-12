import { Component, Inject, TemplateRef, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { IssuanceService } from '../issuance.service';

@Component({
  selector: 'app-approve-issuance',
  templateUrl: './approve-issuance.component.html',
  styleUrl: './approve-issuance.component.css',
  standalone: false
})

export class ApproveIssuanceComponent {
  isLoading = false;

  @ViewChild('confirmationDialog') confirmationDialog!: TemplateRef<any>;
  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private issuanceService: IssuanceService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
  }

  async approve() {
    (await this.issuanceService.approveIssuance(this.data.element.id)).subscribe({
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
