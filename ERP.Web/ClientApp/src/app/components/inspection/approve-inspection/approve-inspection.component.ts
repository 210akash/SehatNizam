import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { InspectionService } from '../inspection.service';

@Component({
    selector: 'app-approve-inspection',
    templateUrl: './approve-inspection.component.html',
    styleUrl: './approve-inspection.component.css',
    standalone: false
})

export class ApproveInspectionComponent {
  isLoading = false;
  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private inspectionService: InspectionService,  @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
  }

  async approve() {
    (await this.inspectionService.approveInspection(this.data.element.id)).subscribe({
      next: (data) => {
        if (data == true) {
          this.isLoading = false;
          this.notificationsService.showNotification('Approved Successfully', 'snack-bar-success');
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
