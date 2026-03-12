import { Component, Inject, TemplateRef, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { DispatchService } from '../dispatch.service';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-approve-dispatch',
  templateUrl: './approve-dispatch.component.html',
  styleUrl: './approve-dispatch.component.css',
  standalone: false
})

export class ApproveDispatchComponent {
  isLoading = false;
  isEditMode: boolean = false;

  gElement: any;

  grandTotalAmount: any;
  grandTotalWeight: any;
  vehicleCapacity: any;

  @ViewChild('confirmationDialog') confirmationDialog!: TemplateRef<any>;
  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private dispatchService: DispatchService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

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

  // async approve() {
  //   (await this.dispatchService.approveDispatch(this.data.element.id)).subscribe({
  //     next: (data) => {
  //       if (data == true) {
  //         this.isLoading = false;
  //         this.notificationsService.showNotification(data.Data, 'snack-bar-success');
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
  async approve() {
    const dialogRef = this.dialog.open(this.confirmationDialog,{
        disableClose: false
    });
    const confirmed = await firstValueFrom(dialogRef.afterClosed());

      if (confirmed) {
      if (this.isLoading) return; // Prevent double click
        // Proceed with approval if user confirmed
        this.isLoading = true;

        (this.dispatchService.approveDispatch(this.data.element.id)).subscribe({
          next: (data) => {
            if (data.item1 === 200) {
              this.isLoading = false;
              this.notificationsService.showNotification(data.item2, 'snack-bar-success');
              this.dialog.closeAll();
            }
            else if (data.item1 === 501) {
              this.isLoading = false;
              this.notificationsService.showNotification(data.item2, 'snack-bar-danger');
            }
            else if (data.item1 === 502) {
              this.isLoading = false;
              this.notificationsService.showNotification(data.item2, 'snack-bar-danger');
            }
            else if (data.item1 === 503) {
              this.isLoading = false;
              this.notificationsService.showNotification(data.item2, 'snack-bar-danger');
            }
            else {
              this.isLoading = false;
              this.notificationsService.showNotification('Error Approving, Please contact system admin!', 'snack-bar-danger');
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
        console.log('Approval canceled');
      }
    }

    async reject() {
      (await this.dispatchService.rejectDispatch(this.data.element.id)).subscribe({
        next: (data) => {
          if (data.item1 === 200) {
            this.isLoading = false;
            this.notificationsService.showNotification(data.item2, 'snack-bar-success');
            this.dialog.closeAll();
          }
          else if (data.item1 === 501) {
            this.isLoading = false;
            this.notificationsService.showNotification(data.item2, 'snack-bar-danger');
          }
          else if (data.item1 === 502) {
            this.isLoading = false;
            this.notificationsService.showNotification(data.item2, 'snack-bar-danger');
          }
          else if (data.item1 === 503) {
            this.isLoading = false;
            this.notificationsService.showNotification(data.item2, 'snack-bar-danger');
          }
          else {
            this.isLoading = false;
            this.notificationsService.showNotification('Error Approving, Please contact system admin!', 'snack-bar-danger');
          }
        },
        error: (error: string) => {
          console.log(error);
          this.notificationsService.showNotification(error, 'snack-bar-danger');
          this.isLoading = false;
        }
      });
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
