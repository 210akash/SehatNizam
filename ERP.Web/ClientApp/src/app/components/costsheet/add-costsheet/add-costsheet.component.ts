import { Component, Inject } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { CostSheetService } from '../costsheet.service';
import { CategoryService } from '../../category/category.service';
import { ItemService } from '../../item/item.service';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { IndentTypeService } from '../../indenttype/indenttype.service';
import { StoreService } from '../../store/store.service';
import { ProjectService } from '../../project/project.service';

@Component({
  selector: 'app-add-costsheet',
  templateUrl: './add-costsheet.component.html',
  styleUrl: './add-costsheet.component.css',
  standalone: false
})

export class AddCostSheetComponent {
  costsheetForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  indentTypeList : any;
  storeList : any;
  productList : any;
  costsheetTypeList: any;
  itemList: any[] = [];
  isdataload: boolean = false;
  TMaterialCost! : number;
  TFillingPerPet!: number;
  TCostOfProduction! : number;
  CostPerPet! : number;
  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private indentTypeService: IndentTypeService, private storeService: StoreService, private costsheetService: CostSheetService,private projectService: ProjectService, private categoryService: CategoryService, private itemService: ItemService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.costsheetForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      createdDate: [new Date(), Validators.required],
      itemId: [0, Validators.required],
      quantity: [0, [Validators.required, Validators.min(1)]], // Validation
      tollFillRate : [0, [Validators.required, Validators.min(1)]], // Validation
      advSaleTaxPer : [0, [Validators.required, Validators.min(0)]], // Validation
      advSaleTaxAmt : [0, [Validators.required, Validators.min(0)]], // Validation
      advFEDPer : [0, [Validators.required, Validators.min(0)]], // Validation
      advFEDAmt : [0, [Validators.required, Validators.min(0)]], // Validation
      tmaterialCost : [0, [Validators.required, Validators.min(0)]], // Validation
      tfillingPerPet : [0, [Validators.required, Validators.min(0)]], // Validation
      costPerPet : [0, [Validators.required, Validators.min(0)]], // Validation
      status: [''], // Validation
      statusName: ['New'], // Validation
      statusId: [1], // Validation
      costSheetDetail: this.formBuilder.array([]) // Initialize as a FormArray
    });
    this.LoadData(this.data.element);
    this.getProductList();
  }

  get costSheetDetail(): FormArray {
    return this.costsheetForm.get('costSheetDetail') as FormArray;
  }

  addCostSheetDetail(index: number) {
    const newDetailGroup = this.formBuilder.group({
      id: [0], // Default value
      costSheetId: [0], // Default value
      itemId: ['', Validators.required], // Validation
      itemName: ['', Validators.required], // Validation
      item: ['', Validators.required], // Validation
      quantity: [0, [Validators.required, Validators.min(0.001)]], // Validation
      rate :  [0, [Validators.required, Validators.min(0.001)]], // Validation
      amount: [0], // Validation

    });

    // Insert the new group after the current index
    this.costSheetDetail.insert(index + 1, newDetailGroup);
  }

  removeCostSheetDetail(index: number) {
    if (this.costSheetDetail.length > 1) {
      this.costSheetDetail.removeAt(index);
    } else {
      this.notificationsService.showNotification('At least one item is required.', 'snack-bar-danger');
    }
  }

  getuomValue(index: number): any {
    const detailControl = (this.costsheetForm.get('costSheetDetail') as FormArray).at(index);
    return detailControl?.value?.item?.uom?.name || '';
  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.costsheetForm);

      // Populate the costSheetDetail FormArray
      const detailsArray = this.costsheetForm.get('costSheetDetail') as FormArray;
      detailsArray.clear(); // Clear existing data

      if (element.costSheetDetail && element.costSheetDetail.length > 0) {
        element.costSheetDetail.forEach((detail: any) => {
          const detailGroup = this.formBuilder.group({
            id: [detail.id],
            costSheetId: [detail.costSheetId],
            itemId: [detail.itemId, Validators.required],
            item: [detail.item ? detail.item : '', Validators.required],
            itemName: [detail.item ? detail.item.code + ':' +  detail.item.name : '', Validators.required],
            quantity: [detail.quantity, [Validators.required, Validators.min(0.001)]],
            rate: [detail.rate, [Validators.required, Validators.min(1)]],
            amount: [detail.quantity * detail.rate, [Validators.required, Validators.min(1)]]
          });
          detailsArray.push(detailGroup);
        });
      }
      this.calculateTotal();
    }
    else {
      this.addCostSheetDetail(0);
      this.costsheetForm.get('createdDate')?.patchValue(this.constantService.formatDate(new Date()));
    }
  }

  SaveData() {
    if (this.costsheetForm.invalid) {
      this.constantService.markFormGroupTouched(this.costsheetForm);
      this.notificationsService.showNotification('Please Fill Required Fields', 'snack-bar-danger');
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.costsheetForm.value);

    this.costsheetService.saveCostSheet(_clienttemperatureForm).subscribe({
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

  getProductList() {
      this.itemService.getItemByName('',3).subscribe((data: any) => {
        this.productList = data;
      });
  }

  getItemList(event: any) {
      var filter = event.currentTarget.value;
      this.itemService.getItemByName(filter,0).subscribe((data: any) => {
        this.itemList = data;
      });
  }

  onOptionSelected(event: MatAutocompleteSelectedEvent, index: number): void {
    const selectedValue = event.option.value;

    const costSheetDetailArray = this.costsheetForm.get('costSheetDetail') as FormArray;

    if (!selectedValue) {
      console.error('Option value is undefined. Ensure mat-option [value] is correctly bound.');

      return;
    }

    // Get the FormArray for costSheetDetail

    // Check if index is valid
    if (!costSheetDetailArray || index < 0 || index >= costSheetDetailArray.length) {
      console.error('Invalid index or FormArray is not initialized properly.');
      return;
    }

    // Check if the selected itemId already exists in the form array (excluding the current index)
    const duplicateItem = costSheetDetailArray.controls
      .filter((control: AbstractControl, controlIndex: number) => controlIndex !== index) // Exclude the current index
      .some((control: AbstractControl) => {
        const formGroup = control as FormGroup;  // Cast AbstractControl to FormGroup
        return formGroup.get('itemId')?.value === selectedValue.id;  // Check if the itemId already exists
      });

    if (duplicateItem) {
      this.notificationsService.showNotification('This item has already been selected.', 'snack-bar-danger');
      const currentFormGroup = costSheetDetailArray.at(index) as FormGroup;
      currentFormGroup.get('id')?.patchValue(0);
      currentFormGroup.get('costSheetId')?.patchValue(0);
      currentFormGroup.get('itemId')?.patchValue(0);
      currentFormGroup.get('itemName')?.patchValue('');
      currentFormGroup.get('item')?.patchValue(null);
      currentFormGroup.get('quantity')?.patchValue(0);
      currentFormGroup.get('rate')?.patchValue(0);
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
      const detailFormGroup = costSheetDetailArray.at(index) as FormGroup;

      // Patch the values into the form group
      detailFormGroup.get('itemId')?.patchValue(selectedValue.id);
      detailFormGroup.get('item')?.patchValue(selectedValue);
      detailFormGroup.get('itemName')?.patchValue(selectedValue.code +':' +selectedValue.name);
      detailFormGroup.get('rate')?.patchValue(selectedValue.rate);
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

    // If it's just an itemId (number), find the item in the costSheetDetail array
    else if (typeof item === 'number' && this.isdataload == false) {
      // Find the first FormGroup where the itemId matches

      var selectedItem = this.data.element.costSheetDetail.filter((element: any) => {
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
    const inputValue = (event.target as HTMLInputElement)?.value;
    console.log('Current Input Value:', inputValue); // Debugging output

    if (!inputValue.trim()) {
      console.log(`Input cleared at row index: ${index}`);
      this.resetitem(index); // Call a function when cleared
    }
  }

  resetitem(index: number) {
    const costSheetDetailArray = this.costsheetForm.get('costSheetDetail') as FormArray;

    // Check if index is valid
    if (!costSheetDetailArray || index < 0 || index >= costSheetDetailArray.length) {
      console.error('Invalid index or FormArray is not initialized properly.');
      return;
    }

    const currentFormGroup = costSheetDetailArray.at(index) as FormGroup;
    currentFormGroup.reset();  // This will clear all the values of the current form group
    return; // Do nothing if the itemId already exists
  }

  reset() {
    this.costsheetForm.get('code')?.patchValue('');
    this.costsheetTypeList = [];
  }

    validateQty(index: number): any {
      const detailControl = (this.costsheetForm.get('costSheetDetail') as FormArray).at(index);
      var amount  = (detailControl?.value.rate * detailControl?.value.quantity).toFixed(2);
        detailControl.get('amount')?.patchValue(amount);
        this.calculateTotal();
    }

    calculateTotal() {
      this.TMaterialCost = 0;
      this.TFillingPerPet = 0;
      this.TCostOfProduction = 0;
      this.CostPerPet = 0;
      this.costSheetDetail.controls.forEach(control => {
      const amount = +control.get('amount')?.value || 0;
      this.TMaterialCost += amount;
      });
      this.TMaterialCost = +this.TMaterialCost.toFixed(2);
       this.costsheetForm.get('tmaterialCost')?.patchValue(this.TMaterialCost);
       this.TFillingPerPet = +(this.costsheetForm.get('quantity')?.value * this.costsheetForm.get('tollFillRate')?.value).toFixed(2);
      this.costsheetForm.get('tfillingPerPet')?.patchValue(this.TFillingPerPet);
      this.TCostOfProduction = this.TMaterialCost + this.TFillingPerPet;
      this.CostPerPet = +(this.TCostOfProduction / this.costsheetForm.get('quantity')?.value).toFixed(2);
      this.costsheetForm.get('costPerPet')?.patchValue(this.CostPerPet);
      this.costsheetForm.get('advSaleTaxAmt')?.patchValue((this.TFillingPerPet * this.costsheetForm.get('advSaleTaxPer')?.value) / 100);
      this.costsheetForm.get('advFEDAmt')?.patchValue((this.TFillingPerPet * this.costsheetForm.get('advFEDPer')?.value) / 100);
    }
}
