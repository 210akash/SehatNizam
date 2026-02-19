import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { AccountCategoryService } from '../accountcategory.service';
import { AccountHeadService } from '../../accounthead/accounthead.service';

@Component({
    selector: 'app-add-accountcategory',
    templateUrl: './add-accountcategory.component.html',
    styleUrl: './add-accountcategory.component.css',
    standalone: false
})

export class AddAccountCategoryComponent {
  accountcategoryForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  storeList: any;
  selectedRolls: any;
  accountHeadList : any;

  constructor(
    private accountHeadService: AccountHeadService,
    private dialog: MatDialog, 
    private notificationsService: NotificationsService, 
    private formBuilder: FormBuilder, 
    private accountcategoryService: AccountCategoryService, 
    private constantService: ConstantService, 
    @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.accountcategoryForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      name: ['', Validators.required],
      description: ['', Validators.required],
      accountHeadId: ['', Validators.required],
      companyId: [0],
    });
    
    this.LoadData(this.data.element);
    this.getaccountHeadList();
  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.accountcategoryForm);
    }
    else   
     this.getAccountCategoryCode();
  }

  SaveData() {
    if (this.accountcategoryForm.invalid) {
      this.constantService.markFormGroupTouched(this.accountcategoryForm);
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.accountcategoryForm.value);

    this.accountcategoryService.saveAccountCategory(_clienttemperatureForm).subscribe({
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

  getAccountCategoryCode() {
    this.accountcategoryService.getAccountCategoryCode().subscribe((data: any) => {
      this.accountcategoryForm.get('code')?.patchValue(data.code);
      console.log(data.code);
      console.log(this.accountcategoryForm.get('code')?.value);
    });
  }

  getaccountHeadList() {
    let _CategoryFilter: any = {};
    this.accountHeadService.getAllAccountHeads(_CategoryFilter).subscribe((data: any) => {
     this.accountHeadList = data.item1;
    });
  }
}
