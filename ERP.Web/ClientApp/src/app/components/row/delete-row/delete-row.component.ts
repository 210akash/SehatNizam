import { Component, Inject } from '@angular/core';
import { NotificationsService } from '../../../Service/notification.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { RowService } from '../row.service';

@Component({
  selector: 'app-delete-row',
  standalone: false,
  templateUrl: './delete-row.component.html',
  styleUrl: './delete-row.component.css'
})
export class DeleteRowComponent {
  isLoading = false;
  isEditMode: boolean = false;
  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private rowService: RowService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  async delete() {
    (await this.rowService.deleteRow(this.data.element.id)).subscribe({
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
