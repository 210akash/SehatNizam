import { Component, Inject } from '@angular/core';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { TransactionService } from '../../transaction/transaction.service';
import { DocumentViewerComponent } from '../../document-viewer/document-viewer.component';

@Component({
    selector: 'app-process-cpv',
    templateUrl: './process-cpv.component.html',
    styleUrl: './process-cpv.component.css',
    standalone: false
})

export class ProcessCpvComponent {
  isLoading = false;
  isEditMode: boolean = false;
  urlSafe: SafeResourceUrl | undefined;
  dialogRef: any;
  tDebit = 0;
  tCredit = 0;
  
  constructor(private dialog: MatDialog, public sanitizer: DomSanitizer,  private notificationsService: NotificationsService, private transactionService: TransactionService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.calculateTotals();
  }

  openDocumentViewer(path: string) {
    this.dialog.open(DocumentViewerComponent, {
      data: { path },
      width: '70%',
      maxHeight: '90vh',
      disableClose: true
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

  async process() {
    (await this.transactionService.processTransaction(this.data.element.id)).subscribe({
      next: (data) => {
        if (data == true) {
          this.isLoading = false;
          this.notificationsService.showNotification('Voucher Proceed successfully', 'snack-bar-success');
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
