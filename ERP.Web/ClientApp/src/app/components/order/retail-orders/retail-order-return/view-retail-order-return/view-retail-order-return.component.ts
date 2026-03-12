import { Component, Inject } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'app-view-retail-order-return',
    templateUrl: './view-retail-order-return.component.html',
    styleUrl: './view-retail-order-return.component.css',
    standalone: false
})

export class ViewRetailOrderReturnComponent {
  RetailOrderReturnForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = true;
  grandTotals : any;
  qtyTotals : any;
  constructor(@Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.updateTotals();
  }
  updateTotals(): void {
  const details = this.data?.element?.retailOrderReturnDetail || [];
  let totalQty = 0;
  for (const detail of details) {
    const qty = detail.quantity || 0;
    totalQty += qty;
  }
  this.qtyTotals = totalQty;
}
}
