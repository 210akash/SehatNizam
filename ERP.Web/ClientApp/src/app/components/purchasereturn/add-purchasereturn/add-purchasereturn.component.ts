import { Component, Inject } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { PurchaseReturnService } from '../purchasereturn.service';
import { DepartmentService } from '../../department/department.service';
import { MatOptionSelectionChange } from '@angular/material/core';
import { PurchaseOrderService } from '../../purchaseorder/purchaseorder.service';
import { firstValueFrom } from 'rxjs';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';

@Component({
  selector: 'app-add-purchasereturn',
  templateUrl: './add-purchasereturn.component.html',
  styleUrl: './add-purchasereturn.component.css',
  standalone: false
})

export class AddPurchaseReturnComponent {
  purchaseReturnForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  grnList: any[] = [];
  itemList: any[] = [];
  isdataload: boolean = false;
  purchasereturnTypeList: any;
  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private purchaseReturnService: PurchaseReturnService, private grnService: PurchaseOrderService, private departmentService: DepartmentService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.purchaseReturnForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      createdDate: [new Date(), Validators.required],
      grnId: [0, Validators.required],
      grnCode: ['',Validators.required],
      projectId: [0, Validators.required],
      status: [''],
      statusName: ['New'],
      statusId: [1],
      remarks: [''],
      purchaseReturnDetail: this.formBuilder.array([])
    });

    this.LoadData(this.data.element);
  }

  get purchaseReturnDetail(): FormArray {
    return this.purchaseReturnForm.get('purchaseReturnDetail') as FormArray;
  }

  addPurchaseReturnDetail(index: number) {
    const newDetailGroup = this.formBuilder.group({
      id: [0],
      purchaseReturnId: [0],
      grnDetailId: [0, Validators.required],
      itemId: [0, Validators.required],
      grnedQuantity: [0, Validators.required],
      quantity: [null, Validators.required],
    });

    this.purchaseReturnDetail.insert(index + 1, newDetailGroup);
  }

  removePurchaseReturnDetail(index: number) {
    if (this.purchaseReturnDetail.length > 1) {
      this.purchaseReturnDetail.removeAt(index);
    } else {
      this.notificationsService.showNotification(
        'At least one item is required.',
        'snack-bar-danger'
      );
    }
  }

  getIndexValue(index: number): any {
    const detailControl = (this.purchaseReturnForm.get('purchaseReturnDetail') as FormArray).at(index);
    return detailControl?.value.item || '';
  }

  async LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.purchaseReturnForm);
      this.purchaseReturnForm.get('grnCode')?.patchValue(element.grn?.code);
      this.getPendingGRNList(this.data.element.grnId, element.grn?.code);
      // Wait for getGRNDetails to complete
      await this.getGRNDetails(true);  // This will now properly wait for the async operation

      const detailsArray = this.purchaseReturnForm.get('purchaseReturnDetail') as FormArray;
      detailsArray.clear();

      if (element.purchaseReturnDetail && element.purchaseReturnDetail.length > 0) {
        element.purchaseReturnDetail.forEach((detail: any) => {
          const detailGroup = this.formBuilder.group({
            id: [detail.id],
            purchaseReturnId: [detail.purchaseReturnId],
            grnDetailId: [detail.grnDetailId, Validators.required],
            itemId: [detail?.grnDetail?.inspectionDetail?.igpDetail?.purchaseOrderDetail?.purchaseDemandDetail?.itemId, Validators.required],
            grnedQuantity: [detail.grnDetail?.quantity, Validators.required],
            project: [detail?.project?.name, Validators.required],
            quantity: [detail.quantity, Validators.required],
          });

          detailsArray.push(detailGroup);
        });
      }
    } else {
      this.getPurchaseReturnCode();
      this.purchaseReturnForm.get('createdDate')?.patchValue(this.constantService.formatDate(new Date()));
      this.addPurchaseReturnDetail(0);
      // this.getPendingGRN(0);
    }
  }

  checkInvaliGRNontrols(formGroup: FormGroup) {
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
    if (this.purchaseReturnForm.invalid) {
      this.constantService.markFormGroupTouched(this.purchaseReturnForm);
      this.checkInvaliGRNontrols(this.purchaseReturnForm);
      this.notificationsService.showNotification('Please Fill Required Fields', 'snack-bar-danger');
      return;
    }

    this.isLoading = true;
    let _purchaseReturnFormForm: any = {};
    _purchaseReturnFormForm = Object.assign(_purchaseReturnFormForm, this.purchaseReturnForm.value);

    this.purchaseReturnService.savePurchaseReturn(_purchaseReturnFormForm).subscribe({
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

  getPurchaseReturnCode() {
    this.purchaseReturnService.getPurchaseReturnCode().subscribe((data: any) => {
      this.purchaseReturnForm.get('code')?.patchValue(data.code);
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

    const duplicateItem = this.purchaseReturnDetail.controls
      .filter((control: AbstractControl, controlIndex: number) => controlIndex !== index)
      .some((control: AbstractControl) => {
        const formGroup = control as FormGroup;
        return formGroup.get('itemId')?.value === selectedValue;
      });

    if (duplicateItem) {
      this.notificationsService.showNotification('This item has already been selected.', 'snack-bar-danger');
      const currentFormGroup = this.purchaseReturnDetail.at(index) as FormGroup;
      currentFormGroup.get('itemId')?.setValue('');
      currentFormGroup.get('grnedQuantity')?.patchValue(null);
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

      const detailFormGroup = this.purchaseReturnDetail.at(index) as FormGroup;
      detailFormGroup.get('itemId')?.patchValue(selectedItem?.inspectionDetail?.igpDetail?.purchaseOrderDetail?.purchaseDemandDetail?.itemId);
      detailFormGroup.get('grnDetailId')?.patchValue(selectedItem.id);
      detailFormGroup.get('grnedQuantity')?.patchValue(selectedItem.received);
      detailFormGroup.get('quantity')?.setValidators([Validators.required, Validators.min(1), Validators.max(selectedItem.received)]);
      detailFormGroup.updateValueAndValidity();
      this.isUpdating = false;
    }
  }

  validateQty(index: number): any {
    const detailControl = (this.purchaseReturnForm.get('purchaseReturnDetail') as FormArray).at(index);
    if (detailControl?.value.quantity > detailControl?.value.grnedQuantity) {
      detailControl.get('quantity')?.patchValue(detailControl?.value.grnedQuantity);
    }
  }

  getItemData(itemId: string) {
    return this.itemList.find(x => x.inspectionDetail?.igpDetail?.purchaseOrderDetail?.purchaseDemandDetail?.itemId === itemId);
  }

  getGRNData() {
    const grnId = this.purchaseReturnForm.get('grnId')?.value;
    return this.grnList.find(x => x.id === grnId);
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
    const purchaseReturnDetailArray = this.purchaseReturnForm.get('purchaseReturnDetail') as FormArray;
    if (!purchaseReturnDetailArray || index < 0 || index >= purchaseReturnDetailArray.length) {
      console.error('Invalid index or FormArray is not initialized properly.');
      return;
    }

    const currentFormGroup = purchaseReturnDetailArray.at(index) as FormGroup;
    currentFormGroup.reset();
    return;
  }



  removeAllPurchaseReturnDetails() {
    if (this.purchaseReturnDetail.length > 0) {
      this.purchaseReturnDetail.clear();
      this.addPurchaseReturnDetail(0);
    } else {
      this.notificationsService.showNotification('No items to remove.', 'snack-bar-warning');
    }
  }

  reset() {
    this.purchaseReturnForm.get('code')?.patchValue('');
  }

  async getGRNDetails(isload:boolean): Promise<void> {
    const grnId = this.purchaseReturnForm.get('grnId')?.value;
    const purchaseReturnId = this.purchaseReturnForm.get('id')?.value;
    try {
      const data = await (await this.purchaseReturnService.getPendingGRNItems(grnId, purchaseReturnId)).toPromise();
      this.itemList = data;

      if(!isload){
 const detailsArray = this.purchaseReturnForm.get('purchaseReturnDetail') as FormArray;
      detailsArray.clear();

      if (this.itemList && this.itemList.length > 0) {
        this.itemList.forEach((detail: any) => {
          const detailGroup = this.formBuilder.group({
            id: [0],
            purchaseReturnId: [this.purchaseReturnForm.get('id')?.value],
            grnDetailId: [detail.id, Validators.required],
            itemId: [detail?.inspectionDetail?.igpDetail?.purchaseOrderDetail?.purchaseDemandDetail?.itemId, Validators.required],
            grnedQuantity: [detail.received, Validators.required],
            project: [detail?.inspectionDetail?.igpDetail?.purchaseOrderDetail?.purchaseDemandDetail?.project?.name, Validators.required],
            quantity: [0, Validators.required],
          });

          detailsArray.push(detailGroup);
        });
      }
      console.log(detailsArray);
      }
    } catch (error) {
      console.error('Error fetching pending purchase order items:', error);
    }
  }

  async getPendingGRNList(grnId: any, filter: any): Promise<any[]> {
    const data = await firstValueFrom(await this.purchaseReturnService.getPendingGRN(grnId, filter));
    this.grnList = data;
    return data;
  }

  async getPendingGRN(event: any) {
    try {
      const filter = event.currentTarget.value;
      if (!filter || filter.length < 2) {
        this.grnList = []; // Optionally clear the list
        return; // Skip API call if filter is less than 2 characters
      }
      var grnId = this.purchaseReturnForm.get('grnId')?.value;
      this.grnList = await this.getPendingGRNList(grnId, filter);
      //firstValueFrom(await this.purchaseReturnService.getPendingGRN(grnId, filter));
    } catch (error) {
      console.error('Error fetching demand list:', error);
    }
  }

  onGrnSelected(event: MatAutocompleteSelectedEvent): void {
    const selectedValue = event.option.value;
    if (selectedValue) {
      this.purchaseReturnForm.get('grnId')?.patchValue(selectedValue.id);
      this.purchaseReturnForm.get('grnCode')?.patchValue(selectedValue.code);
      this.getGRNDetails(false);
    } else {
      this.purchaseReturnForm.get('grnId')?.patchValue(0);
      this.itemList = [];
    }
  }

  onGrnInputCleared(event: Event): void {
    const inputValue = (event.target as HTMLInputElement)?.value;
    if (!inputValue.trim()) {
      this.purchaseReturnForm.get('grnId')?.patchValue(0);
      this.removeAllPurchaseReturnDetails();
      this.itemList = [];
    }
  }
}
