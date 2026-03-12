import { Component, ElementRef, Inject, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { CancelDispatchService } from '../canceldispatch.service';
import { NotificationsService } from '../../../Service/notification.service';
import { OrderStatusEnum } from '../../../Service/constant.service';

@Component({
  selector: 'app-process-cancel-dispatch',
  templateUrl: './process-cancel-dispatch.component.html',
  styleUrl: './process-cancel-dispatch.component.css',
  standalone: false,
})
export class ProcessCancelDispatchComponent {
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

      case OrderStatusEnum.CancelDispatchCreated:
        return OrderStatusEnum.CancelDispatchForward;

      case OrderStatusEnum.CancelDispatchForward:
        return OrderStatusEnum.CancelDispatchSalesReviewed;

      case OrderStatusEnum.CancelDispatchSalesReviewed:
        return OrderStatusEnum.CancelDispatchAccountReviewed;

      case OrderStatusEnum.CancelDispatchAccountReviewed:
        return OrderStatusEnum.CancelDispatchConfirm;

      default:
        return 0;
    }
  }

  async process() {

    let command = {
      'id': this.data.element.id,
      'statusId': this.getNewStatusId(this.data.element),
      'isReject': false,
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
