import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { AccountSubcategoryService } from '../accountsubcategory.service';
import { AccountCategoryService } from '../../accountcategory/accountcategory.service';

@Component({
    selector: 'app-add-accountsubcategory',
    templateUrl: './add-accountsubcategory.component.html',
    styleUrl: './add-accountsubcategory.component.css',
    standalone: false
})

export class AddAccountSubcategoryComponent {
  accountsubcategoryForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  categoryList :any;

  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private accountsubcategoryService: AccountSubcategoryService, private categoryService: AccountCategoryService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) {
    
   }

  ngOnInit(): void {
    this.accountsubcategoryForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      name: ['', Validators.required],
      description: [''],
      accountCategoryId: ['', Validators.required],
      companyId: [0],
    });
    
    this.LoadData(this.data.element);
    this.getcategoryList();

  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.accountsubcategoryForm);
    }
  }

  SaveData() {
    if (this.accountsubcategoryForm.invalid) {
      this.constantService.markFormGroupTouched(this.accountsubcategoryForm);
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.accountsubcategoryForm.value);

    this.accountsubcategoryService.saveAccountSubcategory(_clienttemperatureForm).subscribe({
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

  getAccountSubcategoryCode() {
    var AccountCategoryId =  this.accountsubcategoryForm.get('accountCategoryId')?.value;
    var Id =  this.accountsubcategoryForm.get('id')?.value;
    this.accountsubcategoryService.getAccountSubcategoryCode(AccountCategoryId,Id).subscribe((data: any) => {
      this.accountsubcategoryForm.get('code')?.patchValue(data.code);
      console.log(data.code);
      console.log(this.accountsubcategoryForm.get('code')?.value);
    });
  }
}
