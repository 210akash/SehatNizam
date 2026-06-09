import { Component, Inject, ViewChild } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { AdmissionBedListComponent } from '../admissionbed-list/admissionbed-list.component';
import { BedService } from '../../bed/bed.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { ConstantService } from '../../../../Service/constant.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { WardService } from '../../ward/ward.service';
import { RoomService } from '../../room/room.service';
import { AdmissionBedService } from '../admissionbed.service';

@Component({
  selector: 'app-add-admissionbed',
  templateUrl: './add-admissionbed.component.html',
  styleUrl: './add-admissionbed.component.css',
  standalone: false
})
export class AddAdmissionBedComponent {
  admissionBedForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  wardList :any;
  roomList :any;
  bedList: any;

  constructor( private admissionBedService: AdmissionBedService,private roomService: RoomService, private wardService: WardService,private formBuilder: FormBuilder, private bedService: BedService, private dialog: MatDialog, private notificationsService: NotificationsService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }
  @ViewChild(AdmissionBedListComponent) admissionbedListComponent!: AdmissionBedListComponent;

  ngOnInit(): void {
    this.admissionBedForm = this.formBuilder.group({
      id: [0],
      admissionId: [this.data.element.id],
      wardId: [Validators.required],
      roomId: [Validators.required],
      bedId: [Validators.required],
    });
    this.getwardList();
  }

  SaveData() {
    debugger
    if (this.admissionBedForm.invalid) {
      this.constantService.markFormGroupTouched(this.admissionBedForm);
      return;
    }

    this.isLoading = true;
    let _admissionBedForm: any = {};
    _admissionBedForm = Object.assign(_admissionBedForm, this.admissionBedForm.value);

    this.admissionBedService.saveAdmissionBed(_admissionBedForm).subscribe({
      next: (data) => {
        if (data.Status == 200) {
          this.notificationsService.showNotification(data.Data, 'snack-bar-success');
          // this.admissionbedListComponent.bindData();
          this.admissionBedForm.reset();
          this.dialog.closeAll();
        }
        else
          this.notificationsService.showNotification(data.Data, 'snack-bar-danger');
        this.isLoading = false;
      },
      error: (error) => {
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        this.isLoading = false;
      }
    });
  }

  getwardList() {
    let _WardFilter: any = {};
    this.wardService.getAllWards(_WardFilter).subscribe((data: any) => {
     this.wardList = data.item1;
    });
  }

  getroomList() {
    var WardId =  this.admissionBedForm.get('wardId')?.value;
    this.roomService.getRoomByWard(WardId).subscribe((data: any) => {
     this.roomList = data;
    });
  }

  getbedList(): void {
    var roomId =  this.admissionBedForm.get('roomId')?.value;
    this.bedService.getBedByRoom(roomId,true).subscribe(data => {
      this.bedList = data;
    });
  }

 reset(){
  this.admissionBedForm.get('roomId')?.patchValue('');
  this.admissionBedForm.get('bedId')?.patchValue('');
 }

}
