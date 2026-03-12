import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { IssuanceService } from '../issuance.service';

@Component({
    selector: 'app-delete-issuance',
    templateUrl: './delete-issuance.component.html',
    styleUrl: './delete-issuance.component.css',
    standalone: false
})

export class DeleteIssuanceComponent {
  issuanceForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;

  constructor(private dialog: MatDialog, private formBuilder: FormBuilder, private notificationsService: NotificationsService, private issuanceService: IssuanceService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    
  }

  async delete() {
    (await this.issuanceService.deleteIssuance(this.data.element.id)).subscribe({
      next: (data) => {
        if (data == true) {
          this.isLoading = false;
          this.notificationsService.showNotification('Issance Deleted!', 'snack-bar-success');
          this.dialog.closeAll();
        }
      },
      error: (error) => {
        console.log(error);
        this.notificationsService.showNotification('Error Deleting Issuance!', 'snack-bar-danger');
        this.isLoading = false;
      }
    });
  }
}
