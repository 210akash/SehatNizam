import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { SubcategoryService } from '../subcategory.service';
import { CategoryService } from '../../category/category.service';

@Component({
    selector: 'app-add-subcategory',
    templateUrl: './add-subcategory.component.html',
    styleUrl: './add-subcategory.component.css',
    standalone: false
})

export class AddSubcategoryComponent {
  subcategoryForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  categoryList :any;

  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private subcategoryService: SubcategoryService, private categoryService: CategoryService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) {
    
   }

  ngOnInit(): void {
    this.subcategoryForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      name: ['', Validators.required],
      description: ['', Validators.required],
      categoryId: ['', Validators.required],
      companyId: [0],
    });
    
    this.LoadData(this.data.element);
    this.getcategoryList();

  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.subcategoryForm);
    }
    // else   
    //  this.getSubcategoryCode();
  }

  SaveData() {
    if (this.subcategoryForm.invalid) {
      this.constantService.markFormGroupTouched(this.subcategoryForm);
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.subcategoryForm.value);

    this.subcategoryService.saveSubcategory(_clienttemperatureForm).subscribe({
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

  getSubcategoryCode() {
    var CategoryId =  this.subcategoryForm.get('categoryId')?.value;
    var Id =  this.subcategoryForm.get('id')?.value;
    this.subcategoryService.getSubcategoryCode(CategoryId,Id).subscribe((data: any) => {
      this.subcategoryForm.get('code')?.patchValue(data.code);
      console.log(data.code);
      console.log(this.subcategoryForm.get('code')?.value);
    });
  }
}
