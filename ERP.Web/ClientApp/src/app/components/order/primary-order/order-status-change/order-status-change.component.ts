import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MatPaginator } from '@angular/material/paginator';
import { PrimaryOrderService } from '../order.service';
import { ConstantService, OrderStatusEnum } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';

@Component({
  selector: 'app-order-status-change',
  templateUrl: './order-status-change.component.html',
  styleUrls: ['./order-status-change.component.css'], standalone: false
})

export class OrderStatusChangeComponent implements OnInit {
  isLoading = false;
  isEditMode: boolean = false;
  orderStatusChangeForm!: FormGroup;
  statusEnum: any;

  gElement: any;

  totalQuantity = 0;
  totalShippedQuantity = 0;
  billingAmount = 0;
  totalLeftQuantity = 0;

  constructor(private notificationsService: NotificationsService, private constantService: ConstantService, private dialog: MatDialog, private orderService: PrimaryOrderService,
    private formBuilder: FormBuilder, @Inject(MAT_DIALOG_DATA) public data: { element: any, toStatusId: any, statement: any, shippedOrderQuantityForm: any }) { }
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngOnInit(): void {
    this.statusEnum = OrderStatusEnum;
    this.orderStatusChangeForm = this.formBuilder.group({
      id: ['', Validators.required],
      createdDate: ['', Validators.required],
      dealershipId: ['', Validators.required],
      dealership: ['', Validators.required],
      territory: ['', Validators.required],
      dealershipAddress: ['', Validators.required],
      comments: [''],
      orderItemsList: this.formBuilder.array([]),
      transactionId: ['']
    });

    this.getOrderById();
  }

  LoadData() {
    if (this.gElement.orderStatusId === OrderStatusEnum.Create || this.gElement.orderStatusId === OrderStatusEnum.Dispatched) {
      this.isEditMode = true;
      this.constantService.LoadData(this.gElement, this.orderStatusChangeForm);
      this.orderStatusChangeForm.get('dealership')?.patchValue(this.gElement.dealership?.name);
      this.orderStatusChangeForm.get('territory')?.patchValue(this.gElement.dealership?.territory?.name);
      this.setEditProducts(this.gElement.orderItems);
    }
  }

  async updateOrderStatus() {

    if (this.orderStatusChangeForm.invalid) {
      this.constantService.markFormGroupTouched(this.orderStatusChangeForm);
      return;
    }

    if (this.data.toStatusId == this.statusEnum.InProcess) {
      const hasInvalidQuantity = this.orderItemsList.controls.some(productControl => {
        const orderQty = productControl.get('quantity')?.value || 0;
        const stockQty = productControl.get('leftQuantity')?.value || 0;
        return orderQty > stockQty;
      });

      if (hasInvalidQuantity) {
        this.notificationsService.showNotification('Order quantity cannot be greater than available stock', 'snack-bar-danger');
        return;
      }
    }

    let _updateOrderStatus = {
      orderId: this.data.element.id,
      fromStatusId: this.data.element.orderStatusId,
      toStatusId: this.data.toStatusId,
      comments: this.orderStatusChangeForm.get('comments')?.value,
      transactionId: this.orderStatusChangeForm.get('transactionId')?.value
    };

    (await this.orderService.updateOrderStatus(_updateOrderStatus)).subscribe({
      next: (data: { Status: number; Message: any; }) => {
        if (data.Status == 200) {
          this.isLoading = false;
          this.notificationsService.showNotification(data.Message, 'snack-bar-success');
          this.dialog.closeAll();
        }
        else {
          this.isLoading = false;
          this.notificationsService.showNotification(data.Message, 'snack-bar-danger');
        }
      },
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  get orderItemsList(): FormArray {
    return this.orderStatusChangeForm.get('orderItemsList') as FormArray;
  }

  async setEditProducts(data: any) {
    const productArray = this.orderStatusChangeForm.get('orderItemsList') as FormArray;
    data.forEach((data: { id: any; item: { id: any; name: any; image: any; itemType: any; volume: any; weight: any; quantityInPack: any; retailPrice: any; tradePrice: any; }; quantity: any; shippedQuantity: null; distributorPrice: any; customDistributorPrice: any; leftQuantity: any; }) => {
      productArray.push(this.formBuilder.group({
        id: new FormControl(data.id),
        itemId: new FormControl(data.item.id),
        name: new FormControl(data.item.name),
        quantity: new FormControl(data.quantity),
        shippedQuantity: new FormControl(data.shippedQuantity == null ? data.quantity : data.shippedQuantity),
        image: new FormControl(data.item.image),
        type: new FormControl(data.item.itemType?.name),
        volume: new FormControl(data.item.volume),
        weight: new FormControl(data.item.weight),
        quantityInPack: new FormControl(data.item.quantityInPack),
        retailPrice: new FormControl(data.item.retailPrice),
        tradePrice: new FormControl(data.item.tradePrice),
        distributorPrice: new FormControl(data.distributorPrice),
        customDistributorPrice: new FormControl(data.customDistributorPrice),
        billPrice: new FormControl(data.distributorPrice * data.quantity),
        leftQuantity: new FormControl(data.leftQuantity),
      }));
    });

    this.updateTotals();
  }

  updateValidity() {
    if (this.data.toStatusId === this.statusEnum.AccountReviewed) {
      this.orderStatusChangeForm.get('transactionId')?.setValidators(Validators.required);
      this.orderStatusChangeForm.get('transactionId')?.updateValueAndValidity;
    }
    else {
      this.orderStatusChangeForm.get('transactionId')?.clearValidators;
      this.orderStatusChangeForm.get('transactionId')?.updateValueAndValidity;
    }
  }

  async getOrderById() {
    (await this.orderService.getOrderById(this.data.element.id)).subscribe({
      next: (data: any) => {

        this.gElement = data;
        this.LoadData();
        this.updateValidity();
      },
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  updateTotals() {
    this.totalQuantity = 0;
    this.billingAmount = 0;
    this.totalLeftQuantity = 0;

    this.orderItemsList.controls.forEach(control => {
      const quantity = control.get('quantity')?.value || 0;
      const shippedQuantity = control.get('shippedQuantity')?.value || 0;
      const distributorPrice = control.get('distributorPrice')?.value || 0;
      const leftQuantity = control.get('leftQuantity')?.value || 0;

      this.totalQuantity += quantity;
      this.totalShippedQuantity += shippedQuantity;
      this.billingAmount += shippedQuantity * distributorPrice;
      this.totalLeftQuantity += leftQuantity;
    });
  }


}