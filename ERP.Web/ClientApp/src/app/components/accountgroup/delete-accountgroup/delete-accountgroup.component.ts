import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { AccountGroupService } from '../accountgroup.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';

@Component({
    selector: 'app-delete-accountgroup',
    templateUrl: './delete-accountgroup.component.html',
    styleUrl: './delete-accountgroup.component.css',
    standalone: false
})

export class DeleteAccountGroupComponent {
    isLoading = false;
    isEditMode: boolean = false;
    accountFlow : any;
    constructor(private dialog: MatDialog,  private notificationsService: NotificationsService, private accountgroupService: AccountGroupService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }
  
    ngOnInit(): void {
      this.accountFlow = this.data.element.account.accountFlow.name;
    }
  
    async delete() {
      (await this.accountgroupService.deleteAccountGroup(this.data.element.id)).subscribe({
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
