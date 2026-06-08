import { Component, Inject } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { RoomService } from '../../room/room.service';
import { WardService } from '../../ward/ward.service';
import { ConstantService } from '../../../../Service/constant.service';
import { BedService } from '../bed.service';

@Component({
    selector: 'app-add-bed',
    templateUrl: './add-bed.component.html',
    styleUrl: './add-bed.component.css',
    standalone: false
})

export class AddBedComponent {
  bedForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  wardList :any;
  roomList :any;


  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private bedService: BedService, private roomService: RoomService, private wardService: WardService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.bedForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      bedNo: ['', Validators.required],
      wardId: ['', Validators.required],
      roomId: ['', Validators.required]
    });
    
    this.LoadData(this.data.element);
    this.getwardList();

  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.bedForm);
      this.bedForm.get('wardId')?.patchValue(element.room.ward.id);
      this.getroomList();
    }
    // else   
    //  this.getAccounttypeCode();
  }

  SaveData() {
    if (this.bedForm.invalid) {
      this.constantService.markFormGroupTouched(this.bedForm);
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.bedForm.value);

    this.bedService.saveBed(_clienttemperatureForm).subscribe({
      next: (data: { Status: number; Data: string; }) => {
        if (data.Status == 200) {
          this.notificationsService.showNotification(data.Data, 'snack-bar-success');
          this.dialog.closeAll();
        }
        else
          this.notificationsService.showNotification(data.Data, 'snack-bar-danger');
        this.isLoading = false;
      },
      error: (error: string) => {
        this.notificationsService.showNotification(error, 'snack-bar-danger');
        console.error(error);
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
    var WardId =  this.bedForm.get('wardId')?.value;
    this.roomService.getRoomByWard(WardId).subscribe((data: any) => {
     this.roomList = data;
    });
  }

  getBedCode() {
    var roomId =  this.bedForm.get('roomId')?.value;
    var Id =  this.bedForm.get('id')?.value;
    this.bedService.getBedCode(roomId,Id).subscribe((data: any) => {
      this.bedForm.get('code')?.patchValue(data.code);
      console.log(data.code);
      console.log(this.bedForm.get('code')?.value);
    });
  }

 reset(){
  this.bedForm.get('code')?.patchValue('');
 }
}
