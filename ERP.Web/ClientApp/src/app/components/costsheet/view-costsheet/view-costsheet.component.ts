import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'app-view-costsheet',
    templateUrl: './view-costsheet.component.html',
    styleUrl: './view-costsheet.component.css',
    standalone: false
})

export class ViewCostSheetComponent {
  isLoading = false;
  TMaterialCost! : number;
  TFillingPerPet!: number;
  TCostOfProduction! : number;
  CostPerPet! : number;
  advSaleTaxAmt!: number;
  advFEDAmt!: number;

  constructor(@Inject(MAT_DIALOG_DATA) public data: { element: any }) { }
  ngOnInit(): void {
    this.calculateTotal();
  }
 
  calculateTotal() {
    this.TMaterialCost = 0;
    this.TFillingPerPet = 0;
    this.TCostOfProduction = 0;
    this.CostPerPet = 0;
    this.advSaleTaxAmt = 0;
    this.advFEDAmt = 0;

    const costDetails = this.data.element.costSheetDetail || [];
  
    costDetails.forEach((item: any) => {
      const amount = item.rate * item.quantity || 0;
      this.TMaterialCost += amount;
    });
  
    const quantity = +this.data.element.quantity || 0;
    const tollFillRate = +this.data.element.tollFillRate || 0;
    const advSaleTaxPer = +this.data.element.advSaleTaxPer || 0;
    const advFEDPer = +this.data.element.advFEDPer || 0;
  
    this.TFillingPerPet = quantity * tollFillRate;
    this.TCostOfProduction = this.TMaterialCost + this.TFillingPerPet;
    this.CostPerPet = quantity ? this.TCostOfProduction / quantity : 0;
  
    this.advSaleTaxAmt = (this.TFillingPerPet * advSaleTaxPer) / 100;
    this.advFEDAmt = (this.TFillingPerPet * advFEDPer) / 100;
  }
}
