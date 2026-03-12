import { Component, Inject } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { DispatchService } from '../dispatch.service';
import { SafeHtml } from '@angular/platform-browser';
import { PrintDispatchOrderReceiptComponent } from '../print-dispatch-order-receipt/print-dispatch-order-receipt.component';
import { PrintDispatchOrderInvoiceComponent } from '../print-dispatch-order-invoice/print-dispatch-order-invoice.component';

@Component({
  selector: 'app-print-dispatch-orders-popup',
  templateUrl: './print-dispatch-orders-popup.component.html',
  styleUrl: './print-dispatch-orders-popup.component.css',
  standalone: false
})

export class PrintDispatchOrdersPopupComponent {
  dispatchForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = true;

  ordersPrint: any[] = [];
  innerHtml: string = "";
  dialogRefPrint: any;

  constructor(private dispatchService: DispatchService, private dialog: MatDialog, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.ordersPrint = this.data.element.dispatchOrder;
    console.log(this.data.element);
  }

  // async GetPrint(orderId: number, templateId: number, template: any) {
  //   (await this.dispatchService.getOrderPrint(orderId, templateId, this.data.element.id)).subscribe(
  //     {
  //       next: (data) => {

  //         //   const newTab = window.open('', '_blank');
  //         //   var response = data.Data.replace(/\+/g, '%20');
  //         //   // let printData = decodeURIComponent(response);
  //         //   // let user = JSON.parse(localStorage.getItem('currentUser'));
  //         //   newTab?.document.write(`
  //         //   <html>
  //         //     <head>
  //         //       <title>App Print</title>
  //         //       <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@3.3.7/dist/css/bootstrap.min.css" integrity="sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u" crossorigin="anonymous">
  //         //     </head>
  //         //     <body>
  //         //     ${response}

  //         //     </body>
  //         //   </html>
  //         // `);
  //         //   newTab?.focus();

  //         this.innerHtml = data.Data as SafeHtml as string;

  //         this.dialogRefPrint = this.dialog.open(template, {
  //           width: '50%',
  //           // minHeight: '500px',
  //           disableClose: true,
  //         });

  //       },
  //       error: (error) => {
  //         console.log(error);
  //       }
  //     });
  // }

  // print(event: any) {
  //   const WindowPrt = window.open('', '', 'left=0,top=0,width=1100,height=1100,toolbar=0,scrollbars=0,status=0');
  //   WindowPrt!.document.write(this.innerHtml);
  //   setTimeout(() => {
  //     WindowPrt!.focus();
  //     WindowPrt!.print();
  //     WindowPrt!.close();
  //   }, 1000);
  // }

  openReceiptPrint(orderId: number) {
    const dialogRef = this.dialog.open(PrintDispatchOrderReceiptComponent, {
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      data: {
        element: this.data.element, orderId: orderId
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
    });
  }

  openInvoicePrint(orderId: number) {
    const dialogRef = this.dialog.open(PrintDispatchOrderInvoiceComponent, {
      panelClass: 'cstm_width_1100',
      maxHeight: '90vh',
      data: {
        element: this.data.element, orderId: orderId
      },
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
    });
  }


}