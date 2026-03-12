import { Component, Inject } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { ShopOrderReturnService } from '../shoporderreturn.service';
import { DepartmentService } from '../../department/department.service';
import { MatOptionSelectionChange } from '@angular/material/core';
import { PurchaseOrderService } from '../../purchaseorder/purchaseorder.service';
import { firstValueFrom } from 'rxjs';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';

@Component({
  selector: 'app-add-shoporderreturn',
  templateUrl: './add-shoporderreturn.component.html',
  styleUrl: './add-shoporderreturn.component.css',
  standalone: false
})

export class AddShopOrderReturnComponent {
  shopOrderReturnForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  orderList: any[] = [];
  itemList: any[] = [];
  isdataload: boolean = false;
  shoporderreturnTypeList: any;
  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private shopOrderReturnService: ShopOrderReturnService, private orderService: PurchaseOrderService, private departmentService: DepartmentService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.shopOrderReturnForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      createdDate: [new Date(), Validators.required],
      orderId: [0, Validators.required],
      status: [''],
      statusName: ['New'],
      statusId: [1],
      remarks: [''],
      shopOrderReturnDetail: this.formBuilder.array([])
    });

    this.LoadData(this.data.element);
  }

  get shopOrderReturnDetail(): FormArray {
    return this.shopOrderReturnForm.get('shopOrderReturnDetail') as FormArray;
  }

  addShopOrderReturnDetail(index: number) {
    const newDetailGroup = this.formBuilder.group({
      id: [0],
      shopOrderReturnId: [0],
      orderItemsId: [0, Validators.required],
      itemId: [0],
      orderQuantity: [0, Validators.required],
      quantity: [null, Validators.required],
    });

    this.shopOrderReturnDetail.insert(index + 1, newDetailGroup);
  }

  removeShopOrderReturnDetail(index: number) {
    if (this.shopOrderReturnDetail.length > 1) {
      this.shopOrderReturnDetail.removeAt(index);
    } else {
      this.notificationsService.showNotification(
        'At least one item is required.',
        'snack-bar-danger'
      );
    }
  }

  getIndexValue(index: number): any {
    const detailControl = (this.shopOrderReturnForm.get('shopOrderReturnDetail') as FormArray).at(index);
    return detailControl?.value.item || '';
  }

  async LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.shopOrderReturnForm);
      this.shopOrderReturnForm.get('dcCode')?.patchValue(element.order?.dcCode);
      this.getPendingOrderList(this.data.element.orderId, element.order?.dcCode);
      // Wait for getPurchaseOrdersDetails to complete
      await this.getOrderItems();  // This will now properly wait for the async operation

      const detailsArray = this.shopOrderReturnForm.get('shopOrderReturnDetail') as FormArray;
      detailsArray.clear();

      if (element.shopOrderReturnDetail && element.shopOrderReturnDetail.length > 0) {
        element.shopOrderReturnDetail.forEach((detail: any) => {
          const detailGroup = this.formBuilder.group({
            id: [detail.id],
            shopOrderReturnId: [detail.shopOrderReturnId],
            orderItemsId: [detail.orderItemsId, Validators.required],
            itemId: [detail.dispatchDetail?.orderItem?.itemId, Validators.required],
            orderQuantity: [detail.dispatchDetail?.quantity, Validators.required],
            quantity: [detail.quantity, Validators.required],
          });

          detailsArray.push(detailGroup);
        });
      }
    } else {
      this.getShopOrderReturnCode();
      this.shopOrderReturnForm.get('createdDate')?.patchValue(this.constantService.formatDate(new Date()));
      this.addShopOrderReturnDetail(0);
      // this.getPendingDC(0);
    }
  }

  checkInvalidControls(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(controlName => {
      const control = formGroup.get(controlName);
      if (control && control.invalid) {
        console.log(`Control '${controlName}' is invalid.`);
        console.log(control.errors);
      }
    });

    if (formGroup instanceof FormArray) {
      formGroup.controls.forEach((formControl, index) => {
        if (formControl.invalid) {
          console.log(`FormArray control at index ${index} is invalid.`);
          console.log(formControl.errors);
        }
      });
    }
  }

  SaveData() {
    if (this.shopOrderReturnForm.invalid) {
      this.constantService.markFormGroupTouched(this.shopOrderReturnForm);
      this.checkInvalidControls(this.shopOrderReturnForm);
      this.notificationsService.showNotification('Please Fill Required Fields', 'snack-bar-danger');
      return;
    }

    this.isLoading = true;
    let _shopOrderReturnFormForm: any = {};
    _shopOrderReturnFormForm = Object.assign(_shopOrderReturnFormForm, this.shopOrderReturnForm.value);

    this.shopOrderReturnService.saveShopOrderReturn(_shopOrderReturnFormForm).subscribe({
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

  getShopOrderReturnCode() {
    this.shopOrderReturnService.getShopOrderReturnCode().subscribe((data: any) => {
      this.shopOrderReturnForm.get('code')?.patchValue(data.code);
    });
  }

  isUpdating = false;

  onItemSelected(event: MatOptionSelectionChange, index: number): void {

    if (this.isUpdating) {
      return;
    }

    this.isUpdating = true;

    const selectedValue = event.source.value;

    if (!selectedValue) {
      console.error('Option value is undefined. Ensure mat-option [value] is correctly bound.');
      this.isUpdating = false;
      return;
    }

    const formArray = this.shopOrderReturnDetail as FormArray<FormGroup>;

    const duplicateItem = formArray.controls
      .filter((_, i) => i !== index) // exclude current index
      .some(group => group.get('orderItemsId')?.value === selectedValue);

    if (duplicateItem) {
      this.notificationsService.showNotification('This item has already been selected.', 'snack-bar-danger');
      const currentFormGroup = this.shopOrderReturnDetail.at(index) as FormGroup;
      currentFormGroup.get('orderItemsId')?.setValue(null);
      currentFormGroup.get('orderQuantity')?.patchValue(null);
      currentFormGroup.updateValueAndValidity();
      // currentFormGroup.get('received')?.patchValue('');
      this.isUpdating = false;
      return;
    }
    else {
      const selectedItem = this.getItemData(selectedValue);
      if (!selectedItem) {
        console.error('Selected item not found.');
        this.isUpdating = false;
        return;
      }

      const detailFormGroup = this.shopOrderReturnDetail.at(index) as FormGroup;
      detailFormGroup.get('orderItemsId')?.patchValue(selectedItem.id);
      detailFormGroup.get('orderQuantity')?.patchValue(selectedItem.quantity);
      detailFormGroup.get('quantity')?.setValidators([Validators.required, Validators.min(1), Validators.max(selectedItem.quantity)]);
      detailFormGroup.updateValueAndValidity();
      this.isUpdating = false;
    }
  }

  validateQty(index: number): any {
    const detailControl = (this.shopOrderReturnForm.get('shopOrderReturnDetail') as FormArray).at(index);
    if (detailControl?.value.quantity > detailControl?.value.orderQuantity) {
      detailControl.get('quantity')?.patchValue(detailControl?.value.orderQuantity);
    }
  }

  getItemData(itemId: string) {
    return this.itemList.find(x => x.id === itemId);
  }

  getDCData() {
    const orderId = this.shopOrderReturnForm.get('orderId')?.value;
    return this.orderList.find(x => x.id === orderId);
  }

  onInputCleared(event: Event, index: number): void {
    const inputValue = (event.target as HTMLInputElement)?.value;
    console.log('Current Input Value:', inputValue);

    if (!inputValue.trim()) {
      console.log(`Input cleared at row index: ${index}`);
      this.resetitem(index);
    }
  }

  resetitem(index: number) {
    const shopOrderReturnDetailArray = this.shopOrderReturnForm.get('shopOrderReturnDetail') as FormArray;
    if (!shopOrderReturnDetailArray || index < 0 || index >= shopOrderReturnDetailArray.length) {
      console.error('Invalid index or FormArray is not initialized properly.');
      return;
    }

    const currentFormGroup = shopOrderReturnDetailArray.at(index) as FormGroup;
    currentFormGroup.reset();
    return;
  }



  removeAllShopOrderReturnDetails() {
    if (this.shopOrderReturnDetail.length > 0) {
      this.shopOrderReturnDetail.clear();
      this.addShopOrderReturnDetail(0);
    } else {
      this.notificationsService.showNotification('No items to remove.', 'snack-bar-warning');
    }
  }

  reset() {
    this.shopOrderReturnForm.get('code')?.patchValue('');
  }

  async getOrderItems(): Promise<void> {
    const orderId = this.shopOrderReturnForm.get('orderId')?.value;
    const shopOrderReturnId = this.shopOrderReturnForm.get('id')?.value;
    try {
      const data = await (await this.shopOrderReturnService.getPendingShopOrderItems(orderId, shopOrderReturnId)).toPromise();
      this.itemList = data;

      const detailsArray = this.shopOrderReturnForm.get('shopOrderReturnDetail') as FormArray;
      detailsArray.clear();

      if (this.itemList && this.itemList.length > 0) {
        this.itemList.forEach((detail: any) => {
          const detailGroup = this.formBuilder.group({
            id: [0],
            shopOrderReturnId: [this.shopOrderReturnForm.get('id')?.value],
            orderItemsId: [detail.id, Validators.required],
            orderQuantity: [detail.quantity, Validators.required],
            quantity: [0, Validators.required],
          });

          detailsArray.push(detailGroup);
        });
      }
    } catch (error) {
      console.error('Error fetching pending purchase order items:', error);
    }
  }

  async getPendingOrderList(orderId: any, filter: any): Promise<any[]> {
    const data = await firstValueFrom(await this.shopOrderReturnService.getPendingShopOrder(orderId, filter));
    this.orderList = data;
    return data;
  }

  async getPendingOrder(event: any) {
    try {
      const filter = event.currentTarget.value;
      if (!filter || filter.length < 2) {
        this.orderList = []; // Optionally clear the list
        return; // Skip API call if filter is less than 2 characters
      }
      var orderId = this.shopOrderReturnForm.get('orderId')?.value;
      this.orderList = await this.getPendingOrderList(orderId, filter);
      //firstValueFrom(await this.shopOrderReturnService.getPendingDC(orderId, filter));
    } catch (error) {
      console.error('Error fetching demand list:', error);
    }
  }

  onOrderSelected(event: MatAutocompleteSelectedEvent): void {
    const selectedValue = event.option.value;
    if (selectedValue) {
      this.shopOrderReturnForm.get('orderId')?.patchValue(selectedValue.id);
      this.shopOrderReturnForm.get('dcCode')?.patchValue(selectedValue.dcCode);
      this.getOrderItems();
    } else {
      this.shopOrderReturnForm.get('orderId')?.patchValue(0);
      this.itemList = [];
    }
  }

  onOrderInputCleared(event: Event): void {
    const inputValue = (event.target as HTMLInputElement)?.value;
    if (!inputValue.trim()) {
      this.shopOrderReturnForm.get('orderId')?.patchValue(0);
      this.shopOrderReturnForm.get('dcCode')?.patchValue('');
      this.removeAllShopOrderReturnDetails();
      this.itemList = [];
    }
  }
}
