import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { SaleMaterialReturnEndPoints } from './salematerialreturn.endpoints';

@Injectable({
    providedIn: 'root'
})

export class SaleMaterialReturnService extends BaseService<any> {

    endPointControllerName = "SaleMaterialReturn";
    constructor(httpClient: HttpClient, private http: HttpClient, private saleMaterialReturnEndPoints: SaleMaterialReturnEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getAllSaleMaterialReturns(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.saleMaterialReturnEndPoints.getAllSaleMaterialReturns)
            .pipe(map((data: any) => data));
    }

    saveSaleMaterialReturn(saveSaleMaterialReturnCommand: any) {
        return this.post(saveSaleMaterialReturnCommand, this.endPointControllerName + this.saleMaterialReturnEndPoints.saveSaleMaterialReturn)
            .pipe(map((data: any) => data));
    }

    deleteSaleMaterialReturn(id: number) {
        return this.delete(id, this.endPointControllerName + this.saleMaterialReturnEndPoints.deleteSaleMaterialReturn)
            .pipe();
    }

    getSaleMaterialReturnCode() {
        return this.get(this.endPointControllerName + this.saleMaterialReturnEndPoints.getSaleMaterialReturnCode)
            .pipe(map((data: any) => data));
    }

    processSaleMaterialReturn(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.saleMaterialReturnEndPoints.processSaleMaterialReturn)
            .pipe();
    }

    getSaleMaterialReturnCount(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.saleMaterialReturnEndPoints.getSaleMaterialReturnCount)
            .pipe(map((data: any) => data));
    }

    approveSaleMaterialReturn(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.saleMaterialReturnEndPoints.approveSaleMaterialReturn)
            .pipe();
    }

    // getPendingDemand(purchaseDemandId: number, saleMaterialReturnId: number) {
    //     return this.get('?purchaseDemandId=' + purchaseDemandId + '&saleMaterialReturnId=' + saleMaterialReturnId, this.endPointControllerName + this.saleMaterialReturnEndPoints.getPendingDemand)
    //         .pipe();
    // }

    async getPendingSaleMaterial(grnId: any, searchParam: any) {
        // Make sure to send the purchaseDemandIds as a JSON array in the body of the request.
        const body = {
            SaleMaterialId: grnId,
            searchParam: searchParam
        };

        return this.post(body, this.endPointControllerName + this.saleMaterialReturnEndPoints.getPendingSaleMaterial)
            .pipe();
    }

    async getPendingSaleMaterialItems(grnId: number, saleMaterialReturnId: number) {
        return this.get('?SaleMaterialId=' + grnId + '&saleMaterialReturnId=' + saleMaterialReturnId, this.endPointControllerName + this.saleMaterialReturnEndPoints.getPendingSaleMaterialItems)
            .pipe();
    }

}
