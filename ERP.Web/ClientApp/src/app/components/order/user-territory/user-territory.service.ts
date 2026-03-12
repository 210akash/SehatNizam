import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { UserTerritoryEndPoints } from './user-territory.endpoints';
import { BaseService } from '../../../Service/base.service';
import { environment } from '../../../../environments/environment';

@Injectable({
    providedIn: 'root'
})

export class UserTerritoryService extends BaseService<any> {

    endPointControllerName = 'UserTerritory';

    constructor(private http: HttpClient, httpClient: HttpClient, private userTerritoryEndPoints: UserTerritoryEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async saveUserTerritory(createDeviceRegistratonForm: any) {
        return await this.post(createDeviceRegistratonForm, this.endPointControllerName + this.userTerritoryEndPoints.saveUserTerritory)
            .pipe(map((data: any) => data));
    }

    async getAllUserTerritory(deviceRegistratonFilterForm: any) {
        return await this.post(deviceRegistratonFilterForm, this.endPointControllerName + this.userTerritoryEndPoints.getAllUserTerritory)
            .pipe(map((data: any) => data));
    }

    async getUserTerritoryById(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.userTerritoryEndPoints.getUserTerritoryById)
            .pipe(map((data: any) => data));
    }

    async deleteUserTerritory(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.userTerritoryEndPoints.deleteUserTerritory)
            .pipe(map((data: any) => data));
    }

    async getZonesByUserInTerritory(userId: any, roleId: any) {
        return this.get('?userId=' + userId + '&roleId=' + roleId, this.endPointControllerName + this.userTerritoryEndPoints.getZonesByUserInTerritory)
            .pipe(map((data: any) => data));
    }


}