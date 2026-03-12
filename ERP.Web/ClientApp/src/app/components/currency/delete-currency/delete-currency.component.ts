import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup,  } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { CurrencyService } from '../currency.service';

@Component({
    selector: 'app-delete-currency',
    templateUrl: './delete-currency.component.html',
    styleUrl: './delete-currency.component.css',
    standalone: false
})

export class DeleteCurrencyComponent {
  currencyForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;

  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private currencyService: CurrencyService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
  }

  async delete() {
    (await this.currencyService.deleteCurrency(this.data.element.id)).subscribe({
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
