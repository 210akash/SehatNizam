import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { RetailOrderService } from '../retail-order.service';

@Component({
  selector: 'app-delete-retail-order',
  templateUrl: './delete-retail-order.component.html',
  styleUrls: ['./delete-retail-order.component.css'], standalone: false
})

export class DeleteRetailOrderComponent implements OnInit {
  deleteShopOrderForm!: FormGroup;
  isLoading = false;
  dataSource!: any;
  roleList: any;

  constructor(private notificationsService: NotificationsService, private dialog: MatDialog, private retailOrderService: RetailOrderService,
    private formBuilder: FormBuilder, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.deleteShopOrderForm = this.formBuilder.group({
      id: [0],
      name: [''],
      description: [''],
      coordinates: ['']
    });

    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    this.constantService.LoadData(element, this.deleteShopOrderForm);
  }

  async delete() {
    (await this.retailOrderService.deleteRetailOrder(this.data.element.id)).subscribe({
      next: (data) => {
        if (data.Status == 200) {
          this.isLoading = false;
          this.notificationsService.showNotification(data.Message, 'snack-bar-success');
          this.dialog.closeAll();
        }
        else {
          this.isLoading = false;
          this.notificationsService.showNotification(data.Message, 'snack-bar-danger');
        }
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }


}