import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { SaleMaterialReturnService } from '../salematerialreturn.service';

@Component({
  selector: 'app-delete-salematerialreturn',
  templateUrl: './delete-salematerialreturn.component.html',
  styleUrl: './delete-salematerialreturn.component.css',
  standalone: false
})

export class DeleteSaleMaterialReturnComponent {
  saleMaterialReturnForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  grandTotals: any;
  qtyTotals: any;
  constructor(private dialog: MatDialog, private formBuilder: FormBuilder, private notificationsService: NotificationsService, private saleMaterialReturnService: SaleMaterialReturnService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.updateTotals();
  }

   updateTotals(): void {
  const details = this.data?.element?.saleMaterialReturnDetail || [];
  let totalQty = 0;

  for (const detail of details) {
    const qty = detail.quantity || 0;
    totalQty += qty;
  }

  this.qtyTotals = totalQty;
}

  async delete() {
    (await this.saleMaterialReturnService.deleteSaleMaterialReturn(this.data.element.id)).subscribe({
      next: (data) => {
        if (data == true) {
          this.isLoading = false;
          this.notificationsService.showNotification('Delete Successfully', 'snack-bar-success');
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
