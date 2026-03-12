import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { CostSheetEndPoints } from './costsheet.endpoints';

@Injectable({
    providedIn: 'root'
})

export class CostSheetService extends BaseService<any> {

    endPointControllerName = "CostSheet";
    constructor(httpClient: HttpClient, private http: HttpClient, private costsheetEndPoints: CostSheetEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getAllCostSheets(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.costsheetEndPoints.getAllCostSheets)
            .pipe(map((data: any) => data));
    }

    saveCostSheet(saveCostSheetCommand: any) {
        return this.post(saveCostSheetCommand, this.endPointControllerName + this.costsheetEndPoints.saveCostSheet)
            .pipe(map((data: any) => data));
    }

    deleteCostSheet(id: number) {
        return this.delete(id, this.endPointControllerName + this.costsheetEndPoints.deleteCostSheet)
            .pipe();
    }

    getCostSheetById(id: number) {
        return this.get(id, this.endPointControllerName + this.costsheetEndPoints.getCostSheetById)
            .pipe(map((data: any) => data));
    }

    getCostSheetByName(name: string) {
        return this.get(name, this.endPointControllerName + this.costsheetEndPoints.getCostSheetByName)
            .pipe(map((data: any) => data));
    }

    getCostSheetCode() {
        return this.get(this.endPointControllerName + this.costsheetEndPoints.getCostSheetCode)
            .pipe(map((data: any) => data));
    }
    
    processCostSheet(id: number) {
        return this.get('?id='+id, this.endPointControllerName + this.costsheetEndPoints.processCostSheet)
            .pipe();
    }

    getCostSheetCount(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.costsheetEndPoints.getCostSheetCount)
            .pipe(map((data: any) => data));
    }

    approveCostSheet(id: number) {
        return this.get('?id='+id, this.endPointControllerName + this.costsheetEndPoints.approveCostSheet)
            .pipe();
    }

    getCostSheetByItem(itemId: number) {
        return this.get('?itemId='+itemId, this.endPointControllerName + this.costsheetEndPoints.getCostSheetByItem)
            .pipe();
    }

      rejectCostSheet(id: number) {
        return this.get('?id='+id, this.endPointControllerName + this.costsheetEndPoints.rejectCostSheet)
            .pipe();
    }
}
