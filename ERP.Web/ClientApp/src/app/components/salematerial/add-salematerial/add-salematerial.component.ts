import { Component, Inject } from '@angular/core';
import {
  AbstractControl,
  FormArray,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { SaleMaterialService } from '../salematerial.service';
import { ItemService } from '../../item/item.service';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { DealershipService } from '../../order/dealership/dealership.service';
import { ProjectService } from '../../project/project.service';

@Component({
  selector: 'app-add-salematerial',
  templateUrl: './add-salematerial.component.html',
  styleUrl: './add-salematerial.component.css',
  standalone: false,
})
export class AddSaleMaterialComponent {
  salematerialForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  indentTypeList: any;
  storeList: any;
  productList: any;
  salematerialTypeList: any;
  itemList: any[] = [];
  isdataload: boolean = false;
  TMaterialCost!: number;
  distributorList: any;
  projectList: any;
  constructor(
    private dialog: MatDialog,
    private notificationsService: NotificationsService,
    private formBuilder: FormBuilder,
    private dealershipService: DealershipService,
    private salematerialService: SaleMaterialService,
    private itemService: ItemService,
    private constantService: ConstantService,
    private projectService: ProjectService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) {}

  ngOnInit(): void {
    this.salematerialForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      date: [new Date(), Validators.required],
      dealership: [null, Validators.required],
      dealershipId: [null, Validators.required],
      dealershipName: ['', Validators.required],
      status: [''], // Validation
      statusName: ['New'], // Validation
      statusId: [1], // Validation
      remarks: [''], // Validation
      saleMaterialDetail: this.formBuilder.array([]), // Initialize as a FormArray
    });
    this.LoadData(this.data.element);
        this.getprojectList();
  }

  get saleMaterialDetail(): FormArray {
    return this.salematerialForm.get('saleMaterialDetail') as FormArray;
  }

  addSaleMaterialDetail(index: number) {
    const newDetailGroup = this.formBuilder.group({
      id: [0, Validators.required], // Default value
      saleMaterialId: [0], // Default value
      itemId: ['', Validators.required], // Validation
      itemName: ['', Validators.required], // Validation
      item: ['', Validators.required], // Validation
      projectId: [Validators.required], // Validation
      leftQuantity: [0, [Validators.required, Validators.min(0.001)]], // Validation
      quantity: [0, [Validators.required, Validators.min(0.001)]], // Validation
      rate: [0, [Validators.required, Validators.min(0.001)]], // Validation
      amount: [0,Validators.required], // Validation
    });

    // Insert the new group after the current index
    this.saleMaterialDetail.insert(index + 1, newDetailGroup);
  }

  removeSaleMaterialDetail(index: number) {
    if (this.saleMaterialDetail.length > 1) {
      this.saleMaterialDetail.removeAt(index);
    } else {
      this.notificationsService.showNotification(
        'At least one item is required.',
        'snack-bar-danger'
      );
    }
  }

  getuomValue(index: number): any {
    const detailControl = (
      this.salematerialForm.get('saleMaterialDetail') as FormArray
    ).at(index);
    return detailControl?.value?.item || '';
  }

  async LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.salematerialForm);

      this.salematerialForm
        .get('dealershipId')
        ?.patchValue(element.dealership?.id);
      this.salematerialForm
        .get('dealershipName')
        ?.patchValue(element.dealership?.name);
      this.salematerialForm.get('dealership')?.patchValue(element.dealership);

      // Populate the saleMaterialDetail FormArray
      const detailsArray = this.salematerialForm.get(
        'saleMaterialDetail'
      ) as FormArray;
      detailsArray.clear(); // Clear existing data

      if (element.saleMaterialDetail && element.saleMaterialDetail.length > 0) {
        element.saleMaterialDetail.forEach(async (detail: any) => {
          (await this.getSockByWarehouse(detail.itemId, 0)).subscribe((stockdata: any) => {
            const detailGroup = this.formBuilder.group({
              id: [detail.id],
              saleMaterialId: [detail.saleMaterialId],
              itemId: [detail.itemId, Validators.required],
              item: [detail.item ? detail.item : '', Validators.required],
              itemName: [
                detail.item ? detail.item.code + ':' + detail.item.name : '',
                Validators.required,
              ],
              quantity: [
                detail.quantity,
                [Validators.required, Validators.min(0.001)],
              ],
              projectId: [detail.projectId, Validators.required],
              leftQuantity: [stockdata?.quantity ?? 0, [Validators.required, Validators.min(0.001)]],
              rate: [stockdata?.rate, [Validators.required, Validators.min(0.001)]],
              amount: [
                (detail.quantity * detail.rate).toFixed(2),
                [Validators.required, Validators.min(1)],
              ],
            });
            detailsArray.push(detailGroup);
            this.TMaterialCost += Number((detail.quantity * detail.rate).toFixed(2));
          });
        });
       await  this.calculateTotal();
      }
    } else {
      this.getSaleMaterialCode();
      this.addSaleMaterialDetail(0);
      this.salematerialForm
        .get('createdDate')
        ?.patchValue(this.constantService.formatDate(new Date()));
    }
  }

  SaveData() {
    if (this.salematerialForm.invalid) {
      this.constantService.markFormGroupTouched(this.salematerialForm);
      this.notificationsService.showNotification(
        'Please Fill Required Fields',
        'snack-bar-danger'
      );
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(
      _clienttemperatureForm,
      this.salematerialForm.value
    );

    this.salematerialService
      .saveSaleMaterial(_clienttemperatureForm)
      .subscribe({
        next: (data: { Status: number; Data: string }) => {
          if (data.Status == 200) {
            this.notificationsService.showNotification(
              data.Data,
              'snack-bar-success'
            );
            this.dialog.closeAll();
          } else
            this.notificationsService.showNotification(
              data.Data,
              'snack-bar-danger'
            );
          this.isLoading = false;
        },
        error: (error: string) => {
          this.notificationsService.showNotification(error, 'snack-bar-danger');
          console.error(error);
          this.isLoading = false;
        },
      });
  }

  getItemList(event: any) {
    var filter = event.currentTarget.value;
    this.itemService.getItemByName(filter, 0).subscribe((data: any) => {
      this.itemList = data;
    });
  }

  async onOptionSelected(event: MatAutocompleteSelectedEvent, index: number): Promise<void> {
    const selectedValue = event.option.value;

    const saleMaterialDetailArray = this.salematerialForm.get(
      'saleMaterialDetail'
    ) as FormArray;

    if (!selectedValue) {
      console.error(
        'Option value is undefined. Ensure mat-option [value] is correctly bound.'
      );

      return;
    }

    // Get the FormArray for saleMaterialDetail

    // Check if index is valid
    if (
      !saleMaterialDetailArray ||
      index < 0 ||
      index >= saleMaterialDetailArray.length
    ) {
      console.error('Invalid index or FormArray is not initialized properly.');
      return;
    }

    // Check if the selected itemId already exists in the form array (excluding the current index)
    const duplicateItem = saleMaterialDetailArray.controls
      .filter(
        (control: AbstractControl, controlIndex: number) =>
          controlIndex !== index
      ) // Exclude the current index
      .some((control: AbstractControl) => {
        const formGroup = control as FormGroup; // Cast AbstractControl to FormGroup
        return formGroup.get('itemId')?.value === selectedValue.id; // Check if the itemId already exists
      });

    if (duplicateItem) {
      this.notificationsService.showNotification(
        'This item has already been selected.',
        'snack-bar-danger'
      );
      const currentFormGroup = saleMaterialDetailArray.at(index) as FormGroup;
      currentFormGroup.get('id')?.patchValue(0);
      currentFormGroup.get('saleMaterialId')?.patchValue(0);
      currentFormGroup.get('itemId')?.patchValue(0);
      currentFormGroup.get('itemName')?.patchValue('');
      currentFormGroup.get('item')?.patchValue(null);
      currentFormGroup.get('leftQuantity')?.patchValue(0);
      currentFormGroup.get('quantity')?.patchValue(0);
      // This will clear all the values of the current form group
      return; // Do nothing if the itemId already exists
    } else {
      // Get the selected item details from your getitem method
      const selectedItem = this.getitem(selectedValue.id);
      if (!selectedItem) {
        console.error('Selected item not found.');
        return;
      }

      // Get the form group for the current index
      const detailFormGroup = saleMaterialDetailArray.at(index) as FormGroup;

      // Patch the values into the form group
      detailFormGroup.get('id')?.patchValue(0);
      detailFormGroup.get('saleMaterialId')?.patchValue(0);
      detailFormGroup.get('itemId')?.patchValue(selectedValue.id);
      detailFormGroup.get('item')?.patchValue(selectedValue);
      detailFormGroup
        .get('itemName')
        ?.patchValue(selectedValue.code + ':' + selectedValue.name);
      detailFormGroup.get('quantity')?.patchValue(0);
      (await this.getSockByWarehouse(selectedValue.id, 0)).subscribe((data: any) => {
      detailFormGroup.get('leftQuantity')?.patchValue(data.quantity);
      detailFormGroup.get('rate')?.patchValue(data.rate);
    });

    }
  }

  getitem(itemId: string) {
    return this.itemList.find((option: { id: string }) => option.id === itemId);
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
    const saleMaterialDetailArray = this.salematerialForm.get(
      'saleMaterialDetail'
    ) as FormArray;

    // Check if index is valid
    if (
      !saleMaterialDetailArray ||
      index < 0 ||
      index >= saleMaterialDetailArray.length
    ) {
      console.error('Invalid index or FormArray is not initialized properly.');
      return;
    }

    const currentFormGroup = saleMaterialDetailArray.at(index) as FormGroup;
    currentFormGroup.reset(); // This will clear all the values of the current form group
    return; // Do nothing if the itemId already exists
  }

  reset() {
    this.salematerialForm.get('code')?.patchValue('');
    this.salematerialTypeList = [];
  }


   async validateQty(index: number): Promise<any> {
    const detailControl = (this.salematerialForm.get('saleMaterialDetail') as FormArray).at(index);
    if (detailControl?.value?.quantity  > detailControl?.value?.leftQuantity) {
      detailControl.get('quantity')?.patchValue(detailControl?.value?.leftQuantity);
    }
      detailControl.get('amount')?.patchValue((detailControl?.value.rate * detailControl?.value.quantity).toFixed(2));
    await   this.calculateTotal();
  }

async calculateTotal() {
  this.TMaterialCost = 0;

  for (const control of this.saleMaterialDetail.controls) {
    const amount = +control.get('amount')?.value || 0;

    // Simulate future async logic (e.g., fetching discount/tax from API per item)
    // await this.simulateDelay(); // optional placeholder
    this.TMaterialCost += amount;
  }
}

  getSaleMaterialCode() {
    this.salematerialService.getSaleMaterialCode().subscribe((data: any) => {
      this.salematerialForm.get('code')?.patchValue(data.code);
    });
  }

  async getCustomersList(event: any) {
    const filter = event.currentTarget.value;
    this.distributorList = []; // Empty the list before updating
    (await this.dealershipService.getCustomerByName(filter)).subscribe(
      (data: any) => {
    
        this.distributorList = data || []; // Ensure it's an array even if no data is returned
      },
      (error: any) => {
        console.error('Error fetching distributor list:', error);
        this.distributorList = []; // Reset in case of an error
      }
    );
  }

  onDistributorSelected(event: MatAutocompleteSelectedEvent): void {
    const selectedValue = event.option.value;

    if (!selectedValue) {
      console.error(
        'Option value is undefined. Ensure mat-option [value] is correctly bound.'
      );
      return;
    }

    const selectedItem = this.getDistributor(selectedValue.id);
    if (!selectedItem) {
      console.error('Selected item not found.');
      return;
    }

    this.salematerialForm.get('dealershipId')?.patchValue(selectedValue.id);
    this.salematerialForm.get('dealershipName')?.patchValue(selectedValue.name);
    this.salematerialForm.get('dealership')?.patchValue(selectedValue);
  }

  getDistributor(distributorId: any) {
    return this.distributorList.find(
      (option: { id: any }) => option.id === distributorId
    );
  }

  getprojectList() {
    let _projectFilter: any = {};
    this.projectService.getAllProjects(_projectFilter).subscribe((data: any) => {
      this.projectList = data.item1;
    });
  }

  async onProjectChange(event: any, index: number) {
  const selectedProjectId = event.value;
  const saleMaterialDetailArray = this.salematerialForm.get('saleMaterialDetail') as FormArray;
  const detailFormGroup = saleMaterialDetailArray.at(index) as FormGroup;
  const itemId = detailFormGroup.get('itemId')?.value;

  if (!itemId) {
    this.notificationsService.showNotification(
      'Please select item first',
      'snack-bar-danger'
    );
    return; // Prevent further execution
  }

   if (selectedProjectId) {
  (await this.getSockByWarehouse(itemId, selectedProjectId)).subscribe((data: any) => {
      detailFormGroup.get('stock')?.patchValue(data.quantity);
    });
  } else {
    this.itemList = [];
  }
}

async getSockByWarehouse(itemId: number, projectId: number) {
   return await this.itemService.getSockByWarehouse(itemId, projectId);
}

}
