import { Component, Inject } from '@angular/core';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { TransactionService } from '../../transaction/transaction.service';

@Component({
    selector: 'app-delete-brv',
    templateUrl: './delete-brv.component.html',
    styleUrl: './delete-brv.component.css',
    standalone: false
})

export class DeleteBrvComponent {
  isLoading = false;
  urlSafe: SafeResourceUrl | undefined;
  dialogRef: any;
  tDebit = 0;
  tCredit = 0;

  constructor(private dialog: MatDialog, public sanitizer: DomSanitizer,private notificationsService: NotificationsService, private transactionService: TransactionService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }
  ngOnInit(): void {
    this.calculateTotals();
  }

  GetDocument(event: any, path: any, template: any) {
    this.urlSafe = this.sanitizer.bypassSecurityTrustResourceUrl(path + '#toolbar=0');
    this.dialogRef = this.dialog.open(template, {
      width: '50%',
      height: '50%',
      disableClose: true,
    });
  }

  async calculateTotals() {
      var _tdebitAmount = 0;
      var _tcreditAmount = 0;
  
        (this.data.element.transactionDetails).forEach((detail: any) => {
          // Access item and update the unitRate value
          _tdebitAmount  = _tdebitAmount + detail.debitAmount;
          _tcreditAmount  = _tcreditAmount + detail.creditAmount;
        });
  
        this.tDebit = _tdebitAmount;
        this.tCredit = _tcreditAmount;
    }
  async delete() {
    (await this.transactionService.deleteTransaction(this.data.element.id)).subscribe({
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
