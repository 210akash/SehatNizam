import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { BedService } from '../bed.service';
import { NotificationsService } from '../../../../Service/notification.service';

@Component({
    selector: 'app-delete-bed',
    templateUrl: './delete-bed.component.html',
    styleUrl: './delete-bed.component.css',
    standalone: false
})

export class DeleteBedComponent {
  isLoading = false;
  isEditMode: boolean = false;

  constructor(private dialog: MatDialog,  private notificationsService: NotificationsService, private bedService: BedService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
  }
  async delete() {
    (await this.bedService.deleteBed(this.data.element.id)).subscribe({
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
