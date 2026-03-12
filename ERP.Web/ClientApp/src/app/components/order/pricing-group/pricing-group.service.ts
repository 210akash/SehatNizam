import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs';
import { PricingGroupEndPoints } from './pricing-group.endpoints';
import { environment } from '../../../../environments/environment';
import { BaseService } from '../../../Service/base.service';

@Injectable({
    providedIn: 'root'
})

export class PricingGroupService extends BaseService<any> {

    endPointControllerName = 'PricingGroup';

    constructor(private http: HttpClient, httpClient: HttpClient, private pricingGroupEndPoints: PricingGroupEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getPricingGroupByName(name: string) {
        return await this.get('?name=' + name, this.endPointControllerName + this.pricingGroupEndPoints.getPricingGroupByName)
            .pipe(map((data: any) => data));
    }

    async savePricingGroup(createPricingGroupForm: any) {
        return await this.post(createPricingGroupForm, this.endPointControllerName + this.pricingGroupEndPoints.savePricingGroup)
            .pipe(map((data: any) => data));
    }

    async getAllPricingGroup(regionFilterForm: any) {
        return await this.post(regionFilterForm, this.endPointControllerName + this.pricingGroupEndPoints.getAllPricingGroup)
            .pipe(map((data: any) => data));
    }

    async getPricingGroupById(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.pricingGroupEndPoints.getPricingGroupById)
            .pipe(map((data: any) => data));
    }

    async deletePricingGroup(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.pricingGroupEndPoints.deletePricingGroup)
            .pipe(map((data: any) => data));
    }

    async getProductGroupDetailsByGroupId(groupId: number) {
        return this.get('?groupId=' + groupId, this.endPointControllerName + this.pricingGroupEndPoints.getProductGroupDetailsByGroupId)
            .pipe(map((data: any) => data));
    }


    async saveProductPricingDetails(regionFilterForm: any) {
        return await this.post(regionFilterForm, this.endPointControllerName + this.pricingGroupEndPoints.saveProductPricingDetails)
            .pipe(map((data: any) => data));
    }

    async getAllDistributorByGroupId(groupId: number) {
        return this.get('?groupId=' + groupId, this.endPointControllerName + this.pricingGroupEndPoints.getAllDistributorByGroupId)
            .pipe(map((data: any) => data));
    }

    async saveDistributorPricingGroup(createPricingGroupForm: any) {
        return await this.post(createPricingGroupForm, this.endPointControllerName + this.pricingGroupEndPoints.saveDistributorPricingGroup)
            .pipe(map((data: any) => data));
    }

        async copyPriceGroup(copyPricingGroupForm: any) {
        return await this.post(copyPricingGroupForm, this.endPointControllerName + this.pricingGroupEndPoints.copyPriceGroup)
            .pipe(map((data: any) => data));
    }
}