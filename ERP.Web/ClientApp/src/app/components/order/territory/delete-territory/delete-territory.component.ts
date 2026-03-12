import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { TerritoryService } from '../territory.service';
import { ConstantService } from '../../../../Service/constant.service';
import { NotificationsService } from '../../../../Service/notification.service';

@Component({
  selector: 'app-delete-territory',
  templateUrl: './delete-territory.component.html',
  styleUrls: ['./delete-territory.component.css'],standalone: false
})

export class DeleteTerritoryComponent implements OnInit {
  deleteTerritoryForm!: FormGroup;
  isLoading = false;
  dataSource!: any;
  roleList: any;

  constructor(private notificationsService: NotificationsService, private dialog:MatDialog, private territoryService: TerritoryService, private formBuilder: FormBuilder, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
  }
  async delete(){
    (await this.territoryService.deleteTerritory(this.data.element.id)).subscribe({
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
