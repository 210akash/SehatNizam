import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { TerritoryEndPoints } from './territory.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class TerritoryService extends BaseService<any> {

    endPointControllerName = 'Territory';

    constructor(private http: HttpClient, httpClient: HttpClient, private territoryEndPoints: TerritoryEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getTerritoryByName(name: string) {
        return await this.get('?name=' + name, this.endPointControllerName + this.territoryEndPoints.getTerritoryByName)
            .pipe(map((data: any) => data));
    }

    async saveTerritory(createTerritoryForm: any) {
        return await this.post(createTerritoryForm, this.endPointControllerName + this.territoryEndPoints.saveTerritory)
            .pipe(map((data: any) => data));
    }

    async getAllTerritory(territoryFilterForm: any) {
        return await this.post(territoryFilterForm, this.endPointControllerName + this.territoryEndPoints.getAllTerritory)
            .pipe(map((data: any) => data));
    }

    async getTerritoryById(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.territoryEndPoints.getTerritoryById)
            .pipe(map((data: any) => data));
    }

    async deleteTerritory(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.territoryEndPoints.deleteTerritory)
            .pipe(map((data: any) => data));
    }

    async getTerritoryByAreaId(areaId: any) {
        return await this.get('?areaId=' + areaId, this.endPointControllerName + this.territoryEndPoints.getTerritoryByAreaId)
            .pipe(map((data: any) => data));
    }

    async getDsfByTerritoryId(territoryId: number) {
        return this.get('?territoryId=' + territoryId, this.endPointControllerName + this.territoryEndPoints.getDsfByTerritoryId)
            .pipe(map((data: any) => data));
    }

    async getTerritoryBySaleModel(areaId: any, saleModel: any) {
        return await this.get('?areaId=' + areaId + '&saleModel=' + saleModel, this.endPointControllerName + this.territoryEndPoints.getTerritoryBySaleModel)
            .pipe(map((data: any) => data));
    }


}