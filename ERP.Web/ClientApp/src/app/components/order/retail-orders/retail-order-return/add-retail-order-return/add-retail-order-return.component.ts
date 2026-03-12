import { Component, Inject } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { MatOptionSelectionChange } from '@angular/material/core';
import { firstValueFrom } from 'rxjs';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { ConstantService } from '../../../../../Service/constant.service';
import { NotificationsService } from '../../../../../Service/notification.service';
import { DepartmentService } from '../../../../department/department.service';
import { PurchaseOrderService } from '../../../../purchaseorder/purchaseorder.service';
import { RetailOrderReturnService } from '../retail-order-return.service';

@Component({
  selector: 'app-add-retail-order-return',
  templateUrl: './add-retail-order-return.component.html',
  styleUrl: './add-retail-order-return.component.css',
  standalone: false
})

export class AddRetailOrderReturnComponent {
  retailOrderReturnForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  orderList: any[] = [];
  itemList: any[] = [];
  isdataload: boolean = false;
  retailorderreturnTypeList: any;
  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private retailOrderReturnService: RetailOrderReturnService, private orderService: PurchaseOrderService, private departmentService: DepartmentService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.retailOrderReturnForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      createdDate: [new Date(), Validators.required],
      retailOrderId: [0, Validators.required],
      status: [''],
      statusName: ['New'],
      statusId: [1],
      remarks: [''],
      retailOrderReturnDetail: this.formBuilder.array([])
    });

    this.LoadData(this.data.element);
  }

  get retailOrderReturnDetail(): FormArray {
    return this.retailOrderReturnForm.get('retailOrderReturnDetail') as FormArray;
  }

  addRetailOrderReturnDetail(index: number) {
    const newDetailGroup = this.formBuilder.group({
      id: [0],
      retailOrderReturnId: [0],
      retailOrderItemsId: [0, Validators.required],
      itemId: [0],
      orderQuantity: [0, Validators.required],
      quantity: [null, Validators.required],
    });

    this.retailOrderReturnDetail.insert(index + 1, newDetailGroup);
  }

  removeRetailOrderReturnDetail(index: number) {
    if (this.retailOrderReturnDetail.length > 1) {
      this.retailOrderReturnDetail.removeAt(index);
    } else {
      this.notificationsService.showNotification(
        'At least one item is required.',
        'snack-bar-danger'
      );
    }
  }

  getIndexValue(index: number): any {
    const detailControl = (this.retailOrderReturnForm.get('retailOrderReturnDetail') as FormArray).at(index);
    return detailControl?.value.item || '';
  }

  async LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.retailOrderReturnForm);
      // this.retailOrderReturnForm.get('dcCode')?.patchValue(element.order?.dcCode);
      this.getPendingOrderList(this.data.element.retailOrderId, "");
      // Wait for getPurchaseOrdersDetails to complete
      await this.getRetailOrderItems();  // This will now properly wait for the async operation

      const detailsArray = this.retailOrderReturnForm.get('retailOrderReturnDetail') as FormArray;
      detailsArray.clear();

      if (element.retailOrderReturnDetail && element.retailOrderReturnDetail.length > 0) {
        element.retailOrderReturnDetail.forEach((detail: any) => {
          const detailGroup = this.formBuilder.group({
            id: [detail.id],
            retailOrderReturnId: [detail.retailOrderReturnId],
            retailOrderItemsId: [detail.retailOrderItemsId, Validators.required],
            itemId: [detail.retailOrderItems?.itemId, Validators.required],
            orderQuantity: [detail.dispatchDetail?.quantity, Validators.required],
            quantity: [detail.quantity, Validators.required],
          });

          detailsArray.push(detailGroup);
        });
      }
    } else {
      this.getRetailOrderReturnCode();
      this.retailOrderReturnForm.get('createdDate')?.patchValue(this.constantService.formatDate(new Date()));
      this.addRetailOrderReturnDetail(0);
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
    console.log(this.retailOrderReturnForm);
    if (this.retailOrderReturnForm.invalid) {
      this.constantService.markFormGroupTouched(this.retailOrderReturnForm);
      this.checkInvalidControls(this.retailOrderReturnForm);
      this.notificationsService.showNotification('Please Fill Required Fields', 'snack-bar-danger');
      return;
    }

    this.isLoading = true;
    let _retailOrderReturnFormForm: any = {};
    _retailOrderReturnFormForm = Object.assign(_retailOrderReturnFormForm, this.retailOrderReturnForm.value);

    this.retailOrderReturnService.saveRetailOrderReturn(_retailOrderReturnFormForm).subscribe({
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

  getRetailOrderReturnCode() {
    this.retailOrderReturnService.getRetailOrderReturnCode().subscribe((data: any) => {
      this.retailOrderReturnForm.get('code')?.patchValue(data.code);
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

    const formArray = this.retailOrderReturnDetail as FormArray<FormGroup>;

    const duplicateItem = formArray.controls
      .filter((_, i) => i !== index) // exclude current index
      .some(group => group.get('retailOrderItemsId')?.value === selectedValue);

    if (duplicateItem) {
      this.notificationsService.showNotification('This item has already been selected.', 'snack-bar-danger');
      const currentFormGroup = this.retailOrderReturnDetail.at(index) as FormGroup;
      currentFormGroup.get('retailOrderItemsId')?.setValue(null);
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

      const detailFormGroup = this.retailOrderReturnDetail.at(index) as FormGroup;
      detailFormGroup.get('retailOrderItemsId')?.patchValue(selectedItem.id);
      detailFormGroup.get('orderQuantity')?.patchValue(selectedItem.quantity);
      detailFormGroup.get('quantity')?.setValidators([Validators.required, Validators.min(1), Validators.max(selectedItem.quantity)]);
      detailFormGroup.updateValueAndValidity();
      this.isUpdating = false;
    }
  }

  validateQty(index: number): any {
    const detailControl = (this.retailOrderReturnForm.get('retailOrderReturnDetail') as FormArray).at(index);
    if (detailControl?.value.quantity > detailControl?.value.orderQuantity) {
      detailControl.get('quantity')?.patchValue(detailControl?.value.orderQuantity);
    }
  }

  getItemData(itemId: string) {
    return this.itemList.find(x => x.id === itemId);
  }

  getDCData() {
    const retailOrderId = this.retailOrderReturnForm.get('retailOrderId')?.value;
    return this.orderList.find(x => x.id === retailOrderId);
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
    const retailOrderReturnDetailArray = this.retailOrderReturnForm.get('retailOrderReturnDetail') as FormArray;
    if (!retailOrderReturnDetailArray || index < 0 || index >= retailOrderReturnDetailArray.length) {
      console.error('Invalid index or FormArray is not initialized properly.');
      return;
    }

    const currentFormGroup = retailOrderReturnDetailArray.at(index) as FormGroup;
    currentFormGroup.reset();
    return;
  }



  removeAllRetailOrderReturnDetails() {
    if (this.retailOrderReturnDetail.length > 0) {
      this.retailOrderReturnDetail.clear();
      this.addRetailOrderReturnDetail(0);
    } else {
      this.notificationsService.showNotification('No items to remove.', 'snack-bar-warning');
    }
  }

  reset() {
    this.retailOrderReturnForm.get('code')?.patchValue('');
  }

  async getRetailOrderItems(): Promise<void> {
    const retailOrderId = this.retailOrderReturnForm.get('retailOrderId')?.value;
    const retailOrderReturnId = this.retailOrderReturnForm.get('id')?.value;
    try {
      const data = await (await this.retailOrderReturnService.getPendingRetailOrderItems(retailOrderId, retailOrderReturnId)).toPromise();
      this.itemList = data;

      const detailsArray = this.retailOrderReturnForm.get('retailOrderReturnDetail') as FormArray;
      detailsArray.clear();

      if (this.itemList && this.itemList.length > 0) {
        this.itemList.forEach((detail: any) => {
          const detailGroup = this.formBuilder.group({
            id: [0],
            retailOrderReturnId: [this.retailOrderReturnForm.get('id')?.value],
            retailOrderItemsId: [detail.id, Validators.required],
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

  async getPendingOrderList(retailOrderId: any, filter: any): Promise<any[]> {
    const data = await firstValueFrom(await this.retailOrderReturnService.getPendingRetailOrder(retailOrderId, filter));
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
      var retailOrderId = this.retailOrderReturnForm.get('retailOrderId')?.value;
      this.orderList = await this.getPendingOrderList(retailOrderId, filter);
      //firstValueFrom(await this.retailOrderReturnService.getPendingDC(retailOrderId, filter));
    } catch (error) {
      console.error('Error fetching demand list:', error);
    }
  }

  onOrderSelected(event: MatAutocompleteSelectedEvent): void {
    const selectedValue = event.option.value;
    if (selectedValue) {
      this.retailOrderReturnForm.get('retailOrderId')?.patchValue(selectedValue.id);
      this.retailOrderReturnForm.get('dcCode')?.patchValue(selectedValue.dcCode);
      this.getRetailOrderItems();
    } else {
      this.retailOrderReturnForm.get('retailOrderId')?.patchValue(0);
      this.itemList = [];
    }
  }

  onOrderInputCleared(event: Event): void {
    const inputValue = (event.target as HTMLInputElement)?.value;
    if (!inputValue.trim()) {
      this.retailOrderReturnForm.get('retailOrderId')?.patchValue(0);
      this.retailOrderReturnForm.get('dcCode')?.patchValue('');
      this.removeAllRetailOrderReturnDetails();
      this.itemList = [];
    }
  }
}
