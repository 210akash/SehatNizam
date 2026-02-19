import { Component, Inject } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { IndentrequestService } from '../indentrequest.service';
import { CategoryService } from '../../category/category.service';
import { SubcategoryService } from '../../subcategory/subcategory.service';
import { UomService } from '../../uom/uom.service';
import { ItemtypeService } from '../../itemtype/itemtype.service';
import { ItemService } from '../../item/item.service';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { IndentTypeService } from '../../indenttype/indenttype.service';
import { StoreService } from '../../store/store.service';

@Component({
  selector: 'app-add-indentrequest',
  templateUrl: './add-indentrequest.component.html',
  styleUrl: './add-indentrequest.component.css',
  standalone: false
})

export class AddIndentrequestComponent {
  indentrequestForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  indentTypeList : any;
  storeList : any;
  indentrequestTypeList: any;
  itemList: any[] = [];
  isdataload: boolean = false;
  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private indentTypeService: IndentTypeService, private storeService: StoreService, private indentrequestService: IndentrequestService, private categoryService: CategoryService, private itemService: ItemService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.indentrequestForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      requiredDate: ['', Validators.required],
      createdDate: [new Date()],
      departmentId: [0, Validators.required],
      indentTypeId: [0, Validators.required],
      storeId: [0, Validators.required],
      status: [''], // Validation
      statusName: ['New'], // Validation
      statusId: [1], // Validation
      indentRequestDetail: this.formBuilder.array([]) // Initialize as a FormArray
    });
    this.LoadData(this.data.element);
    this.getindentTypeList();
    this.getstoreList();

  }

  get indentRequestDetail(): FormArray {
    return this.indentrequestForm.get('indentRequestDetail') as FormArray;
  }

  addIndentRequestDetail(index: number) {
    const newDetailGroup = this.formBuilder.group({
      id: [0], // Default value
      indentRequestId: [0], // Default value
      itemId: ['', Validators.required], // Validation
      itemName: ['', Validators.required], // Validation
      item: ['', Validators.required], // Validation
      required: [0, [Validators.required, Validators.min(1)]], // Validation
      description: [''], // Validation

    });

    // Insert the new group after the current index
    this.indentRequestDetail.insert(index + 1, newDetailGroup);
  }

  removeIndentRequestDetail(index: number) {
    if (this.indentRequestDetail.length > 1) {
      this.indentRequestDetail.removeAt(index);
    } else {
      this.notificationsService.showNotification('At least one item is required.', 'snack-bar-danger');
    }
  }

  getuomValue(index: number): any {
    const detailControl = (this.indentrequestForm.get('indentRequestDetail') as FormArray).at(index);
    return detailControl?.value.item?.uom?.name || '';
  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.indentrequestForm);

      // Populate the indentRequestDetail FormArray
      const detailsArray = this.indentrequestForm.get('indentRequestDetail') as FormArray;
      detailsArray.clear(); // Clear existing data

      if (element.indentRequestDetail && element.indentRequestDetail.length > 0) {
        element.indentRequestDetail.forEach((detail: any) => {
          const detailGroup = this.formBuilder.group({
            id: [detail.id],
            indentRequestId: [detail.indentRequestId],
            itemId: [detail.itemId, Validators.required],
            item: [detail.item ? detail.item : '', Validators.required],
            itemName: [detail.item ? detail.item.code + ':' +  detail.item.name : '', Validators.required],
            required: [detail.required, [Validators.required, Validators.min(1)]],
            description: [detail.description]
          });
          detailsArray.push(detailGroup);
        });
      }
    }
    else {
      this.addIndentRequestDetail(0);
      this.indentrequestForm.get('createdDate')?.patchValue(this.constantService.formatDate(new Date()));
      this.getIndentrequestCode();
    }
  }

  SaveData() {
    if (this.indentrequestForm.invalid) {
      this.constantService.markFormGroupTouched(this.indentrequestForm);
      this.notificationsService.showNotification('Please Fill Required Fields', 'snack-bar-danger');
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.indentrequestForm.value);
    let requiredDate = new Date(this.indentrequestForm.get('requiredDate')?.value);
    _clienttemperatureForm['requiredDate'] = requiredDate.toLocaleDateString();

    this.indentrequestService.saveIndentrequest(_clienttemperatureForm).subscribe({
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

  getIndentrequestCode() {
    this.indentrequestService.getIndentrequestCode().subscribe((data: any) => {
      this.indentrequestForm.get('code')?.patchValue(data.code);
    });
  }

  getItemList(event: any) {
    var filter = event.currentTarget.value;
    this.itemService.getItemByName(filter).subscribe((data: any) => {
      this.itemList = data;
    });
  }

  onOptionSelected(event: MatAutocompleteSelectedEvent, index: number): void {
    const selectedValue = event.option.value;

    const indentRequestDetailArray = this.indentrequestForm.get('indentRequestDetail') as FormArray;

    if (!selectedValue) {
      console.error('Option value is undefined. Ensure mat-option [value] is correctly bound.');

      return;
    }

    // Get the FormArray for indentRequestDetail

    // Check if index is valid
    if (!indentRequestDetailArray || index < 0 || index >= indentRequestDetailArray.length) {
      console.error('Invalid index or FormArray is not initialized properly.');
      return;
    }

    // Check if the selected itemId already exists in the form array (excluding the current index)
    const duplicateItem = indentRequestDetailArray.controls
      .filter((control: AbstractControl, controlIndex: number) => controlIndex !== index) // Exclude the current index
      .some((control: AbstractControl) => {
        const formGroup = control as FormGroup;  // Cast AbstractControl to FormGroup
        return formGroup.get('itemId')?.value === selectedValue.id;  // Check if the itemId already exists
      });

    if (duplicateItem) {
      this.notificationsService.showNotification('This item has already been selected.', 'snack-bar-danger');
      const currentFormGroup = indentRequestDetailArray.at(index) as FormGroup;
      currentFormGroup.get('id')?.patchValue(0);
      currentFormGroup.get('indentRequestId')?.patchValue(0);
      currentFormGroup.get('itemId')?.patchValue(0);
      currentFormGroup.get('itemName')?.patchValue('');
      currentFormGroup.get('item')?.patchValue(null);
      currentFormGroup.get('required')?.patchValue(0);
      currentFormGroup.get('description')?.patchValue(0);
      // This will clear all the values of the current form group
      return; // Do nothing if the itemId already exists
    }
    else {
      // Get the selected item details from your getitem method
      const selectedItem = this.getitem(selectedValue.id);
      if (!selectedItem) {
        console.error('Selected item not found.');
        return;
      }

      // Get the form group for the current index
      const detailFormGroup = indentRequestDetailArray.at(index) as FormGroup;

      // Patch the values into the form group
      detailFormGroup.get('itemId')?.patchValue(selectedValue.id);
      detailFormGroup.get('item')?.patchValue(selectedValue);
      detailFormGroup.get('itemName')?.patchValue(selectedValue.code +':' +selectedValue.name);
    }
  }

  getitem(itemId: string) {
    return this.itemList.find((option: { id: string; }) => option.id === itemId);
  }

  displayFn(item: any): string {
    if(item != ""){

    // If the item is an object, display its code and name
    if (item && item.code && item.name) {
      return `${item.code} : ${item.name}`;
    }

    // If it's just an itemId (number), find the item in the indentRequestDetail array
    else if (typeof item === 'number' && this.isdataload == false) {
      // Find the first FormGroup where the itemId matches

      var selectedItem = this.data.element.indentRequestDetail.filter((element: any) => {
        return element.itemId == item;
      })

      // If found, return the formatted string, else return an empty string
      return selectedItem[0] ? `${selectedItem[0].item?.code} : ${selectedItem[0].item.name}` : '';
    }

    // Return empty string by default if no valid item found
    return '';
  }
  else
  return '';
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
    const indentRequestDetailArray = this.indentrequestForm.get('indentRequestDetail') as FormArray;

    // Check if index is valid
    if (!indentRequestDetailArray || index < 0 || index >= indentRequestDetailArray.length) {
      console.error('Invalid index or FormArray is not initialized properly.');
      return;
    }

    const currentFormGroup = indentRequestDetailArray.at(index) as FormGroup;
    currentFormGroup.reset();  // This will clear all the values of the current form group
    return; // Do nothing if the itemId already exists
  }

  getindentTypeList() {
    let _indentTypeFilter: any = {};
    this.indentTypeService.getAllIndentTypes(_indentTypeFilter).subscribe((data: any) => {
     this.indentTypeList = data.item1;
    });
  }

  getstoreList() {
    let _storeFilter: any = {};
    this.storeService.getAllStores(_storeFilter).subscribe((data: any) => {
     this.storeList = data.item1;
    });
  }

  reset() {
    this.indentrequestForm.get('code')?.patchValue('');
    this.indentrequestTypeList = [];
  }
}
