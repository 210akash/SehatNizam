import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { RosterService } from '../roster.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { ConstantService } from '../../../../Service/constant.service';

@Component({
    selector: 'app-process-roster',
    templateUrl: './process-roster.component.html',
    styleUrl: './process-roster.component.css',
    standalone: false
})

export class ProcessRosterComponent {
  isLoading = false;
  TMaterialCost! : number;
  TFillingPerPet!: number;
  TCostOfProduction! : number;
  CostPerPet! : number;
  advSaleTaxAmt!: number;
  advFEDAmt!: number;
  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private rosterService: RosterService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

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
  
    this.advSaleTaxAmt = (this.TFillingPerPet * advSaleTaxPer) / 100;
    this.advFEDAmt = (this.TFillingPerPet * advFEDPer) / 100;
  }

  async process() {
    (await this.rosterService.processRoster(this.data.element.id)).subscribe({
      next: (data: boolean) => {
        if (data == true) {
          this.isLoading = false;
          this.notificationsService.showNotification('Proceed successfully', 'snack-bar-success');
          this.dialog.closeAll();
        }
      },
      error: (error: string) => {
        console.log(error);
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        this.isLoading = false;
      }
    });
  }
}
