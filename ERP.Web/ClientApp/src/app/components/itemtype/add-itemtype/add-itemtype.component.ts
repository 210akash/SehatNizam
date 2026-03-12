import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { ItemtypeService } from '../itemtype.service';
import { CategoryService } from '../../category/category.service';
import { SubcategoryService } from '../../subcategory/subcategory.service';

@Component({
    selector: 'app-add-itemtype',
    templateUrl: './add-itemtype.component.html',
    styleUrl: './add-itemtype.component.css',
    standalone: false
})

export class AddItemtypeComponent {
  itemtypeForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  categoryList :any;
  subcategoryList :any;


  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private itemtypeService: ItemtypeService, private subcategoryService: SubcategoryService, private categoryService: CategoryService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.itemtypeForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      name: ['', Validators.required],
      description: ['', Validators.required],
      categoryId: ['', Validators.required],
      subCategoryId: ['', Validators.required],
      companyId: [0],
    });
    
    this.LoadData(this.data.element);
    this.getcategoryList();

  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.itemtypeForm);
      this.itemtypeForm.get('categoryId')?.patchValue(element.subCategory.category.id);
      this.getsubcategoryList();
    }
    // else   
    //  this.getItemtypeCode();
  }

  SaveData() {
    if (this.itemtypeForm.invalid) {
      this.constantService.markFormGroupTouched(this.itemtypeForm);
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.itemtypeForm.value);

    this.itemtypeService.saveItemtype(_clienttemperatureForm).subscribe({
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
    var CategoryId =  this.itemtypeForm.get('categoryId')?.value;
    this.subcategoryService.getSubCategoryByCategory(CategoryId).subscribe((data: any) => {
     this.subcategoryList = data;
    });
  }

  getItemtypeCode() {
    var SubCategoryId =  this.itemtypeForm.get('subCategoryId')?.value;
    var Id =  this.itemtypeForm.get('id')?.value;
    this.itemtypeService.getItemtypeCode(SubCategoryId,Id).subscribe((data: any) => {
      this.itemtypeForm.get('code')?.patchValue(data.code);
      console.log(data.code);
      console.log(this.itemtypeForm.get('code')?.value);
    });
  }

 reset(){
  this.itemtypeForm.get('code')?.patchValue('');
 }
}
