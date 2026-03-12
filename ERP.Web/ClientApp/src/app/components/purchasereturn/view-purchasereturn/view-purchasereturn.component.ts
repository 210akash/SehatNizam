import { Component, Inject } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'app-view-purchasereturn',
    templateUrl: './view-purchasereturn.component.html',
    styleUrl: './view-purchasereturn.component.css',
    standalone: false
})

export class ViewPurchaseReturnComponent {
  PurchaseReturnForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = true;
  qtyTotals : any;
  constructor(@Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.updateTotals();
  }
  updateTotals(): void {
  const details = this.data?.element?.purchaseReturnDetail || [];
  let totalQty = 0;

  for (const detail of details) {
    const qty = detail.quantity || 0;
    totalQty += qty;
  }

  this.qtyTotals = totalQty;
}
}
