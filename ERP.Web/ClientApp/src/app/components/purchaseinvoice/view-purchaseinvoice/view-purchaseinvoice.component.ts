import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { ConstantService } from '../../../Service/constant.service';
import { GRNService } from '../../grn/grn.service';

@Component({
  selector: 'app-view-purchaseinvoice',
  templateUrl: './view-purchaseinvoice.component.html',
  styleUrl: './view-purchaseinvoice.component.css',
  standalone: false,
})
export class ViewPurchaseInvoiceComponent {
  viewPurchaseInvoiceForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = true;
  tAmount: any = 0;
  tsaleTax: any = 0;
  tfed: any = 0;

  constructor(
    private formBuilder: FormBuilder,
    private notificationsService: NotificationsService,
    private constantService: ConstantService,
    private grnService: GRNService,
    private dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: { element: any, check: any }
  ) { }

  ngOnInit(): void {
    this.viewPurchaseInvoiceForm = this.formBuilder.group({
      id: [this.data.element.id],
      isWHTTax: [0],
      wHTPercentage: [0],
      wHTAmount: [0],
    });

    this.viewPurchaseInvoiceForm
      .get('isWHTTax')
      ?.valueChanges.subscribe((value) => {
        const whtControl = this.viewPurchaseInvoiceForm.get('wHTPercentage');
        if (value) {
          whtControl?.setValidators([Validators.required]); // Add any validators if needed
          whtControl?.updateValueAndValidity();
        } else {
          whtControl?.clearValidators();
          whtControl?.updateValueAndValidity();
        }
      });

    if (this.data.check == 1) {
      this.viewPurchaseInvoiceForm.get('wHTPercentage')?.enable();
    } else {
      this.viewPurchaseInvoiceForm.get('wHTPercentage')?.disable();
    }

    this.calculateTotals();
    this.LoadData();
  }

  async LoadData() {
    if (this.data.element.whtPercentage != null) {
      this.viewPurchaseInvoiceForm.get('isWHTTax')?.patchValue(true);

      this.viewPurchaseInvoiceForm
        .get('wHTPercentage')
        ?.patchValue(this.data.element.whtPercentage);

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
    let wHTPercentage =
      this.viewPurchaseInvoiceForm.get('wHTPercentage')?.value;
    let cal = (this.tAmount * wHTPercentage) / 100;
    this.viewPurchaseInvoiceForm
      .get('wHTAmount')
      ?.patchValue(Number(cal.toFixed(2)));
  }

  SaveData() {
    this.checkValidity();

    if (this.viewPurchaseInvoiceForm.invalid) {
      this.constantService.markFormGroupTouched(this.viewPurchaseInvoiceForm);
      return;
    }

    this.isLoading = true;
    let _viewPurchaseInvoiceForm: any = {};
    _viewPurchaseInvoiceForm = Object.assign(
      _viewPurchaseInvoiceForm,
      this.viewPurchaseInvoiceForm.value
    );

    if (
      this.viewPurchaseInvoiceForm.get('isWHTTax')?.value == false ||
      this.viewPurchaseInvoiceForm.get('wHTPercentage')?.value == 0
    ) {
      _viewPurchaseInvoiceForm['wHTPercentage'] = null;
    }

    this.grnService.updateWHTPercentage(_viewPurchaseInvoiceForm).subscribe({
      next: (data: { Status: number; Data: string }) => {
        if (data.Status == 200) {
          this.notificationsService.showNotification(
            data.Data,
            'snack-bar-success'
          );
          this.dialog.closeAll();
        } else
          this.notificationsService.showNotification(
            data.Data,
            'snack-bar-danger'
          );
        this.isLoading = false;
      },
      error: (error: string) => {
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        console.error(error);
        this.isLoading = false;
      },
    });
  }

  checkValidity() {
    let isWHTTax = this.viewPurchaseInvoiceForm.get('isWHTTax')?.value;

    if (isWHTTax === false) {
      this.viewPurchaseInvoiceForm.get('wHTPercentage')?.clearValidators();
      this.viewPurchaseInvoiceForm
        .get('wHTPercentage')
        ?.updateValueAndValidity();
    } else if (isWHTTax === true) {
      this.viewPurchaseInvoiceForm
        .get('wHTPercentage')
        ?.setValidators([Validators.required]);
      this.viewPurchaseInvoiceForm
        .get('wHTPercentage')
        ?.updateValueAndValidity();
    }
  }

  changeCheckbox() {
    this.viewPurchaseInvoiceForm.get('wHTPercentage')?.patchValue(0);
    this.viewPurchaseInvoiceForm.get('wHTAmount')?.patchValue(0);
  }


}