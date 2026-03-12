import { Component, OnInit, Inject } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { DatePipe } from '@angular/common';
import { SalesTargetService } from '../sales-target.service';
import { NotificationsService } from '../../../../Service/notification.service';

@Component({
  selector: 'app-delete-sales-target',
  templateUrl: './delete-sales-target.component.html',
  styleUrls: ['./delete-sales-target.component.css'], standalone: false,
  providers: [DatePipe]  // Add DatePipe to the providers array
})

export class DeleteSalesTargetComponent implements OnInit {
  deleteSalesTargetForm!: FormGroup;
  isLoading = false;

  salesData: any;

  constructor(private notificationsService: NotificationsService, private dialog: MatDialog, private salesTargetService: SalesTargetService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.getSalesTargetByZoneId(this.data.element.zoneId, this.data.element.targetMonth);
  }

  async getSalesTargetByZoneId(zoneId: any, targetMonth: any) {
    (await this.salesTargetService.getSalesTargetByZoneId(zoneId, targetMonth)).subscribe({
      next: (data: any[]) => {
        this.salesData = data[0];
        this.isLoading = false;
      },
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  async delete() {
    (await this.salesTargetService.deleteSalesTarget(this.data.element.zoneId, this.data.element.targetMonth)).subscribe({
      next: (data: { Status: number; }) => {
        if (data.Status == 200) {
          this.isLoading = false;
          this.notificationsService.showNotification('Sales Target Deleted Successfully!', 'snack-bar-success');
          this.dialog.closeAll();
        }
      },
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }


}