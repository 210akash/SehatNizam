import { Component, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { MatTableDataSource } from '@angular/material/table';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ConstantService } from '../../../../Service/constant.service';
import { LabOrderService } from '../lab-order.service';
import { ViewLabOrderComponent } from '../view-lab-order/view-lab-order.component';
import { DeleteLabOrderComponent } from '../delete-lab-order/delete-lab-order.component';
import { Router } from '@angular/router';

@Component({
  selector: 'app-lab-order-list',
  templateUrl: './lab-order-list.component.html',
  styleUrls: ['./lab-order-list.component.css'],
  standalone: false
})
export class LabOrderListComponent implements OnInit {
  dataSource: any;
  form!: FormGroup;
  displayedColumns: string[] = ['appointmentId', 'labOrderTypeId', 'statusId', 'actions'];
  isLoading = false;
  currentPage = 0;
  pageSize = 10;
  @ViewChild(MatPaginator) paginator!: MatPaginator;

  constructor(private dialog: MatDialog, private fb: FormBuilder, private constantService: ConstantService, private service: LabOrderService, private router: Router) { }
  ngOnInit(): void {
    this.pageSize = this.constantService.defaultItemPerPage;
    this.form = this.fb.group({ appointmentId: [null] });
    this.bindData();
  }
  async bindData(): Promise<void> {
    const filter: any = { ...this.form.value, pagingData: { currentPage: this.currentPage, take: this.pageSize } };
    this.isLoading = true;
    this.service.getAllLabOrders(filter).subscribe((data: any) => {
      this.dataSource = new MatTableDataSource(data.item1);
      this.isLoading = false;
    });
  }
  pageChanged(event: PageEvent): void { this.pageSize = event.pageSize; this.currentPage = event.pageIndex; this.bindData(); }
  openAdd(element: any = {}): void {
    this.router.navigate(['/newlaborder'], { state: { element } });
  }
  openView(element: any): void { this.dialog.open(ViewLabOrderComponent, { data: { element }, width: '30%', disableClose: true }); }
  openDelete(element: any): void { this.dialog.open(DeleteLabOrderComponent, { data: { element }, width: '30%', disableClose: true }).afterClosed().subscribe(() => this.bindData()); }
}
