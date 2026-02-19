import { Component, Inject } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { IGPService } from '../igp.service';
import { DepartmentService } from '../../department/department.service';
import { MatOptionSelectionChange } from '@angular/material/core';
import { PurchaseOrderService } from '../../purchaseorder/purchaseorder.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-add-igp',
  templateUrl: './add-igp.component.html',
  styleUrl: './add-igp.component.css',
  standalone: false
})

export class AddIGPComponent {
  iGPForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  purchaseOrderList: any;
  itemList: any[] = [];
  isdataload: boolean = false;

  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private iGPService: IGPService, private purchaseOrderService: PurchaseOrderService, private departmentService: DepartmentService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.iGPForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      createdDate: [new Date()],
      purchaseOrderId: [0, Validators.required],
      status: [''],
      statusName: ['New'],
      statusId: [1],
      remarks: [''],
      iGPDetails: this.formBuilder.array([])
    });


    this.LoadData(this.data.element);
  }

  get iGPDetails(): FormArray {
    return this.iGPForm.get('iGPDetails') as FormArray;
  }

  addIGPDetail(index: number) {
    const newDetailGroup = this.formBuilder.group({
      id: [0],
      iGPId: [0],
      purchaseOrderDetailId: [0, Validators.required],
      itemId: [0],
      orderedQuantity: [0, Validators.required],
      received: [null, Validators.required],
    });

    this.iGPDetails.insert(index + 1, newDetailGroup);
  }

  removeIGPDetail(index: number) {
    if (this.iGPDetails.length > 1) {
      this.iGPDetails.removeAt(index);
    } else {
      this.notificationsService.showNotification(
        'At least one item is required.',
        'snack-bar-danger'
      );
    }
  }

  getIndexValue(index: number): any {
    const detailControl = (this.iGPForm.get('iGPDetails') as FormArray).at(index);
    return detailControl?.value.item || '';
  }

  // async LoadData(element: any) {
  //   if (element != null) {
  //     this.isEditMode = true;
  //     this.constantService.LoadData(element, this.iGPForm);
  //     // this.getPendingPurchaseOrders();
  //     await this.getPurchaseOrdersDetails();

  //     const detailsArray = this.iGPForm.get('iGPDetails') as FormArray;
  //     detailsArray.clear();

  //     if (element.igpDetails && element.igpDetails.length > 0) {
  //       element.igpDetails.forEach((detail: any) => {
  //         debugger
  //         console.log('<<<<<>>>>>',this.itemList);
  //         const detailGroup = this.formBuilder.group({
  //           id: [detail.id],
  //           iGPId: [detail.igpId],
  //           purchaseOrderDetailId: [detail.purchaseOrderDetailId, Validators.required],
  //           itemId: [detail.purchaseOrderDetail?.purchaseDemandDetail?.item?.id || null, Validators.required],
  //           orderedQuantity: [detail.purchaseOrderDetail?.quantity || null, Validators.required],
  //           received: [detail.received || null, Validators.required],
  //         });

  //         detailsArray.push(detailGroup);
  //       });
  //       console.log(detailsArray);
  //     }
  //   } else {
  //     this.getIGPCode();
  //     this.iGPForm.get('createdDate')?.patchValue(this.constantService.formatDate(new Date()));
  //     this.addIGPDetail(0);
  //   }
  // }
  async LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.iGPForm);
    this.getPendingPurchaseOrders(this.data.element.purchaseOrderId);
      // Wait for getPurchaseOrdersDetails to complete
      await this.getPurchaseOrdersDetails();  // This will now properly wait for the async operation
  
      const detailsArray = this.iGPForm.get('iGPDetails') as FormArray;
      detailsArray.clear();
  
      if (element.igpDetails && element.igpDetails.length > 0) {
        element.igpDetails.forEach((detail: any) => {
          console.log('<<<<<>>>>>', this.itemList);
          debugger
          const detailGroup = this.formBuilder.group({
            id: [detail.id],
            iGPId: [detail.igpId],
            purchaseOrderDetailId: [detail.purchaseOrderDetailId, Validators.required],
            itemId: [detail.purchaseOrderDetail?.purchaseDemandDetail?.itemId, Validators.required],
            orderedQuantity: [detail.purchaseOrderDetail?.quantity, Validators.required],
            received: [detail.received, Validators.required],
          });
  
          detailsArray.push(detailGroup);
        });
        console.log(detailsArray);
        console.log(this.iGPForm.value);
      }
    } else {
      this.getIGPCode();
      this.iGPForm.get('createdDate')?.patchValue(this.constantService.formatDate(new Date()));
      this.addIGPDetail(0);
      this.getPendingPurchaseOrders(0);
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
    if (this.iGPForm.invalid) {
      this.constantService.markFormGroupTouched(this.iGPForm);
      this.checkInvalidControls(this.iGPForm);
      this.notificationsService.showNotification('Please Fill Required Fields', 'snack-bar-danger');
      return;
    }

    this.isLoading = true;
    let _iGPFormForm: any = {};
    _iGPFormForm = Object.assign(_iGPFormForm, this.iGPForm.value);

    this.iGPService.saveIGP(_iGPFormForm).subscribe({
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

  getIGPCode() {
    this.iGPService.getIGPCode().subscribe((data: any) => {
      this.iGPForm.get('code')?.patchValue(data.code);
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

    const duplicateItem = this.iGPDetails.controls
      .filter((control: AbstractControl, controlIndex: number) => controlIndex !== index)
      .some((control: AbstractControl) => {
        const formGroup = control as FormGroup;
        return formGroup.get('itemId')?.value === selectedValue;
      });

    if (duplicateItem) {
      this.notificationsService.showNotification('This item has already been selected.', 'snack-bar-danger');
      const currentFormGroup = this.iGPDetails.at(index) as FormGroup;
      currentFormGroup.get('itemId')?.patchValue(0);
      currentFormGroup.get('orderedQuantity')?.patchValue(null);
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

      const detailFormGroup = this.iGPDetails.at(index) as FormGroup;
      detailFormGroup.get('itemId')?.patchValue(selectedItem?.purchaseDemandDetail?.item.id);
      detailFormGroup.get('purchaseOrderDetailId')?.patchValue(selectedItem.id);
      detailFormGroup.get('orderedQuantity')?.patchValue(selectedItem.quantity);
      detailFormGroup.get('received')?.setValidators([Validators.required, Validators.min(1), Validators.max(selectedItem.quantity)]);
      detailFormGroup.updateValueAndValidity();
      this.isUpdating = false;
    }
  }

  validateQty(index: number): any {
    const detailControl = (this.iGPForm.get('iGPDetails') as FormArray).at(index);
    if (detailControl?.value.received > detailControl?.value.orderedQuantity) {
      detailControl.get('received')?.patchValue(detailControl?.value.orderedQuantity);
    }
  }

  getItemData(itemId: string) {
    return this.itemList.find(x => x.purchaseDemandDetail?.item?.id === itemId);
  }

  onInputCleared(event: Event, index: number): void {
    const inputValue = (event.target as HTMLInputElement).value;
    console.log('Current Input Value:', inputValue);

    if (!inputValue.trim()) {
      console.log(`Input cleared at row index: ${index}`);
      this.resetitem(index);
    }
  }

  resetitem(index: number) {
    const iGPDetailArray = this.iGPForm.get('iGPDetails') as FormArray;
    if (!iGPDetailArray || index < 0 || index >= iGPDetailArray.length) {
      console.error('Invalid index or FormArray is not initialized properly.');
      return;
    }

    const currentFormGroup = iGPDetailArray.at(index) as FormGroup;
    currentFormGroup.reset();
    return;
  }

  getPendingPurchaseOrders(purchaseOrderId:any) {
    this.iGPService.getPendingPurchaseOrders(purchaseOrderId).subscribe((data: any) => {
      this.purchaseOrderList = data;
    });
  }

  removeAllIGPDetails() {
    if (this.iGPDetails.length > 0) {
      this.iGPDetails.clear();
      this.addIGPDetail(0);
    } else {
      this.notificationsService.showNotification('No items to remove.', 'snack-bar-warning');
    }
  }

  reset() {
    this.iGPForm.get('code')?.patchValue('');
  }

  async getPurchaseOrdersDetails(): Promise<void> {
    const purchaseOrderId = this.iGPForm.get('purchaseOrderId')?.value;

    try {
      const data = await (await this.iGPService.getPendingPOItems(purchaseOrderId)).toPromise();
      this.itemList = data;
    } catch (error) {
      console.error('Error fetching pending purchase order items:', error);
    }
  }



}
