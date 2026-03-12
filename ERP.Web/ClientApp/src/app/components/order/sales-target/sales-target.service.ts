import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { SalesTargetEndPoints } from './sales-target.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class SalesTargetService extends BaseService<any> {

    endPointControllerName = 'SalesTarget';

    constructor(private http: HttpClient, httpClient: HttpClient, private salesTargetEndPoints: SalesTargetEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async saveSalesTarget(createSalesTargetForm: any) {
        return await this.post(createSalesTargetForm, this.endPointControllerName + this.salesTargetEndPoints.saveSalesTarget)
            .pipe(map((data: any) => data));
    }

    async getAllSalesTarget(salesTargetFilterForm: any) {
        return await this.post(salesTargetFilterForm, this.endPointControllerName + this.salesTargetEndPoints.getAllSalesTarget)
            .pipe(map((data: any) => data));
    }

    async getSalesTargetByZoneId(zoneId: number, targetMonth: any) {
        return this.get('?zoneId=' + zoneId + '&targetMonth=' + targetMonth, this.endPointControllerName + this.salesTargetEndPoints.getSalesTargetByZoneId)
            .pipe(map((data: any) => data));
    }

    async deleteSalesTarget(zoneId: any, targetMonth: any) {
        return this.get('?zoneId=' + zoneId + '&targetMonth=' + targetMonth, this.endPointControllerName + this.salesTargetEndPoints.deleteSalesTarget)
            .pipe(map((data: any) => data));
    }

    async isZoneTargetExist(zoneId: number) {
        return this.get('?zoneId=' + zoneId, this.endPointControllerName + this.salesTargetEndPoints.isZoneTargetExist)
            .pipe(map((data: any) => data));
    }

    async saveTerritoryTarget(createTerritoryTargetForm: any) {
        return await this.post(createTerritoryTargetForm, this.endPointControllerName + this.salesTargetEndPoints.saveTerritoryTarget)
            .pipe(map((data: any) => data));
    }

    async getTerritoryTargetsByZoneId(zoneId: any, targetMonth: any) {
        return await this.get('?zoneId=' + zoneId + '&targetMonth=' + targetMonth, this.endPointControllerName + this.salesTargetEndPoints.getTerritoryTargetsByZoneId)
            .pipe(map((data: any) => data));
    }

    async getDSFByZoneId(zoneId: any) {
        return await this.get('?zoneId=' + zoneId, this.endPointControllerName + this.salesTargetEndPoints.getDSFByZoneId)
            .pipe(map((data: any) => data));
    }

    async saveDSFTarget(createDSFTargetForm: any) {
        return await this.post(createDSFTargetForm, this.endPointControllerName + this.salesTargetEndPoints.saveDSFTarget)
            .pipe(map((data: any) => data));
    }

    async getDSFTargetsByTerritoryId(territoryId: any, targetMonth: any) {
        return await this.get('?territoryId=' + territoryId + '&targetMonth=' + targetMonth, this.endPointControllerName + this.salesTargetEndPoints.getDSFTargetsByTerritoryId)
            .pipe(map((data: any) => data));
    }

    async getTargetsByTerritoryId(territoryId: any, targetMonth: any) {
        return await this.get('?territoryId=' + territoryId + '&targetMonth=' + targetMonth, this.endPointControllerName + this.salesTargetEndPoints.getTargetsByTerritoryId)
            .pipe(map((data: any) => data));
    }
}