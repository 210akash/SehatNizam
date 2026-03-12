import { Component, Inject } from '@angular/core';
import { FormGroup} from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { IssuanceService } from '../issuance.service';

@Component({
    selector: 'app-process-issuance',
    templateUrl: './process-issuance.component.html',
    styleUrl: './process-issuance.component.css',
    standalone: false
})

export class ProcessIssuanceComponent {
  issuanceForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;

  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private issuanceService: IssuanceService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
  }

  async process() {
    (await this.issuanceService.processIssuance(this.data.element.id)).subscribe({
      next: (data) => {
        if (data == true) {
          this.isLoading = false;
          this.notificationsService.showNotification('Issuance Processed!', 'snack-bar-success');
          this.dialog.closeAll();
        }
      },
      error: (error) => {
        console.log(error);
        this.notificationsService.showNotification('Error Processing Issuance!', 'snack-bar-danger');
        this.isLoading = false;
      }
    });
  }
}
