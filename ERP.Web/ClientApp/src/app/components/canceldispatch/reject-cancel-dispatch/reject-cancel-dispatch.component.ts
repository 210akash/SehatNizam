import { Component, ElementRef, Inject, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { CancelDispatchService } from '../canceldispatch.service';
import { NotificationsService } from '../../../Service/notification.service';
import { OrderStatusEnum } from '../../../Service/constant.service';

@Component({
  selector: 'app-reject-cancel-dispatch',
  templateUrl: './reject-cancel-dispatch.component.html',
  styleUrl: './reject-cancel-dispatch.component.css',
  standalone: false,
})

export class RejectCancelDispatchComponent {
  @ViewChild('inputElement') inputElement!: ElementRef;
  isLoading = false;
  isEditMode: boolean = false;

  constructor(
    private cancelDispatchService: CancelDispatchService,
    private notificationsService: NotificationsService,
    private dialog: MatDialog,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) {}

  ngOnInit(): void {}

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
        (sum: number, item: any) =>
          sum + item.quantity * item.orderItem?.distributorPrice,
        0
      ) || 0
    );
  }

  getNewStatusId(element: any): number {
    switch (element.statusId) {

      case OrderStatusEnum.CancelDispatchForward:
        return OrderStatusEnum.CancelDispatchCreated;

      case OrderStatusEnum.CancelDispatchSalesReviewed:
        return OrderStatusEnum.CancelDispatchForward;

      case OrderStatusEnum.CancelDispatchAccountReviewed:
        return OrderStatusEnum.CancelDispatchSalesReviewed;

      default:
        return 0;
    }
  }

  async process() {

    let command = {
      'id': this.data.element.id,
      'statusId': this.getNewStatusId(this.data.element),
      'isReject': true,
      'remarks': this.inputElement.nativeElement.value
    };

    (
      await this.cancelDispatchService.processCancelDispatch(command)
    ).subscribe({
      next: (data: boolean) => {
        if (data == true) {
          this.isLoading = false;
          this.notificationsService.showNotification(
            'Dispatch Processed!',
            'snack-bar-success'
          );
          this.dialog.closeAll();
        }
      },
      error: (error: string) => {
        console.log(error);
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        this.isLoading = false;
      },
    });
  }
}
