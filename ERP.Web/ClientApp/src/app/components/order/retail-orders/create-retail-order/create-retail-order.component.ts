import { Component, OnInit, Inject } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { createMask } from '@ngneat/input-mask';
import { AuthenticationService } from '../../../../Auth/authentication.service';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { RetailOrderService } from '../retail-order.service';
import { ShopService } from '../../shop/shop.service';

@Component({
  selector: 'app-create-retail-order',
  templateUrl: './create-retail-order.component.html',
  styleUrls: ['./create-retail-order.component.css'],
  standalone: false,
})

export class CreateRetailOrderComponent implements OnInit {
  createShopOrderForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;

  zoneList: any[] = [];
  territoryList: any[] = [];
  dsfList: any[] = [];
  routeList: any[] = [];
  shopList: any[] = [];
  productsList: any[] = [];
  currentuser: any;
  phoneNoInputMask = createMask('0399-9999999');

  totalQuantity = 0;
  billingAmount = 0;

  constructor(
    private auth: AuthenticationService,
    private notificationsService: NotificationsService,
    private dialog: MatDialog,
    private formBuilder: FormBuilder,
    private constantService: ConstantService,
    private retailOrderService: RetailOrderService,
    private shopService: ShopService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  ngOnInit(): void {
    this.currentuser = this.auth.currentUserValue;
    this.getShopById();
    this.createShopOrderForm = this.formBuilder.group({
      id: [0],
      shopId: [0, Validators.required],
      name: [''],
      address: [''],

      reference: ['', Validators.required],
      department: ['', Validators.required],
      comments: ['', Validators.required],

      retailOrderItemsList: this.formBuilder.array([]),
    });

    if (this.data.element != null) {
      this.getShopOrderById();
    } else {
      this.getAllProducts();
    }
  }

  get f() {
    return this.createShopOrderForm.controls;
  }

  async saveShopOrder() {
    this.isLoading = true;

    if (this.createShopOrderForm.invalid) {
      this.constantService.markFormGroupTouched(this.createShopOrderForm);
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
      this.createShopOrderForm.value
    );

    (await this.retailOrderService.saveRetailOrder(_createShopOrderForm)).subscribe(
      {
        next: (data) => {
          if (data.Status == 200) {
            this.notificationsService.showNotification(
              'Shop Order Saved Successfully!',
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
      }
    );
  }

  async LoadData(element: any) {
    if (this.data.element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.createShopOrderForm);
      this.setEditProducts(element.retailOrderItems);
    }
  }

  async getAllProducts() {
    // Cast to FormArray before calling clear()
    (this.createShopOrderForm.get('retailOrderItemsList') as FormArray).clear();
    (
      await this.retailOrderService.getKCItemsByDistributor()).subscribe({
        next: (data) => {
          this.setAddProducts(data);
        },
        error: (error) => {
          console.log(error);
          this.isLoading = false;
        },
      });
  }

  async setAddProducts(data: any) {
    const productArray = this.createShopOrderForm.get(
      'retailOrderItemsList'
    ) as FormArray;
    data
      .filter((x: { isActive: boolean }) => x.isActive == true)
      .forEach(
        (product: {
          id: any;
          name: any;
          quantity: null;
          image: any;
          type: any;
          volume: any;
          quantityInPack: any;
          retailPrice: any;
          tradePrice: any;
          distributorPrice: any;
          holdQuantity: any;
          leftQuantity: any;
        }) => {
          productArray.push(
            this.formBuilder.group({
              itemId: new FormControl(product.id),
              name: new FormControl(product.name),
              quantity: new FormControl(
                product.quantity == null ? 0 : product.quantity
              ),
              image: new FormControl(product.image),
              type: new FormControl(product.type),
              volume: new FormControl(product.volume),
              quantityInPack: new FormControl(product.quantityInPack),
              retailPrice: new FormControl(product.retailPrice),
              tradePrice: new FormControl(product.tradePrice),
              distributorPrice: new FormControl(product.distributorPrice),
              holdQuantity: new FormControl(product.holdQuantity),
              leftQuantity: new FormControl(product.leftQuantity),
              // billPrice: new FormControl(product.quantity == null ? 0 : product.quantity * product.distributorPrice)
            })
          );
        }
      );
  }

  async setEditProducts(data: any) {
    const productArray = this.createShopOrderForm.get('retailOrderItemsList') as FormArray;
    data.forEach(
      (data: {
        retailPrice: any;
        tradePrice: any;
        distributorPrice: any;
        leftQuantity: any;
        id: any;
        item: {
          id: any;
          name: any;
          image: any;
          itemType: any;
          volume: any;
          quantityInPack: any;
          retailPrice: any;
          tradePrice: any;
          distributorPrice: any;
          holdQuantity: any;
          leftQuantity: any;
        };
        quantity: any;
      }) => {
        productArray.push(
          this.formBuilder.group({
            id: new FormControl(data.id),
            itemId: new FormControl(data.item?.id),
            name: new FormControl(data.item?.name),
            quantity: new FormControl(data.quantity),
            image: new FormControl(data.item?.image),
            type: new FormControl(data.item?.itemType?.name),
            volume: new FormControl(data.item?.volume),
            quantityInPack: new FormControl(data.item?.quantityInPack),
            retailPrice: new FormControl(data.retailPrice),
            tradePrice: new FormControl(data.tradePrice),
            distributorPrice: new FormControl(data.distributorPrice),
            leftQuantity: new FormControl(data.leftQuantity),
            // billPrice: new FormControl(data.item?.tradePrice * data.quantity)
          })
        );
      }
    );

    this.updateTotals();
  }

  get retailOrderItemsList(): FormArray {
    return this.createShopOrderForm.get('retailOrderItemsList') as FormArray;
  }

  checkIfAtLeastOneNonZero(): boolean {
    return this.retailOrderItemsList.controls.some((formGroup) => {
      const quantityControl = (formGroup as FormGroup).get('quantity');
      return quantityControl && quantityControl.value > 0;
    });
  }

  validateQuantity(event: any, productControl: any): void {
    // if (this.isEditMode === false) {
    const maxQuantity = productControl.get('leftQuantity').value;
    const currentValue = event.target.value;

    // If the entered value exceeds the max allowed quantity, reset it to max
    if (currentValue > maxQuantity) {
      event.target.value = maxQuantity;
      productControl.get('quantity').setValue(maxQuantity); // Update form control value
    }
    // }

    this.updateTotals();
  }

  updateTotals() {
    this.totalQuantity = 0;
    this.billingAmount = 0;

    this.retailOrderItemsList.controls.forEach((control) => {
      const quantity = control.get('quantity')?.value || 0;
      const tradePrice = control.get('tradePrice')?.value || 0;

      this.totalQuantity += quantity;
      this.billingAmount += quantity * tradePrice;
    });
  }

  async getShopOrderById() {
    (await this.retailOrderService.getRetailOrderById(this.data.element.id)).subscribe({
      next: (data: any) => {
        this.LoadData(data);
      },
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      },
    });
  }

  async getShopById() {
    (await this.shopService.getShopById(this.currentuser?.retailUserShopId)).subscribe({
      next: (data: any) => {
        this.createShopOrderForm.get('shopId')?.patchValue(data.id);
        this.createShopOrderForm.get('name')?.patchValue(data.name);
        this.createShopOrderForm.get('address')?.patchValue(data.address);
      },
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      },
    });
  }


}