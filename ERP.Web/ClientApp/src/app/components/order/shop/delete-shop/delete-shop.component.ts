import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { ShopService } from '../shop.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';

@Component({
  selector: 'app-delete-shop',
  templateUrl: './delete-shop.component.html',
  styleUrls: ['./delete-shop.component.css'],standalone: false
})

export class DeleteShopComponent implements OnInit {
  deleteShopForm!: FormGroup;
  isLoading = false;
  dialogRef: any;
  
  documents: any[] = [];
  urlSafe: SafeResourceUrl | undefined;

  constructor(private sanitizer: DomSanitizer, private notificationsService: NotificationsService, private dialog:MatDialog, private shopService: ShopService, private formBuilder: FormBuilder, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.deleteShopForm = this.formBuilder.group({
      id: [0],
      name: [''],
      phoneNo: [''],
      address: [''],
      pinLocation: [''],
      region: [''],
      territory: [''],
      scheduler: [''],

      ownerName: [''],
      openingTime: [''],
      closingTime: [''],
      isVerified: [''],
      isTagFromMob: [''],
    });

    this.LoadData(this.data.element);
    this.deleteShopForm.get('region')?.patchValue(this.data.element.territory?.area?.zone?.region?.name);
    this.deleteShopForm.get('territory')?.patchValue(this.data.element.territory.name);
    this.deleteShopForm.get('scheduler')?.patchValue(this.data.element.scheduler?.name);
  }

  async LoadData(element: any) {
    this.constantService.LoadData(element, this.deleteShopForm);

    this.deleteShopForm.get('openingTime')?.patchValue(await this.convertToAmPmFormat(element.openingTime));
    this.deleteShopForm.get('closingTime')?.patchValue(await this.convertToAmPmFormat(element.closingTime));
    this.deleteShopForm.get('isVerified')?.patchValue(element.isVerified == true ? 'Yes' : 'No');
    this.deleteShopForm.get('isTagFromMob')?.patchValue(element.isTagFromMob == true ? 'Mobile' : 'Web');

    this.documents = element?.attachments.filter((x: { isActive: boolean; }) => x.isActive == true);
  }

  async delete(){
    (await this.shopService.deleteShop(this.data.element.id)).subscribe({
      next: (data) => {
        if(data.Status == 200){
          this.isLoading = false;
          this.notificationsService.showNotification(data.Message, 'snack-bar-success');
          this.dialog.closeAll();
        }
        else{
          this.isLoading = false;
          this.notificationsService.showNotification(data.Message, 'snack-bar-danger');
        }
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }

  GetDocument(event: any, index: number, template: any) {
    this.urlSafe = this.sanitizer.bypassSecurityTrustResourceUrl(this.documents[index].fileSource + '#toolbar=0');
    this.dialogRef = this.dialog.open(template, {
      width: '50%',
      height: '70%',
      disableClose: true,
    });
  }

  // Method to convert TimeSpan string to AM/PM format
  async convertToAmPmFormat(timeString: string): Promise<string>{
    const parts = timeString.split(':');
    const date = new Date();
    date.setHours(parseInt(parts[0], 10));
    date.setMinutes(parseInt(parts[1], 10));
    date.setSeconds(parseInt(parts[2], 10));

    return date.toLocaleTimeString([], {
      hour: '2-digit',
      minute: '2-digit',
      hour12: true
    });
  }
}
