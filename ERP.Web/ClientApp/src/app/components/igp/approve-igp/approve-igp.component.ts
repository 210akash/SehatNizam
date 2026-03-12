import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { IGPService } from '../igp.service';

@Component({
    selector: 'app-approve-igp',
    templateUrl: './approve-igp.component.html',
    styleUrl: './approve-igp.component.css',
    standalone: false
})

export class ApproveIGPComponent {
  isLoading = false;
  tAmount: any = 0;
  tDiscount: any = 0;
  tExpense: any = 0;
  tSaleTax: any = 0;
  partyAmount: any = 0;
  netAmount: any = 0;
  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private igpService: IGPService,  @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.calculateTotals();
  }

  async calculateTotals() {
    var _tAmount = 0;
    var _tSaleTax = 0;
    this.tExpense = this.data.element.deliveryCharges + this.data.element.otherCharges;
    this.tDiscount = this.data.element.discount;

    (this.data.element.purchaseOrderDetail).forEach((detail: any) => {
      // Access item and update the unitRate value
      var value = detail.value;
      _tAmount = _tAmount + value;
      var gst = detail.gst;
      _tSaleTax = _tSaleTax + gst;
    });
    this.tAmount = _tAmount;
    this.tSaleTax = _tSaleTax;
    this.netAmount = this.tAmount + this.tSaleTax + this.tExpense - this.tDiscount;
    this.partyAmount = (this.tAmount + this.tExpense) - this.tDiscount;
  }

  async approve() {
    (await this.igpService.approveIGP(this.data.element.id)).subscribe({
      next: (data) => {
        if (data == true) {
          this.isLoading = false;
          this.notificationsService.showNotification('Approved Successfully', 'snack-bar-success');
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
