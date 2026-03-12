import { Component, Inject } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'app-view-salematerialreturn',
    templateUrl: './view-salematerialreturn.component.html',
    styleUrl: './view-salematerialreturn.component.css',
    standalone: false
})

export class ViewSaleMaterialReturnComponent {
  SaleMaterialReturnForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = true;
  qtyTotals : any;
  constructor(@Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.updateTotals();
  }
  updateTotals(): void {
  const details = this.data?.element?.saleMaterialReturnDetail || [];
  let totalQty = 0;

  for (const detail of details) {
    const qty = detail.quantity || 0;
    totalQty += qty;
  }

  this.qtyTotals = totalQty;
}
}
