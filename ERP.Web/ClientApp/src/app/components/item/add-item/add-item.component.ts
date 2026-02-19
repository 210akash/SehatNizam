import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { ItemService } from '../item.service';
import { CategoryService } from '../../category/category.service';
import { SubcategoryService } from '../../subcategory/subcategory.service';
import { ItemtypeService } from '../../itemtype/itemtype.service';
import { UomService } from '../../uom/uom.service';

@Component({
    selector: 'app-add-item',
    templateUrl: './add-item.component.html',
    styleUrl: './add-item.component.css',
    standalone: false
})

export class AddItemComponent {
  itemForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  categoryList :any;
  subcategoryList :any;
  itemTypeList :any;
  UomList :any;

  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private itemService: ItemService, private subcategoryService: SubcategoryService, private itemtypeService: ItemtypeService,private uomService: UomService, private categoryService: CategoryService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.itemForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      name: ['', Validators.required],
      description: ['', Validators.required],
      categoryId: ['', Validators.required],
      subCategoryId: ['', Validators.required],
      itemTypeId: ['', Validators.required],
      uomId: ['', Validators.required],
      recordLevel: [0, Validators.required],
      leadTime: [0, Validators.required],
      rate : [0, Validators.required],
      weight : [0, Validators.required],
      length : [0, Validators.required],
      height : [0, Validators.required],
      width : [0, Validators.required],
      model : [0, Validators.required],
      make : [0, Validators.required],
      excessQtyPer : [0, Validators.required],
      openingQty : [0, Validators.required],
      companyId: [0],
    });
    
    this.LoadData(this.data.element);
    this.getcategoryList();
    this.getuomList();
  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.itemForm);
      this.itemForm.get('categoryId')?.patchValue(element.itemType.subCategory.category.id);
      this.itemForm.get('subCategoryId')?.patchValue(element.itemType.subCategory.id);
      this.getsubcategoryList();
      this.getItemTypeList();
    }
    // else   
    //  this.getItemCode();
  }

  SaveData() {
    if (this.itemForm.invalid) {
      this.constantService.markFormGroupTouched(this.itemForm);
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.itemForm.value);

    this.itemService.saveItem(_clienttemperatureForm).subscribe({
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

  getcategoryList() {
    let _CategoryFilter: any = {};
    this.categoryService.getAllCategorys(_CategoryFilter).subscribe((data: any) => {
     this.categoryList = data.item1;
    });
  }

  getsubcategoryList() {
    var CategoryId =  this.itemForm.get('categoryId')?.value;
    this.subcategoryService.getSubCategoryByCategory(CategoryId).subscribe((data: any) => {
     this.subcategoryList = data;
    });
  }

  getItemTypeList() {
    var subCategoryId =  this.itemForm.get('subCategoryId')?.value;
    this.itemtypeService.getItemtypeBySubCategory(subCategoryId).subscribe((data: any) => {
     this.itemTypeList = data;
    });
  }

  getuomList() {
    this.uomService.GetUOMByCompany(0).subscribe((data: any) => {
     this.UomList = data;
    });
  }

  getItemCode() {
    var ItemTypeId =  this.itemForm.get('itemTypeId')?.value;
    var Id =  this.itemForm.get('id')?.value;
    this.itemService.getItemCode(ItemTypeId,Id).subscribe((data: any) => {
      this.itemForm.get('code')?.patchValue(data.code);
    });
  }

 reset(){
  this.itemForm.get('code')?.patchValue('');
  this.itemTypeList = [] ;
 }
}
