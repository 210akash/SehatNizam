import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { DealershipEndPoints } from './dealership.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class DealershipService extends BaseService<any> {

    endPointControllerName = 'Dealership';

    constructor(private http: HttpClient, httpClient: HttpClient, private dealershipEndPoints: DealershipEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getDealershipByName(name: string) {
        return await this.get('?name=' + name, this.endPointControllerName + this.dealershipEndPoints.getDealershipByName)
            .pipe(map((data: any) => data));
    }

    async saveDealership(createDealershipForm: any) {
        return await this.post(createDealershipForm, this.endPointControllerName + this.dealershipEndPoints.saveDealership)
            .pipe(map((data: any) => data));
    }

    async getAllDealership(dealershipFilterForm: any) {
        return await this.post(dealershipFilterForm, this.endPointControllerName + this.dealershipEndPoints.getAllDealership)
            .pipe(map((data: any) => data));
    }

    async getAllDealershipList(dealershipFilterForm: any) {
        return await this.post(dealershipFilterForm, this.endPointControllerName + this.dealershipEndPoints.getAllDealershipList)
            .pipe(map((data: any) => data));
    }

    async getDealershipById(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.dealershipEndPoints.getDealershipById)
            .pipe(map((data: any) => data));
    }

    async deleteDealership(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.dealershipEndPoints.deleteDealership)
            .pipe(map((data: any) => data));
    }

    async getDealershipByTerritoryId(territoryId: number) {
        return this.get('?territoryId=' + territoryId, this.endPointControllerName + this.dealershipEndPoints.getDealershipByTerritoryId)
            .pipe(map((data: any) => data));
    }

    async getDealershipByTerritorySaleModel(saleModel: any, territoryId: any) {
        return this.get('?saleModel=' + saleModel + '&territoryId=' + territoryId, this.endPointControllerName + this.dealershipEndPoints.getDealershipByTerritorySaleModel)
            .pipe(map((data: any) => data));
    }

    async getCustomerByName(name: string) {
        return await this.get('?name=' + name, this.endPointControllerName + this.dealershipEndPoints.getCustomerByName)
            .pipe(map((data: any) => data));
    }

    async getAllByName(name: string) {
        return await this.get('?name=' + name, this.endPointControllerName + this.dealershipEndPoints.getAllByName)
            .pipe(map((data: any) => data));
    }
    async getAllActiveByName(name: string) {
        return await this.get('?name=' + name, this.endPointControllerName + this.dealershipEndPoints.getAllActiveByName)
            .pipe(map((data: any) => data));
    }
    async getAllDistributorType() {
        return await this.get(this.endPointControllerName + this.dealershipEndPoints.getAllDistributorType)
            .pipe(map((data: any) => data));
    }
}