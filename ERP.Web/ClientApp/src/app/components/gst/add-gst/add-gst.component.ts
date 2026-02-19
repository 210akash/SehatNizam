import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { GSTService } from '../gst.service';
import { CategoryService } from '../../category/category.service';
import { SubcategoryService } from '../../subcategory/subcategory.service';

@Component({
    selector: 'app-add-gst',
    templateUrl: './add-gst.component.html',
    styleUrl: './add-gst.component.css',
    standalone: false
})

export class AddGSTComponent {
  gstForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  categoryList :any;
  subcategoryList :any;

  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private gstService: GSTService, private subcategoryService: SubcategoryService, private categoryService: CategoryService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.gstForm = this.formBuilder.group({
      id: [0],
      fDate: ['', Validators.required],
      tDate: ['', Validators.required],
      gSTPer:[0, Validators.required],
      companyId: [0],
    });
    
    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.gstForm);
    }
  }

  SaveData() {
    if (this.gstForm.invalid) {
      this.constantService.markFormGroupTouched(this.gstForm);
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.gstForm.value);
    let fDate = new Date(this.gstForm.get('fDate')?.value);
    _clienttemperatureForm['fDate'] = fDate.toLocaleDateString();
    let tDate = new Date(this.gstForm.get('tDate')?.value);
    _clienttemperatureForm['tDate'] = tDate.toLocaleDateString();

    this.gstService.saveGST(_clienttemperatureForm).subscribe({
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
