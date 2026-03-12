import { Component, Inject } from '@angular/core';
import { AbstractControl, FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { GRNService } from '../grn.service';
import { DepartmentService } from '../../department/department.service';
import { MatOptionSelectionChange } from '@angular/material/core';
import { PurchaseOrderService } from '../../purchaseorder/purchaseorder.service';
import { firstValueFrom, Observable } from 'rxjs';
import { RowService } from '../../row/row.service';
import { RackService } from '../../rack/rack.service';
import { SectionService } from '../../section/section.service';
import { CostSheetService } from '../../costsheet/costsheet.service';

@Component({
  selector: 'app-add-grn',
  templateUrl: './add-grn.component.html',
  styleUrl: './add-grn.component.css',
  standalone: false
})

export class AddGRNComponent {
  grnForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  inspectionList: any[] = [];
  itemList: any[] = [];
  isdataload: boolean = false;
  yesterday = new Date();
  rackList : any;
  rowList: any;
  sectionList: any;
  costSheetList : any;
  selectedInspection: any;
  constructor(private costSheetService: CostSheetService,private sectionService: SectionService,private rowService: RowService,private rackService: RackService,private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private grnService: GRNService, private inspectionService: PurchaseOrderService, private departmentService: DepartmentService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.yesterday.setDate(this.yesterday.getDate() - 0);
    this.grnForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      createdDate: [new Date()],
      inspectionId: [0, Validators.required],
      status: [''],
      statusName: ['New'],
      statusId: [1],
      remarks: [''],
      grnDetail: this.formBuilder.array([])
    });


    this.LoadData(this.data.element);
    this.getRackList();
  }

  get grnDetail(): FormArray {
    return this.grnForm.get('grnDetail') as FormArray;
  }

  addGRNDetail(index: number) {
    const newDetailGroup = this.formBuilder.group({
      id: [0],
      grnId: [0],
      inspectionDetailId: [0, Validators.required],
      approved: [0, Validators.required],
      received: [0, Validators.required],
      expireDate: [new Date(), Validators.required],
      refernace: [''],
      sectionId: [null],
      costSheetId: [null],
      costSheetList: [[]],
      rowId : [null],
      rackId: [null],
      rowList: [[]],
      sectionList: [[]],

    });

    this.grnDetail.insert(index + 1, newDetailGroup);
  }

  removeGRNDetail(index: number) {
    if (this.grnDetail.length > 1) {
      this.grnDetail.removeAt(index);
    } else {
      this.notificationsService.showNotification(
        'At least one item is required.',
        'snack-bar-danger'
      );
    }
  }

  getIndexValue(index: number): any {
    const detailControl = (this.grnForm.get('grnDetail') as FormArray).at(index);
    return detailControl?.value;
  }

  async LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.grnForm);
    this.getPendingInspection(this.data.element.inspectionId);
      await this.getInspectionDetails();
      const detailsArray = this.grnForm.get('grnDetail') as FormArray;
      detailsArray.clear();
      if (element.grnDetail && element.grnDetail.length > 0) {
        element.grnDetail.forEach(async (detail: any) => {
          await this.getCostSheetByItem(detail.inspectionDetail?.igpDetail?.purchaseOrderDetail?.purchaseDemandDetail?.item.id,detail.costSheetId);
          const detailGroup = this.formBuilder.group({
            id: [detail.id],
            grnId: [detail.grnId],
            inspectionDetailId: [detail.inspectionDetailId, Validators.required],
            item: [detail.inspectionDetail?.igpDetail?.purchaseOrderDetail?.purchaseDemandDetail?.item, Validators.required],
            rowList: [[]],
            sectionList: [[]],
            costSheetId: [detail.costSheetId],
            costSheetList: [this.costSheetList],
            approved: [detail.inspectionDetail?.approved , Validators.required],
            received: [detail.received, Validators.required],
            expireDate: [detail.expireDate, Validators.required],
            refernace: [detail.refernace],
            sectionId: [detail.sectionId ?? null],
            rowId : [detail.section?.rowId ?? null],
            rackId: [detail.section?.row?.rackId ?? null],
          });
          detailsArray.push(detailGroup);
        });
      }
    } else {
      this.getGRNCode();
      this.grnForm.get('createdDate')?.patchValue(this.constantService.formatDate(new Date()));
      this.addGRNDetail(0);
      this.getPendingInspection(0);
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
    if (this.grnForm.invalid) {
      this.constantService.markFormGroupTouched(this.grnForm);
      this.checkInvalidControls(this.grnForm);
      this.notificationsService.showNotification('Please Fill Required Fields', 'snack-bar-danger');
      return;
    }

    this.isLoading = true;
    let _grnFormForm: any = {};
    _grnFormForm = Object.assign(_grnFormForm, this.grnForm.value);

    this.grnService.saveGRN(_grnFormForm).subscribe({
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

  getGRNCode() {
    this.grnService.getGRNCode().subscribe((data: any) => {
      this.grnForm.get('code')?.patchValue(data.code);
    });
  }

  isUpdating = false;

  async onItemSelected(event: MatOptionSelectionChange, index: number): Promise<void> {

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

    const duplicateItem = this.grnDetail.controls
      .filter((control: AbstractControl, controlIndex: number) => controlIndex !== index)
      .some((control: AbstractControl) => {
        const formGroup = control as FormGroup;
        return formGroup.get('inspectionDetailId')?.value === selectedValue;
      });

    if (duplicateItem) {
      this.notificationsService.showNotification('This item has already been selected.', 'snack-bar-danger');
      const currentFormGroup = this.grnDetail.at(index) as FormGroup;
      currentFormGroup.get('inspectionDetailId')?.patchValue('0');
      currentFormGroup.get('item')?.patchValue(null);
      currentFormGroup.get('approved')?.patchValue(null);
       currentFormGroup.get('received')?.patchValue(0);
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

  if (
  selectedItem?.igpDetail?.purchaseOrderDetail?.purchaseDemandDetail?.item?.itemType?.subCategory?.category?.categoryStores?.some(
    (    store: { storeId: number; isActive: any; }) => store.storeId === 3 && store.isActive
  )
) {
  await this.getCostSheetListByItem(
    index,
    selectedItem?.igpDetail?.purchaseOrderDetail?.purchaseDemandDetail?.item.id
  );
}
      const detailFormGroup = this.grnDetail.at(index) as FormGroup;
      detailFormGroup.get('inspectionDetailId')?.patchValue(selectedItem?.id);
      detailFormGroup.get('item')?.patchValue(selectedItem?.igpDetail?.purchaseOrderDetail?.purchaseDemandDetail?.item);
      detailFormGroup.get('approved')?.patchValue(selectedItem.approved);
      detailFormGroup.get('received')?.setValidators([Validators.required, Validators.min(1), Validators.max(selectedItem.approved)]);
      detailFormGroup.updateValueAndValidity();
      this.isUpdating = false;
    }
  }

  validateQty(index: number): any {
    const detailControl = (this.grnForm.get('grnDetail') as FormArray).at(index);
    if (detailControl?.value.received > detailControl?.value.approved) {
      detailControl.get('received')?.patchValue(detailControl?.value.approved);
    }
  }

  getItemData(itemId: string) {
    return this.itemList.find(x => x.id === itemId);
  }

  getPOData() {
    const inspectionId = this.grnForm.get('inspectionId')?.value;
    return this.inspectionList.find(x => x.id === inspectionId);
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
    const grnDetailArray = this.grnForm.get('grnDetail') as FormArray;
    if (!grnDetailArray || index < 0 || index >= grnDetailArray.length) {
      console.error('Invalid index or FormArray is not initialized properly.');
      return;
    }

    const currentFormGroup = grnDetailArray.at(index) as FormGroup;
    currentFormGroup.reset();
    return;
  }

  getPendingInspection(inspectionId:any) {
    this.grnService.getPendingInspection(inspectionId).subscribe((data: any) => {
      this.inspectionList = data;
      const selectedItem = this.inspectionList.find((item: any) => item.id === inspectionId);
     this.selectedInspection = selectedItem;
    });
  }

  removeAllgrnDetail() {
    if (this.grnDetail.length > 0) {
      this.grnDetail.clear();
      this.addGRNDetail(0);
    } else {
      this.notificationsService.showNotification('No items to remove.', 'snack-bar-warning');
    }
  }

  reset() {
    this.grnForm.get('code')?.patchValue('');
  }

  async getInspectionDetails(): Promise<void> {
    const inspectionId = this.grnForm.get('inspectionId')?.value;
    const IgpId = this.grnForm.get('id')?.value;

    const selectedItem = this.inspectionList.find((item: any) => item.id === inspectionId);
    this.selectedInspection = selectedItem;

    try {
      const data = await (await this.grnService.getPendingInspectionItems(inspectionId,IgpId)).toPromise();
      this.itemList = data;
    } catch (error) {
      console.error('Error fetching pending purchase order items:', error);
    }
  }

  async getRackList() {
    let _rackForm: any = {};
    (await this.rackService.getAllRack(_rackForm)).subscribe((data) => {
     this.rackList = Object.values(data.item1 ?? {});
    });
  }

  async getRowListById(index: number) {
    const grnDetailArray = this.grnForm.get('grnDetail') as FormArray;
    if (grnDetailArray && grnDetailArray.at(index)) {
      const grnDetailGroup = grnDetailArray.at(index) as FormGroup;
      const rackId = grnDetailGroup.get('rackId')?.value;
      grnDetailGroup.get('rowList')?.patchValue([]);
      grnDetailGroup.get('sectionList')?.patchValue([]);
      if (rackId !== undefined) {
        (await this.rowService.getRowByRackId(rackId)).subscribe((data: any) => {
          grnDetailGroup.get('rowList')?.patchValue(data);
        });
      } else {
        console.error('rackId not found at the given index');
      }
    } else {
      console.error('No detail found at the given index');
    }
  }

  async getSectionListById(index: number) {
    const grnDetailArray = this.grnForm.get('grnDetail') as FormArray;
    if (grnDetailArray && grnDetailArray.at(index)) {
      const grnDetailGroup = grnDetailArray.at(index) as FormGroup;
      const rackId = grnDetailGroup.get('rowId')?.value;
        grnDetailGroup.get('sectionList')?.patchValue([]);
      if (rackId !== undefined) {
        (await this.sectionService.getSectionByRowId(rackId)).subscribe((data: any) => {
          grnDetailGroup.get('sectionList')?.patchValue(data);
        });
      } else {
        console.error('rackId not found at the given index');
      }
    } else {
      console.error('No detail found at the given index');
    }
  }

  async getCostSheetListByItem(index: number,itemId : any) {
    const grnDetailArray = this.grnForm.get('grnDetail') as FormArray;
    if (grnDetailArray && grnDetailArray.at(index)) {

      const grnDetailGroup = grnDetailArray.at(index) as FormGroup;
      const costSheetControl = grnDetailGroup.get('costSheetId');
      var costSheetId = costSheetControl?.value ?? 0;

      grnDetailGroup.get('costSheetList')?.patchValue([]);
      
      if (itemId !== undefined) {
        var data  = await this.getCostSheetByItem(itemId,0);
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

  async getCostSheetByItem(itemId: any,costSheetId : any) {
    try {
      this.costSheetList = [];
      // Use firstValueFrom to convert the observable to a promise
      const data = await firstValueFrom(await this.grnService.getPendingCostSheet(itemId, costSheetId));
      this.costSheetList = data || [];
      return this.costSheetList;
    } catch (error) {
      console.error('Error loading pending indent items:', error);
    }
  }

  
}
