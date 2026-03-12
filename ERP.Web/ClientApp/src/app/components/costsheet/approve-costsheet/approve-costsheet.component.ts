import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { CostSheetService } from '../costsheet.service';

@Component({
    selector: 'app-approve-costsheet',
    templateUrl: './approve-costsheet.component.html',
    styleUrl: './approve-costsheet.component.css',
    standalone: false
})

export class ApproveCostSheetComponent {
  isLoading = false;
  TMaterialCost! : number;
  TFillingPerPet!: number;
  TCostOfProduction! : number;
  CostPerPet! : number;
  advSaleTaxAmt!: number;
  advFEDAmt!: number;

  constructor(private dialog: MatDialog,  private notificationsService: NotificationsService, private costsheetService: CostSheetService,@Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.calculateTotal();
  }

  calculateTotal() {
    this.TMaterialCost = 0;
    this.TFillingPerPet = 0;
    this.TCostOfProduction = 0;
    this.CostPerPet = 0;
    this.advSaleTaxAmt = 0;
    this.advFEDAmt = 0;

    const costDetails = this.data.element.costSheetDetail || [];
  
    costDetails.forEach((item: any) => {
      const amount = item.rate * item.quantity || 0;
      this.TMaterialCost += amount;
    });
  
    const quantity = +this.data.element.quantity || 0;
    const tollFillRate = +this.data.element.tollFillRate || 0;
    const advSaleTaxPer = +this.data.element.advSaleTaxPer || 0;
    const advFEDPer = +this.data.element.advFEDPer || 0;
  
    this.TFillingPerPet = quantity * tollFillRate;
    this.TCostOfProduction = this.TMaterialCost + this.TFillingPerPet;
    this.CostPerPet = quantity ? this.TCostOfProduction / quantity : 0;
  
     this.advSaleTaxAmt = this.TFillingPerPet * advSaleTaxPer;
     this.advFEDAmt = this.TFillingPerPet * advFEDPer;
  }

  async approve() {
    (await this.costsheetService.approveCostSheet(this.data.element.id)).subscribe({
      next: (data) => {
        if (data == true) {
          this.isLoading = false;
          this.notificationsService.showNotification('Approve Successfully', 'snack-bar-success');
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

   async reject() {
    (await this.costsheetService.rejectCostSheet(this.data.element.id)).subscribe({
      next: (data) => {
        if (data == true) {
          this.isLoading = false;
          this.notificationsService.showNotification('Reject Successfully', 'snack-bar-success');
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
