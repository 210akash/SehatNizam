import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { ShipmentModeService } from '../shipmentmode.service';
import { CategoryService } from '../../category/category.service';
import { SubcategoryService } from '../../subcategory/subcategory.service';

@Component({
    selector: 'app-add-shipmentmode',
    templateUrl: './add-shipmentmode.component.html',
    styleUrl: './add-shipmentmode.component.css',
    standalone: false
})

export class AddShipmentModeComponent {
  shipmentmodeForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  categoryList :any;
  subcategoryList :any;


  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private shipmentmodeService: ShipmentModeService, private subcategoryService: SubcategoryService, private categoryService: CategoryService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.shipmentmodeForm = this.formBuilder.group({
      id: [0],
      name: ['', Validators.required],
      companyId: [0],
    });
    
    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.shipmentmodeForm);
    }
    // else   
    //  this.getShipmentModeCode();
  }

  SaveData() {
    if (this.shipmentmodeForm.invalid) {
      this.constantService.markFormGroupTouched(this.shipmentmodeForm);
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.shipmentmodeForm.value);

    this.shipmentmodeService.saveShipmentMode(_clienttemperatureForm).subscribe({
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
}
