import { Component, Inject } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { SaleReturnService } from '../salereturn.service';
import { DepartmentService } from '../../department/department.service';
import { MatOptionSelectionChange } from '@angular/material/core';
import { PurchaseOrderService } from '../../purchaseorder/purchaseorder.service';
import { firstValueFrom } from 'rxjs';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { ProjectService } from '../../project/project.service';

@Component({
  selector: 'app-add-salereturn',
  templateUrl: './add-salereturn.component.html',
  styleUrl: './add-salereturn.component.css',
  standalone: false
})

export class AddSaleReturnComponent {
  saleReturnForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  dispatchOrderList: any[] = [];
  itemList: any[] = [];
  isdataload: boolean = false;
  salereturnTypeList: any;
  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder,
     private projectService: ProjectService,
    private saleReturnService: SaleReturnService, private dispatchOrderService: PurchaseOrderService, private departmentService: DepartmentService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.saleReturnForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      createdDate: [new Date(), Validators.required],
      dispatchOrderId: [0, Validators.required],
      dcCode: ['', Validators.required],
      status: [''],
      statusName: ['New'],
      statusId: [1],
      remarks: [''],
      saleReturnDetail: this.formBuilder.array([])
    });

    this.LoadData(this.data.element);
  }

  get saleReturnDetail(): FormArray {
    return this.saleReturnForm.get('saleReturnDetail') as FormArray;
  }

  addSaleReturnDetail(index: number) {
    const newDetailGroup = this.formBuilder.group({
      id: [0],
      saleReturnId: [0],
      dispatchDetailId: [0, Validators.required],
      itemId: [0],
      dispatchedQuantity: [0, Validators.required],
      quantity: [null, Validators.required],
    });

    this.saleReturnDetail.insert(index + 1, newDetailGroup);
  }

  removeSaleReturnDetail(index: number) {
    if (this.saleReturnDetail.length > 1) {
      this.saleReturnDetail.removeAt(index);
    } else {
      this.notificationsService.showNotification(
        'At least one item is required.',
        'snack-bar-danger'
      );
    }
  }

  getIndexValue(index: number): any {
    const detailControl = (this.saleReturnForm.get('saleReturnDetail') as FormArray).at(index);
    return detailControl?.value.item || '';
  }

  async LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.saleReturnForm);
      this.saleReturnForm.get('dcCode')?.patchValue(element.dispatchOrder?.dcCode);
      this.getPendingOrderList(this.data.element.dispatchOrderId, element.dispatchOrder?.dcCode);
      // Wait for getPurchaseOrdersDetails to complete
      await this.getDispatchedDetails();  // This will now properly wait for the async operation

      const detailsArray = this.saleReturnForm.get('saleReturnDetail') as FormArray;
      detailsArray.clear();

      if (element.saleReturnDetail && element.saleReturnDetail.length > 0) {
        element.saleReturnDetail.forEach((detail: any) => {
          const detailGroup = this.formBuilder.group({
            id: [detail.id],
            saleReturnId: [detail.saleReturnId],
            dispatchDetailId: [detail.dispatchDetailId, Validators.required],
            itemId: [detail.dispatchDetail?.orderItem?.itemId, Validators.required],
            dispatchedQuantity: [detail.dispatchDetail?.quantity, Validators.required],
            quantity: [detail.quantity, Validators.required],
          });

          detailsArray.push(detailGroup);
        });
      }
    } else {
      this.getSaleReturnCode();
      this.saleReturnForm.get('createdDate')?.patchValue(this.constantService.formatDate(new Date()));
      this.addSaleReturnDetail(0);
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
    if (this.saleReturnForm.invalid) {
      this.constantService.markFormGroupTouched(this.saleReturnForm);
      this.checkInvalidControls(this.saleReturnForm);
      this.notificationsService.showNotification('Please Fill Required Fields', 'snack-bar-danger');
      return;
    }

    this.isLoading = true;
    let _saleReturnFormForm: any = {};
    _saleReturnFormForm = Object.assign(_saleReturnFormForm, this.saleReturnForm.value);

    this.saleReturnService.saveSaleReturn(_saleReturnFormForm).subscribe({
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

  getSaleReturnCode() {
    this.saleReturnService.getSaleReturnCode().subscribe((data: any) => {
      this.saleReturnForm.get('code')?.patchValue(data.code);
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

    const duplicateItem = this.saleReturnDetail.controls
      .filter((control: AbstractControl, controlIndex: number) => controlIndex !== index)
      .some((control: AbstractControl) => {
        const formGroup = control as FormGroup;
        return formGroup.get('itemId')?.value === selectedValue;
      });

    if (duplicateItem) {
      this.notificationsService.showNotification('This item has already been selected.', 'snack-bar-danger');
      const currentFormGroup = this.saleReturnDetail.at(index) as FormGroup;
      currentFormGroup.get('itemId')?.setValue("0");
      currentFormGroup.get('dispatchedQuantity')?.patchValue(null);
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

      const detailFormGroup = this.saleReturnDetail.at(index) as FormGroup;
      detailFormGroup.get('itemId')?.patchValue(selectedItem?.orderItem?.item.id);
      detailFormGroup.get('dispatchDetailId')?.patchValue(selectedItem.id);
      detailFormGroup.get('dispatchedQuantity')?.patchValue(selectedItem.quantity);
      detailFormGroup.get('quantity')?.setValidators([Validators.required, Validators.min(1), Validators.max(selectedItem.quantity)]);
      detailFormGroup.updateValueAndValidity();
      this.isUpdating = false;
    }
  }

  validateQty(index: number): any {
    const detailControl = (this.saleReturnForm.get('saleReturnDetail') as FormArray).at(index);
    if (detailControl?.value.quantity > detailControl?.value.dispatchedQuantity) {
      detailControl.get('quantity')?.patchValue(detailControl?.value.dispatchedQuantity);
    }
  }

  getItemData(itemId: string) {
    return this.itemList.find(x => x.orderItem?.item?.id === itemId);
  }

  getDCData() {
    const dispatchOrderId = this.saleReturnForm.get('dispatchOrderId')?.value;
    return this.dispatchOrderList.find(x => x.id === dispatchOrderId);
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
    const saleReturnDetailArray = this.saleReturnForm.get('saleReturnDetail') as FormArray;
    if (!saleReturnDetailArray || index < 0 || index >= saleReturnDetailArray.length) {
      console.error('Invalid index or FormArray is not initialized properly.');
      return;
    }

    const currentFormGroup = saleReturnDetailArray.at(index) as FormGroup;
    currentFormGroup.reset();
    return;
  }



  removeAllSaleReturnDetails() {
    if (this.saleReturnDetail.length > 0) {
      this.saleReturnDetail.clear();
      this.addSaleReturnDetail(0);
    } else {
      this.notificationsService.showNotification('No items to remove.', 'snack-bar-warning');
    }
  }

  reset() {
    this.saleReturnForm.get('code')?.patchValue('');
  }

  async getDispatchedDetails(): Promise<void> {
    const dispatchOrderId = this.saleReturnForm.get('dispatchOrderId')?.value;
    const saleReturnId = this.saleReturnForm.get('id')?.value;
    try {
      const data = await (await this.saleReturnService.getPendingDCItems(dispatchOrderId, saleReturnId)).toPromise();
      this.itemList = data;

      const detailsArray = this.saleReturnForm.get('saleReturnDetail') as FormArray;
      detailsArray.clear();

      if (this.itemList && this.itemList.length > 0) {
        this.itemList.forEach((detail: any) => {
          const detailGroup = this.formBuilder.group({
            id: [0],
            saleReturnId: [this.saleReturnForm.get('id')?.value],
            dispatchDetailId: [detail.id, Validators.required],
            itemId: [detail?.orderItem?.itemId, Validators.required],
            dispatchedQuantity: [detail.quantity, Validators.required],
            quantity: [0, Validators.required],
          });

          detailsArray.push(detailGroup);
        });
      }
    } catch (error) {
      console.error('Error fetching pending purchase order items:', error);
    }
  }

  async getPendingOrderList(dispatchOrderId: any, filter: any): Promise<any[]> {
    const data = await firstValueFrom(await this.saleReturnService.getPendingDC(dispatchOrderId, filter));
    this.dispatchOrderList = data;
    return data;
  }

  async getPendingOrder(event: any) {
    try {
      const filter = event.currentTarget.value;
      if (!filter || filter.length < 2) {
        this.dispatchOrderList = []; // Optionally clear the list
        return; // Skip API call if filter is less than 2 characters
      }
      var dispatchOrderId = this.saleReturnForm.get('dispatchOrderId')?.value;
      this.dispatchOrderList = await this.getPendingOrderList(dispatchOrderId, filter);
      //firstValueFrom(await this.saleReturnService.getPendingDC(dispatchOrderId, filter));
    } catch (error) {
      console.error('Error fetching demand list:', error);
    }
  }

  onDcSelected(event: MatAutocompleteSelectedEvent): void {
    const selectedValue = event.option.value;
    if (selectedValue) {
      this.saleReturnForm.get('dispatchOrderId')?.patchValue(selectedValue.id);
      this.saleReturnForm.get('dcCode')?.patchValue(selectedValue.dcCode);
      this.getDispatchedDetails();
    } else {
      this.saleReturnForm.get('dispatchOrderId')?.patchValue(0);
      this.itemList = [];
    }
  }

  onDcInputCleared(event: Event): void {
    const inputValue = (event.target as HTMLInputElement)?.value;
    if (!inputValue.trim()) {
      this.saleReturnForm.get('dispatchOrderId')?.patchValue(0);
      this.saleReturnForm.get('dcCode')?.patchValue('');
      this.removeAllSaleReturnDetails();
      this.itemList = [];
    }
  }
}
