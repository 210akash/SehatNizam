import { RackService } from './../rack.service';
import { Component, Inject } from '@angular/core';
import { ConstantService } from '../../../Service/constant.service';
import { NotificationsService } from '../../../Service/notification.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { FormBuilder, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-delete-rack',
  standalone: false,
  templateUrl: './delete-rack.component.html',
  styleUrl: './delete-rack.component.css'
})
export class DeleteRackComponent {
  isLoading = false;
  isEditMode: boolean = false;
  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private rackService: RackService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  async delete() {
    (await this.rackService.deleteRack(this.data.element.id)).subscribe({
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
