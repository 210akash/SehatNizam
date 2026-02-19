import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

@Component({
    selector: 'app-view-crv',
    templateUrl: './view-crv.component.html',
    styleUrl: './view-crv.component.css',
    standalone: false
})

export class ViewCrvComponent {
  isLoading = false;
  urlSafe: SafeResourceUrl | undefined;
  dialogRef: any;
  tDebit = 0;
  tCredit = 0;

  constructor(private dialog: MatDialog, public sanitizer: DomSanitizer,@Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.calculateTotals();
  }

  GetDocument(event: any, path: any, template: any) {
    this.urlSafe = this.sanitizer.bypassSecurityTrustResourceUrl(path + '#toolbar=0');
    this.dialogRef = this.dialog.open(template, {
      width: '70%',
      maxHeight: '90vh',
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
}
