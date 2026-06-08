import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { ConstantService } from '../../../../Service/constant.service';
import { RoomService } from '../room.service';

@Component({
    selector: 'app-delete-room',
    templateUrl: './delete-room.component.html',
    styleUrl: './delete-room.component.css',
    standalone: false
})

export class DeleteRoomComponent {
  isLoading = false;
  isEditMode: boolean = false;

  constructor(private dialog: MatDialog,  private notificationsService: NotificationsService, private roomService: RoomService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
  }

  async delete() {
    (await this.roomService.deleteRoom(this.data.element.id)).subscribe({
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
