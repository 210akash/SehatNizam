import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { CancelDispatchService } from '../canceldispatch.service';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-add-cancel-dispatch',
  templateUrl: './add-cancel-dispatch.component.html',
  styleUrl: './add-cancel-dispatch.component.css',
  standalone: false
})

export class AddCancelDispatchComponent {
  orderList: any;
  dispatchForm!: FormGroup;
  pendingOrderItemsList: any[] = []; 
  isLoading = false;
  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private cancelDispatchService: CancelDispatchService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.dispatchForm = this.formBuilder.group({
      id: [0],
      orderId : [0],
      code: ['', Validators.required],
      vehicleId: [0, Validators.required],
      createdDate: [new Date()],
      status: [''],
      statusName: ['New'],
      statusId: [1],
      remarks: [''],
      biltyNo: [0, Validators.required],
      freightCharges: [0, Validators.required],
      dispatchOrder: this.formBuilder.array([])
    });

    this.getCancelDispatchCode();
    this.getPendingOrder();
  }

  getCancelDispatchCode() {
    this.cancelDispatchService.getCancelDispatchCode().subscribe((data: any) => {
      this.dispatchForm.get('code')?.patchValue(data.code);
    });
  }

  async getPendingOrder() {
    (await this.cancelDispatchService.getPendingCancelOrder(0)).subscribe((data: any) => {
      this.orderList = data;
    });
  }

  get totalQuantity(): number {
    return this.pendingOrderItemsList?.reduce((sum: number, item: any) => sum + item.quantity, 0) || 0;
  }
  
  get totalPrice(): number {
    return this.pendingOrderItemsList?.reduce((sum: number, item: any) => sum + (item.quantity * item.distributorPrice), 0) || 0;
  }

  async getPendingCancelOrderItems(orderId: any) {
    try {
     const data = await firstValueFrom(await this.cancelDispatchService.getPendingCancelOrderItems(orderId, 0));
     this.pendingOrderItemsList = data || [];
    } catch (error) {
      console.error('Error loading pending indent items:', error);
    }
  }
  
  SaveData() {
    this.isLoading = true;

    let _canceldispatchForm: any = {};
    _canceldispatchForm = Object.assign(_canceldispatchForm, this.dispatchForm.value);


    let _cancelDispatchcommand: any = {
      orderId: _canceldispatchForm.orderId,
      remarks: _canceldispatchForm.remarks,
      getOrderItems: this.pendingOrderItemsList // assuming each item matches GetOrderItems model
    };

    this.cancelDispatchService.saveCancelDispatch(_cancelDispatchcommand).subscribe({
      next: (data: { Status: number; Data: string; }) => {
        if (data.Status == 200) {
          this.notificationsService.showNotification(data.Data, 'snack-bar-success');
          this.dialog.closeAll();
        }
        else
          this.notificationsService.showNotification(data.Data, 'snack-bar-danger');
        this.isLoading = false;
      },
      error: (error: string) => {
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        console.error(error);
        this.isLoading = false;
      }
    });
  }
}
