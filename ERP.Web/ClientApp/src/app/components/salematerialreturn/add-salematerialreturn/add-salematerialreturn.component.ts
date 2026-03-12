import { Component, Inject } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { SaleMaterialReturnService } from '../salematerialreturn.service';
import { DepartmentService } from '../../department/department.service';
import { MatOptionSelectionChange } from '@angular/material/core';
import { PurchaseOrderService } from '../../purchaseorder/purchaseorder.service';
import { firstValueFrom } from 'rxjs';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';

@Component({
  selector: 'app-add-salematerialreturn',
  templateUrl: './add-salematerialreturn.component.html',
  styleUrl: './add-salematerialreturn.component.css',
  standalone: false
})

export class AddSaleMaterialReturnComponent {
  saleMaterialReturnForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  salematerialList: any[] = [];
  itemList: any[] = [];
  isdataload: boolean = false;
  salematerialreturnTypeList: any;
  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private saleMaterialReturnService: SaleMaterialReturnService, private salematerialService: PurchaseOrderService, private departmentService: DepartmentService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.saleMaterialReturnForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      createdDate: [new Date(), Validators.required],
      salematerialId: [0, Validators.required],
      salematerialCode: ['', Validators.required],
      projectId: [0, Validators.required],
      status: [''],
      statusName: ['New'],
      statusId: [1],
      remarks: [''],
      saleMaterialReturnDetail: this.formBuilder.array([])
    });

    this.LoadData(this.data.element);
  }

  get saleMaterialReturnDetail(): FormArray {
    return this.saleMaterialReturnForm.get('saleMaterialReturnDetail') as FormArray;
  }

  addSaleMaterialReturnDetail(index: number) {
    const newDetailGroup = this.formBuilder.group({
      id: [0],
      saleMaterialReturnId: [0],
      salematerialDetailId: [0, Validators.required],
      itemId: [0, Validators.required],
      salematerialQuantity: [0, Validators.required],
      quantity: [0.00, Validators.required],
    });

    this.saleMaterialReturnDetail.insert(index + 1, newDetailGroup);
  }

  removeSaleMaterialReturnDetail(index: number) {
    if (this.saleMaterialReturnDetail.length > 1) {
      this.saleMaterialReturnDetail.removeAt(index);
    } else {
      this.notificationsService.showNotification(
        'At least one item is required.',
        'snack-bar-danger'
      );
    }
  }

  getIndexValue(index: number): any {
    const detailControl = (this.saleMaterialReturnForm.get('saleMaterialReturnDetail') as FormArray).at(index);
    return detailControl?.value.item || '';
  }

  async LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.saleMaterialReturnForm);
      this.saleMaterialReturnForm.get('salematerialCode')?.patchValue(element.saleMaterial?.code);
      this.getPendingSaleMaterialList(this.data.element.salematerialId, element.saleMaterial?.code);
      // Wait for getSaleMaterialDetails to complete
      await this.getSaleMaterialDetails(true);  // This will now properly wait for the async operation

      const detailsArray = this.saleMaterialReturnForm.get('saleMaterialReturnDetail') as FormArray;
      detailsArray.clear();

      if (element.saleMaterialReturnDetail && element.saleMaterialReturnDetail.length > 0) {
        element.saleMaterialReturnDetail.forEach((detail: any) => {
          const detailGroup = this.formBuilder.group({
            id: [detail.id],
            saleMaterialReturnId: [detail.saleMaterialReturnId],
            salematerialDetailId: [detail.salematerialDetailId, Validators.required],
            itemId: [detail?.saleMaterialDetail?.itemId, Validators.required],
            salematerialQuantity: [detail.salematerialDetail?.quantity, Validators.required],
            quantity: [detail.quantity, Validators.required],
          });

          detailsArray.push(detailGroup);
        });
      }
    } else {
      this.getSaleMaterialReturnCode();
      this.saleMaterialReturnForm.get('createdDate')?.patchValue(this.constantService.formatDate(new Date()));
      this.addSaleMaterialReturnDetail(0);
      // this.getPendingSaleMaterial(0);
    }
  }

  checkInvaliSaleMaterialontrols(formGroup: FormGroup) {
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
    if (this.saleMaterialReturnForm.invalid) {
      this.constantService.markFormGroupTouched(this.saleMaterialReturnForm);
      this.checkInvaliSaleMaterialontrols(this.saleMaterialReturnForm);
      this.notificationsService.showNotification('Please Fill Required Fields', 'snack-bar-danger');
      return;
    }

    this.isLoading = true;
    let _saleMaterialReturnFormForm: any = {};
    _saleMaterialReturnFormForm = Object.assign(_saleMaterialReturnFormForm, this.saleMaterialReturnForm.value);

    this.saleMaterialReturnService.saveSaleMaterialReturn(_saleMaterialReturnFormForm).subscribe({
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

  getSaleMaterialReturnCode() {
    this.saleMaterialReturnService.getSaleMaterialReturnCode().subscribe((data: any) => {
      this.saleMaterialReturnForm.get('code')?.patchValue(data.code);
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

    const duplicateItem = this.saleMaterialReturnDetail.controls
      .filter((control: AbstractControl, controlIndex: number) => controlIndex !== index)
      .some((control: AbstractControl) => {
        const formGroup = control as FormGroup;
        return formGroup.get('itemId')?.value === selectedValue;
      });

    if (duplicateItem) {
      this.notificationsService.showNotification('This item has already been selected.', 'snack-bar-danger');
      const currentFormGroup = this.saleMaterialReturnDetail.at(index) as FormGroup;
      currentFormGroup.get('itemId')?.setValue(0);
      currentFormGroup.get('salematerialQuantity')?.patchValue(null);
      currentFormGroup.updateValueAndValidity();
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

      const detailFormGroup = this.saleMaterialReturnDetail.at(index) as FormGroup;
      detailFormGroup.get('itemId')?.patchValue(selectedItem?.itemId);
      detailFormGroup.get('salematerialDetailId')?.patchValue(selectedItem.id);
      detailFormGroup.get('salematerialQuantity')?.patchValue(selectedItem.quantity);
      detailFormGroup.get('quantity')?.setValidators([Validators.required, Validators.min(0.01), Validators.max(selectedItem.quantity)]);
      detailFormGroup.updateValueAndValidity();
      this.isUpdating = false;
    }
  }

  validateQty(index: number): any {
    const detailControl = (this.saleMaterialReturnForm.get('saleMaterialReturnDetail') as FormArray).at(index);
    if (detailControl?.value.quantity > detailControl?.value.salematerialQuantity) {
      detailControl.get('quantity')?.patchValue(detailControl?.value.salematerialQuantity);
    }
  }

  getItemData(itemId: string) {
    return this.itemList.find(x => x.itemId === itemId);
  }

  getSaleMaterialData() {
    const salematerialId = this.saleMaterialReturnForm.get('salematerialId')?.value;
    return this.salematerialList.find(x => x.id === salematerialId);
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
    const saleMaterialReturnDetailArray = this.saleMaterialReturnForm.get('saleMaterialReturnDetail') as FormArray;
    if (!saleMaterialReturnDetailArray || index < 0 || index >= saleMaterialReturnDetailArray.length) {
      console.error('Invalid index or FormArray is not initialized properly.');
      return;
    }

    const currentFormGroup = saleMaterialReturnDetailArray.at(index) as FormGroup;
    currentFormGroup.reset();
    return;
  }



  removeAllSaleMaterialReturnDetails() {
    if (this.saleMaterialReturnDetail.length > 0) {
      this.saleMaterialReturnDetail.clear();
      this.addSaleMaterialReturnDetail(0);
    } else {
      this.notificationsService.showNotification('No items to remove.', 'snack-bar-warning');
    }
  }

  reset() {
    this.saleMaterialReturnForm.get('code')?.patchValue('');
  }

  async getSaleMaterialDetails(isload: boolean): Promise<void> {
    const salematerialId = this.saleMaterialReturnForm.get('salematerialId')?.value;
    const saleMaterialReturnId = this.saleMaterialReturnForm.get('id')?.value;
    try {
      const data = await (await this.saleMaterialReturnService.getPendingSaleMaterialItems(salematerialId, saleMaterialReturnId)).toPromise();
      this.itemList = data;

      if (!isload) {
        const detailsArray = this.saleMaterialReturnForm.get('saleMaterialReturnDetail') as FormArray;
        detailsArray.clear();

        if (this.itemList && this.itemList.length > 0) {
          this.itemList.forEach((detail: any) => {
            const detailGroup = this.formBuilder.group({
              id: [0],
              saleMaterialReturnId: [this.saleMaterialReturnForm.get('id')?.value],
              salematerialDetailId: [detail.id, Validators.required],
              itemId: [detail?.itemId, Validators.required],
              salematerialQuantity: [detail.quantity, Validators.required],
              quantity: [0.00, Validators.required],
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

  async getPendingSaleMaterialList(salematerialId: any, filter: any): Promise<any[]> {
    const data = await firstValueFrom(await this.saleMaterialReturnService.getPendingSaleMaterial(salematerialId, filter));
    this.salematerialList = data;
    return data;
  }

  async getPendingSaleMaterial(event: any) {
    try {
      const filter = event.currentTarget.value;
      if (!filter || filter.length < 2) {
        this.salematerialList = []; // Optionally clear the list
        return; // Skip API call if filter is less than 2 characters
      }
      var salematerialId = this.saleMaterialReturnForm.get('salematerialId')?.value;
      this.salematerialList = await this.getPendingSaleMaterialList(salematerialId, filter);
      //firstValueFrom(await this.saleMaterialReturnService.getPendingSaleMaterial(salematerialId, filter));
    } catch (error) {
      console.error('Error fetching demand list:', error);
    }
  }

  onGrnSelected(event: MatAutocompleteSelectedEvent): void {
    const selectedValue = event.option.value;
    if (selectedValue) {
      this.saleMaterialReturnForm.get('salematerialId')?.patchValue(selectedValue.id);
      this.saleMaterialReturnForm.get('salematerialCode')?.patchValue(selectedValue.code);
      this.getSaleMaterialDetails(false);
    } else {
      this.saleMaterialReturnForm.get('salematerialId')?.patchValue(0);
      this.itemList = [];
    }
  }

  onGrnInputCleared(event: Event): void {
    const inputValue = (event.target as HTMLInputElement)?.value;
    if (!inputValue.trim()) {
      this.saleMaterialReturnForm.get('salematerialId')?.patchValue(0);
      this.removeAllSaleMaterialReturnDetails();
      this.itemList = [];
    }
  }
}
