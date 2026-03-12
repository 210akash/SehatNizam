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
import { WarehouseTransferService } from '../warehousetransfer.service';
import { ItemService } from '../../item/item.service';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { DealershipService } from '../../order/dealership/dealership.service';
import { ProjectService } from '../../project/project.service';
import { firstValueFrom, Observable } from 'rxjs';
import { MatSelectChange } from '@angular/material/select';

@Component({
  selector: 'app-add-warehousetransfer',
  templateUrl: './add-warehousetransfer.component.html',
  styleUrl: './add-warehousetransfer.component.css',
  standalone: false,
})
export class AddWarehouseTransferComponent {
  warehousetransferForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  indentTypeList: any;
  storeList: any;
  productList: any;
  warehousetransferTypeList: any;
  itemList: any[] = [];
  isdataload: boolean = false;
  TMaterialCost!: number;
  Quantity!: number;
  distributorList: any;
  projectList: any;
  costSheetList: any;
  constructor(
    private dialog: MatDialog,
    private notificationsService: NotificationsService,
    private formBuilder: FormBuilder,
    private dealershipService: DealershipService,
    private warehousetransferService: WarehouseTransferService,
    private itemService: ItemService,
    private constantService: ConstantService,
    private projectService: ProjectService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) { }

  ngOnInit(): void {
    this.warehousetransferForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      date: [new Date(), Validators.required],
      transferFromId: [null], // Validation
      transferToId: [null, Validators.required],
      status: [''], // Validation
      statusName: ['New'], // Validation
      statusId: [1], // Validation
      remarks: [''], // Validation
      warehouseTransferDetail: this.formBuilder.array([]), // Initialize as a FormArray
    });
    this.LoadData(this.data.element);
    this.getprojectList();
  }

  get warehouseTransferDetail(): FormArray {
    return this.warehousetransferForm.get('warehouseTransferDetail') as FormArray;
  }

  addWarehouseTransferDetail(index: number) {
    const newDetailGroup = this.formBuilder.group({
      id: [0, Validators.required], // Default value
      warehouseTransferId: [0], // Default value
      itemId: ['', Validators.required], // Validation
      itemName: ['', Validators.required], // Validation
      item: ['', Validators.required], // Validation
      costSheetId: [null],
      costSheetList: [[]],
      leftQuantity: [0, [Validators.required, Validators.min(0.001)]], // Validation
      quantity: [0, [Validators.required, Validators.min(0.001)]], // Validation
      rate: [0, [Validators.required, Validators.min(0.001)]], // Validation
      amount: [0, Validators.required], // Validation
    });

    // Insert the new group after the current index
    this.warehouseTransferDetail.insert(index + 1, newDetailGroup);
  }

  removeWarehouseTransferDetail(index: number) {
    if (this.warehouseTransferDetail.length > 1) {
      this.warehouseTransferDetail.removeAt(index);
    } else {
      this.notificationsService.showNotification(
        'At least one item is required.',
        'snack-bar-danger'
      );
    }
  }

  getuomValue(index: number): any {
    const detailControl = (
      this.warehousetransferForm.get('warehouseTransferDetail') as FormArray
    ).at(index);
    return detailControl?.value?.item || '';
  }

  async LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.warehousetransferForm);

      this.warehousetransferForm
        .get('dealershipId')
        ?.patchValue(element.dealership?.id);
      this.warehousetransferForm
        .get('dealershipName')
        ?.patchValue(element.dealership?.name);
      this.warehousetransferForm.get('dealership')?.patchValue(element.dealership);

      // Populate the warehouseTransferDetail FormArray
      const detailsArray = this.warehousetransferForm.get(
        'warehouseTransferDetail'
      ) as FormArray;
      detailsArray.clear(); // Clear existing data

      var transferFromId = this.warehousetransferForm.get('transferFromId')?.value;

      if (element.warehouseTransferDetail && element.warehouseTransferDetail.length > 0) {
        element.warehouseTransferDetail.forEach(async (detail: any) => {
          if (detail.costSheetId == null) {
            (await this.getSockByWarehouse(detail.itemId, transferFromId)).subscribe((stockdata: any) => {
              const detailGroup = this.formBuilder.group({
                id: [detail.id],
                warehouseTransferId: [detail.warehouseTransferId],
                itemId: [detail.itemId, Validators.required],
                item: [detail.item ? detail.item : '', Validators.required],
                itemName: [
                  detail.item ? detail.item.code + ':' + detail.item.name : '',
                  Validators.required,
                ],
               costSheetId: [null],
               costSheetList: [[]],
                quantity: [
                  detail.quantity,
                  [Validators.required, Validators.min(0.001)],
                ],
                leftQuantity: [stockdata?.quantity ?? 0, [Validators.required, Validators.min(0.001)]],
                rate: [stockdata?.rate, [Validators.required, Validators.min(0.001)]],
                amount: [
                  (detail.quantity * stockdata?.rate).toFixed(2),
                  [Validators.required, Validators.min(1)],
                ],
              });
              detailsArray.push(detailGroup);
              this.Quantity += Number((detail.quantity).toFixed(2));
              this.TMaterialCost += Number((detail.quantity * detail.rate).toFixed(2));
            });
          }
          else {
            const stockdata: any = await this.getCostSheetByItem(detail.itemId, detail.costSheetId);

              // Find the specific cost sheet matching the current detail's costSheetId
                  const selectedCostSheet = stockdata.find(
                    (cs: any) => cs.id === detail.costSheetId
                  );

            const detailGroup = this.formBuilder.group({
              id: [detail.id],
              warehouseTransferId: [detail.warehouseTransferId],
              itemId: [detail.itemId, Validators.required],
              item: [detail.item ? detail.item : '', Validators.required],
              itemName: [
                detail.item ? detail.item.code + ':' + detail.item.name : '',
                Validators.required,
              ],
              costSheetId: [detail.costSheetId],
              costSheetList: [this.costSheetList],
              quantity: [
                detail.quantity,
                [Validators.required, Validators.min(0.001)],
              ],
              leftQuantity: [selectedCostSheet.quantity, [Validators.required, Validators.min(0.001)]],
              rate: [detail?.costSheet?.costPerPet
                , [Validators.required, Validators.min(0.001)]],
              amount: [
                (detail.quantity * (detail?.costSheet?.costPerPet
                  ?? 0)).toFixed(2),
                [Validators.required, Validators.min(1)],
              ],
            });

            detailsArray.push(detailGroup);
            this.Quantity += Number((detail.quantity).toFixed(2));
            this.TMaterialCost += Number((detail.quantity * detail.rate).toFixed(2));
          }

        });
        await this.calculateTotal();
      }
    } else {
      this.getWarehouseTransferCode();
      this.addWarehouseTransferDetail(0);
      this.warehousetransferForm
        .get('createdDate')
        ?.patchValue(this.constantService.formatDate(new Date()));
    }
  }

  SaveData() {
    if (this.warehousetransferForm.invalid) {
      this.constantService.markFormGroupTouched(this.warehousetransferForm);
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
      this.warehousetransferForm.value
    );

    this.warehousetransferService
      .saveWarehouseTransfer(_clienttemperatureForm)
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

    const warehouseTransferDetailArray = this.warehousetransferForm.get(
      'warehouseTransferDetail'
    ) as FormArray;

    if (!selectedValue) {
      console.error(
        'Option value is undefined. Ensure mat-option [value] is correctly bound.'
      );

      return;
    }

    // Get the FormArray for warehouseTransferDetail

    // Check if index is valid
    if (
      !warehouseTransferDetailArray ||
      index < 0 ||
      index >= warehouseTransferDetailArray.length
    ) {
      console.error('Invalid index or FormArray is not initialized properly.');
      return;
    }

    // Check if the selected itemId already exists in the form array (excluding the current index)
    const duplicateItem = warehouseTransferDetailArray.controls
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
      const currentFormGroup = warehouseTransferDetailArray.at(index) as FormGroup;
      currentFormGroup.get('id')?.patchValue(0);
      currentFormGroup.get('warehouseTransferId')?.patchValue(0);
      currentFormGroup.get('itemId')?.patchValue(0);
      currentFormGroup.get('itemName')?.patchValue('');
      currentFormGroup.get('item')?.patchValue(null);
      currentFormGroup.get('rate')?.patchValue(0);
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
      const detailFormGroup = warehouseTransferDetailArray.at(index) as FormGroup;
      // Patch the values into the form group
      detailFormGroup.get('itemId')?.patchValue(selectedValue.id);
      detailFormGroup.get('item')?.patchValue(selectedValue);
      detailFormGroup
        .get('itemName')
        ?.patchValue(selectedValue.code + ':' + selectedValue.name);
      detailFormGroup.get('quantity')?.patchValue(0);
      if (
        selectedItem?.itemType?.subCategory?.category?.categoryStores?.some(
          (store: { storeId: number; isActive: any; }) => store.storeId === 3 && store.isActive
        )
      ) {
        await this.getCostSheetListByItem(
          index,
          selectedItem?.id
        );
      }
      else {
        (await this.getSockByWarehouse(selectedValue.id, 0)).subscribe((data: any) => {
          detailFormGroup.get('leftQuantity')?.patchValue(data.quantity);
          detailFormGroup.get('rate')?.patchValue(data.rate);
        });
      }
    }
  }

  getIndexValue(index: number): any {
    const detailControl = (this.warehousetransferForm.get('warehouseTransferDetail') as FormArray).at(index);
    return detailControl?.value;
  }

  onCostSheetSelected(event: MatSelectChange, rowIndex: number): void {
    const selectedCostSheetId = event.value;

    const costSheetList = this.getIndexValue(rowIndex)?.costSheetList ?? [];
    const chosen = costSheetList.find((cs: { id: any }) => cs.id === selectedCostSheetId);

    const warehouseTransferDetailArray = this.warehousetransferForm.get(
      'warehouseTransferDetail'
    ) as FormArray;

    const currentFormGroup = warehouseTransferDetailArray.at(rowIndex) as FormGroup;

    // If no duplicate, set pending quantity
    currentFormGroup.get('leftQuantity')?.patchValue(
      chosen?.quantity ?? null
    );

    // If no duplicate, set pending quantity
    currentFormGroup.get('rate')?.patchValue(
      chosen?.costPerPet ?? null
    );
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
    const warehouseTransferDetailArray = this.warehousetransferForm.get(
      'warehouseTransferDetail'
    ) as FormArray;

    // Check if index is valid
    if (
      !warehouseTransferDetailArray ||
      index < 0 ||
      index >= warehouseTransferDetailArray.length
    ) {
      console.error('Invalid index or FormArray is not initialized properly.');
      return;
    }

    const currentFormGroup = warehouseTransferDetailArray.at(index) as FormGroup;
    currentFormGroup.reset(); // This will clear all the values of the current form group
    return; // Do nothing if the itemId already exists
  }

  reset() {
    this.warehousetransferForm.get('code')?.patchValue('');
    this.warehousetransferTypeList = [];
  }


  async validateQty(index: number): Promise<any> {
    const detailControl = (this.warehousetransferForm.get('warehouseTransferDetail') as FormArray).at(index);
    if (detailControl?.value?.quantity > detailControl?.value?.leftQuantity) {
      detailControl.get('quantity')?.patchValue(detailControl?.value?.leftQuantity);
    }
    detailControl.get('amount')?.patchValue((detailControl?.value.rate * detailControl?.value.quantity).toFixed(2));
    await this.calculateTotal();
  }

  async calculateTotal() {
    this.TMaterialCost = 0;
    this.Quantity = 0;

    for (const control of this.warehouseTransferDetail.controls) {
      const amount = +control.get('amount')?.value || 0;
      const quantity = +control.get('quantity')?.value || 0;

      // Simulate future async logic (e.g., fetching discount/tax from API per item)
      // await this.simulateDelay(); // optional placeholder
      this.TMaterialCost += amount;
      this.Quantity += quantity;
    }
  }

  getWarehouseTransferCode() {
    this.warehousetransferService.getWarehouseTransferCode().subscribe((data: any) => {
      this.warehousetransferForm.get('code')?.patchValue(data.code);
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

    this.warehousetransferForm.get('dealershipId')?.patchValue(selectedValue.id);
    this.warehousetransferForm.get('dealershipName')?.patchValue(selectedValue.name);
    this.warehousetransferForm.get('dealership')?.patchValue(selectedValue);
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
    const selectedtransferFromId = event.value;
    const warehouseTransferDetailArray = this.warehousetransferForm.get('warehouseTransferDetail') as FormArray;
    const detailFormGroup = warehouseTransferDetailArray.at(index) as FormGroup;
    const itemId = detailFormGroup.get('itemId')?.value;

    if (!itemId) {
      this.notificationsService.showNotification(
        'Please select item first',
        'snack-bar-danger'
      );
      return; // Prevent further execution
    }

    if (selectedtransferFromId) {
      (await this.getSockByWarehouse(itemId, selectedtransferFromId)).subscribe((data: any) => {
        detailFormGroup.get('leftQuantity')?.patchValue(data.quantity);
        detailFormGroup.get('rate')?.patchValue(data.rate);
      });
    } else {
      this.itemList = [];
    }
  }

  async getSockByWarehouse(itemId: number, transferFromId: number) {
    return await this.itemService.getSockByWarehouse(itemId, transferFromId);
  }

  async getCostSheetListByItem(index: number, itemId: any) {
    const grnDetailArray = this.warehousetransferForm.get('warehouseTransferDetail') as FormArray;
    if (grnDetailArray && grnDetailArray.at(index)) {

      const grnDetailGroup = grnDetailArray.at(index) as FormGroup;
      const costSheetControl = grnDetailGroup.get('costSheetId');
      var costSheetId = costSheetControl?.value ?? 0;

      grnDetailGroup.get('costSheetList')?.patchValue([]);

      if (itemId !== undefined) {
        var data = await this.getCostSheetByItem(itemId, 0);
        grnDetailGroup.get('costSheetList')?.patchValue(data);

        if (data && data.length > 0) {
          costSheetControl?.setValidators([Validators.required]);
        } else {
          costSheetControl?.clearValidators();
        }

        costSheetControl?.updateValueAndValidity();

      } else {
        console.error('itemId not found at the given index');
      }
    } else {
      console.error('No detail found at the given index');
    }
  }

  async getCostSheetByItem(itemId: any, costSheetId: any): Promise<any[]> {
    try {
      this.costSheetList = [];
      const data = await firstValueFrom(
        this.warehousetransferService.getPendingCostSheet(itemId, costSheetId)
      );
      this.costSheetList = data || [];
      return this.costSheetList;
    } catch (error) {
      console.error('Error loading pending indent items:', error);
      return [];
    }
  }
}
