import { Component, Inject } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { PurchaseOrderService } from '../purchaseorder.service';
import { MatOptionSelectionChange } from '@angular/material/core';
import { firstValueFrom } from 'rxjs';
import { CurrencyService } from '../../currency/currency.service';
import { ShipmentModeService } from '../../shipmentmode/shipmentmode.service';
import { PaymentModeService } from '../../paymentmode/paymentmode.service';
import { VendorService } from '../../vendor/vendor.service';
import { AuthenticationService } from '../../../Auth/authentication.service';
import { MatSelectChange } from '@angular/material/select';
import { DeliveryTermsService } from '../../deliveryterms/deliveryterms.service';
import { GSTService } from '../../gst/gst.service';
import { ChangeDetectorRef } from '@angular/core';

@Component({
  selector: 'app-add-purchaseorder',
  templateUrl: './add-purchaseorder.component.html',
  styleUrl: './add-purchaseorder.component.css',
  standalone: false
})

export class AddPurchaseOrderComponent {
  purchaseorderForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  demandList: any;
  currencyList: any;
  shipmentModeList: any;
  paymentModeList: any;
  vendorList: any;
  pendingindentItemList: any;
  itemList: any[] = [];
  isdataload: boolean = false;
  iscswise: boolean = false;
  currentUser: any;
  selectedVendor: any;
  yesterday = new Date();
  deliveryTermsList: any;
  tAmount : any = 0;
  tDiscount : any = 0;
  tExpense : any = 0;
  tSaleTax : any = 0;
  partyAmount : any = 0;
  netAmount : any = 0;
  gstper : any = 0;
  isUpdating = false;
  isUpdatingitem = false;

  constructor(private cdr: ChangeDetectorRef,private deliveryTermsService: DeliveryTermsService, private authenticationService: AuthenticationService, private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private vendorService: VendorService, private currencyService: CurrencyService, private shipmentModeService: ShipmentModeService, private paymentModeService: PaymentModeService, private purchaseorderService: PurchaseOrderService,private gSTService: GSTService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.yesterday.setDate(this.yesterday.getDate() - 0);

    this.currentUser = this.authenticationService.currentUserValue;
    this.purchaseorderForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      createdDate: [new Date()],
      vendorId: [0, Validators.required],
      currencyId: [1, Validators.required],
      shipmentModeId: [1, Validators.required],
      paymentModeId: [0, Validators.required],
      deliveryDate: [new Date()],
      deliveryTermsId: [0, Validators.required], // Validation
      deliveryCharges: [0, [Validators.required, Validators.min(1)]],
      otherCharges: [0, [Validators.required, Validators.min(1)]],
      refNo: [''], // Validation
      note: ['ALL invoices, packing lists, Delivery Challan & Bill of lading shall address to Procurment Manager and above Purchase order Number must appear on all.'], // Validation
      remarks: [''], // Validation
      status: [''], // Validation
      statusName: ['New'], // Validation
      statusId: [1], // Validation
      iscspo: [false, Validators.required],
      isFixDiscount: [true, Validators.required],
      discount :[0, [Validators.required, Validators.min(1)]], // Validation
      purchaseOrderDetail: this.formBuilder.array([]) // Initialize as a FormArray
    });
    this.getGst();
    this.getindentList();
    this.getcurrencyList();
    this.getshipmentModesList();
    this.getpaymentModesList();
    this.getvendorModesList();
    this.getdeliveryTermsList();
    this.LoadData(this.data.element);
  }

  ngAfterViewInit(): void {

  }

  get purchaseOrderDetail(): FormArray {
    return this.purchaseorderForm.get('purchaseOrderDetail') as FormArray;
  }

  addPurchaseOrderDetail(index: number) {
    const newDetailGroup = this.formBuilder.group({
      id: [0], // Default value
      purchaseOrderId: [0], // Default value
      purchaseDemandId: [0], // Default value
      purchaseDemandDetailId: [0, Validators.required], // Default value
      comparativeStatementVendorId : [0, Validators.required], // Default value
      itemList: [[], Validators.required], // Validation
      itemName: ['', Validators.required], // Validation
      item: [''], // Validation
      // demandQty: [0, [Validators.required, Validators.min(1)]], // Validation
      remarks: [''], // Validation,
      unitRate: [0, [Validators.required, Validators.min(1)]], // Validation
      quantity: [0, [Validators.required, Validators.min(1)]], // Validation
      value: [0, [Validators.required, Validators.min(1)]], // Validation
      fed: [0, [Validators.required, Validators.min(0)]], // Validation
      isgst: [false, Validators.required],
      gst: [0, [Validators.required, Validators.min(0)]], // Validation
      description: [''], // Validation,
    }
    );

    // Insert the new group after the current index
    this.purchaseOrderDetail.insert(index + 1, newDetailGroup);
  }

  removePurchaseOrderDetail(index: number) {
    if (this.purchaseOrderDetail.length > 1) {
      this.purchaseOrderDetail.removeAt(index);
    } else {
      this.notificationsService.showNotification('At least one item is required.', 'snack-bar-danger');
    }
  }

  getIndexValue(index: number): any {
    const detailControl = (this.purchaseorderForm.get('purchaseOrderDetail') as FormArray).at(index);
    return detailControl?.value;
  }

 
async LoadData(element: any) {
  if (element != null) {
    this.isEditMode = true;
    this.isUpdating = true;
    this.isUpdatingitem = true;

    // Detach change detection
    // this.cdr.detach();

    try {
      await this.getindentList();
      this.selectedVendor = element.vendor;

      if (this.demandList && this.demandList.length > 0) {
        this.constantService.LoadData(element, this.purchaseorderForm);

        const detailsArray = this.purchaseorderForm.get('purchaseOrderDetail') as FormArray;
        detailsArray.clear();

        if (element.purchaseOrderDetail && element.purchaseOrderDetail.length > 0) {
          for (const detail of element.purchaseOrderDetail) {
            this.pendingindentItemList = await this.getPendingDemandItems(detail.purchaseDemandDetail.purchaseDemandId);

            const selectedItem = this.pendingindentItemList.find(
              (item: any) => item.id === detail.purchaseDemandDetailId
            );

            if (selectedItem) {
              const detailGroup = this.formBuilder.group({
                id: [detail.id],
                purchaseOrderId: [detail.purchaseOrderId],
                purchaseDemandDetailId: [detail.purchaseDemandDetailId],
                purchaseDemandId: [detail.purchaseDemandDetail.purchaseDemandId],
                itemList: [this.pendingindentItemList, Validators.required],
                itemName: [selectedItem, Validators.required],
                item: [selectedItem, Validators.required],
                unitRate: [detail.unitRate, Validators.required],
                quantity: [detail.quantity + selectedItem.demandQty, Validators.required],
                value: [detail.value, Validators.required],
                fed: [detail.fed, Validators.required],
                isgst: [detail.gst == 0 ? false : true],
                gst: [detail.gst, Validators.required],
              });

              detailsArray.push(detailGroup);
            }
          }

          this.calculateTotals();
        }
      }

      // Reattach change detection
    } catch (error) {
      console.error('Error loading pending indent items:', error);
      // this.cdr.reattach();
    }
  } else {
    this.getPurchaseOrderCode();
    this.purchaseorderForm.get('createdDate')?.patchValue(this.constantService.formatDate(new Date()));
    this.addPurchaseOrderDetail(0);
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
    if (this.purchaseorderForm.invalid) {
      // Mark all controls as touched to trigger validation messages
      this.constantService.markFormGroupTouched(this.purchaseorderForm);

      // Check each control in the FormGroup to see which one is invalid
      this.checkInvalidControls(this.purchaseorderForm);

      this.notificationsService.showNotification('Please Fill Required Fields', 'snack-bar-danger');
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.purchaseorderForm.value);
    let requiredDate = new Date(this.purchaseorderForm.get('requiredDate')?.value);
    _clienttemperatureForm['requiredDate'] = requiredDate.toLocaleDateString();

    this.purchaseorderService.savePurchaseOrder(_clienttemperatureForm).subscribe({
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

  getPurchaseOrderCode() {
    this.purchaseorderService.getPurchaseOrderCode().subscribe((data: any) => {
      this.purchaseorderForm.get('code')?.patchValue(data.code);
    });
  }

  onOptionSelected(event: MatOptionSelectionChange, index: number): void {
    // Prevent triggering the event while updating form controls
    if (this.isUpdating) {
      return;
    }

    this.isUpdating = true; // Set the flag to prevent recursion
    const selectedValue = event.source.value; // Access the selected value

    const purchaseOrderDetailArray = this.purchaseorderForm.get('purchaseOrderDetail') as FormArray;
    if (!selectedValue) {
      console.error('Option value is undefined. Ensure mat-option [value] is correctly bound.');
      this.isUpdating = false; // Reset the flag
      return;
    }

    if (!purchaseOrderDetailArray || index < 0 || index >= purchaseOrderDetailArray.length) {
      console.error('Invalid index or FormArray is not initialized properly.');
      this.isUpdating = false; // Reset the flag
      return;
    }

    const duplicateItem = purchaseOrderDetailArray.controls
      .filter((control: AbstractControl, controlIndex: number) => controlIndex !== index) // Exclude the current index
      .some((control: AbstractControl) => {
        const formGroup = control as FormGroup;
        return formGroup.get('purchaseDemandDetailId')?.value === selectedValue;  // Check if the indentRequestId already exists
      });

    const currentFormGroup = purchaseOrderDetailArray.at(index) as FormGroup;
    if (duplicateItem) {
      this.notificationsService.showNotification('This item has already been selected.', 'snack-bar-danger');
      const currentFormGroup = purchaseOrderDetailArray.at(index) as FormGroup;
      event.source.value = null; // Reset the selection
      currentFormGroup.get('id')?.patchValue(0);
      currentFormGroup.get('purchaseDemandDetailId')?.patchValue(0);
      currentFormGroup.get('itemName')?.patchValue(undefined);
      currentFormGroup.get('item')?.patchValue(null);
      currentFormGroup.get('required')?.patchValue(0);
      currentFormGroup.get('remarks')?.patchValue('');
      currentFormGroup.get('quantity')?.patchValue(0);
      currentFormGroup.get('description')?.patchValue('');
      this.isUpdating = false; // Reset the flag
      return;
    }

    // Get the selected item details from your getitem method
    const selectedItem = currentFormGroup.value.itemList.find((option: { id: string; }) => option.id === selectedValue);
    if (!selectedItem) {
      console.error('Selected item not found.');
      this.isUpdating = false; // Reset the flag
      return;
    }

    // Get the form group for the current index
    const detailFormGroup = purchaseOrderDetailArray.at(index) as FormGroup;

    // Patch the selected item values into the form group
    detailFormGroup.get('purchaseDemandDetailId')?.patchValue(selectedValue);
    // detailFormGroup.get('demandQty')?.patchValue(this.isEditMode ? selectedValue.demandQty + detailFormGroup.get('quantity')?.value : selectedValue.demandQty);
    detailFormGroup.get('item')?.patchValue(selectedItem);
    detailFormGroup.get('itemName')?.patchValue(selectedItem);
    detailFormGroup.get('remarks')?.patchValue(selectedItem.description);

    if (this.iscswise){
      detailFormGroup.get('unitRate')?.patchValue(selectedItem.comparativeStatementDetail?.comparativeStatementVendor?.[0]?.price);
      detailFormGroup.get('comparativeStatementVendorId')?.patchValue(selectedItem.comparativeStatementDetail?.comparativeStatementVendor?.[0]?.id);
    }
    else{
      detailFormGroup.get('unitRate')?.patchValue(0);
      detailFormGroup.get('comparativeStatementVendorId')?.patchValue(0);
    }

    detailFormGroup.get('value')?.patchValue(0);
    detailFormGroup.get('quantity')?.patchValue(0);
    detailFormGroup.get('quantity')?.setValidators([Validators.required, Validators.min(1), Validators.max(selectedItem.demandQty)]);
    detailFormGroup.updateValueAndValidity();

    this.isUpdating = false; // Reset the flag
  }


  validateQty(index: number): any {
    const detailControl = (this.purchaseorderForm.get('purchaseOrderDetail') as FormArray).at(index);
    if (detailControl?.value.quantity > detailControl?.value.demandQty) {
      detailControl.get('quantity')?.patchValue(detailControl?.value.demandQty);
    }
    this.calculateTotals();
  }

  calculateGst(event : any,index: number): any {
    const detailControl = (this.purchaseorderForm.get('purchaseOrderDetail') as FormArray).at(index);
    if (event.checked) {
    var amt = detailControl?.value.quantity * detailControl?.value.unitRate;
    var gst = amt * this.gstper / 100;
      detailControl.get('gst')?.patchValue(gst);
    }
    else{
      detailControl.get('gst')?.patchValue(0);
    }
    this.calculateTotals();
  }

  calculateAmt(index: number): any {
    const detailControl = (this.purchaseorderForm.get('purchaseOrderDetail') as FormArray).at(index);
      detailControl.get('value')?.patchValue(detailControl?.value.quantity * detailControl?.value.unitRate );
      this.calculateTotals();
  }

  getitem(indentRequestId: string) {
    return this.pendingindentItemList.find((option: { id: string; }) => option.id === indentRequestId);
  }

  getorder(demandId: string) {
    return this.demandList.find((option: { id: string; }) => option.id === demandId);
  }

  async getindentList() {
    try {
      this.demandList = await firstValueFrom(await this.purchaseorderService.getPendingDemand(this.purchaseorderForm.get('id')?.value));
      console.log('Demand List fetched successfully:', this.demandList);
    } catch (error) {
      console.error('Error fetching demand list:', error);
    }
  }

  getcurrencyList() {
    let _currencyFilter: any = {};
    this.currencyService.getAllCurrencys(_currencyFilter).subscribe((data: any) => {
      this.currencyList = data.item1;
    });
  }

  getshipmentModesList() {
    let _shipmentModesFilter: any = {};
    this.shipmentModeService.getAllShipmentModes(_shipmentModesFilter).subscribe((data: any) => {
      this.shipmentModeList = data.item1;
    });
  }

  getpaymentModesList() {
    let _paymentModesFilter: any = {};
    this.paymentModeService.getAllPaymentModes(_paymentModesFilter).subscribe((data: any) => {
      this.paymentModeList = data.item1;
    });
  }

  getvendorModesList() {
    let _vendorFilter: any = {};
    this.vendorService.getAllVendors(_vendorFilter).subscribe((data: any) => {
      this.vendorList = data.item1;
    });
  }

  getdeliveryTermsList() {
    let _deliveryTermsFilter: any = {};
    this.deliveryTermsService.getAllDeliveryTerms(_deliveryTermsFilter).subscribe((data: any) => {
      this.deliveryTermsList = data.item1;
    });
  }

  async getPendingDemandItemList(event: MatOptionSelectionChange, index: number): Promise<void> {
    // Prevent triggering the event while updating form controls
    if (this.isUpdatingitem) {
      return;
    }

    this.isUpdatingitem = true; // Set the flag to prevent recursion
    const selectedValue = event.source.value; // Access the selected value
    const purchaseOrderDetailArray = this.purchaseorderForm.get('purchaseOrderDetail') as FormArray;
    if (!selectedValue) {
      console.error('Option value is undefined. Ensure mat-option [value] is correctly bound.');
      this.isUpdatingitem = false; // Reset the flag
      return;
    }

    if (!purchaseOrderDetailArray || index < 0 || index >= purchaseOrderDetailArray.length) {
      console.error('Invalid index or FormArray is not initialized properly.');
      this.isUpdatingitem = false; // Reset the flag
      return;
    }

    // Get the selected item details from your getitem method
    const selectedDemand = this.getorder(selectedValue);
    if (!selectedDemand) {
      console.error('Selected Demand not found.');
      this.isUpdatingitem = false; // Reset the flag
      return;
    }

    // Get the form group for the current index
    const detailFormGroup = purchaseOrderDetailArray.at(index) as FormGroup;

    // Patch the selected item values into the form group

    detailFormGroup.get('purchaseDemandId')?.patchValue(selectedValue);
    var items = await this.getPendingDemandItems(selectedValue);
    detailFormGroup.get('itemList')?.patchValue(items);
    detailFormGroup.get('unitRate')?.patchValue(0);
    detailFormGroup.get('quantity')?.patchValue(0);
    detailFormGroup.get('value')?.patchValue(0);
    this.isUpdatingitem = false; // Reset the flag
  }

  async getPendingDemandItems(demandId: any) {
    try {
      this.pendingindentItemList = [];
      const id = this.isEditMode == true ? this.data?.element?.id : 0;
      const vendorId = this.purchaseorderForm.get('vendorId')?.value;

      // Use firstValueFrom to convert the observable to a promise
      const data = await firstValueFrom(await this.purchaseorderService.getPendingIndentItems(demandId, id, vendorId));
      this.pendingindentItemList = data || [];
      return this.pendingindentItemList;
    } catch (error) {
      console.error('Error loading pending indent items:', error);
    }
  }

  removeAllPurchaseOrderDetails() {
    // Check if there are any rows
    if (this.purchaseOrderDetail.length > 0) {
      // Clear all items in the FormArray
      this.purchaseOrderDetail.clear();
      this.addPurchaseOrderDetail(0);
    } else {
      this.notificationsService.showNotification('No items to remove.', 'snack-bar-warning');
    }
  }

  vendorChange(event: MatSelectChange) {
    const id = event.source.value; // Access the selected value
    const selectedItem = this.vendorList.find((item: any) => item.id === id);
    this.selectedVendor = selectedItem;
  }

  reset() {
    this.purchaseorderForm.get('code')?.patchValue('');
  }

  async applyRates(event: any) {
    if (event.checked) {
      this.iscswise = true;
  
      // Loop through the purchaseOrderDetail FormArray
      (this.purchaseOrderDetail.controls as FormGroup[]).forEach((control: FormGroup) => {
        // Access item and update the unitRate value
        const item = control.get('item')?.value;
        const price = item?.comparativeStatementDetail?.comparativeStatementVendor?.[0]?.price;
  
        if (price !== undefined) {
          // Set the unitRate value in the current FormGroup
          control.get('unitRate')?.setValue(price);
          control.get('value')?.setValue(control.get('quantity')?.value * price);
        }
      });
    }
    else{
      // Loop through the purchaseOrderDetail FormArray
      (this.purchaseOrderDetail.controls as FormGroup[]).forEach((control: FormGroup) => {
          // Set the unitRate value in the current FormGroup
          control.get('unitRate')?.setValue(0);
          control.get('value')?.setValue(0);
      });
    }
    this.calculateTotals();
  }

  onRadioButtonChange(event:any) {
    if(event.value == 'true'){
    this.purchaseorderForm.get('isFixDiscount')?.patchValue(true);
    }
    else{
      this.purchaseorderForm.get('isFixDiscount')?.patchValue(false);
    }
    this.calculateTotals();
  }

  async calculateTotals() {
    var _tAmount = 0;
    var _tSaleTax = 0;
    this.tExpense = this.purchaseorderForm.get('deliveryCharges')?.value + this.purchaseorderForm.get('otherCharges')?.value;
    this.tDiscount = this.purchaseorderForm.get('discount')?.value;

      (this.purchaseOrderDetail.controls as FormGroup[]).forEach((control: FormGroup) => {
        // Access item and update the unitRate value
       var value  = control.get('value')?.value;
       _tAmount =  _tAmount + value;
       var gst  = control.get('gst')?.value;
       _tSaleTax =  _tSaleTax + gst;
      });
      this.tAmount = _tAmount;
      this.tSaleTax = _tSaleTax;
      this.netAmount = this.tAmount +  this.tSaleTax + this.tExpense - this.tDiscount;
      this.partyAmount = (this.tAmount + this.tExpense) - this.tDiscount;
  }

  async getGst() {
    try {
      const data = await firstValueFrom(await this.gSTService.getCurrentGST());
      this.gstper = data.gstPer || [];
    } catch (error) {
      console.error('Error fetching GST:', error);
    }
  }
}
