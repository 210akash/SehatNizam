import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { ItemService } from '../item.service';

@Component({
    selector: 'app-delete-item',
    templateUrl: './delete-item.component.html',
    styleUrl: './delete-item.component.css',
    standalone: false
})

export class DeleteItemComponent {
  itemForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;

  constructor(private dialog: MatDialog, private formBuilder: FormBuilder, private notificationsService: NotificationsService, private itemService: ItemService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.itemForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      name: ['', Validators.required],
      description: ['', Validators.required],
      categoryId: ['', Validators.required],
      subCategoryId: ['', Validators.required],
      itemTypeId: ['', Validators.required],
      uomId: ['', Validators.required],
      recordLevel: [0, Validators.required],
      leadTime: [0, Validators.required],
      rate : [0, Validators.required],
      weight : [0, Validators.required],
      length : [0, Validators.required],
      height : [0, Validators.required],
      width : [0, Validators.required],
      model : [0, Validators.required],
      make : [0, Validators.required],
      excessQtyPer : [0, Validators.required],
      companyId: [0],
    });
    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    if (this.data.element.id != null) {
      this.isEditMode = true;
    }
    this.constantService.LoadData(element, this.itemForm);
  }

  async delete() {
    (await this.itemService.deleteItem(this.data.element.id)).subscribe({
      next: (data) => {
        if (data == true) {
          this.isLoading = false;
          this.notificationsService.showNotification(data.Data, 'snack-bar-success');
          this.dialog.closeAll();
        }
      },
      error: (error) => {
        console.log(error);
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        this.isLoading = false;
      }
    });
  }
}
