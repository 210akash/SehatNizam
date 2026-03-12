import { Component, OnInit, Inject } from '@angular/core';
import {
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import {
  ConstantService,
  OrderStatusEnum,
} from '../../../../Service/constant.service';
import { RetailOrderService } from '../retail-order.service';

@Component({
  selector: 'app-view-retail-order',
  templateUrl: './view-retail-order.component.html',
  styleUrls: ['./view-retail-order.component.css'],
  standalone: false,
})

export class ViewRetailOrderComponent implements OnInit {
  viewShopOrderForm!: FormGroup;
  isLoading = false;

  productsList: any[] = [];

  totalQuantity = 0;
  totalShippedQuantity = 0;
  billingAmount = 0;

  statusEnum: any;

  constructor(
    private formBuilder: FormBuilder,
    private constantService: ConstantService,
    private retailOrderService: RetailOrderService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  ngOnInit(): void {
    this.statusEnum = OrderStatusEnum;
    this.viewShopOrderForm = this.formBuilder.group({
      id: [0],
      retailOrderItemsList: this.formBuilder.array([]),
    });

    this.getShopOrderById();
  }

  LoadData(element: any) {
    this.constantService.LoadData(element, this.viewShopOrderForm);
    this.setEditProducts(element.retailOrderItems);
  }

  async setEditProducts(data: any) {
    const productArray = this.viewShopOrderForm.get(
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
          tradePrice: number;
        };
        quantity: number;
        shippedQuantity: null;
        tradePrice: any;
        customTradePrice: null;
      }) => {

        productArray.push(
          this.formBuilder.group({
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
            billPrice: new FormControl(data?.tradePrice * data.quantity),
          })
        );
      }
    );

    this.updateTotals();
  }

  get retailOrderItemsList(): FormArray {
    return this.viewShopOrderForm.get('retailOrderItemsList') as FormArray;
  }

  updateTotals() {
    this.totalQuantity = 0;
    this.billingAmount = 0;

    this.retailOrderItemsList.controls.forEach((control) => {
      const quantity = control.get('quantity')?.value || 0;
      const shippedQuantity = control.get('shippedQuantity')?.value || 0;
      const tradePrice = control.get('customTradePrice')?.value || 0;

      this.totalQuantity += quantity;
      this.totalShippedQuantity += shippedQuantity;
      this.billingAmount += shippedQuantity * tradePrice;
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


}