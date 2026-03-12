import { Component, OnInit, Inject } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { PrimaryOrderService } from '../order.service';
import { ItemService } from '../../../item/item.service';
import { ConstantService, OrderStatusEnum } from '../../../../Service/constant.service';

@Component({
  selector: 'app-view-order',
  templateUrl: './view-order.component.html',
  styleUrls: ['./view-order.component.css'], standalone: false
})

export class ViewOrderComponent implements OnInit {
  dialogRef: any;
  viewOrderForm!: FormGroup;
  isLoading = false;

  productsList: any[] = [];
  documents: any[] = [];

  totalQuantity = 0;
  totalShippedQuantity = 0;
  billingAmount = 0;
  totalLeftQuantity = 0;
  urlSafe: SafeResourceUrl | undefined;

  statusEnum: any;

  constructor(private sanitizer: DomSanitizer, private itemService: ItemService, private dialog: MatDialog, private formBuilder: FormBuilder, private constantService: ConstantService,
    private orderService: PrimaryOrderService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.statusEnum = OrderStatusEnum;
    this.viewOrderForm = this.formBuilder.group({
      id: [0],
      dealershipId: ['', Validators.required],
      territoryId: ['', Validators.required],
      dealershipAddress: ['', Validators.required],
      orderItemsList: this.formBuilder.array([]),

      deliveryDateTime: [null, Validators.required],
      driverName: ['', Validators.required],
      driverPhoneNo: ['', Validators.required],
      vehicleNo: ['', Validators.required],
      deliveryChallanCode: ['']
    });

    this.getOrderById();
  }

  LoadData(element: any) {
    this.constantService.LoadData(element, this.viewOrderForm);
    this.viewOrderForm.get('dealershipId')?.patchValue(element.dealership?.name);
    this.viewOrderForm.get('territoryId')?.patchValue(element.dealership?.territory?.name);
    this.setEditProducts(element.orderItems);

    this.viewOrderForm.get('deliveryDateTime')?.patchValue(element.dispatchOrderDetails[0]?.deliveryDateTime);
    this.viewOrderForm.get('driverName')?.patchValue(element.dispatchOrderDetails[0]?.vehicle?.driverName);
    this.viewOrderForm.get('driverPhoneNo')?.patchValue(element.dispatchOrderDetails[0]?.vehicle?.driverPhoneNo);
    this.viewOrderForm.get('vehicleNo')?.patchValue(element.dispatchOrderDetails[0]?.vehicle?.registrationNumber);
    this.viewOrderForm.get('deliveryChallanCode')?.patchValue(element.dispatchOrderDetails[0]?.deliveryChallanCode);
    this.documents = element?.orderAttachments.filter((x: { isActive: boolean; }) => x.isActive == true);
  }

  async setEditProducts(data: any) {
    const productArray = this.viewOrderForm.get('orderItemsList') as FormArray;
    data.forEach((data: { id: any; item: { id: any; name: any; image: any; itemType: any; volume: any; quantityInPack: any; retailPrice: any; tradePrice: any; }; quantity: number; shippedQuantity: null; distributorPrice: number; customDistributorPrice: any; leftQuantity : number;}) => {
      productArray.push(this.formBuilder.group({
        id: new FormControl(data.id),
        itemId: new FormControl(data.item.id),
        name: new FormControl(data.item?.name),
        quantity: new FormControl(data?.quantity),
        shippedQuantity: new FormControl(data.shippedQuantity == null ? data.quantity : data.shippedQuantity),
        image: new FormControl(data.item?.image),
        type: new FormControl(data.item?.itemType?.name),
        volume: new FormControl(data.item.volume),
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

  get orderItemsList(): FormArray {
    return this.viewOrderForm.get('orderItemsList') as FormArray;
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

  GetDocument(event: any, index: number, template: any) {
    this.urlSafe = this.sanitizer.bypassSecurityTrustResourceUrl(this.documents[index].fileSource + '#toolbar=0');
    this.dialogRef = this.dialog.open(template, {
      width: '50%',
      height: '70%',
      disableClose: true,
    });
  }

  async getOrderById() {
    (await this.orderService.getOrderById(this.data.element.id)).subscribe({
      next: (data: any) => {
    
        this.LoadData(data);
      },
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }


}
