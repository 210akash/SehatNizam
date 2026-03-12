import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { DealershipService } from '../dealership.service';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';

@Component({
  selector: 'app-delete-dealership',
  templateUrl: './delete-dealership.component.html',
  styleUrls: ['./delete-dealership.component.css'],standalone: false
})

export class DeleteDealershipComponent implements OnInit {
  deleteDealershipForm!: FormGroup;
  isLoading = false;
  dialogRef: any;

  documents: any[] = [];
  urlSafe: SafeResourceUrl | undefined;

  constructor(private sanitizer: DomSanitizer, private notificationsService: NotificationsService, private dialog:MatDialog, private dealershipService: DealershipService, private formBuilder: FormBuilder, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.deleteDealershipForm = this.formBuilder.group({
      id: [0],
      name: [''],
      phoneNo: [''],
      address: [''],
      pinLocation: [''],
      zone: [''],
      territory: ['']
    });

    this.LoadData(this.data.element);
    this.deleteDealershipForm.get('zone')?.patchValue(this.data.element.territory.zone.name);
    this.deleteDealershipForm.get('territory')?.patchValue(this.data.element.territory.name);
  }

  LoadData(element: any) {
    this.constantService.LoadData(element, this.deleteDealershipForm);
    this.documents = element?.attachments.filter((x: { isActive: boolean; }) => x.isActive == true);
  }

  async delete(){
    (await this.dealershipService.deleteDealership(this.data.element.id)).subscribe({
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


}
