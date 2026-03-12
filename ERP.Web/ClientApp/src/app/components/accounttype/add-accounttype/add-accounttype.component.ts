import { Component, Inject } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { CategoryService } from '../../category/category.service';
import { SubcategoryService } from '../../subcategory/subcategory.service';
import { AccountTypeService } from '../accounttype.service';
import { AccountCategoryService } from '../../accountcategory/accountcategory.service';
import { AccountSubcategoryService } from '../../accountsubcategory/accountsubcategory.service';

@Component({
    selector: 'app-add-accounttype',
    templateUrl: './add-accounttype.component.html',
    styleUrl: './add-accounttype.component.css',
    standalone: false
})

export class AddAccountTypeComponent {
  accounttypeForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  categoryList :any;
  subcategoryList :any;


  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private accounttypeService: AccountTypeService, private subcategoryService: AccountSubcategoryService, private categoryService: AccountCategoryService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.accounttypeForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      name: ['', Validators.required],
      description: [''],
      accountCategoryId: ['', Validators.required],
      accountSubCategoryId: ['', Validators.required],
      companyId: [0],
    });
    
    this.LoadData(this.data.element);
    this.getcategoryList();

  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.accounttypeForm);
      this.accounttypeForm.get('accountCategoryId')?.patchValue(element.accountSubCategory.accountCategory.id);
      this.getsubcategoryList();
    }
    // else   
    //  this.getAccounttypeCode();
  }

  SaveData() {
    if (this.accounttypeForm.invalid) {
      this.constantService.markFormGroupTouched(this.accounttypeForm);
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.accounttypeForm.value);

    this.accounttypeService.saveAccounttype(_clienttemperatureForm).subscribe({
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
    this.categoryService.getAllAccountCategorys(_CategoryFilter).subscribe((data: any) => {
     this.categoryList = data.item1;
    });
  }

  getsubcategoryList() {
    var CategoryId =  this.accounttypeForm.get('accountCategoryId')?.value;
    this.subcategoryService.getSubCategoryByCategory(CategoryId).subscribe((data: any) => {
     this.subcategoryList = data;
    });
  }

  getAccounttypeCode() {
    var SubCategoryId =  this.accounttypeForm.get('accountSubCategoryId')?.value;
    var Id =  this.accounttypeForm.get('id')?.value;
    this.accounttypeService.getAccounttypeCode(SubCategoryId,Id).subscribe((data: any) => {
      this.accounttypeForm.get('code')?.patchValue(data.code);
      console.log(data.code);
      console.log(this.accounttypeForm.get('code')?.value);
    });
  }

 reset(){
  this.accounttypeForm.get('code')?.patchValue('');
 }
}
