import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { DealershipService } from '../../order/dealership/dealership.service';
import { ConstantService } from '../../../Service/constant.service';
import { NotificationsService } from '../../../Service/notification.service';

@Component({
  selector: 'app-delete-customer',
  templateUrl: './delete-customer.component.html',
  styleUrls: ['./delete-customer.component.css'],
  standalone: false,
})
export class DeleteCustomerComponent implements OnInit {
  deleteCustomerForm!: FormGroup;
  isLoading = false;
  dialogRef: any;

  constructor(
    private notificationsService: NotificationsService,
    private dialog: MatDialog,
    private customerService: DealershipService,
    private formBuilder: FormBuilder,
    private constantService: ConstantService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) {}

  ngOnInit(): void {
    this.deleteCustomerForm = this.formBuilder.group({
      id: [0],
    });

    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    this.constantService.LoadData(element, this.deleteCustomerForm);
  }

  async delete() {
    (
      await this.customerService.deleteDealership(this.data.element.id)
    ).subscribe({
      next: (data) => {
        if (data.Status == 200) {
          this.isLoading = false;
          this.notificationsService.showNotification(
            data.Message,
            'snack-bar-success'
          );
          this.dialog.closeAll();
        } else {
          this.isLoading = false;
          this.notificationsService.showNotification(
            data.Message,
            'snack-bar-danger'
          );
        }
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      },
    });
  }
}
