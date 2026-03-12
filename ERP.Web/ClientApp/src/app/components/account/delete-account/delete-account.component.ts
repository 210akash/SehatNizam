import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { AccountService } from '../account.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';

@Component({
    selector: 'app-delete-account',
    templateUrl: './delete-account.component.html',
    styleUrl: './delete-account.component.css',
    standalone: false
})

export class DeleteAccountComponent {
    deleteAccountForm!: FormGroup;
    isLoading = false;
    isEditMode: boolean = false;
  
    constructor(private dialog: MatDialog, private formBuilder: FormBuilder, private notificationsService: NotificationsService, private accountService: AccountService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }
  
    ngOnInit(): void {
      this.deleteAccountForm = this.formBuilder.group({
        id: [0],
        code: ['', Validators.required],
        name: ['', Validators.required],
        description: ['', Validators.required],
      });
      this.LoadData(this.data.element);
    }
  
    LoadData(element: any) {
      if (this.data.element.id != null) {
        this.isEditMode = true;
      }
      this.constantService.LoadData(element, this.deleteAccountForm);
    }
  
    async delete() {
      (await this.accountService.deleteAccount(this.data.element.id)).subscribe({
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
