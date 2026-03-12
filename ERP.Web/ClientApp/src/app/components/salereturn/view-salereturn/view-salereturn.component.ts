import { Component, Inject } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'app-view-salereturn',
    templateUrl: './view-salereturn.component.html',
    styleUrl: './view-salereturn.component.css',
    standalone: false
})

export class ViewSaleReturnComponent {
  SaleReturnForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = true;
  grandTotals : any;
  qtyTotals : any;
  constructor(@Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.updateTotals();
  }
  updateTotals(): void {
  const details = this.data?.element?.saleReturnDetail || [];
  let totalQty = 0;
  let totalAmount = 0;

  for (const detail of details) {
    const price = detail.dispatchDetail?.orderItem?.retailPrice || 0;
    const qty = detail.quantity || 0;
    totalQty += qty;
    totalAmount += price * qty;
  }

  this.qtyTotals = totalQty;
  this.grandTotals = totalAmount;
}
}
