import { Component, Inject } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { ItemService } from '../../item/item.service';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { ComparativeStatementService } from '../comparativestatement.service';
import { firstValueFrom } from 'rxjs';
import { DepartmentService } from '../../department/department.service';
import { VendorService } from '../../vendor/vendor.service';
import { MatOptionSelectionChange } from '@angular/material/core';
import { MatSelectChange } from '@angular/material/select';

@Component({
  selector: 'app-add-comparativestatement',
  templateUrl: './add-comparativestatement.component.html',
  styleUrl: './add-comparativestatement.component.css',
  standalone: false
})

export class AddComparativeStatementComponent {
  comparativestatementForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  vendorList: any;
  demandList: any;
  pendingindentItemList: any;
  itemList: any[] = [];
  isdataload: boolean = false;
  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private vendorService: VendorService, private comparativestatementService: ComparativeStatementService, private itemService: ItemService, private departmentService: DepartmentService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.comparativestatementForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      createdDate: [new Date()],
      purchaseDemandId: [0, Validators.required], // Validation
      status: [''], // Validation
      statusName: ['New'], // Validation
      statusId: [1], // Validation
      remarks: [''],
      comparativeStatementDetail: this.formBuilder.array([]) // Initialize as a FormArray
    });
    this.LoadData(this.data.element);
    this.getvendorList();
    this.getdemandList();
  }

  get comparativeStatementDetail(): FormArray {
    return this.comparativestatementForm.get('comparativeStatementDetail') as FormArray;
  }

  comparativeStatementVendor(detailIndex: number): FormArray {
    return this.comparativeStatementDetail
      .at(detailIndex)
      .get('comparativeStatementVendor') as FormArray;
  }

  addComparativeStatementDetail(index: number) {
    const newDetailGroup = this.formBuilder.group({
      id: [0], // Default value
      comparativeStatementId: [0], // Default value
      purchaseDemandDetailId: [0], // Default value
      item: [null, Validators.required], // Validation
      comparativeStatementVendor: this.formBuilder.array([
        this.createComparativeStatementVendor()
      ]) // Initialize as a FormArray
    });

    // Insert the new group after the current index
    this.comparativeStatementDetail.insert(index + 1, newDetailGroup);
  }

  createComparativeStatementVendor() {
    return this.formBuilder.group({
      id: [0], // Default value
      comparativeStatementDetailId: [0], // Default value
      vendorId: ['', Validators.required], // Validation
      vendorName: ['', Validators.required], // Validation
      vendor: [null, Validators.required], // Validation
      price: [0, [Validators.required, Validators.min(1)]], // Validation
    });
  }

  addComparativeStatementVendor(index: number, detailIndex: number) {
    const vendorArray = this.comparativeStatementVendor(detailIndex);
    // Insert the new vendor FormGroup at the specified index
    vendorArray.insert(index + 1, this.createComparativeStatementVendor());
  }

  removeComparativeStatementVendor(vendorIndex: number, detailIndex: number) {
    const vendorArray = this.comparativeStatementVendor(detailIndex);
    if (vendorArray.length > 1) {
      vendorArray.removeAt(vendorIndex);
    } else {
      this.notificationsService.showNotification(
        'At least one vendor is required for each item.',
        'snack-bar-danger'
      );
    }
  }

  removeComparativeStatementDetail(index: number) {
    if (this.comparativeStatementDetail.length > 1) {
      this.comparativeStatementDetail.removeAt(index);
    } else {
      this.notificationsService.showNotification(
        'At least one item is required.',
        'snack-bar-danger'
      );
    }
  }

  getIndexValue(index: number): any {
    const detailControl = (this.comparativestatementForm.get('comparativeStatementDetail') as FormArray).at(index);
    return detailControl?.value.item || '';
  }

  getIndexValueVendor(index: number, detailIndex: number): any {
    const vendorArrays = this.comparativeStatementVendor(detailIndex);
    const vendorArray = vendorArrays.at(index);
    return vendorArray?.value.vendor || '';
  }


  getItemList(event: any) {
    var filter = event.currentTarget.value;
    this.itemService.getItemByName(filter).subscribe((data: any) => {
      this.itemList = data;
    });
  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.comparativestatementForm);
      this.getdemandList();
      this.getPendingDemandItems();
      this.getvendorList();

      // Populate the FormArray
      const detailsArray = this.comparativestatementForm.get('comparativeStatementDetail') as FormArray;
      detailsArray.clear(); // Clear existing data

      if (element.comparativeStatementDetail && element.comparativeStatementDetail.length > 0) {
        element.comparativeStatementDetail.forEach((detail: any) => {
          // Map vendor details to FormArray
          const vendorArray = this.formBuilder.array(
            detail.comparativeStatementVendor.map((vendordetail: any) =>
              this.formBuilder.group({
                id: [vendordetail.id],
                comparativeStatementDetailId: [vendordetail.comparativeStatementDetailId],
                vendorId: [vendordetail.vendorId, Validators.required],
                vendor: [vendordetail.vendor],
                vendorName: [vendordetail.vendor],
                price: [vendordetail.price, [Validators.required, Validators.min(1)]]
              })
            )
          );
          //vendorName: [vendordetail.vendor ? vendordetail.vendor.name : '', Validators.required],

          // Map detail group
          const detailGroup = this.formBuilder.group({
            id: [detail.id],
            comparativeStatementId: [detail.comparativeStatementId],
            purchaseDemandDetailId: [detail.purchaseDemandDetailId, Validators.required],
            item: [detail.purchaseDemandDetail || null, Validators.required], // Set the selected item
            itemName: [detail.purchaseDemandDetail, Validators.required],
            comparativeStatementVendor: vendorArray
          });

          detailsArray.push(detailGroup);
        });
        console.log(detailsArray);
      }
    } else {
      // Default initialization for new entries
      this.getComparativeStatementCode();
      this.comparativestatementForm.get('createdDate')?.patchValue(this.constantService.formatDate(new Date()));
      this.addComparativeStatementDetail(0);
    }
  }

  checkInvalidControls(formGroup: FormGroup) {
    // Loop through each control in the FormGroup
    Object.keys(formGroup.controls).forEach(controlName => {
      const control = formGroup.get(controlName);

      // Check if the control is invalid
      if (control && control.invalid) {
        console.log(`Control '${controlName}' is invalid.`);
        // You can further log the specific errors for each control
        console.log(control.errors);
      }
    });

    // If there are FormArrays, check their controls as well
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
    if (this.comparativestatementForm.invalid) {
      // Mark all controls as touched to trigger validation messages
      this.constantService.markFormGroupTouched(this.comparativestatementForm);

      // Check each control in the FormGroup to see which one is invalid
      this.checkInvalidControls(this.comparativestatementForm);

      this.notificationsService.showNotification('Please Fill Required Fields', 'snack-bar-danger');
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.comparativestatementForm.value);
    let requiredDate = new Date(this.comparativestatementForm.get('requiredDate')?.value);
    _clienttemperatureForm['requiredDate'] = requiredDate.toLocaleDateString();

    this.comparativestatementService.saveComparativeStatement(_clienttemperatureForm).subscribe({
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

  getComparativeStatementCode() {
    this.comparativestatementService.getComparativeStatementCode().subscribe((data: any) => {
      this.comparativestatementForm.get('code')?.patchValue(data.code);
    });
  }

  isUpdating = false; // Flag to prevent recursive calls

  onOptionSelected(event: MatOptionSelectionChange, index: number): void {
    // Prevent triggering the event while updating form controls
    if (this.isUpdating) {
      return;
    }

    this.isUpdating = true; // Set the flag to prevent recursion

    const selectedValue = event.source.value;

    const indentRequestDetailArray = this.comparativestatementForm.get('comparativeStatementDetail') as FormArray;

    if (!selectedValue) {
      console.error('Option value is undefined. Ensure mat-option [value] is correctly bound.');
      this.isUpdating = false; // Reset the flag
      return;
    }

    // Get the FormArray for indentRequestDetail

    // Check if index is valid
    if (!indentRequestDetailArray || index < 0 || index >= indentRequestDetailArray.length) {
      console.error('Invalid index or FormArray is not initialized properly.');
      this.isUpdating = false; // Reset the flag
      return;
    }

    // Check if the selected itemId already exists in the form array (excluding the current index)
    const duplicateItem = indentRequestDetailArray.controls
      .filter((control: AbstractControl, controlIndex: number) => controlIndex !== index) // Exclude the current index
      .some((control: AbstractControl) => {
        const formGroup = control as FormGroup;  // Cast AbstractControl to FormGroup
        return formGroup.get('purchaseDemandDetailId')?.value === selectedValue;  // Check if the itemId already exists
      });

    if (duplicateItem) {
      this.notificationsService.showNotification('This item has already been selected.', 'snack-bar-danger');
      const currentFormGroup = indentRequestDetailArray.at(index) as FormGroup;

      // Reset the form group values
      currentFormGroup.get('purchaseDemandDetailId')?.patchValue(0);
      currentFormGroup.get('item')?.patchValue(null);
      this.isUpdating = false; // Reset the flag
      return;
    }
    else {
      // Get the selected item details from your getitem method
      const selectedItem = this.getitem(selectedValue);
      if (!selectedItem) {
        console.error('Selected item not found.');
        this.isUpdating = false; // Reset the flag
        return;
      }

      // Get the form group for the current index
      const detailFormGroup = indentRequestDetailArray.at(index) as FormGroup;

      // Patch the values into the form group
      detailFormGroup.get('purchaseDemandDetailId')?.patchValue(selectedItem.id);
      detailFormGroup.get('item')?.patchValue(selectedItem);
      // detailFormGroup.get('itemName')?.patchValue(selectedValue.item.code +':' + selectedValue.item.name);
      this.isUpdating = false; // Reset the flag
    }
  }

  isUpdatingvendor = false; // Flag to prevent recursive calls
  onVendorSelected(event: MatOptionSelectionChange, index: number, detailIndex: number): void {
    // Prevent triggering the event while updating form controls
    if (this.isUpdatingvendor) {
      return;
    }
    this.isUpdatingvendor = true; // Set the flag to prevent recursion

    const selectedValue = event.source.value;
    const indentRequestDetailArray = this.comparativeStatementVendor(detailIndex);

    if (!selectedValue) {
      console.error('Option value is undefined. Ensure mat-option [value] is correctly bound.');
      this.isUpdatingvendor = false; // Reset the flag
      return;
    }

    // Get the FormArray for indentRequestDetail

    // Check if index is valid
    if (!indentRequestDetailArray || index < 0 || index >= indentRequestDetailArray.length) {
      console.error('Invalid index or FormArray is not initialized properly.');
      this.isUpdatingvendor = false; // Reset the flag
      return;
    }

    // Check if the selected itemId already exists in the form array (excluding the current index)
    const duplicateItem = indentRequestDetailArray.controls
      .filter((control: AbstractControl, controlIndex: number) => controlIndex !== index) // Exclude the current index
      .some((control: AbstractControl) => {
        const formGroup = control as FormGroup;  // Cast AbstractControl to FormGroup
        return formGroup.get('vendorId')?.value === selectedValue;  // Check if the itemId already exists
      });

    if (duplicateItem) {
      this.notificationsService.showNotification('This item has already been selected.', 'snack-bar-danger');
      const currentFormGroup = indentRequestDetailArray.at(index) as FormGroup;
      currentFormGroup.get('vendorId')?.patchValue(0);
      currentFormGroup.get('vendor')?.patchValue(null);
      currentFormGroup.get('vendorName')?.patchValue('');
      currentFormGroup.get('price')?.patchValue(0);
      this.isUpdatingvendor = false; // Reset the flag
      // This will clear all the values of the current form group
      return; // Do nothing if the itemId already exists
    }
    else {
      // Get the selected item details from your getitem method
      const selectedItem = this.getvendor(selectedValue);
      if (!selectedItem) {
        console.error('Selected item not found.');
        this.isUpdatingvendor = false; // Reset the flag
        return;
      }

      // Get the form group for the current index
      const detailFormGroup = indentRequestDetailArray.at(index) as FormGroup;

      // Patch the values into the form group
      detailFormGroup.get('vendorId')?.patchValue(selectedItem.id);
      detailFormGroup.get('vendor')?.patchValue(selectedItem);
      detailFormGroup.get('vendorName')?.patchValue(selectedItem.name);
      this.isUpdatingvendor = false; // Reset the flag
    }
  }

  getitem(itemId: string) {
    return this.itemList.find((option: { id: string; }) => option.id === itemId);
  }

  getvendor(vendorId: string) {
    return this.vendorList.find((option: { id: string; }) => option.id === vendorId);
  }

  onInputCleared(event: Event, index: number): void {
    const inputValue = (event.target as HTMLInputElement).value;
    console.log('Current Input Value:', inputValue); // Debugging output

    if (!inputValue.trim()) {
      console.log(`Input cleared at row index: ${index}`);
      this.resetitem(index); // Call a function when cleared
    }
  }

  resetitem(index: number) {
    const comparativeStatementDetailArray = this.comparativestatementForm.get('comparativeStatementDetail') as FormArray;

    // Check if index is valid
    if (!comparativeStatementDetailArray || index < 0 || index >= comparativeStatementDetailArray.length) {
      console.error('Invalid index or FormArray is not initialized properly.');
      return;
    }

    const currentFormGroup = comparativeStatementDetailArray.at(index) as FormGroup;
    currentFormGroup.reset();  // This will clear all the values of the current form group
    return; // Do nothing if the itemId already exists
  }

  getvendorList() {
    let _vendorFilter: any = {};
    this.vendorService.getAllVendors(_vendorFilter).subscribe((data: any) => {
      this.vendorList = data.item1;
    });
  }

  async getdemandList() {
    (await this.comparativestatementService.getPendingDemand(this.comparativestatementForm.get('purchaseDemandId')?.value, this.comparativestatementForm.get('id')?.value)).subscribe((data: any) => {
      this.demandList = data;
    });
  }

  async getPendingindentItemList() {
    try {
      const indentRequestId = this.comparativestatementForm.get('indentRequestId')?.value;
      const id = this.isEditMode == true ? this.data?.element?.id : 0;

      // Use firstValueFrom to convert the observable to a promise
      const data = await firstValueFrom(await this.comparativestatementService.getPendingDemandItems(indentRequestId, id));
      this.pendingindentItemList = data || [];
    } catch (error) {
      console.error('Error loading pending indent items:', error);
    }
  }

  removeAllComparativeStatementDetails() {
    // Check if there are any rows
    if (this.comparativeStatementDetail.length > 0) {
      // Clear all items in the FormArray
      this.comparativeStatementDetail.clear();
      this.addComparativeStatementDetail(0);
    } else {
      this.notificationsService.showNotification('No items to remove.', 'snack-bar-warning');
    }
  }

  onitemSelected(event: any, index: number): void {


  }

  getPendingDemandItemList(event: MatOptionSelectionChange): void {
    // Prevent triggering the event while updating form controls
    if (this.isUpdating) {
      return;
    }

    this.isUpdating = true; // Set the flag to prevent recursion

    console.log('onSelectionChange triggered');
    const selectedValue = event.source.value; // Access the selected value
    console.log('Selected Value:', selectedValue);

    // const purchaseOrderDetailArray = this.comparativestatementForm.get('comparativeStatementDetails') as FormArray;
    // if (!selectedValue) {
    //   console.error('Option value is undefined. Ensure mat-option [value] is correctly bound.');
    //   this.isUpdating = false; // Reset the flag
    //   return;
    // }

    // if (!purchaseOrderDetailArray || index < 0 || index >= purchaseOrderDetailArray.length) {
    //   console.error('Invalid index or FormArray is not initialized properly.');
    //   this.isUpdating = false; // Reset the flag
    //   return;
    // }

    // // Get the selected item details from your getitem method
    // // const selectedDemand = this.getorder(selectedValue.id);
    // // if (!selectedDemand) {
    // //   console.error('Selected Demand not found.');
    // //   this.isUpdating = false; // Reset the flag
    // //   return;
    // // }

    // // Get the form group for the current index
    // const detailFormGroup = purchaseOrderDetailArray.at(index) as FormGroup;

    // // Patch the selected item values into the form group
    // detailFormGroup.get('purchaseDemandId')?.patchValue(selectedValue.id);
    // detailFormGroup.get('demandCode')?.patchValue(selectedValue.code);
    this.isUpdating = false; // Reset the flag
  }

  async getPendingDemandItems() {
    try {
      const demandId = this.comparativestatementForm.get('purchaseDemandId')?.value;
      this.itemList = [];
      const id = this.isEditMode == true ? this.data?.element?.id : 0;

      // Use firstValueFrom to convert the observable to a promise
      const data = await firstValueFrom(await this.comparativestatementService.getPendingDemandItems(demandId, id));
      this.itemList = data || [];
    } catch (error) {
      console.error('Error loading pending indent items:', error);
    }
  }

  reset() {
    this.comparativestatementForm.get('code')?.patchValue('');
  }
}
