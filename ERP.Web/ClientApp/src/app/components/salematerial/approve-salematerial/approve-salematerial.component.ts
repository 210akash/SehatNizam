import { Component, Inject, TemplateRef, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../Service/notification.service';
import { SaleMaterialService } from '../salematerial.service';

@Component({
  selector: 'app-approve-salematerial',
  templateUrl: './approve-salematerial.component.html',
  styleUrl: './approve-salematerial.component.css',
  standalone: false,
})
export class ApproveSaleMaterialComponent {
  isLoading = false;
  TMaterialCost!: number;
  TFillingPerPet!: number;
  TCostOfProduction!: number;
  CostPerPet!: number;
  advSaleTaxAmt!: number;
  advFEDAmt!: number;

  @ViewChild('confirmationDialog') confirmationDialog!: TemplateRef<any>;
  constructor(
    private dialog: MatDialog,
    private notificationsService: NotificationsService,
    private salematerialService: SaleMaterialService,
    @Inject(MAT_DIALOG_DATA) public data: { element: any }
  ) {}

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

    const costDetails = this.data.element.saleMaterialDetail || [];

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
    // Open the confirmation dialog using the template reference
    const dialogRef = this.dialog.open(this.confirmationDialog);

    // Wait for the dialog to be closed and get the result
    const confirmed = await dialogRef.afterClosed().toPromise();

    if (confirmed) {
      // Proceed with approval if user confirmed
      this.isLoading = true;

      (
        await this.salematerialService.approveSaleMaterial(this.data.element.id)
      ).subscribe({
        next: (data: { item1: number; item2: string; }) => {
          if (data.item1 === 200) {
            this.isLoading = false;
            this.notificationsService.showNotification(
              data.item2,
              'snack-bar-success'
            );
            this.dialog.closeAll();
          } else if (data.item1 === 501) {
            this.isLoading = false;
            this.notificationsService.showNotification(
              data.item2,
              'snack-bar-danger'
            );
          } else if (data.item1 === 502) {
            this.isLoading = false;
            this.notificationsService.showNotification(
              data.item2,
              'snack-bar-danger'
            );
          } else if (data.item1 === 503) {
            this.isLoading = false;
            this.notificationsService.showNotification(
              data.item2,
              'snack-bar-danger'
            );
          } else {
            this.isLoading = false;
            this.notificationsService.showNotification(
              'Error Approving, Please contact system admin!',
              'snack-bar-danger'
            );
          }
        },
        error: (error: string) => {
          console.log(error);
          this.notificationsService.showNotification(error, 'snack-bar-danger');
          this.isLoading = false;
        },
      });
    } else {
      // User canceled, don't proceed
      console.log('Approval canceled');
    }
  }

    async reject() {
    (await this.salematerialService.rejectSaleMaterial(this.data.element.id)).subscribe({
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
