import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-order-history',
  templateUrl: './order-history.component.html',
  styleUrls: ['./order-history.component.css'], standalone: false
})

export class OrderHistoryComponent implements OnInit {
  isLoading = false;
  dataSource: any;

  displayedColumns: string[] = ['fromStatus', 'toStatus', 'comments', 'user', 'createdDate'];

  constructor(@Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.bindData(this.data.element?.orderProcess);
  }

  bindData(element: any) {
    this.dataSource = new MatTableDataSource(element);
  }


}
