import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-cancel-dispatch-history',
  templateUrl: './cancel-dispatch-history.component.html',
  styleUrls: ['./cancel-dispatch-history.component.css'],
  standalone: false,
})
export class CancelDispatchHistoryComponent implements OnInit {
  isLoading = false;
  dataSource: any;

  displayedColumns: string[] = [
    'fromStatus',
    'toStatus',
    'comments',
    'isReject',
    'user',
    'createdDate',
  ];

  constructor(@Inject(MAT_DIALOG_DATA) public data: { element: any }) {}

  ngOnInit(): void {
    this.bindData(this.data.element?.orderProcess);
  }

  bindData(element: any) {
    this.dataSource = new MatTableDataSource(element);
  }
}
