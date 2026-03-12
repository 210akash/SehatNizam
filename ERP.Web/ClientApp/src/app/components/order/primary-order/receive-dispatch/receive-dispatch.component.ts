import { Component, OnInit, Inject, TemplateRef, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ConstantService, OrderStatusEnum } from '../../../../Service/constant.service';
import { DispatchService } from '../../../dispatch/dispatch.service';
import { NotificationsService } from '../../../../Service/notification.service';

@Component({
  selector: 'app-receive-dispatch',
  templateUrl: './receive-dispatch.component.html',
  styleUrls: ['./receive-dispatch.component.css'], standalone: false
})

export class ReceiveDispatchComponent implements OnInit {
  isLoading = false;
  receiveDispatchForm!: FormGroup;
  dialogRef: any;

  dispatchList: any[] = [];
  statusEnum: any;

  gDispatchOrder: any;

  @ViewChild('confirmationDialog') confirmationDialog!: TemplateRef<any>;
  constructor(private dispatchService: DispatchService, private formBuilder: FormBuilder, private constantService: ConstantService, private dialog: MatDialog,
    private notificationsService: NotificationsService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.statusEnum = OrderStatusEnum;
    this.receiveDispatchForm = this.formBuilder.group({
      deliveryChallanCode: ['']
    });

    this.getDispatchByOrderId();
  }

  async getDispatchByOrderId() {
    (await this.dispatchService.getDispatchByOrderId(this.data.element.id)).subscribe({
      next: (data: any) => {
        this.dispatchList = data;
      },
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  // async receiveDispatch(dispatchOrder: any) {
  //   if (this.receiveDispatchForm.get('deliveryChallanCode')?.value === dispatchOrder.dcCode) {
  //     (await this.dispatchService.receiveDispatchOrder(dispatchOrder.id)).subscribe({
  //       next: (data: any) => {
  //         if (data == true) {
  //           this.notificationsService.showNotification('Dispatch Received!', 'snack-bar-success');
  //         }
  //         else {
  //           this.notificationsService.showNotification('Error Receiving Dispatch!', 'snack-bar-danger');
  //         }
  //       },
  //       error: (error: any) => {
  //         console.log(error);
  //         this.isLoading = false;
  //       }
  //     });
  //   }
  //   else {
  //     this.notificationsService.showNotification('Please Enter Correct Delivery Challan Code!', 'snack-bar-danger');
  //     return;
  //   }
  // }
  async receiveDispatch(dispatchOrder: any) {
    // Open the confirmation dialog using the template reference
    const dialogRef = this.dialog.open(this.confirmationDialog);

    // Wait for the dialog to be closed and get the result
    const confirmed = await dialogRef.afterClosed().toPromise();

    if (confirmed) {

      if (this.receiveDispatchForm.get('deliveryChallanCode')?.value === dispatchOrder.dcCode) {
        // Proceed with approval if user confirmed
        this.isLoading = true;

        (await this.dispatchService.receiveDispatchOrder(dispatchOrder.id)).subscribe({
          next: (data) => {
            if (data == true) {
              this.notificationsService.showNotification('Dispatch Received!', 'snack-bar-success');
              this.receiveDispatchForm.get('deliveryChallanCode')?.patchValue('');
              this.getDispatchByOrderId();
            }
            else {
              this.notificationsService.showNotification('Error Receiving Dispatch!', 'snack-bar-danger');
            }
          },
          error: (error) => {
            console.log(error);
            this.notificationsService.showNotification(error, 'snack-bar-danger');
            this.isLoading = false;
          }
        });
      }
      else {
        this.notificationsService.showNotification('Please Enter Correct Delivery Challan Code!', 'snack-bar-danger');
        return;
      }

    } else {
      // User canceled, don't proceed
      console.log('Approval canceled');
    }
  }

  viewDispatch(element: any, template: any) {
    this.gDispatchOrder = element;
    const dialogRef = this.dialog.open(template, {
      data: { element: element },
      width: '35%',
      maxHeight: '95vh',
      autoFocus: true,
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log(`Dialog result: ${result}`);
    });
  }

  getTotalQuantity(): number {
  return this.gDispatchOrder?.dispatchDetail?.reduce(
    (sum: number, item: any) => sum + (item.quantity || 0),
    0
  ) || 0;
}
}
