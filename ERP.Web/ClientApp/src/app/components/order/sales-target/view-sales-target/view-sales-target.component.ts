import { DatePipe } from '@angular/common';
import { Component, OnInit, Inject } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { SalesTargetService } from '../sales-target.service';

@Component({
  selector: 'app-view-sales-target',
  templateUrl: './view-sales-target.component.html',
  styleUrls: ['./view-sales-target.component.css'], standalone: false,
  providers: [DatePipe]  // Add DatePipe to the providers array
})

export class ViewSalesTargetComponent implements OnInit {
  viewSalesTarget!: FormGroup;
  isLoading = false;

  salesData: any;

  constructor(private salesTargetService: SalesTargetService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {

    this.getSalesTargetByZoneId(this.data.element?.user?.userTerritory[0]?.zoneId, this.data.element.targetMonth);
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


}
