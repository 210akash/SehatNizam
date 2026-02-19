import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { GSTService } from '../gst.service';

@Component({
    selector: 'app-delete-gst',
    templateUrl: './delete-gst.component.html',
    styleUrl: './delete-gst.component.css',
    standalone: false
})

export class DeleteGSTComponent {
  isLoading = false;

  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private gstService: GSTService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
  }

  async delete() {
    (await this.gstService.deleteGST(this.data.element.id)).subscribe({
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
