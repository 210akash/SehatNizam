import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { ComparativeStatementService } from '../comparativestatement.service';

@Component({
    selector: 'app-approve-comparativestatement',
    templateUrl: './approve-comparativestatement.component.html',
    styleUrl: './approve-comparativestatement.component.css',
    standalone: false
})

export class ApproveComparativeStatementComponent {
  isLoading = false;
  isEditMode: boolean = false;

  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private comparativestatementService: ComparativeStatementService,  @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
  }

  async approve() {
    (await this.comparativestatementService.approveComparativeStatement(this.data.element.id)).subscribe({
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
