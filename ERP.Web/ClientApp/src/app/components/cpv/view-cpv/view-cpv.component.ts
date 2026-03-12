import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { DocumentViewerComponent } from '../../document-viewer/document-viewer.component';

@Component({
    selector: 'app-view-cpv',
    templateUrl: './view-cpv.component.html',
    styleUrl: './view-cpv.component.css',
    standalone: false
})

export class ViewCpvComponent {
  isLoading = false;
  urlSafe: SafeResourceUrl | undefined;
  dialogRef: any;
  tDebit = 0;
  tCredit = 0;
  constructor(private dialog: MatDialog, public sanitizer: DomSanitizer,@Inject(MAT_DIALOG_DATA) public data: { element: any }) { }
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
}
