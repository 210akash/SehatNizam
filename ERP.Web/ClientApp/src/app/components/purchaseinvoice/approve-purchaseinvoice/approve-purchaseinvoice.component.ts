import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { GRNService } from '../../grn/grn.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-approve-purchaseinvoice',
  templateUrl: './approve-purchaseinvoice.component.html',
  styleUrl: './approve-purchaseinvoice.component.css',
  standalone: false,
})
export class ApprovePurchaseInvoiceComponent {
  approvePurchaseInvoiceForm!: FormGroup;
  isLoading = false;
  tAmount: any = 0;
  tsaleTax: any = 0;
  tfed: any = 0;

  constructor(
    private dialog: MatDialog,
    private notificationsService: NotificationsService,
    private grnService: GRNService,
    private formBuilder: FormBuilder,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  ngOnInit(): void {
    this.approvePurchaseInvoiceForm = this.formBuilder.group({
      id: [this.data.element.id],
      wHTAmount: [0],
      comments: ['']
    });

    this.calculateTotals();
    this.LoadData();
  }

  async approve() {
    (
      await this.grnService.approvePurchaseInvoice(this.data.element.id)
    ).subscribe({
      next: (data: any) => {
        if (data.item1 === 200) {
          this.isLoading = false;
          this.notificationsService.showNotification(data.item2, 'snack-bar-success');
          this.dialog.closeAll();
        }
        else if (data.item1 === 501) {
          this.isLoading = false;
          this.notificationsService.showNotification(data.item2, 'snack-bar-danger');
        }
        else if (data.item1 === 502) {
          this.isLoading = false;
          this.notificationsService.showNotification(data.item2, 'snack-bar-danger');
        }
        else if (data.item1 === 503) {
          this.isLoading = false;
          this.notificationsService.showNotification(data.item2, 'snack-bar-danger');
        }
        else {
          this.isLoading = false;
          this.notificationsService.showNotification('Error Approving, Please contact system admin!', 'snack-bar-danger');
        }
      },
      error: (error: any) => {
        console.log(error);
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        this.isLoading = false;
      },
    });
  }

  async reject() {
    let comm = this.approvePurchaseInvoiceForm.get('comments')?.value;
    (
      await this.grnService.rejectPurchaseInvoice(this.data.element.id, comm)
    ).subscribe({
      next: (data: any) => {
        if (data == 200) {
          this.isLoading = false;
          this.notificationsService.showNotification('Successful!', 'snack-bar-success');
          this.dialog.closeAll();
        }
        else {
          this.isLoading = false;
          this.notificationsService.showNotification('Error Processing, Please contact system admin!', 'snack-bar-danger');
        }
      },
      error: (error: any) => {
        console.log(error);
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        this.isLoading = false;
      },
    });
  }

  async LoadData() {
    if (this.data.element.whtPercentage != null) {
      this.calculateWHTAmount();
    }
  }

  async calculateTotals() {
    let totalAmount = 0;
    let totalsaleTax = 0;
    let totalfed = 0;
    this.data.element.grnDetail.forEach((detail: any) => {
      const received = detail?.received ?? 0;
      const unitRate = detail?.inspectionDetail?.igpDetail?.purchaseOrderDetail?.unitRate ?? 0;
      const saletax = detail?.inspectionDetail?.igpDetail?.purchaseOrderDetail?.gst / detail?.inspectionDetail?.igpDetail?.purchaseOrderDetail?.quantity * received;
      const fedTax = detail?.inspectionDetail?.igpDetail?.purchaseOrderDetail?.fed / detail?.inspectionDetail?.igpDetail?.purchaseOrderDetail?.quantity * received;
      totalAmount += received * unitRate;
      totalsaleTax += saletax;
      totalfed += fedTax;
    });

    this.tAmount = totalAmount;
    this.tsaleTax = totalsaleTax;
    this.tfed = totalfed;
  }

  calculateWHTAmount() {
    let cal = (this.tAmount * this.data.element.whtPercentage) / 100;
    this.approvePurchaseInvoiceForm.get('wHTAmount')?.patchValue(Number(cal).toFixed(2));
  }


}