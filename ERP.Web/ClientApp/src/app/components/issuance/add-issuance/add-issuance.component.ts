import { Component, Inject } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { IssuanceService } from '../issuance.service';
import { MatSelectChange } from '@angular/material/select';
import { LedgerService } from '../../ledger/ledger.service';
import { firstValueFrom } from 'rxjs';
import { ItemService } from '../../item/item.service';
import { ProjectService } from '../../project/project.service';
import { AccountService } from '../../account/account.service';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { DispatchService } from '../../dispatch/dispatch.service';

@Component({
  selector: 'app-add-issuance',
  templateUrl: './add-issuance.component.html',
  styleUrl: './add-issuance.component.css',
  standalone: false
})

export class AddIssuanceComponent {
  issuanceForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  indentRequestList: any[] = [];
  itemList: any[] = [];
  isdataload: boolean = false;
  selectedIndent: any;
  projectList: any;
  accountList: any[] = [];

  constructor(private ledgerService: LedgerService, private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private issuanceService: IssuanceService,
    private itemService: ItemService, private projectService: ProjectService,
    private accountService: AccountService, private dispatchService: DispatchService,
    private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.issuanceForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      date: [new Date(), Validators.required],
      indentRequestId: [0, Validators.required],
      accountId: [null, Validators.required],
      accountName: ['', Validators.required],
      account: [null, Validators.required], // Validation
      status: [''],
      statusName: ['New'],
      statusId: [1],
      remarks: [''],
      issuanceDetail: this.formBuilder.array([])
    });
    this.LoadData(this.data.element);
    this.getprojectList();
  }

  get issuanceDetail(): FormArray {
    return this.issuanceForm.get('issuanceDetail') as FormArray;
  }

  addIssuanceDetail(index: number) {
    const newDetailGroup = this.formBuilder.group({
      id: [0],
      issuanceId: [0],
      indentRequestDetailId: [0, Validators.required],
      itemId: [0],
      costSheetId: [0],
      costSheetList: [[]],
      projectId: [Validators.required], // Validation
      required: [0, Validators.required],
      quantity: [0.000, [Validators.required, Validators.min(0.000)]], // Validation
      rate: [0, Validators.required],
      stock: [null, Validators.required],
    });

    this.issuanceDetail.insert(index + 1, newDetailGroup);
  }

  removeIssuanceDetail(index: number) {
    if (this.issuanceDetail.length > 1) {
      this.issuanceDetail.removeAt(index);
    } else {
      this.notificationsService.showNotification(
        'At least one item is required.',
        'snack-bar-danger'
      );
    }
  }

  getIndexValue(index: number): any {
    const detailControl = (this.issuanceForm.get('issuanceDetail') as FormArray).at(index);
    return detailControl?.value || '';
  }

  async LoadData(element: any) {
    if (element != null) {
      this.getPendingIndentRequest(this.data.element.indentRequestId);
      this.isEditMode = true;
      this.constantService.LoadData(element, this.issuanceForm);
      this.issuanceForm.get('date')?.patchValue(element?.date.toLocaleString());
      this.issuanceForm.get('accountId')?.patchValue(element.account?.id);
      this.issuanceForm.get('accountName')?.patchValue(element.account?.code + ' : ' + element.account?.name);
      this.issuanceForm.get('account')?.patchValue(element.account);
      await this.getPendingIndentRequestItems();

      const detailsArray = this.issuanceForm.get('issuanceDetail') as FormArray;
      detailsArray.clear();

      this.selectedIndent = element.indentRequest;
      const isStore3 = this.selectedIndent?.storeId === 3;

      if (element.issuanceDetail && element.issuanceDetail.length > 0) {
        element.issuanceDetail.forEach(async (detail: any) => {

          let costsheets: any;
          let selectedCostSheet: any;

          if (isStore3) {
            costsheets = await this.getCostSheetByItem(detail.indentRequestDetail?.itemId, 0, detail.costSheetId); // your service call

            // Find the specific cost sheet matching the current detail's costSheetId
            selectedCostSheet = costsheets.find(
              (cs: any) => cs.id === detail.costSheetId
            );
          }

          (this.getSockByWarehouse(detail.indentRequestDetail?.itemId)).subscribe((stockdata: any) => {
            const detailGroup = this.formBuilder.group({
              id: [detail.id],
              issuanceId: [detail.issuanceId],
              indentRequestDetailId: [detail.indentRequestDetailId, Validators.required],
              itemId: [detail.indentRequestDetail?.itemId, Validators.required],
              required: [detail.indentRequestDetail?.required, Validators.required],
              costSheetId: [detail.costSheetId],
              costSheetList: [costsheets],
              quantity: [detail.quantity, [Validators.required, Validators.min(0.000)]], // Validation
              projectId: [detail?.projectId, Validators.required],
              stock: [isStore3 ? selectedCostSheet.quantity : stockdata.quantity, Validators.required],
              rate: [isStore3 ? selectedCostSheet.costPerPet : stockdata.rate, Validators.required],
            });

            detailsArray.push(detailGroup);
          });
        });
        console.log(detailsArray);
        console.log(this.issuanceForm.value);
      }
    } else {
      this.getIssuanceCode();
      this.issuanceForm.get('date')?.patchValue(this.constantService.formatDate(new Date()));
      this.addIssuanceDetail(0);
      this.getPendingIndentRequest(0);
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
    if (this.issuanceForm.invalid) {
      this.constantService.markFormGroupTouched(this.issuanceForm);
      this.checkInvalidControls(this.issuanceForm);
      this.notificationsService.showNotification('Please Fill Required Fields', 'snack-bar-danger');
      return;
    }

    // if (this.issuanceForm.value.filter((x: any) => x.issuanceDetail.stock === 0 && x.issuanceDetail.quantity === 0)) {
    //   this.notificationsService.showNotification('Stock is zero please remove quantity!', 'snack-bar-danger');
    //   return;
    // }

    this.isLoading = true;
    let _issuanceFormForm: any = {};
    _issuanceFormForm = Object.assign(_issuanceFormForm, this.issuanceForm.value);

    let issueDate = new Date(this.issuanceForm.get('date')?.value);
    _issuanceFormForm['date'] = issueDate.toLocaleString();

    this.issuanceService.saveIssuance(_issuanceFormForm).subscribe({
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

  getIssuanceCode() {
    this.issuanceService.getIssuanceCode().subscribe((data: any) => {
      this.issuanceForm.get('code')?.patchValue(data.code);
    });
  }

  isUpdating = false;

  async onItemSelected(event: MatSelectChange, index: number) {
    if (this.isUpdating) {
      return;
    }
    this.isUpdating = true;

    const selectedValue = event.value;
    if (!selectedValue) {
      console.error('Option value is undefined. Ensure mat-option [value] is correctly bound.');
      this.isUpdating = false;
      return;
    }

    const duplicateItem = this.issuanceDetail.controls
      .filter((control: AbstractControl, controlIndex: number) => controlIndex !== index)
      .some((control: AbstractControl) => {
        const formGroup = control as FormGroup;
        return formGroup.get('itemId')?.value === selectedValue;
      });

    if (duplicateItem) {
      this.notificationsService.showNotification('This item has already been selected.', 'snack-bar-danger');
      const currentFormGroup = this.issuanceDetail.at(index) as FormGroup;
      currentFormGroup.get('itemId')?.patchValue("");
      currentFormGroup.get('required')?.patchValue(null);
      currentFormGroup.get('quantity')?.patchValue('');
      this.isUpdating = false;
      return;
    } else {
      const selectedItem = this.getItemData(selectedValue);
      if (!selectedItem) {
        console.error('Selected item not found.');
        this.isUpdating = false;
        return;
      }

      const detailFormGroup = this.issuanceDetail.at(index) as FormGroup;
      detailFormGroup.get('itemId')?.patchValue(selectedItem?.item.id);
      detailFormGroup.get('indentRequestDetailId')?.patchValue(selectedItem.id);
      detailFormGroup.get('required')?.patchValue(selectedItem.required);
      detailFormGroup.updateValueAndValidity();

      const data = await this.getCostSheetByItem(selectedItem.item.id, 0, detailFormGroup.get('costSheetId')?.value ?? 0); // your service call
      detailFormGroup.get('costSheetList')?.patchValue(data);

      this.isUpdating = false;
    }
  }

  validateQty(index: number): any {
    const detailControl = (this.issuanceForm.get('issuanceDetail') as FormArray).at(index);
    if (detailControl?.value.quantity > detailControl?.value.required) {
      this.notificationsService.showNotification('Quantity can not be greater than required quantity!', 'snack-bar-danger');
      detailControl.get('quantity')?.patchValue(detailControl?.value.required);
    }
    if (detailControl?.value.quantity > detailControl?.value.stock) {
      this.notificationsService.showNotification('Quantity can not be greater than stock quantity!', 'snack-bar-danger');
      detailControl.get('quantity')?.patchValue(detailControl?.value.stock);
    }
  }

  getItemData(itemId: string) {
    return this.itemList.find(x => x.item?.id === itemId);
  }

  // getPOData() {
  //   const indentRequestId = this.issuanceForm.get('indentRequestId')?.value;
  //   return this.indentRequestList.find(x => x.id === indentRequestId);
  // }

  onInputCleared(event: Event, index: number): void {
    const inputValue = (event.target as HTMLInputElement)?.value;
    console.log('Current Input Value:', inputValue);

    if (!inputValue.trim()) {
      console.log(`Input cleared at row index: ${index}`);
      this.resetitem(index);
    }
  }

  resetitem(index: number) {
    const issuanceDetailArray = this.issuanceForm.get('issuanceDetail') as FormArray;
    if (!issuanceDetailArray || index < 0 || index >= issuanceDetailArray.length) {
      console.error('Invalid index or FormArray is not initialized properly.');
      return;
    }

    const currentFormGroup = issuanceDetailArray.at(index) as FormGroup;
    currentFormGroup.reset();
    return;
  }

  getPendingIndentRequest(indentRequestId: any) {
    this.issuanceService.getPendingIndentRequest(indentRequestId).subscribe((data: any) => {
      this.indentRequestList = data;
    });
  }

  removeAllIssuanceDetail() {
    if (this.issuanceDetail.length > 0) {
      this.issuanceDetail.clear();
      this.addIssuanceDetail(0);
    } else {
      this.notificationsService.showNotification('No items to remove.', 'snack-bar-warning');
    }
  }

  reset() {
    this.issuanceForm.get('code')?.patchValue('');
  }

  async getPendingIndentRequestItems(): Promise<void> {

    this.selectedIndent = this.indentRequestList.filter(
      (x: any) => x.id === this.issuanceForm.get('indentRequestId')?.value
    )[0];

    const indentRequestId = this.issuanceForm.get('indentRequestId')?.value;
    const issuanceId = this.issuanceForm.get('id')?.value;

    try {
      const data = await (await this.issuanceService.getPendingIndentRequestItems(indentRequestId, issuanceId)).toPromise();
      this.itemList = data;

      console.log('this.data.element', this.data.element);
      if (this.data.element == null) {
        this.createIssuanceDetails();
      }

    } catch (error) {
      console.error('Error fetching pending purchase order items:', error);
    }
  }


  isUpdatingCret = true;

  async createIssuanceDetails() {
    // if (this.isUpdatingCret) {
    //   return;
    // }
    // Clear existing issuance detail
    while (this.issuanceDetail.length) {
      this.issuanceDetail.removeAt(0);
    }

    // Iterate over itemList
    for (const item of this.itemList) {
      (this.getSockByWarehouse(item?.itemId)).subscribe(async (stockdata: any) => {
        const itemId = item?.itemId;
        const requiredQuantity = item?.required || 0;
        const indentRequestDetailId = item?.id;

        const isStore3 = this.selectedIndent?.storeId === 3;
        let costsheets: any[] = [];

        if (isStore3) {
          costsheets = await this.getCostSheetByItem(item?.itemId, 0, 0);
        }

        // Create the form group
        const formGroup = this.formBuilder.group({
          id: [0],
          itemId: [itemId],
          indentRequestDetailId: [indentRequestDetailId, Validators.required],
          required: [requiredQuantity, Validators.required],
          costSheetId: [null, isStore3 ? [Validators.required] : []],
          costSheetList: [costsheets],
          quantity: [0, [Validators.required, Validators.min(0.000)]], // Validation
          projectId: [0, Validators.required], // Assuming projectId is required
          stock: [this.selectedIndent?.storeId === 3 ? 0 : stockdata.quantity, Validators.required],
          rate: [this.selectedIndent?.storeId === 3 ? 0 : stockdata.rate, Validators.required],
        });

        // Push the form group to the issuanceDetail array
        this.issuanceDetail.push(formGroup);
      }
      )
    };

  }

  removeAllExceptFirst() {
    while (this.issuanceDetail.length > 1) {
      this.issuanceDetail.removeAt(1);
    }

    this.issuanceDetail.at(0).patchValue({ itemId: '' });
    this.issuanceDetail.at(0).patchValue({ required: 0 });
    this.issuanceDetail.at(0).patchValue({ quantity: 0.00 });
    this.issuanceDetail.at(0).patchValue({ projectId: 0 });
    this.issuanceDetail.at(0).patchValue({ stock: 0 });
    this.issuanceDetail.at(0).patchValue({ rate: 0 });
  }

  async getItemCurrentBalance(itemId: number): Promise<any> {
    try {
      // Using firstValueFrom() to get the first value emitted by the observable
      const data = await firstValueFrom(await this.ledgerService.itemCurrentBalance(itemId));
      console.log('Value Stock : ' + data);
      return data;  // Assuming `data` is of type `any` or you can replace it with a more specific type

    } catch (error) {
      console.error('Error fetching current balance for item:', error);
      throw error; // Rethrow the error if you want to propagate it
    }
  }

  async onProjectChange(event: any, index: number) {
    const selectedProjectId = event.value;
    const issuanceDetailArray = this.issuanceForm.get('issuanceDetail') as FormArray;
    const detailFormGroup = issuanceDetailArray.at(index) as FormGroup;
    const itemId = detailFormGroup.get('itemId')?.value;

    if (!itemId) {
      this.notificationsService.showNotification(
        'Please select item first',
        'snack-bar-danger'
      );
      return; // Prevent further execution
    }

    if (selectedProjectId) {
      (await this.getSockByWarehouse(itemId)).subscribe((data: any) => {
        detailFormGroup.get('stock')?.patchValue(data.quantity);
        detailFormGroup.get('rate')?.patchValue(data.rate);
      });
    } else {
      this.itemList = [];
    }
  }

  getSockByWarehouse(itemId: number) {
    return this.itemService.getSockByWarehouse_new(itemId);
  }

  getprojectList() {
    let _projectFilter: any = {};
    this.projectService.getAllProjects(_projectFilter).subscribe((data: any) => {
      this.projectList = data.item1;
    });
  }

  getAccountList(event: any) {
    var filter = event.currentTarget.value;
    this.getAccountByName(filter);
  }

  getAccountByName(filter: any) {
    var accountFlow = [''];
    this.accountService.getAccountByName(filter, accountFlow)
      .subscribe((data: any) => {
        this.accountList = data;
      });
  }

  onOptionAccountSelected(event: MatAutocompleteSelectedEvent): void {
    const selectedValue = event.option.value;
    this.issuanceForm.get('accountId')?.patchValue(selectedValue.id);
    this.issuanceForm.get('accountName')?.patchValue(selectedValue.code + ' : ' + selectedValue.name);
    this.issuanceForm.get('account')?.patchValue(selectedValue);
  }

  getaccount(itemId: string) {
    return this.accountList.find((option: { id: string; }) => option.id === itemId);
  }

  onAccountCleared(event: Event): void {
    const inputValue = (event.target as HTMLInputElement)?.value;
    console.log('Current Input Value:', inputValue);

    if (!inputValue.trim()) {
      this.issuanceForm.get('accountId')?.patchValue(null);
      this.issuanceForm.get('accountName')?.patchValue('');
      this.issuanceForm.get('account')?.patchValue(null);
    }
  }

  async getCostSheetByItem(itemId: any, projectId: any, costSheetId: any): Promise<any> {
    try {
      // Use firstValueFrom to convert the observable to a promise
      const data = await firstValueFrom(await this.dispatchService.getPendingCostSheet(itemId, projectId, costSheetId));
      return data;
    } catch (error) {
      console.error('Error loading pending indent items:', error);
    }
  }

  onCostSheetSelected(event: MatSelectChange, rowIndex: number): void {
    const selectedCostSheetId = event.value;

    const costSheetList = this.getIndexValue(rowIndex)?.costSheetList ?? [];
    const chosen = costSheetList.find((cs: { id: any }) => cs.id === selectedCostSheetId);

    const issuanceDetailArray = this.issuanceForm.get('issuanceDetail') as FormArray;
    const detailFormGroup = issuanceDetailArray.at(rowIndex) as FormGroup;

    detailFormGroup.get('stock')?.patchValue(chosen?.quantity ?? null);
    detailFormGroup.get('rate')?.patchValue(chosen?.costPerPet ?? null);
    detailFormGroup.get('quantity')?.patchValue(0);
  }


}