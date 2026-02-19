import { Component, Inject } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { ItemService } from '../../item/item.service';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { IndentTypeService } from '../../indenttype/indenttype.service';
import { StoreService } from '../../store/store.service';
import { PurchaseDemandService } from '../purchasedemand.service';
import { IndentrequestService } from '../../indentrequest/indentrequest.service';
import { PriorityService } from '../../priority/priority.service';
import { LocationService } from '../../location/location.service';
import { MatOptionSelectionChange } from '@angular/material/core';
import { firstValueFrom } from 'rxjs';
import { DepartmentService } from '../../department/department.service';
import { ProjectService } from '../../project/project.service';

@Component({
  selector: 'app-add-purchasedemand',
  templateUrl: './add-purchasedemand.component.html',
  styleUrl: './add-purchasedemand.component.css',
  standalone: false
})

export class AddPurchaseDemandComponent {
  purchasedemandForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  indentRequestList : any;
  departmentList : any;
  priorityList:any;
  indentTypeList : any;
  locationList : any;
  projectList : any;
  pendingindentItemList : any;
  itemList: any[] = [];
  isdataload: boolean = false;
  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder,private indentTypeService : IndentTypeService, private indentrequestService: IndentrequestService, private priorityService: PriorityService, private locationService: LocationService,private projectService: ProjectService, private purchasedemandService: PurchaseDemandService,  private itemService: ItemService,private departmentService: DepartmentService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.purchasedemandForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      createdDate: [new Date()],
      priorityId: [0, Validators.required],
      indentRequestId: [0, Validators.required],
      requestNo: ['', Validators.required],
      requestDate: [new Date(), Validators.required],
    
      indentTypeId :  [0, Validators.required],
      locationId: [0, Validators.required],
      status: [''], // Validation
      statusName: ['New'], // Validation
      statusId: [1], // Validation
      remarks: [''], // Validation,
      purchaseDemandDetail: this.formBuilder.array([]) // Initialize as a FormArray
    });
    this.LoadData(this.data.element);
    this.getindentList();
    this.getpriorityList();
    this.getlocationList();
    this.getdepartmentList();
    this.getindentTypeList();
    this.getprojectList();
  }

  get purchaseDemandDetail(): FormArray {
    return this.purchasedemandForm.get('purchaseDemandDetail') as FormArray;
  }

  addPurchaseDemandDetail(index: number) {
    const newDetailGroup = this.formBuilder.group({
      id: [0], // Default value
      purchaseDemandId: [0], // Default value
      purchaseDemandDetailId: [0], // Default value
      projectId : [0, Validators.required], // Default value
      departmentId: [0, Validators.required], // Default value
      itemId: ['', Validators.required], // Validation
      itemName: ['', Validators.required], // Validation
      item: ['', Validators.required], // Validation
      demandQty: [0, [Validators.required, Validators.min(1)]], // Validation
      requiredDate: [new Date()], // Validation,
      description: [''], // Validation,
    }
  );

    // Insert the new group after the current index
    this.purchaseDemandDetail.insert(index + 1, newDetailGroup);
  }

  removePurchaseDemandDetail(index: number) {
    if (this.purchaseDemandDetail.length > 1) {
      this.purchaseDemandDetail.removeAt(index);
    } else {
      this.notificationsService.showNotification('At least one item is required.', 'snack-bar-danger');
    }
  }

  getIndexValue(index: number): any {
    const detailControl = (this.purchasedemandForm.get('purchaseDemandDetail') as FormArray).at(index);
    return detailControl?.value;
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
      this.constantService.LoadData(element, this.purchasedemandForm);

      // Populate the indentRequestDetail FormArray
      const detailsArray = this.purchasedemandForm.get('purchaseDemandDetail') as FormArray;
      detailsArray.clear(); // Clear existing data

      if (element.purchaseDemandDetail && element.purchaseDemandDetail.length > 0) {
        element.purchaseDemandDetail.forEach((detail: any) => {
          const detailGroup = this.formBuilder.group({
            id: [detail.id],
            purchaseDemandId: [detail.id],
            itemId: [detail.itemId, Validators.required],
            item: [detail.item ? detail.item : '', Validators.required],
            itemName: [detail.item ? detail.item.code + ':' +  detail.item.name : '', Validators.required],
            requiredDate : [detail.requiredDate, Validators.required],
            demandQty: [detail.demandQty, [Validators.required, Validators.min(1)]],
            projectId: [detail.projectId, Validators.required],
            departmentId: [detail.departmentId, Validators.required],
            description: [detail.description]
          });
          detailsArray.push(detailGroup);
        });
      }
    }
    else {
      this.getPurchaseDemandCode();
      this.purchasedemandForm.get('createdDate')?.patchValue(this.constantService.formatDate(new Date()));
      this.addPurchaseDemandDetail(0);
    }
  }

async LoadData1(element: any) {
  if (element != null) {
    this.isEditMode = true;
    
    // Set the indentRequestId before loading items
    this.purchasedemandForm.get('indentRequestId')?.patchValue(element.indentRequestId);
    
    try {
      // Ensure pendingindentItemList is populated first
      await this.getPendingindentItemList();
      
      // After pendingindentItemList is populated, apply LoadData
      this.constantService.LoadData(element, this.purchasedemandForm);
      
      // Now that pendingindentItemList is loaded, populate the FormArray
      if (this.pendingindentItemList && this.pendingindentItemList.length > 0) {
        const detailsArray = this.purchasedemandForm.get('purchaseDemandDetail') as FormArray;
        detailsArray.clear(); // Clear existing data

        if (element.purchaseDemandDetail && element.purchaseDemandDetail.length > 0) {
          element.purchaseDemandDetail.forEach((detail: any) => {
            // Make sure pendingindentItemList is valid
            const selectedItem = this.pendingindentItemList.find((item: any) => item.id === detail.indentRequestDetailId);
            
            if (selectedItem) {
              const detailGroup = this.formBuilder.group({
                id: [detail.id],
                purchaseDemandId: [detail.purchaseDemandId],
                indentRequestDetailId: [detail.indentRequestDetailId],
                item: [selectedItem, Validators.required], // Bind the full item object to 'item'
                itemName: [selectedItem , Validators.required],
                required: [detail.indentRequestDetail.required + detail.indentRequestDetail.demandQty , [Validators.required, Validators.min(1)]],
                requiredDate : [detail.requiredDate, Validators.required],
                demandQty: [detail.demandQty, [Validators.required, Validators.min(1)]],
                itemDescription: [detail.itemDescription || ''],
                description: [detail.description || '']
              });
              detailsArray.push(detailGroup);
            } else {
              console.error('Selected item not found for itemId:', detail.itemId);
            }
          });
        }
      } else {
        console.error('pendingindentItemList is empty or undefined');
      }
    } catch (error) {
      console.error('Error loading pending indent items:', error);
    }
  } else {
    // Handle the case when element is null (create new Purchase Demand)
    this.getPurchaseDemandCode();
    this.purchasedemandForm.get('createdDate')?.patchValue(this.constantService.formatDate(new Date()));
    this.addPurchaseDemandDetail(0);
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
    if (this.purchasedemandForm.invalid) {
      // Mark all controls as touched to trigger validation messages
      this.constantService.markFormGroupTouched(this.purchasedemandForm);
    
      // Check each control in the FormGroup to see which one is invalid
      this.checkInvalidControls(this.purchasedemandForm);
    
      this.notificationsService.showNotification('Please Fill Required Fields', 'snack-bar-danger');
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.purchasedemandForm.value);
    let requiredDate = new Date(this.purchasedemandForm.get('requiredDate')?.value);
    _clienttemperatureForm['requiredDate'] = requiredDate.toLocaleDateString();

    this.purchasedemandService.savePurchaseDemand(_clienttemperatureForm).subscribe({
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

  getPurchaseDemandCode() {
    this.purchasedemandService.getPurchaseDemandCode().subscribe((data: any) => {
      this.purchasedemandForm.get('code')?.patchValue(data.code);
    });
  }
  
  isUpdating = false; // Flag to prevent recursive calls

  onOptionSelected(event: MatAutocompleteSelectedEvent, index: number): void {
    const selectedValue = event.option.value;

    const indentRequestDetailArray = this.purchasedemandForm.get('purchaseDemandDetail') as FormArray;

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
      detailFormGroup.get('itemName')?.patchValue(selectedValue.code +':' +selectedValue.name
      );
    }
  }

  validateQty(index: number): any {
    const detailControl = (this.purchasedemandForm.get('purchaseDemandDetail') as FormArray).at(index);
    if(detailControl?.value.demandQty > detailControl?.value.required)
    {
      detailControl.get('demandQty')?.patchValue(detailControl?.value.required);
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

    // If it's just an itemId (number), find the item in the purchaseDemandDetail array
    else if (typeof item === 'number' && this.isdataload == false) {
      // Find the first FormGroup where the itemId matches

      var selectedItem = this.data.element.purchaseDemandDetail.filter((element: any) => {
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
    const purchaseDemandDetailArray = this.purchasedemandForm.get('purchaseDemandDetail') as FormArray;

    // Check if index is valid
    if (!purchaseDemandDetailArray || index < 0 || index >= purchaseDemandDetailArray.length) {
      console.error('Invalid index or FormArray is not initialized properly.');
      return;
    }

    const currentFormGroup = purchaseDemandDetailArray.at(index) as FormGroup;
    currentFormGroup.reset();  // This will clear all the values of the current form group
    return; // Do nothing if the itemId already exists
  }

  getindentList() {
    this.purchasedemandService.getPendingIndentRequest(this.purchasedemandForm.get('indentRequestId')?.value).subscribe((data: any) => {
     this.indentRequestList = data;
    });
  }

  getdepartmentList() {
    let _departmentFilter: any = {};
    this.departmentService.getAllDepartments(_departmentFilter).subscribe((data: any) => {
     this.departmentList = data.item1;
    });
  }
  
  getindentTypeList() {
    let _indentTypeFilter: any = {};
    this.indentTypeService.getAllIndentTypes(_indentTypeFilter).subscribe((data: any) => {
     this.indentTypeList = data.item1;
    });
  }

  getpriorityList() {
    let _priorityFilter: any = {};
    this.priorityService.getAllPrioritys(_priorityFilter).subscribe((data: any) => {
     this.priorityList = data.item1;
    });
  }

  getlocationList() {
    let _locationFilter: any = {};
    this.locationService.getAllLocations(_locationFilter).subscribe((data: any) => {
     this.locationList = data.item1;
    });
  }

  getprojectList() {
    let _projectFilter: any = {};
    this.projectService.getAllProjects(_projectFilter).subscribe((data: any) => {
     this.projectList = data.item1;
    });
  }

  async getPendingindentItemList() {
    try {
      const indentRequestId = this.purchasedemandForm.get('indentRequestId')?.value;
      const id = this.isEditMode == true ? this.data?.element?.id : 0;

      // Use firstValueFrom to convert the observable to a promise
      const data = await firstValueFrom(await this.purchasedemandService.getPendingIndentItems(indentRequestId,id));
      this.pendingindentItemList = data || [];
    } catch (error) {
      console.error('Error loading pending indent items:', error);
    }
  }

  removeAllPurchaseDemandDetails() {
    // Check if there are any rows
    if (this.purchaseDemandDetail.length > 0) {
      // Clear all items in the FormArray
      this.purchaseDemandDetail.clear();
      this.addPurchaseDemandDetail(0);
    } else {
      this.notificationsService.showNotification('No items to remove.', 'snack-bar-warning');
    }
  }

  onitemSelected(event: any, index: number): void {


  }

  reset() {
    this.purchasedemandForm.get('code')?.patchValue('');
  }
}


export function demandQtyLessThanRequired(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const formGroup = control as any; // This allows access to the entire form group
    const demandQty = formGroup.get('demandQty')?.value;
    const required = formGroup.get('required')?.value;

    if (demandQty > required) {
      return { 'demandQtyExceedsRequired': true };
    }
    return null;
  };
}