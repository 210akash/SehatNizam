import { Component, Inject } from '@angular/core';
import { NotificationsService } from '../../../Service/notification.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { SectionService } from '../section.service';

@Component({
  selector: 'app-delete-section',
  standalone: false,
  templateUrl: './delete-section.component.html',
  styleUrl: './delete-section.component.css'
})
export class DeleteSectionComponent {
  isLoading = false;
  isEditMode: boolean = false;
  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private sectionService: SectionService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  async delete() {
    (await this.sectionService.deleteSection(this.data.element.id)).subscribe({
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
