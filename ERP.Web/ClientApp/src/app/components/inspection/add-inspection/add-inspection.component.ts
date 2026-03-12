import { Component, Inject } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { InspectionService } from '../inspection.service';
import { DepartmentService } from '../../department/department.service';
import { MatOptionSelectionChange } from '@angular/material/core';
import { PurchaseOrderService } from '../../purchaseorder/purchaseorder.service';
import { Observable } from 'rxjs';
import { RejectReasonService } from '../../rejectreason/rejectreason.service';

@Component({
  selector: 'app-add-inspection',
  templateUrl: './add-inspection.component.html',
  styleUrl: './add-inspection.component.css',
  standalone: false
})

export class AddInspectionComponent {
  inspectionForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  purchaseOrderList: any;
  itemList: any[] = [];
  rejectReasons: any[] = [];
  isdataload: boolean = false;

  constructor(private dialog: MatDialog, private rejectReasonService: RejectReasonService,private notificationsService: NotificationsService, private formBuilder: FormBuilder, private inspectionService: InspectionService, private purchaseOrderService: PurchaseOrderService, private departmentService: DepartmentService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.inspectionForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      createdDate: [new Date()],
      igpId: [0, Validators.required],
      status: [''],
      statusName: ['New'],
      statusId: [1],
      remarks: [''],
      inspectionDetail: this.formBuilder.array([])
    });


    this.getrejectReasons();
    this.LoadData(this.data.element);
  }

  get inspectionDetail(): FormArray {
    return this.inspectionForm.get('inspectionDetail') as FormArray;
  }

  addInspectionDetail(index: number,detail:any) {
    const newDetailGroup = this.formBuilder.group({
      id: [0],
      inspectionId: [0],
      igpDetailId: [detail.id, Validators.required],
      itemId: [detail.purchaseOrderDetail?.purchaseDemandDetail?.itemId],
      item: [detail.purchaseOrderDetail?.purchaseDemandDetail?.item],
      received: [detail.received, Validators.required],
      rejected: [0, Validators.required],
      rejectReasonId: [{ value: null, disabled: true }],
      remarks: [{ value: '', disabled: true }],
    });

    this.inspectionDetail.insert(index + 1, newDetailGroup);
  }

  removeInspectionDetail(index: number) {
    if (this.inspectionDetail.length > 1) {
      this.inspectionDetail.removeAt(index);
    } else {
      this.notificationsService.showNotification(
        'At least one item is required.',
        'snack-bar-danger'
      );
    }
  }

  getIndexValue(index: number): any {
    const detailControl = (this.inspectionForm.get('inspectionDetail') as FormArray).at(index);
    return detailControl?.value.item || '';
  }

  async LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.inspectionForm);
    this.getPendingIGP(this.data.element.igpId);
      // Wait for getIGPDetails to complete
      await this.getIGPDetails();  // This will now properly wait for the async operation
  
      const detailsArray = this.inspectionForm.get('inspectionDetail') as FormArray;
      detailsArray.clear();
  
      if (element.inspectionDetail && element.inspectionDetail.length > 0) {
        element.inspectionDetail.forEach((detail: any) => {
          const detailGroup = this.formBuilder.group({
            id: [detail.id],
            inspectionId: [detail.inspectionId],
            igpDetailId: [detail.igpDetailId, Validators.required],
            item :[detail.igpDetail?.purchaseOrderDetail?.purchaseDemandDetail?.item, Validators.required],
            itemId: [detail.igpDetail?.purchaseOrderDetail?.purchaseDemandDetail?.itemId, Validators.required],
            received: [detail.igpDetail?.received, Validators.required],
            rejected: [detail.rejected, Validators.required],
            rejectReasonId: [{ value: detail.rejectReasonId, disabled: detail.rejected == 0 ? true : false }],
            remarks: [{ value:detail.remarks, disabled: detail.rejected == 0 ? true : false  }],
          });
  
          detailsArray.push(detailGroup);
        });
        console.log(detailsArray);
        console.log(this.inspectionForm.value);
      }
    } else {
      this.getInspectionCode();
      this.inspectionForm.get('createdDate')?.patchValue(this.constantService.formatDate(new Date()));
      // this.addInspectionDetail(0);
      this.getPendingIGP(0);
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
    if (this.inspectionForm.invalid) {
      this.constantService.markFormGroupTouched(this.inspectionForm);
      this.checkInvalidControls(this.inspectionForm);
      this.notificationsService.showNotification('Please Fill Required Fields', 'snack-bar-danger');
      return;
    }

    this.isLoading = true;
    let _inspectionFormForm: any = {};
    _inspectionFormForm = Object.assign(_inspectionFormForm, this.inspectionForm.value);

    this.inspectionService.saveInspection(_inspectionFormForm).subscribe({
      next: (data: { Status: number; Data: string;Message: string; }) => {
        if (data.Status == 200) {
          this.notificationsService.showNotification(data.Data, 'snack-bar-success');
          this.dialog.closeAll();
        }
        else if (data.Status == 500) {
          this.notificationsService.showNotification(data.Message, 'snack-bar-danger');     
        }
        else
          this.notificationsService.showNotification(data.Data, 'snack-bar-danger');
        this.isLoading = false;
      },
      error: (error: any) => {
        const errorMessage = error.error?.Message || error.error?.Data || error.statusText || 'An unexpected error occurred.';
        this.notificationsService.showNotification(errorMessage, 'snack-bar-danger');
        console.error(error);
        this.isLoading = false;
    }
    });
  }

  getInspectionCode() {
    this.inspectionService.getInspectionCode().subscribe((data: any) => {
      this.inspectionForm.get('code')?.patchValue(data.code);
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

    const duplicateItem = this.inspectionDetail.controls
      .filter((control: AbstractControl, controlIndex: number) => controlIndex !== index)
      .some((control: AbstractControl) => {
        const formGroup = control as FormGroup;
        return formGroup.get('itemId')?.value === selectedValue;
      });

    if (duplicateItem) {
      this.notificationsService.showNotification('This item has already been selected.', 'snack-bar-danger');
      const currentFormGroup = this.inspectionDetail.at(index) as FormGroup;
      currentFormGroup.get('itemId')?.patchValue("");
      currentFormGroup.get('igpDetailId')?.patchValue(0);
      currentFormGroup.get('received')?.patchValue(null);
      currentFormGroup.get('rejected')?.patchValue(0);
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

      const detailFormGroup = this.inspectionDetail.at(index) as FormGroup;
      detailFormGroup.get('itemId')?.patchValue(selectedItem?.purchaseOrderDetail?.purchaseDemandDetail?.item.id);
      detailFormGroup.get('igpDetailId')?.patchValue(selectedItem.id);
      detailFormGroup.get('received')?.patchValue(selectedItem.received);
      detailFormGroup.get('rejected')?.setValidators([Validators.required, Validators.min(0), Validators.max(selectedItem.received)]);
      detailFormGroup.updateValueAndValidity();
      this.isUpdating = false;
    }
  }

  validateQty(index: number): any {
    const detailFormGroup = (this.inspectionForm.get('inspectionDetail') as FormArray).at(index);
    if(detailFormGroup?.value.rejected > 0){
      detailFormGroup.get('rejectReasonId')?.enable();
      detailFormGroup.get('remarks')?.enable();
      detailFormGroup.get('rejectReasonId')?.setValidators([Validators.required]);
      detailFormGroup.get('remarks')?.setValidators([Validators.required]);

    if (detailFormGroup?.value.rejected > detailFormGroup?.value.received) {
      detailFormGroup.get('rejected')?.patchValue(detailFormGroup?.value.received);
    }
    detailFormGroup.updateValueAndValidity();
  }
  else{
    detailFormGroup.get('rejectReasonId')?.disable();
    detailFormGroup.get('remarks')?.disable();
    detailFormGroup.get('rejectReasonId')?.patchValue(null);
    detailFormGroup.get('remarks')?.patchValue('');
    detailFormGroup.get('rejectReasonId')?.clearValidators();
    detailFormGroup.get('remarks')?.clearValidators();
    detailFormGroup.updateValueAndValidity();
  }
  }

  getItemData(itemId: string) {
    return this.itemList.find(x => x.purchaseOrderDetail?.purchaseDemandDetail?.item?.id === itemId);
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
    const inspectionDetailArray = this.inspectionForm.get('inspectionDetail') as FormArray;
    if (!inspectionDetailArray || index < 0 || index >= inspectionDetailArray.length) {
      console.error('Invalid index or FormArray is not initialized properly.');
      return;
    }

    const currentFormGroup = inspectionDetailArray.at(index) as FormGroup;
    currentFormGroup.reset();
    return;
  }

  getPendingIGP(purchaseOrderId:any) {
    this.inspectionService.getPendingIGP(purchaseOrderId).subscribe((data: any) => {
      this.purchaseOrderList = data;
    });
  }

  removeAllInspectionDetails() {
    if (this.inspectionDetail.length > 0) {
      this.inspectionDetail.clear();
    } 
  }

  reset() {
    this.inspectionForm.get('code')?.patchValue('');
  }

  async getrejectReasons(): Promise<void> {
       // Clone the form value and add paging data
       const _RejectReasonFilterForm = {
      };
    try {
      const data = await (await this.rejectReasonService.getAllRejectReasons(_RejectReasonFilterForm)).toPromise();
      this.rejectReasons = data.item1;
    } catch (error) {
      console.error('Error fetching pending purchase order items:', error);
    }
  }

  
  async getIGPDetails(): Promise<void> {
    this.removeAllInspectionDetails();
    const IgpId = this.inspectionForm.get('igpId')?.value;
    const InspectionId = this.inspectionForm.get('id')?.value;
    try {
      const data = await (await this.inspectionService.getPendingIGPItems(IgpId,InspectionId)).toPromise();
      this.itemList = data;
    // Add each item to inspection details
    this.itemList.forEach((item) => {
      const currentIndex = this.inspectionDetail.length - 1;
      this.addInspectionDetail(currentIndex, item);
    });
    } catch (error) {
      console.error('Error fetching pending purchase order items:', error);
    }
  }

}
