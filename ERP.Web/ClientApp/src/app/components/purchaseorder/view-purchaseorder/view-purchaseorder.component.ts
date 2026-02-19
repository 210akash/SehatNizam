import { Component, Inject } from '@angular/core';
import { FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-view-purchaseorder',
  templateUrl: './view-purchaseorder.component.html',
  styleUrl: './view-purchaseorder.component.css',
  standalone: false
})

export class ViewPurchaseOrderComponent {
  PurchaseOrderForm!: FormGroup;
  isLoading = false;
  tAmount: any = 0;
  tDiscount: any = 0;
  tExpense: any = 0;
  tSaleTax: any = 0;
  partyAmount: any = 0;
  netAmount: any = 0;

  constructor(@Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.calculateTotals();
  }

  async calculateTotals() {
    var _tAmount = 0;
    var _tSaleTax = 0;
    this.tExpense = this.data.element.deliveryCharges + this.data.element.otherCharges;
    this.tDiscount = this.data.element.discount;

    (this.data.element.purchaseOrderDetail).forEach((detail: any) => {
      // Access item and update the unitRate value
      var value = detail.value;
      _tAmount = _tAmount + value;
      var gst = detail.gst;
      _tSaleTax = _tSaleTax + gst;
    });
    this.tAmount = _tAmount;
    this.tSaleTax = _tSaleTax;
    this.netAmount = this.tAmount + this.tSaleTax + this.tExpense - this.tDiscount;
    this.partyAmount = (this.tAmount + this.tExpense) - this.tDiscount;
  }
}
