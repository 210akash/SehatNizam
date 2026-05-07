import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { RadiologyTypeService } from '../radiologytype.service';

@Component({
  selector: 'app-delete-radiologytype',
  templateUrl: './delete-radiologytype.component.html',
  styleUrls: ['./delete-radiologytype.component.css'],
  standalone: false
})
export class DeleteRadiologyTypeComponent {
  constructor(
    private dialog: MatDialog,
    private service: RadiologyTypeService,
    private notifications: NotificationsService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  delete(): void {
    this.service.deleteRadiologyType(this.data.element.id).subscribe({
      next: (res: any) => {
        if (res.Status === 200) {
          this.notifications.showNotification('Radiology Type Deleted Successfully!', 'snack-bar-success');
          this.dialog.closeAll();
        } else {
          this.notifications.showNotification(res.Message || 'Error deleting radiology type!', 'snack-bar-danger');
        }
      },
      error: () => {
        this.notifications.showNotification('An error occurred', 'snack-bar-danger');
      }
    });
  }
}
