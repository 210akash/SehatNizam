import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { FormArray, FormBuilder, FormControl, FormGroup } from '@angular/forms';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { ConstantService, OrderStatusEnum } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { RetailOrderService } from '../retail-order.service';

@Component({
  selector: 'app-retail-order-status-change',
  templateUrl: './retail-order-status-change.component.html',
  styleUrls: ['./retail-order-status-change.component.css'], standalone: false
})

export class RetailOrderStatusChangeComponent implements OnInit {
  shopOrderStatusChangeForm!: FormGroup;
  isEditMode: boolean = false;
  isLoading = false;
  element: any;
  blob: any;

  currentPage = 0;
  pageSize = 0;
  totalRows = 0;
  pageSizeOptions: number[] = [5, 10, 25, 100];

  statusEnum: any;

  constructor(private notificationsService: NotificationsService, private constantService: ConstantService, private dialog: MatDialog,
    private retailOrderService: RetailOrderService, private formBuilder: FormBuilder,
    @Inject(MAT_DIALOG_DATA) public data: { element: any, toStatusId: any, statement: any, shippedOrderQuantityForm: any }) { }

  @ViewChild(MatPaginator) paginator!: MatPaginator;

  ngOnInit(): void {
    this.statusEnum = OrderStatusEnum;
    this.shopOrderStatusChangeForm = this.formBuilder.group({
      comments: [''],
      retailOrderItemsList: this.formBuilder.array([]),
    });

    this.loadProducts();
  }

  loadProducts() {
    if (this.data.element.retailOrderStatusId === OrderStatusEnum.Dispatched) {
      this.setEditProducts(this.data.element.retailOrderItems);
    }
  }

  async addshippedOrderQuantityForm(): Promise<boolean> {
    return new Promise(async (resolve, reject) => {
      (await this.retailOrderService.ConfirmRetailOrderQuantity(this.data.shippedOrderQuantityForm)).subscribe({
        next: (data) => {
          if (data.Status === 200) {
            resolve(true);  // Return true on success
          } else {
            resolve(false); // Return false if status is not 200
          }
        },
        error: (error) => {
          console.error(error);  // Handle error if needed
          resolve(false);        // Return false on error
        }
      });
    });
  }

  async updateShopOrderStatus() {
    let _updateShopOrderStatus = {
      retailOrderId: this.data.element.id,
      fromStatusId: this.data.element.retailOrderStatusId,
      toStatusId: this.data.toStatusId,
      comments: this.shopOrderStatusChangeForm.get('comments')?.value
    };

    if (this.data.toStatusId == OrderStatusEnum.Dispatched) {
      const result = await this.addshippedOrderQuantityForm();
      if (result == false) {
        this.isLoading = false;
        this.notificationsService.showNotification("There is an issue while saving the data", 'snack-bar-danger');
        return;
      }
    }

    (await this.retailOrderService.updateRetailOrderStatus(_updateShopOrderStatus)).subscribe({
      next: (data) => {
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
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  get retailOrderItemsList(): FormArray {
    return this.shopOrderStatusChangeForm.get('retailOrderItemsList') as FormArray;
  }

  async setEditProducts(data: any) {
    const productArray = this.shopOrderStatusChangeForm.get('retailOrderItemsList') as FormArray;
    data.forEach((data: { id: any; product: { id: any; name: any; attachments: any[]; type: any; volumeInMl: any; quantityInPack: any; retailPrice: any; }; quantity: any; shippedQuantity: null; tradePrice: any; customTradePrice: null; }) => {
      productArray.push(this.formBuilder.group({
        id: new FormControl(data.id),
        productId: new FormControl(data.product.id),
        name: new FormControl(data.product.name),
        quantity: new FormControl(data.quantity),
        shippedQuantity: new FormControl(data.shippedQuantity == null ? data.quantity : data.shippedQuantity),
        image: new FormControl(data.product.attachments?.filter((x: { isActive: boolean; }) => x.isActive == true)[0]?.imageName),
        type: new FormControl(data.product.type),
        volume: new FormControl(data.product.volumeInMl),
        quantityInPack: new FormControl(data.product.quantityInPack),
        retailPrice: new FormControl(data.product.retailPrice),
        tradePrice: new FormControl(data.tradePrice),
        customTradePrice: new FormControl(data.customTradePrice == null ? data.tradePrice : data.customTradePrice),
      }));
    });
  }


}