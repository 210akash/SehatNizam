import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-retail-order-history',
  templateUrl: './retail-order-history.component.html',
  styleUrls: ['./retail-order-history.component.css'], standalone: false
})

export class RetailOrderHistoryComponent implements OnInit {
  isLoading = false;
  dataSource: any;

  displayedColumns: string[] = ['fromStatus', 'toStatus', 'comments', 'user', 'createdDate'];

  constructor(@Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.bindData(this.data.element?.retailOrderProcess);
  }

  bindData(element: any) {
    this.dataSource = new MatTableDataSource(element);
  }


}
