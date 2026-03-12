import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
    selector: 'app-view-warehousetransfer',
    templateUrl: './view-warehousetransfer.component.html',
    styleUrl: './view-warehousetransfer.component.css',
    standalone: false
})

export class ViewWarehouseTransferComponent {
  isLoading = false;
  TMaterialCost! : number;
  Quantity ! : number;
  constructor(@Inject(MAT_DIALOG_DATA) public data: { element: any }) { }
  ngOnInit(): void {
    this.calculateTotal();
  }
 
  calculateTotal() {
    this.TMaterialCost = 0;
    this.Quantity = 0;
    const costDetails = this.data.element.warehouseTransferDetail || [];
    costDetails.forEach((item: any) => {
      const amount = item.rate * item.quantity || 0;
      const quantity = item.quantity || 0;
      this.TMaterialCost += amount;
      this.Quantity += quantity;
    });
  }
}
