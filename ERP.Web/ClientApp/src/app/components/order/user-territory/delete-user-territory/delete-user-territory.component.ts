import { Component, OnInit, Inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialog } from '@angular/material/dialog';
import { UserTerritoryService } from '../user-territory.service';
import { NotificationsService } from '../../../../Service/notification.service';
import { ConstantService } from '../../../../Service/constant.service';

@Component({
  selector: 'app-delete-user-territory',
  templateUrl: './delete-user-territory.component.html',
  styleUrls: ['./delete-user-territory.component.css'], standalone: false
})

export class DeleteUserTerritoryComponent implements OnInit {
  deleteUserTerritoryForm!: FormGroup;
  isLoading = false;
  territory: any = null;

  constructor(private notificationsService: NotificationsService, private dialog: MatDialog, private userTerritoryService: UserTerritoryService, private formBuilder: FormBuilder, private constantService: ConstantService, @Inject(MAT_DIALOG_DATA) public data: { element: any }) { }

  ngOnInit(): void {
    this.deleteUserTerritoryForm = this.formBuilder.group({
      id: [0],
      zone: [''],
      territory: [''],
      user: [''],
      role: [''],
      region: [''],
      area: [''],
    });

    this.LoadData(this.data.element);
  }

  get f() {
    return this.deleteUserTerritoryForm.controls;
  }

  LoadData(element: any) {
    this.constantService.LoadData(element, this.deleteUserTerritoryForm);
    var FullName = element.user?.firstName + ' ' + element.user?.lastName;
    this.deleteUserTerritoryForm.get('role')?.patchValue(element.user?.aspNetUserRoles[0]?.role?.name);
    this.deleteUserTerritoryForm.get('user')?.patchValue(FullName);
    this.deleteUserTerritoryForm.get('zone')?.patchValue(element.zone?.name);
    this.deleteUserTerritoryForm.get('region')?.patchValue(element.region?.name);
    this.deleteUserTerritoryForm.get('area')?.patchValue(element.area?.name);
    this.territory = element.isAllTerritoryCheck === true ? 'All Territories' : element.territory?.name;
    this.deleteUserTerritoryForm.get('territory')?.patchValue(this.territory);
  }

  async delete() {
    (await this.userTerritoryService.deleteUserTerritory(this.data.element.id)).subscribe({
      next: (data) => {
        if (data.Status == 200) {
          this.isLoading = false;
          this.notificationsService.showNotification('User Territory Deleted Successfully!', 'snack-bar-success');
          this.dialog.closeAll();
        }
      },
      error: (error) => {
        console.log(error);
        this.isLoading = false;
      }
    });
  }


}