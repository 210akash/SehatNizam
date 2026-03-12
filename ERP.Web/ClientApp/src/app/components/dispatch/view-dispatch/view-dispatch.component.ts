import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-view-dispatch',
  templateUrl: './view-dispatch.component.html',
  styleUrl: './view-dispatch.component.css',
  standalone: false
})

export class ViewDispatchComponent {
  isLoading = false;
  isEditMode: boolean = true;

  gElement: any;

  grandTotalAmount: any;
  grandTotalWeight: any;
  vehicleCapacity: any;

  constructor(@Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.isLoading = true;
    this.gElement = this.data.element;
    this.calculateWeight();
  }

  calculateWeight() {
    if (!this.gElement || !this.gElement.dispatchOrder) {
      return;
    }

    let gTotalWeight = 0;
    let gTotalAmount = 0;

    // Iterate over the dispatch orders inside gElement
    this.gElement.dispatchOrder.forEach((order: any) => {
      let totalWeight = 0;
      let totalAmount = 0;

      if (order.dispatchDetail && Array.isArray(order.dispatchDetail)) {
        order.dispatchDetail.forEach((detail: any) => {
          const weight = detail.orderItem.item.weight || 0;
          const amount = detail.orderItem.distributorPrice || 0;
          const quantity = detail.quantity || 0;

          totalWeight += weight * quantity;
          totalAmount += amount * quantity;
        });
      }

      // Assign computed values to the dispatch order
      order.totalWeight = totalWeight;
      order.totalAmount = totalAmount;

      // Add to the grand totals
      gTotalWeight += totalWeight;
      gTotalAmount += totalAmount;

    });

    // Assign grand totals to gElement
    this.gElement.grandTotalWeight = gTotalWeight;
    this.gElement.grandTotalAmount = gTotalAmount;

    // Optionally, store them separately if needed
    this.grandTotalWeight = gTotalWeight;
    this.grandTotalAmount = gTotalAmount;
  }

getTotalQuantity(orderId: number): number {
  const dispatchOrder = this.data.element.dispatchOrder?.find(
    (d: any) => d.orderId === orderId
  );

  return dispatchOrder?.dispatchDetail?.reduce(
    (sum: number, item: any) => sum + (item.quantity || 0),
    0
  ) || 0;
}
}