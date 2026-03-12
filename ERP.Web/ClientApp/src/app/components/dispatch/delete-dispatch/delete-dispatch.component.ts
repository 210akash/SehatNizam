import { Component, Inject, TemplateRef, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { DispatchService } from '../dispatch.service';

@Component({
    selector: 'app-delete-dispatch',
    templateUrl: './delete-dispatch.component.html',
    styleUrl: './delete-dispatch.component.css',
    standalone: false
})

export class DeleteDispatchComponent {
  isLoading = false;
  isEditMode: boolean = false;

  gElement: any;

  grandTotalAmount: any;
  grandTotalWeight: any;
  vehicleCapacity: any;

  @ViewChild('confirmationDialog') confirmationDialog!: TemplateRef<any>;
  constructor(private dialog: MatDialog, private formBuilder: FormBuilder, private notificationsService: NotificationsService, private dispatchService: DispatchService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
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

  // async delete() {
  //   (await this.dispatchService.deleteDispatch(this.data.element.id)).subscribe({
  //     next: (data) => {
  //       if (data == true) {
  //         this.isLoading = false;
  //         this.notificationsService.showNotification('Dispatch Deleted!', 'snack-bar-success');
  //         this.dialog.closeAll();
  //       }
  //     },
  //     error: (error) => {
  //       console.log(error);
  //       this.notificationsService.showNotification(error, 'snack-bar-danger');
  //       this.isLoading = false;
  //     }
  //   });
  // }
  async delete() {
    // Open the confirmation dialog using the template reference
    const dialogRef = this.dialog.open(this.confirmationDialog);

    // Wait for the dialog to be closed and get the result
    const confirmed = await dialogRef.afterClosed().toPromise();

    if (confirmed) {
      // Proceed with approval if user confirmed
      this.isLoading = true;

      (await this.dispatchService.deleteDispatch(this.data.element.id)).subscribe({
        next: (data) => {
          if (data === true) {
            this.isLoading = false;
            this.notificationsService.showNotification('Dispatch Deleted!', 'snack-bar-success');
            this.dialog.closeAll();
          }
        },
        error: (error) => {
          console.log(error);
          this.notificationsService.showNotification(error, 'snack-bar-danger');
          this.isLoading = false;
        }
      });
    } else {
      // User canceled, don't proceed
      console.log('Deleted canceled');
    }
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
