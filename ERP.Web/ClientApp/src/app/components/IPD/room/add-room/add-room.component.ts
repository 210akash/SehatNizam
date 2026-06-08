import { Component, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { NotificationsService } from '../../../../Service/notification.service';
import { RoomService } from '../room.service';
import { ConstantService } from '../../../../Service/constant.service';
import { WardService } from '../../ward/ward.service';

@Component({
    selector: 'app-add-room',
    templateUrl: './add-room.component.html',
    styleUrl: './add-room.component.css',
    standalone: false
})

export class AddRoomComponent {
  roomForm!: FormGroup;
  isLoading = false;
  isEditMode: boolean = false;
  wardList :any;

  constructor(private dialog: MatDialog, private notificationsService: NotificationsService, private formBuilder: FormBuilder, private roomService: RoomService, private wardService: WardService, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) {
    
   }

  ngOnInit(): void {
    this.roomForm = this.formBuilder.group({
      id: [0],
      code: ['', Validators.required],
      name: ['', Validators.required],
      description: [''],
      wardId: ['', Validators.required],
      companyId: [0],
    });
    
    this.LoadData(this.data.element);
    this.getwardList();

  }

  LoadData(element: any) {
    if (element != null) {
      this.isEditMode = true;
      this.constantService.LoadData(element, this.roomForm);
    }
  }

  SaveData() {
    if (this.roomForm.invalid) {
      this.constantService.markFormGroupTouched(this.roomForm);
      return;
    }

    this.isLoading = true;
    let _clienttemperatureForm: any = {};
    _clienttemperatureForm = Object.assign(_clienttemperatureForm, this.roomForm.value);

    this.roomService.saveRoom(_clienttemperatureForm).subscribe({
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
    let _CategoryFilter: any = {};
    this.wardService.getAllWards(_CategoryFilter).subscribe((data: any) => {
     this.wardList = data.item1;
    });
  }

  getRoomCode() {
    var WardId =  this.roomForm.get('wardId')?.value;
    var Id =  this.roomForm.get('id')?.value;
    this.roomService.getRoomCode(WardId,Id).subscribe((data: any) => {
      this.roomForm.get('code')?.patchValue(data.code);
      console.log(data.code);
      console.log(this.roomForm.get('code')?.value);
    });
  }
}
