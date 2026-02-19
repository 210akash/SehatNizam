import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { PaymentModeService } from '../paymentmode.service';
import { CategoryService } from '../../category/category.service';
import { SubcategoryService } from '../../subcategory/subcategory.service';

@Component({
    selector: 'app-add-paymentmode',
    templateUrl: './add-paymentmode.component.html',
    styleUrl: './add-paymentmode.component.css',
    standalone: false
})

export class AddPaymentModeComponent {
  paymentmodeForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  categoryList :any;
  subcategoryList :any;


  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private paymentmodeService: PaymentModeService, private subcategoryService: SubcategoryService, private categoryService: CategoryService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.paymentmodeForm = this.formBuilder.group({
      id: [0],
      name: ['', Validators.required],
      companyId: [0],
    });
    
    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.paymentmodeForm);
    }
    // else   
    //  this.getPaymentModeCode();
  }

  SaveData() {
    if (this.paymentmodeForm.invalid) {
      this.constantService.markFormGroupTouched(this.paymentmodeForm);
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.paymentmodeForm.value);

    this.paymentmodeService.savePaymentMode(_clienttemperatureForm).subscribe({
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

 reset(){
  this.paymentmodeForm.get('code')?.patchValue('');
 }
}
