import { Component, Inject } from '@angular/core';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { TransactionService } from '../transaction.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

@Component({
    selector: 'app-approve-transaction',
    templateUrl: './approve-transaction.component.html',
    styleUrl: './approve-transaction.component.css',
    standalone: false
})

export class ApproveTransactionComponent {
  isLoading = false;
  isEditMode: boolean = false;
  urlSafe: SafeResourceUrl | undefined;
  dialogRef: any;
  tDebit = 0;
  tCredit = 0;

  constructor(private dialog: MatDialog,private notificationsService: NotificationsService,public sanitizer: DomSanitizer, private transactionService: TransactionService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

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
  async approve() {
    (await this.transactionService.approveTransaction(this.data.element.id)).subscribe({
      next: (data) => {
        if (data == true) {
          this.isLoading = false;
          this.notificationsService.showNotification('Voucher Approved', 'snack-bar-success');
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
