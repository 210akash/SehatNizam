import { Component, OnInit, Inject } from '@angular/core';
import {
  AbstractControl,
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { createMask } from '@ngneat/input-mask';
import {
  ConstantService,
  OrderStatusEnum,
} from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { VehicleService } from '../../vehicle/vehicle.service';
import { RetailOrderService } from '../retail-order.service';
import { RetailOrderStatusChangeComponent } from '../retail-order-status-change/retail-order-status-change.component';

@Component({
  selector: 'app-confirm-retail-order-quantity',
  templateUrl: './confirm-retail-order-quantity.component.html',
  styleUrls: ['./confirm-retail-order-quantity.component.css'],
  standalone: false,
})

export class ConfirmRetailOrderQuantityComponent implements OnInit {
  shippedShopOrderQuantityForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;

  dealershipList: any[] = [];
  productsList: any[] = [];

  totalQuantity = 0;
  billingAmount = 0;

  phoneNoInputMask = createMask('0399-9999999');

  vehicleList: any[] = [];

  statusEnum: any;

  constructor(
    private notificationsService: NotificationsService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private constantService: ConstantService,
    private retailOrderService: RetailOrderService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  ngOnInit(): void {
    this.statusEnum = OrderStatusEnum;
    this.shippedShopOrderQuantityForm = this.formBuilder.group({
      id: [0],
      retailOrderItemsList: this.formBuilder.array([]),

      tradePrice: [0],
      customTradePrice: [0],
      totalAmount: [''],
    });

    this.getShopOrderById();
  }

  get f() {
    return this.shippedShopOrderQuantityForm.controls;
  }

  async saveShopOrder() {
    this.isLoading = true;

    if (this.shippedShopOrderQuantityForm.invalid) {
      const retailOrderItemsList = this.shippedShopOrderQuantityForm.get(
        'retailOrderItemsList'
      ) as FormArray;
      if (retailOrderItemsList.invalid) {
        this.notificationsService.showNotification(
          'New Quantity can not be greater!',
          'snack-bar-danger'
        );
      }
      this.constantService.markFormGroupTouched(
        this.shippedShopOrderQuantityForm
      );
      return;
    }

    if (!this.checkIfAtLeastOneNonZero()) {
      this.notificationsService.showNotification(
        'Please Add at least 1 Quantity!',
        'snack-bar-success'
      );
      return;
    }

    let _createShopOrderForm: any = {};
    _createShopOrderForm = Object.assign(
      _createShopOrderForm,
      this.shippedShopOrderQuantityForm.value
    );

    (
      await this.retailOrderService.ConfirmRetailOrderQuantity(_createShopOrderForm)
    ).subscribe({
      next: (data) => {
        if (data.Status == 200) {
          this.notificationsService.showNotification(
            'ShopOrder Saved Successfully!',
            'snack-bar-success'
          );
          this.dialog.closeAll();
          this.isLoading = false;
        } else if (data.Status == 409) {
          this.notificationsService.showNotification(
            'Name already exist!',
            'snack-bar-danger'
          );
          this.isLoading = false;
        }
      },
      error: (error) => {
        this.notificationsService.showNotification(
          'Please Fill the required fields!',
          'snack-bar-danger'
        );
        console.log(error);
        this.isLoading = false;
      },
    });
  }

  async setEditProducts(data: any) {
    const productArray = this.shippedShopOrderQuantityForm.get(
      'retailOrderItemsList'
    ) as FormArray;
    data.forEach(
      (data: {
        id: any;
        item: {
          id: any;
          name: any;
          image: any;
          itemType: any;
          volume: any;
          quantityInPack: any;
          retailPrice: any;
        };
        quantity: any;
        shippedQuantity: null;
        tradePrice: any;
        customTradePrice: null;
      }) => {
        productArray.push(
          this.formBuilder.group(
            {
              id: new FormControl(data.id),
              itemId: new FormControl(data.item?.id),
              name: new FormControl(data.item?.name),
              quantity: new FormControl(data.quantity),
              shippedQuantity: new FormControl(
                data.shippedQuantity == null
                  ? data.quantity
                  : data.shippedQuantity
              ),
              image: new FormControl(data.item?.image),
              type: new FormControl(data.item?.itemType?.name),
              volume: new FormControl(data.item?.volume),
              quantityInPack: new FormControl(data.item?.quantityInPack),
              retailPrice: new FormControl(data.item?.retailPrice),
              tradePrice: new FormControl(data.tradePrice),
              customTradePrice: new FormControl(
                data.customTradePrice == null
                  ? data.tradePrice
                  : data.customTradePrice
              ),
            },
            { validators: this.quantityValidator() }
          )
        );
      }
    );
    this.updateTotals();
  }

  get retailOrderItemsList(): FormArray {
    return this.shippedShopOrderQuantityForm.get(
      'retailOrderItemsList'
    ) as FormArray;
  }

  checkIfAtLeastOneNonZero(): boolean {
    return this.retailOrderItemsList.controls.some((formGroup) => {
      const quantityControl = (formGroup as FormGroup).get('quantity');
      return quantityControl && quantityControl.value > 0;
    });
  }

  quantityValidator(): ValidatorFn {
    return (control: AbstractControl): { [key: string]: any } | null => {
      const formGroup = control as FormGroup;
      const quantity = formGroup.get('quantity')?.value;
      const shippedQuantity = formGroup.get('shippedQuantity')?.value;

      return shippedQuantity > quantity ? { quantityExceeds: true } : null;
    };
  }

  async openStatusChangeDialog(toStatusId: any) {
    this.isLoading = true;

    if (this.shippedShopOrderQuantityForm.invalid) {
      const retailOrderItemsList = this.shippedShopOrderQuantityForm.get(
        'retailOrderItemsList'
      ) as FormArray;
      if (retailOrderItemsList.invalid) {
        this.notificationsService.showNotification(
          'New Quantity can not be greater!',
          'snack-bar-danger'
        );
      }
      this.constantService.markFormGroupTouched(
        this.shippedShopOrderQuantityForm
      );
      return;
    }

    if (!this.checkIfAtLeastOneNonZero()) {
      this.notificationsService.showNotification(
        'Please Add at least 1 Quantity!',
        'snack-bar-success'
      );
      return;
    }

    let _createShopOrderForm: any = {};
    _createShopOrderForm = Object.assign(
      _createShopOrderForm,
      this.shippedShopOrderQuantityForm.value
    );

    const dialogRef = this.dialog.open(RetailOrderStatusChangeComponent, {
      data: {
        element: this.data.element,
        toStatusId: toStatusId,
        statement: 'Are you sure you want to dispatch the order?',
        shippedOrderQuantityForm: _createShopOrderForm,
      },
      width: '30%',
      maxHeight: '95vh',
      autoFocus: true,
      disableClose: true,
    });

    dialogRef.afterClosed().subscribe((result) => {
      console.log(`Dialog result: ${result}`);
    });
  }

  onShippedQuantityChange(index: number): void {
    const productControl = this.retailOrderItemsList.at(index);
    const shippedQuantity = productControl.get('shippedQuantity')?.value;
    const price = productControl.get('tradePrice')?.value;

    console.log(`Product Index: ${index}`);
    console.log(`Shipped Quantity: ${shippedQuantity}`);
    console.log(`Price: ${price}`);

    if (shippedQuantity !== null && price !== null) {
      this.calculateTotalSum();
    }
  }

  calculateTotalSum(): void {
    let totalSum = this.retailOrderItemsList.controls.reduce((sum, control) => {
      const shippedQuantity = control.get('shippedQuantity')?.value || 0;
      const price = control.get('tradePrice')?.value || 0;
      return sum + shippedQuantity * price;
    }, 0);
    this.shippedShopOrderQuantityForm.get('totalAmount')?.patchValue(totalSum);
  }

  updateTotals() {
    this.totalQuantity = 0;
    this.billingAmount = 0;

    this.retailOrderItemsList.controls.forEach((control) => {
      const quantity = control.get('shippedQuantity')?.value || 0;
      const tradePrice = control.get('customTradePrice')?.value || 0;

      this.totalQuantity += quantity;
      this.billingAmount += quantity * tradePrice;
    });
  }

  async getShopOrderById() {
    (await this.retailOrderService.getRetailOrderById(this.data.element.id)).subscribe({
      next: (data) => {
        this.setEditProducts(data.retailOrderItems);
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      },
    });
  }


}