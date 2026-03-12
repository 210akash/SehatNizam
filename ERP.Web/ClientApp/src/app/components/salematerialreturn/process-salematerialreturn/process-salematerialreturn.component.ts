import { Component, Inject } from '@angular/core';
import { FormGroup} from '@angular/forms';
import { ConstantService } from '../../../Service/constant.service';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { SaleMaterialReturnService } from '../salematerialreturn.service';

@Component({
    selector: 'app-process-salematerialreturn',
    templateUrl: './process-salematerialreturn.component.html',
    styleUrl: './process-salematerialreturn.component.css',
    standalone: false
})

export class ProcessSaleMaterialReturnComponent {
  saleMaterialReturnForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  qtyTotals : any;
  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private saleMaterialReturnService: SaleMaterialReturnService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

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

  async process() {
    (await this.saleMaterialReturnService.processSaleMaterialReturn(this.data.element.id)).subscribe({
      next: (data) => {
        if (data == true) {
          this.isLoading = false;
          this.notificationsService.showNotification('Process Successfully', 'snack-bar-success');
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
