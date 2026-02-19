import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { AccountService } from '../account.service';
import { AccountTypeService } from '../../accounttype/accounttype.service';
import { AccountCategoryService } from '../../accountcategory/accountcategory.service';
import { AccountSubcategoryService } from '../../accountsubcategory/accountsubcategory.service';
import { AccountFlowService } from '../../accountflow/accountflow.service';

@Component({
  selector: 'app-add-account',
  templateUrl: './add-account.component.html',
  styleUrl: './add-account.component.css',
  standalone: false
})

export class AddAccountComponent {
  accountForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  categoryList :any;
  subcategoryList :any;
  accountTypeList :any;
  accountFlowList :any;

  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private accountService: AccountService, private subcategoryService: AccountSubcategoryService, private accounttypeService: AccountTypeService, private categoryService: AccountCategoryService,
    private accountFlowService: AccountFlowService,
    private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.accountForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      name: ['', Validators.required],
      description: ['', Validators.required],
      accountCategoryId: ['', Validators.required],
      accountSubCategoryId: ['', Validators.required],
      accountTypeId: ['', Validators.required],
      accountFlowId: ['', Validators.required],
      opening: [0],
    });
    
    this.LoadData(this.data.element);
    this.getcategoryList();
    this.getaccountFlowList();
    // this.getAccountTypeList();
    // this.getAccountCode();
  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
 
      this.constantService.LoadData(element, this.accountForm);
      this.accountForm.get('accountCategoryId')?.patchValue(element.accountType.accountSubCategory.accountCategory.id);
      this.accountForm.get('accountSubCategoryId')?.patchValue(element.accountType.accountSubCategory.id);
      this.accountForm.get('accountTypeId')?.patchValue(element.accountType.id);
      this.getsubcategoryList();
      this.getAccountTypeList();
    }
  }

  SaveData() {
    if (this.accountForm.invalid) {
      this.constantService.markFormGroupTouched(this.accountForm);
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.accountForm.value);
    this.accountService.saveAccount(_clienttemperatureForm).subscribe({
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

  getaccountFlowList() {
    let _CategoryFilter: any = {};
    this.accountFlowService.getAllAccountFlows(_CategoryFilter).subscribe((data: any) => {
     this.accountFlowList = data.item1;
    });
  }

  getsubcategoryList() {
    var AccountCategoryId =  this.accountForm.get('accountCategoryId')?.value;
    this.subcategoryService.getSubCategoryByCategory(AccountCategoryId).subscribe((data: any) => {
     this.subcategoryList = data;
    });
  }

  getAccountTypeList() {
    var accountTypeId =  this.accountForm.get('accountSubCategoryId')?.value;
    this.accounttypeService.getAccounttypeBySubCategory(accountTypeId).subscribe((data: any) => {
     this.accountTypeList = data;
    });
  }

  getAccountCode() {
    var AccountTypeId =  this.accountForm.get('accountTypeId')?.value;
    var Id =  this.accountForm.get('id')?.value;
    this.accountService.getAccountCode(AccountTypeId,Id).subscribe((data: any) => {
      this.accountForm.get('code')?.patchValue(data.code);
    });
  }

 reset(){
  this.accountForm.get('code')?.patchValue('');
  this.accountTypeList = [] ;
 }
}
