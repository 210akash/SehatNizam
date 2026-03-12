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
import { DispatchService } from '../dispatch.service';
import { firstValueFrom } from 'rxjs';
import { VendorService } from '../../vendor/vendor.service';
import { MatOptionSelectionChange } from '@angular/material/core';
import { PrimaryOrderService } from '../../order/primary-order/order.service';
import { VehicleService } from '../../order/vehicle/vehicle.service';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { MatSelectChange } from '@angular/material/select';
import { DatePipe } from '@angular/common';
import { ProjectService } from '../../project/project.service';

@Component({
  selector: 'app-add-dispatch',
  templateUrl: './add-dispatch.component.html',
  styleUrl: './add-dispatch.component.css',
  standalone: false,
})
export class AddDispatchComponent {

  dispatchForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  vendorList: any;
  orderList: any;
  pendingOrderItemsList: any;
  isdataload: boolean = false;
  vehicleList: any[] = [];
  grandTotalAmount = 0;
  grandTotalWeight = 0;
  grandTotalQuantity = 0;
  vehicleCapacity: any;
  projectList: any;
  constructor(private datePipe: DatePipe, private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private vendorService: VendorService, private dispatchService: DispatchService,
    private constantService: ConstantService, private primaryOrderService: PrimaryOrderService,
    private projectService: ProjectService,
    private vehicleService: VehicleService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.dispatchForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      vehicle: [null, Validators.required],
      vehicleId: [null, Validators.required],
      vehicleName: ['', Validators.required],
      projectId: [0],
      createdDate: [new Date(), Validators.required],
      status: [''],
      statusName: ['New'],
      statusId: [1],
      remarks: [''],
      biltyNo: [0, Validators.required],
      freightCharges: [0, Validators.required],
      totalQuantity: [0],
      dispatchOrder: this.formBuilder.array([]),
    });

    this.LoadData(this.data.element);
    this.getprojectList();
  }

  get dispatchOrder(): FormArray {
    return this.dispatchForm.get('dispatchOrder') as FormArray;
  }

  dispatchDetail(detailIndex: number): FormArray {
    return this.dispatchOrder
      .at(detailIndex)
      .get('dispatchDetail') as FormArray;
  }

  addDispatchOrder(index: number) {
    const newDetailGroup = this.formBuilder.group({
      id: [0],
      order: [null, Validators.required],
      orderId: [null, Validators.required],
      orderName: ['', Validators.required],
      itemList: [[], Validators.required],
      dcCode: [''],
      dispatchId: [''],
      totalWeight: [0],
      totalAmount: [0],
      totalQuantity: [0],
      costSheetList: [[]],
      orderFreightCharges: [0],
      dispatchDetail: this.formBuilder.array([this.createDispatchDetail()]),
    });

    // Insert the new group after the current index
    this.dispatchOrder.insert(index + 1, newDetailGroup);
    console.log('Form Array : ' + this.dispatchOrder);
  }

  createDispatchDetail() {
    return this.formBuilder.group({
      id: [0],
      orderItemId: [0, Validators.required],
      item: [null],
      weight: [0],
      amount: [0],
      orderedQuantity: ['', Validators.required],
      quantity: ['', Validators.required],
      dispatchOrderId: [0],
      costSheetId: [0, Validators.required],
      costSheetList: [[]],
      pendingCostSheetQuantity: [0, Validators.required],
      rate: [0, Validators.required],

    });
  }

  addDispatchDetail(index: number, detailIndex: number) {
    const detailArray = this.dispatchDetail(detailIndex);
    // Insert the new vendor FormGroup at the specified index
    detailArray.insert(index + 1, this.createDispatchDetail());
  }

  removeDispatchDetail(detailIndex: number, orderIndex: number) {
    const vendorArray = this.dispatchDetail(orderIndex);
    if (vendorArray.length > 1) {
      vendorArray.removeAt(detailIndex);
    } else {
      this.notificationsService.showNotification(
        'At least one vendor is required for each item.',
        'snack-bar-danger'
      );
    }
  }

  removeDispatchOrder(index: number) {
    if (this.dispatchOrder.length > 1) {
      this.dispatchOrder.removeAt(index);
    } else {
      this.notificationsService.showNotification(
        'At least one item is required.',
        'snack-bar-danger'
      );
    }
  }

  getIndexValueOrder(index: number): any {
    const orderControl = (
      this.dispatchForm.get('dispatchOrder') as FormArray
    ).at(index);
    return orderControl?.value || '';
  }

  getIndexValueDetail(index: number, detailIndex: number): any {
    const detailArrays = this.dispatchDetail(detailIndex);
    const detailArray = detailArrays.at(index);
    return detailArray?.value || '';
  }

  async LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.dispatchForm);
      // this.getVehiclesList();
      // await this.getPendingOrder();

      // Populate the FormArray
      const dispatchOrderArray = this.dispatchForm.get(
        'dispatchOrder'
      ) as FormArray;
      dispatchOrderArray.clear(); // Clear existing data

      if (element.dispatchOrder && element.dispatchOrder.length > 0) {
        let i = 0;

        this.dispatchForm.get('vehicleId')?.patchValue(element.vehicle?.id);
        this.dispatchForm.get('vehicleName')?.patchValue(element.vehicle?.vehicleName + ' : ' + element.vehicle?.registrationNumber);
        this.dispatchForm.get('vehicle')?.patchValue(element.vehicle);
        var projectId = this.dispatchForm.get('projectId')?.value;
        for (const order of element.dispatchOrder) {
          // ✅ Use for...of instead of forEach
          // Fetch pending order items one at a time
          this.pendingOrderItemsList = await this.getPendingOrderItemss(
            order.orderId,
            element.id
          );
          const detailArray = this.formBuilder.array(
            await Promise.all(
              order.dispatchDetail
                .filter((x: any) => x.isActive === true)
                .map(async (detail: any) => {
                  // Find the selected item using detail.orderItemId
                  const selectedItem = this.pendingOrderItemsList.find(
                    (item: any) => item.id === detail.orderItemId
                  );
                  const costsheets = await this.getCostSheetByItem(selectedItem.itemId, projectId, detail.costSheetId); // your service call

                  // Find the specific cost sheet matching the current detail's costSheetId
                  const selectedCostSheet = costsheets.find(
                    (cs: any) => cs.id === detail.costSheetId
                  );

                  return this.formBuilder.group({
                    id: [detail.id],
                    orderItemId: [detail.orderItemId],
                    dispatchOrderId: [detail.dispatchOrderId],
                    item: [selectedItem.itemId],
                    orderedQuantity: [detail.orderItem.quantity, Validators.required],
                    quantity: [detail.quantity],
                    costSheetId: [detail.costSheetId],
                    costSheetList: [costsheets],
                    pendingCostSheetQuantity: [selectedCostSheet.quantity + detail.quantity],
                    rate: [selectedCostSheet.costPerPet],
                    weight: [selectedItem.item?.weight],
                    amount: [detail.orderItem?.distributorPrice],
                  });
                })
            )
          );
          const formattedDate = this.datePipe.transform(order.order?.createdDate, 'mediumDate');
          // Map detail group
          const orderGroup = this.formBuilder.group({
            id: [order.id],
            // orderId: [order.orderId],

            order: [order],
            orderId: [order.order?.id],
            orderName: [order.order?.id + ' : ' + order.order?.dealership?.name + ' | ' + order.order?.dealership?.territory?.name + ' : ' + formattedDate],
            itemList: [this.pendingOrderItemsList, Validators.required],
            dcCode: [order.dcCode],
            dispatchId: [order.dispatchId],
            dispatchDetail: detailArray,
            totalWeight: [0],
            totalAmount: [0],
            totalQuantity: [0],
            orderFreightCharges: [0],
          });

          dispatchOrderArray.push(orderGroup);

          this.calculateWeight(i);
          i++;
        }

      }
    } else {
      // Default initialization for new entries
      // this.getPendingOrder();
      this.getDispatchCode();
      this.dispatchForm
        .get('createdDate')
        ?.patchValue(this.constantService.formatDate(new Date()));
      this.addDispatchOrder(0);
    }
  }

  checkInvalidControls(formGroup: FormGroup) {
    // Loop through each control in the FormGroup
    Object.keys(formGroup.controls).forEach((controlName) => {
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
    this.isLoading = true;
    if (this.dispatchForm.invalid) {
      // Mark all controls as touched to trigger validation messages
      this.constantService.markFormGroupTouched(this.dispatchForm);

      // Check each control in the FormGroup to see which one is invalid
      this.checkInvalidControls(this.dispatchForm);

      this.notificationsService.showNotification(
        'Please Fill Required Fields',
        'snack-bar-danger'
      );
      this.isLoading = false;
      return;
    }

    let _dispatchForm: any = {};
    _dispatchForm = Object.assign(_dispatchForm, this.dispatchForm.value);

    let createdDate = new Date(this.dispatchForm.get('createdDate')?.value);
    _dispatchForm['createdDate'] = createdDate.toLocaleString();

    this.dispatchService.saveDispatch(_dispatchForm).subscribe({
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

  getDispatchCode() {
    this.dispatchService.getDispatchCode().subscribe((data: any) => {
      this.dispatchForm.get('code')?.patchValue(data.code);
    });
  }

  isUpdating = false; // Flag to prevent recursive calls

  onOptionSelected(event: MatAutocompleteSelectedEvent, index: number): void {
    debugger
    // Prevent triggering the event while updating form controls
    if (this.isUpdating) {
      return;
    }

    this.isUpdating = true; // Set the flag to prevent recursion

    const selectedValue = event.option.value;

    const dispatchOrderArray = this.dispatchForm.get('dispatchOrder') as FormArray;

    if (!selectedValue) {
      console.error(
        'Option value is undefined. Ensure mat-option [value] is correctly bound.'
      );
      this.isUpdating = false; // Reset the flag
      return;
    }

    // Check if index is valid
    if (!dispatchOrderArray || index < 0 || index >= dispatchOrderArray.length) {
      console.error('Invalid index or FormArray is not initialized properly.');
      this.isUpdating = false; // Reset the flag
      return;
    }

    // Check if the selected itemId already exists in the form array (excluding the current index)
    const duplicateItem = dispatchOrderArray.controls
      .filter(
        (control: AbstractControl, controlIndex: number) =>
          controlIndex !== index
      ) // Exclude the current index
      .some((control: AbstractControl) => {
        const formGroup = control as FormGroup; // Cast AbstractControl to FormGroup
        return formGroup.get('orderId')?.value === selectedValue.id; // Check if the itemId already exists
      });

    if (duplicateItem) {
      this.notificationsService.showNotification(
        'This order has already been selected.',
        'snack-bar-danger'
      );
      const currentFormGroup = dispatchOrderArray.at(index) as FormGroup;

      // Reset the form group values
      currentFormGroup.get('order')?.patchValue(null);
      currentFormGroup.get('orderId')?.patchValue(null);
      currentFormGroup.get('orderName')?.patchValue(null);
      currentFormGroup.get('order')?.patchValue(null);
      // currentFormGroup.get('item')?.patchValue(null);
      this.isUpdating = false; // Reset the flag
      return;
    } else {
      // Get the selected item details from your getitem method
      const selectedItem = this.getorder(selectedValue.id);
      if (!selectedItem) {
        console.error('Selected item not found.');
        this.isUpdating = false; // Reset the flag
        return;
      }

      // Get the form group for the current index
      const detailFormGroup = dispatchOrderArray.at(index) as FormGroup;

      detailFormGroup.get('order')?.patchValue(selectedValue);
      detailFormGroup.get('orderId')?.patchValue(selectedValue.id);
      const formattedDate = this.datePipe.transform(selectedValue.createdDate, 'mediumDate');
      detailFormGroup.get('orderName')?.patchValue(`${selectedValue.id} : ${selectedValue.dealership?.name} | ${selectedValue.dealership?.territory?.name} : ${formattedDate}`);
      this.isUpdating = false; // Reset the flag
    }
  }

  // Flag to prevent recursive calls
  isUpdatingitem = false;
  async onItemSelected(
    event: MatOptionSelectionChange,
    index: number,
    detailIndex: number
  ): Promise<void> {
    if (this.isUpdatingitem) {
      return;
    }
    this.isUpdatingitem = true;

    const selectedOrderItemId = event.source.value;
    const dispatchDetailArray = this.dispatchDetail(detailIndex);
    const currentFormGroup = dispatchDetailArray.at(index) as FormGroup;
    // currentFormGroup.get('orderedQuantity')?.patchValue(0);
    // currentFormGroup.get('pendingCostSheetQuantity')?.patchValue(0);
    // currentFormGroup.get('rate')?.patchValue(0);
    // currentFormGroup.get('quantity')?.patchValue(0);


    // ---------------- guard clauses ----------------
    if (!selectedOrderItemId) {
      console.error(
        'Option value is undefined. Ensure mat-option [value] is correctly bound.'
      );
      this.isUpdatingitem = false;
      return;
    }

    if (
      !dispatchDetailArray ||
      index < 0 ||
      index >= dispatchDetailArray.length
    ) {
      console.error('Invalid index or FormArray is not initialized properly.');
      this.isUpdatingitem = false;
      return;
    }
    // ------------------------------------------------

    /* ----------------------------------------------------------
       DUPLICATE CHECK
       A row is a duplicate only if BOTH the selected item
       AND the cost‑sheet (when present) are already chosen
       in another row.
    ---------------------------------------------------------- */
    const duplicateLine = dispatchDetailArray.controls
      .filter((_ctrl, ctrlIdx) => ctrlIdx !== index) // skip current row
      .some((ctrl: AbstractControl) => {
        const fg = ctrl as FormGroup;
        const sameItem = fg.get('orderItemId')?.value === selectedOrderItemId;

        // costSheetId may still be null at this point,
        // so treat "null === null" as *not* a duplicate.
        const currentCostSheetId = currentFormGroup.get('costSheetId')?.value;
        const otherCostSheetId = fg.get('costSheetId')?.value;

        const sameCostSheet =
          currentCostSheetId != null &&
          otherCostSheetId != null &&
          currentCostSheetId === otherCostSheetId;

        return sameItem && sameCostSheet;
      });

    if (duplicateLine) {
      this.notificationsService.showNotification(
        'This item / cost‑sheet combination has already been selected.',
        'snack-bar-danger'
      );


      currentFormGroup.reset(); // clear all controls in this row
      this.isUpdatingitem = false;
      return;
    }
    /* ---------- end duplicate check ---------- */

    /* ----------------------------------------------------------
       Fetch cost‑sheet list and update controls as before
    ---------------------------------------------------------- */
    const dispatchOrderArray = this.dispatchForm.get(
      'dispatchOrder'
    ) as FormArray;
    const currentOrderFormGroup = dispatchOrderArray.at(
      detailIndex
    ) as FormGroup;

    const selectedItem = currentOrderFormGroup.value.itemList.find(
      (opt: { id: string }) => opt.id === selectedOrderItemId
    );
    if (!selectedItem) {
      console.error('Selected item not found.');
      this.isUpdatingitem = false;
      return;
    }
    currentFormGroup.get('orderItemId')?.patchValue(selectedItem.id);
    currentFormGroup.get('item')?.patchValue(selectedItem);
    currentFormGroup.get('orderedQuantity')?.patchValue(selectedItem.quantity);
    currentFormGroup.get('weight')?.patchValue(selectedItem.item.weight);
    currentFormGroup.get('amount')?.patchValue(selectedItem.distributorPrice);

    const data = await this.getCostSheetByItem(selectedItem.itemId, 0, currentFormGroup.get('costSheetId')?.value ?? 0); // your service call
    currentFormGroup.get('costSheetList')?.patchValue(data);
    this.isUpdatingitem = false;
  }

  async getCostSheetListByItem(rowIndex: number, parentIndex: number) {
    const dispatchDetailArray = this.dispatchDetail(parentIndex); // FormArray

    if (!dispatchDetailArray || !dispatchDetailArray.at(rowIndex)) {
      console.error('No detail found at the given index');
      return;
    }

    const currentFormGroup = dispatchDetailArray.at(rowIndex) as FormGroup;

    // Clear existing values
    currentFormGroup.get('costSheetList')?.patchValue([]);

    // If itemId is valid, fetch and set costSheetList
    // if (itemId !== undefined && itemId !== null) {
    try {
      const data = await this.getCostSheetByItem(currentFormGroup.get('item')?.value.itemId, currentFormGroup.get('projectId')?.value, currentFormGroup.get('costSheetId')?.value ?? 0); // your service call
      currentFormGroup.get('costSheetList')?.patchValue(data);
    } catch (error) {
      console.error('Error fetching cost sheet list:', error);
    }
    // } else {
    //   console.error('itemId not found at the given index');
    // }
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
  getitem(itemId: string) {
    return this.pendingOrderItemsList.find(
      (option: { itemId: string }) => option.itemId === itemId
    );
  }

  getorder(orderId: any) {
    debugger
    return this.orderList.find(
      (option: { id: any }) => option.id === orderId
    );
  }

  onInputCleared(event: Event, index: number): void {
    const inputValue = (event.target as HTMLInputElement)?.value;
    console.log('Current Input Value:', inputValue); // Debugging output

    if (!inputValue.trim()) {
      console.log(`Input cleared at row index: ${index}`);
      this.resetorder(index); // Call a function when cleared
    }
  }

  resetorder(index: number) {

    const dispatchOrderArray = this.dispatchForm.get(
      'dispatchOrder'
    ) as FormArray;

    // Check if index is valid
    if (
      !dispatchOrderArray ||
      index < 0 ||
      index >= dispatchOrderArray.length
    ) {
      console.error('Invalid index or FormArray is not initialized properly.');
      return;
    }

    const currentFormGroup = dispatchOrderArray.at(index) as FormGroup;
    // Reset the form group values
    currentFormGroup.reset(); // This will clear all the values of the current form group
    currentFormGroup.get('order')?.patchValue(null);
    currentFormGroup.get('orderId')?.patchValue(null);
    currentFormGroup.get('orderName')?.patchValue(null);
    currentFormGroup.get('dcCode')?.patchValue('');
    currentFormGroup.get('totalWeight')?.patchValue(0);
    currentFormGroup.get('totalAmount')?.patchValue(0);
    currentFormGroup.get('totalQuantity')?.patchValue(0);
    currentFormGroup.get('costSheetList')?.patchValue([]);
    currentFormGroup.get('orderFreightCharges')?.patchValue(0);

    return; // Do nothing if the itemId already exists
  }

  resetitem(index: number) {
    const dispatchDetailArray = this.dispatchForm.get(
      'dispatchDetail'
    ) as FormArray;

    // Check if index is valid
    if (
      !dispatchDetailArray ||
      index < 0 ||
      index >= dispatchDetailArray.length
    ) {
      console.error('Invalid index or FormArray is not initialized properly.');
      return;
    }

    const currentFormGroup = dispatchDetailArray.at(index) as FormGroup;
    currentFormGroup.reset(); // This will clear all the values of the current form group
    return; // Do nothing if the itemId already exists
  }

  async getPendingOrder(event: any) {
    try {
      const filter = event.currentTarget.value;

      if (!filter || filter.length < 2) {
        this.orderList = []; // Optionally clear the list
        return; // Skip API call if filter is less than 2 characters
      }

      const orderIds = await this.getOrderIdsArray();
      this.orderList = await firstValueFrom(await this.dispatchService.getPendingOrder(orderIds, filter));
      this.vehicleCapacity = this.vehicleList.find(
        (x) => x.id === this.dispatchForm.get('vehicleId')?.value
      );
    } catch (error) {
      console.error('Error fetching demand list:', error);
    }
  }

  async getOrderIdsArray(): Promise<number[]> {
    try {
      // Ensure that data and dispatchOrder exist
      if (!this.data?.element?.dispatchOrder || !Array.isArray(this.data.element.dispatchOrder)) {
        console.warn('dispatchOrder data is missing or invalid');
        return [];
      }

      // Extract and filter orderIds
      const orderIds = this.data.element.dispatchOrder
        .map((order: any) => order?.orderId)
        .filter((id: any): id is number => id !== undefined && id !== null && !isNaN(id));

      console.log('Collected orderIds:', orderIds);
      return orderIds;
    } catch (error) {
      console.error('Error extracting orderIds:', error);
      return [];
    }
  }

  async getPendingOrderItems(index: any) {
    try {
      const dispatchOrderArray = this.dispatchForm.get(
        'dispatchOrder'
      ) as FormArray;
      const orderFormGroup = dispatchOrderArray.at(index) as FormGroup;

      const orderId = orderFormGroup.get('orderId')?.value;

      const id = this.isEditMode == true ? this.data?.element?.id : 0;

      // Use firstValueFrom to convert the observable to a promise
      const data = await firstValueFrom(
        await this.dispatchService.getPendingOrderItems(orderId, id)
      );
      this.pendingOrderItemsList = data || [];
      orderFormGroup.get('itemList')?.patchValue(data);
    } catch (error) {
      console.error('Error loading pending indent items:', error);
    }
  }

  async getPendingOrderItemss(orderId: any, dispatchId: any): Promise<any> {
    return (
      await this.dispatchService.getPendingOrderItems(orderId, dispatchId)
    ).toPromise();
  }

  removeAllDispatchOrder() {
    // Check if there are any rows
    if (this.dispatchOrder.length > 0) {
      // Clear all items in the FormArray
      this.dispatchOrder.clear();
      this.addDispatchOrder(0);
    } else {
      this.notificationsService.showNotification(
        'No items to remove.',
        'snack-bar-warning'
      );
    }
  }

  reset() {
    this.dispatchForm.get('code')?.patchValue('');
  }

  // async getVehiclesList() {
  //   let vehicleForm = {};
  //   (await this.vehicleService.getAllVehicle(vehicleForm)).subscribe((data: any) => {
  //     this.vehicleList = data.item1;
  //   });
  // }
  async getVehiclesList(event: any) {
    const filter = event.currentTarget.value;
    this.vehicleList = []; // Empty the list before updating
    (await this.vehicleService.getVehicleByName(filter)).subscribe(
      (data: any) => {
        this.vehicleList = data || []; // Ensure it's an array even if no data is returned
      },
      (error) => {
        console.error('Error fetching vehicle list:', error);
        this.vehicleList = []; // Reset in case of an error
      }
    );
  }

  validateQuantity(i: number, j: number): void {
    const dispatchDetails = this.dispatchDetail(i); // FormArray
    const currentRow = dispatchDetails.at(j);
    const quantityControl = currentRow.get('quantity');
    const currentQuantity = quantityControl?.value ?? 0;
    const orderedQuantity = currentRow.get('orderedQuantity')?.value ?? 0;
    const orderItemId = currentRow.get('orderItemId')?.value;

    // Sum all other rows' quantities with the same orderItemId (excluding current row)
    let totalOtherRowsQuantity = 0;
    dispatchDetails.controls.forEach((ctrl, index) => {
      if (index !== j && ctrl.get('orderItemId')?.value === orderItemId) {
        const qty = ctrl.get('quantity')?.value ?? 0;
        totalOtherRowsQuantity += qty;
      }
    });

    const totalPlannedQuantity = totalOtherRowsQuantity + currentQuantity;

    if (totalPlannedQuantity > orderedQuantity) {
      const allowedCurrentQty = Math.max(orderedQuantity - totalOtherRowsQuantity, 0);

      this.notificationsService.showNotification(
        'Shipped quantity cannot exceed the total ordered quantity for this item.',
        'snack-bar-warning'
      );

      quantityControl?.patchValue(allowedCurrentQty);
    }

    this.calculateWeight(i);
  }

  calculateWeight(index: number) {
    const dispatchOrderGroup = this.dispatchOrder.at(index) as FormGroup;
    const dispatchDetailArray = dispatchOrderGroup.get(
      'dispatchDetail'
    ) as FormArray;

    let totalWeight = 0;
    let totalAmount = 0;
    let totalQuantity = 0;
    dispatchDetailArray.controls.forEach((detailControl) => {
      const weight = detailControl.get('weight')?.value;
      const amount = detailControl.get('amount')?.value;
      const quantity = detailControl.get('quantity')?.value;
      if (weight && quantity && amount) {
        totalWeight += weight * quantity;
        totalAmount += amount * quantity;
        totalQuantity += quantity;
      }
    });

    dispatchOrderGroup.patchValue({
      totalWeight: totalWeight,
      totalAmount: totalAmount,
      totalQuantity: totalQuantity,
    });

    dispatchDetailArray.valueChanges.subscribe(() => {
      totalWeight = 0;
      totalAmount = 0;
      totalQuantity = 0;
      dispatchDetailArray.controls.forEach((detailControl) => {
        const weight = detailControl.get('weight')?.value;
        const amount = detailControl.get('amount')?.value;
        const quantity = detailControl.get('quantity')?.value;
        if (weight && quantity && amount) {
          totalWeight += weight * quantity;
          totalAmount += amount * quantity;
        }
      });

      dispatchOrderGroup.patchValue({
        totalWeight: totalWeight,
        totalAmount: totalAmount,
        totalQuantity: totalQuantity,
      });
    });

    this.calculateGrandTotal();

    // let test = totalWeight / this.grandTotalWeight;
    // let freight = test * this.dispatchForm.get('freightCharges')?.value;

    // dispatchOrderGroup.patchValue({
    //   freight: freight,
    // });
  }

  calculateGrandTotal() {
    let gTotalWeight = 0;
    let gTotalAmount = 0;
    let gTotalQuantity = 0;

    // Loop through all dispatch orders
    this.dispatchOrder.controls.forEach((dispatchOrderGroup) => {
      const totalWeight = dispatchOrderGroup.get('totalWeight')?.value || 0;
      const totalAmount = dispatchOrderGroup.get('totalAmount')?.value || 0;
      const totalQuantity = dispatchOrderGroup.get('totalQuantity')?.value || 0;

      // Add to the grand totals
      gTotalWeight += totalWeight;
      gTotalAmount += totalAmount;
      gTotalQuantity += totalQuantity;
    });

    // Update the grand total form fields (if you have them in the form)
    this.dispatchForm.patchValue({
      grandTotalWeight: gTotalWeight,
      grandTotalAmount: gTotalAmount,
      grandTotalQuantity: gTotalQuantity
    });

    this.grandTotalAmount = gTotalAmount;
    this.grandTotalWeight = gTotalWeight;
    this.grandTotalQuantity = gTotalQuantity;

    this.calculateFreightForEachOrder();
  }

  calculateFreightForEachOrder() {
    this.dispatchOrder.controls.forEach((dispatchOrderGroup) => {
      const totalWeight = dispatchOrderGroup.get('totalWeight')?.value || 0;

      let test = totalWeight / this.grandTotalWeight;
      let freight = test * this.dispatchForm.get('freightCharges')?.value;

      dispatchOrderGroup.get('orderFreightCharges')?.patchValue(freight);
    });
  }

  onVehicleSelected(event: MatAutocompleteSelectedEvent): void {
    const selectedValue = event.option.value;

    if (!selectedValue) {
      console.error(
        'Option value is undefined. Ensure mat-option [value] is correctly bound.'
      );
      return;
    }

    const selectedItem = this.getVehicle(selectedValue.id);
    if (!selectedItem) {
      console.error('Selected item not found.');
      return;
    }

    this.dispatchForm.get('vehicleId')?.patchValue(selectedValue.id);
    this.dispatchForm.get('vehicleName')?.patchValue(selectedValue.vehicleName + ' : ' + selectedValue.registrationNumber);
    this.dispatchForm.get('vehicle')?.patchValue(selectedValue);
  }

  getVehicle(vehicleId: any) {
    return this.vehicleList.find((option: { id: any; }) => option.id === vehicleId);
  }
  
  onCostSheetSelected(event: MatSelectChange, rowIndex: number, parentIndex: number): void {
    const selectedCostSheetId = event.value;

    const costSheetList = this.getIndexValueDetail(rowIndex, parentIndex)?.costSheetList ?? [];
    const chosen = costSheetList.find((cs: { id: any }) => cs.id === selectedCostSheetId);

    const dispatchDetailArray = this.dispatchDetail(parentIndex);
    const currentFormGroup = dispatchDetailArray.at(rowIndex) as FormGroup;

    const currentOrderItemId = currentFormGroup.get('orderItemId')?.value;

    // Check for duplicates in other rows
    const isDuplicate = dispatchDetailArray.controls
      .filter((_, i) => i !== rowIndex) // skip current row
      .some((control: AbstractControl) => {
        const formGroup = control as FormGroup;
        return (
          formGroup.get('orderItemId')?.value === currentOrderItemId &&
          formGroup.get('costSheetId')?.value === selectedCostSheetId
        );
      });

    if (isDuplicate) {
      this.notificationsService.showNotification(
        'This item / cost‑sheet combination has already been selected.',
        'snack-bar-danger'
      );

      // Reset costSheetId and pending quantity
      currentFormGroup.get('costSheetId')?.patchValue(null);
      currentFormGroup.get('pendingCostSheetQuantity')?.patchValue(null);
      return;
    }

    // If no duplicate, set pending quantity
    currentFormGroup.get('pendingCostSheetQuantity')?.patchValue(
      chosen?.quantity ?? null
    );

    // If no duplicate, set pending quantity
    currentFormGroup.get('rate')?.patchValue(
      chosen?.costPerPet ?? null
    );
  }

  getprojectList() {
    let _projectFilter: any = {};
    this.projectService.getAllProjects(_projectFilter).subscribe((data: any) => {
      this.projectList = data.item1;
    });
  }


}
