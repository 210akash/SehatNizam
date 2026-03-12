import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { CancelDispatchService } from '../canceldispatch.service';

@Component({
  selector: 'app-delete-cancel-dispatch',
  templateUrl: './delete-cancel-dispatch.component.html',
  styleUrl: './delete-cancel-dispatch.component.css',
  standalone: false,
})
export class DeleteCancelDispatchComponent {
  isLoading = false;
  orderList: any;
  pendingOrderItemsList: any[] = [];

  constructor(
    private cancelDispatchService: CancelDispatchService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) {}

  ngOnInit(): void {
  }

  get totalQuantity(): number {
    return (
      this.data.element.cancelDispatchDetail?.reduce(
        (sum: number, item: any) => sum + item.quantity,
        0
      ) || 0
    );
  }

  get totalPrice(): number {
    return (
      this.data.element.cancelDispatchDetail?.reduce(
        (sum: number, item: any) => sum + item.quantity * item.orderItem?.distributorPrice,
        0
      ) || 0
    );
  }
}
