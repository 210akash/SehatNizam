import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { AreaService } from '../area.service';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';

@Component({
  selector: 'app-delete-area',
  templateUrl: './delete-area.component.html',
  styleUrls: ['./delete-area.component.css'],standalone: false
})

export class DeleteAreaComponent implements OnInit {
  deleteAreaForm!: FormGroup;
  isLoading = false;
  dataSource!: any;
  roleList: any;

  constructor(private notificationsService: NotificationsService, private dialog:MatDialog, private areaService: AreaService, private formBuilder: FormBuilder, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.deleteAreaForm = this.formBuilder.group({
      id: [0],
      name: [''],
      description: [''],
      coordinates: ['']
    });

    this.LoadData(this.data.element);
  }

  LoadData(element: any) {
    this.constantService.LoadData(element, this.deleteAreaForm);
  }

  async delete(){
    (await this.areaService.deleteArea(this.data.element.id)).subscribe({
      next: (data: { Status: number; Message: string; }) => {
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
      error: (error: any) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }
}
