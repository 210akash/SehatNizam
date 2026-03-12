import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { CategoryService } from '../category.service';
import { StoreService } from '../../store/store.service';
import { MatSlideToggleChange } from '@angular/material/slide-toggle';

@Component({
    selector: 'app-add-category',
    templateUrl: './add-category.component.html',
    styleUrl: './add-category.component.css',
    standalone: false
})

export class AddCategoryComponent {
  categoryForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  storeList: any;
  selectedRolls: any;

  constructor(private storeService : StoreService ,private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private categoryService: CategoryService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.categoryForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      name: ['', Validators.required],
      description: ['', Validators.required],
      companyId: [0],
      storeIds: [0],
      fixedAsset: [false, Validators.required]
    });
    
    this.LoadData(this.data.element);
    this.getStoreList();

  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.categoryForm);
    }
    else   
     this.getCategoryCode();
  }

  SaveData() {
    if (this.categoryForm.invalid) {
      this.constantService.markFormGroupTouched(this.categoryForm);
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.categoryForm.value);

    this.categoryService.saveCategory(_clienttemperatureForm).subscribe({
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

  getCategoryCode() {
    this.categoryService.getCategoryCode().subscribe((data: any) => {
      this.categoryForm.get('code')?.patchValue(data.code);
      console.log(data.code);
      console.log(this.categoryForm.get('code')?.value);
    });
  }

  getStoreList() {
    var fixedAsset =  this.categoryForm.get('fixedAsset')?.value;
    this.storeService.getStoreByCompany(0,fixedAsset).subscribe((data) => {
     this.storeList = data;
    });
  }

  async fixedAssetChange(event: MatSlideToggleChange) {
    this.categoryForm.get('fixedAsset')?.patchValue(event.checked);
    this.getStoreList();
  }
}
