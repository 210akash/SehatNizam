import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { InspectionEndPoints } from './inspection.endpoints';

@Injectable({
    providedIn: 'root'
})

export class InspectionService extends BaseService<any> {

    endPointControllerName = "Inspection";
    constructor(httpClient: HttpClient, private http: HttpClient, private InspectionEndPoints: InspectionEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getAllInspections(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.InspectionEndPoints.getAllInspections)
            .pipe(map((data: any) => data));
    }

    saveInspection(saveInspectionCommand: any) {
        return this.post(saveInspectionCommand, this.endPointControllerName + this.InspectionEndPoints.saveInspection)
            .pipe(map((data: any) => data));
    }

    deleteInspection(id: number) {
        return this.delete(id, this.endPointControllerName + this.InspectionEndPoints.deleteInspection)
            .pipe();
    }

    getInspectionById(id: number) {
        return this.get(id, this.endPointControllerName + this.InspectionEndPoints.getInspectionById)
            .pipe(map((data: any) => data));
    }

    getInspectionByName(name: string) {
        return this.get(name, this.endPointControllerName + this.InspectionEndPoints.getInspectionByName)
            .pipe(map((data: any) => data));
    }

    getInspectionCode() {
        return this.get(this.endPointControllerName + this.InspectionEndPoints.getInspectionCode)
            .pipe(map((data: any) => data));
    }

    processInspection(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.InspectionEndPoints.processInspection)
            .pipe();
    }
    
    getInspectionCount(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.InspectionEndPoints.getInspectionCount)
            .pipe(map((data: any) => data));
    }

    approveInspection(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.InspectionEndPoints.approveInspection)
            .pipe();
    }

    // getPendingDemand(purchaseDemandId: number, iGPId: number) {
    //     return this.get('?purchaseDemandId=' + purchaseDemandId + '&iGPId=' + iGPId, this.endPointControllerName + this.InspectionEndPoints.getPendingDemand)
    //         .pipe();
    // }

    getPendingIGP(igpId: any) {
        return this.get('?igpId=' + igpId, this.endPointControllerName + this.InspectionEndPoints.getPendingIGPs)
            .pipe();
    }

    async getPendingIGPItems(IgpId: number,InspectionId :number) {
        return this.get('?IgpId=' + IgpId + '&inspectionId=' + InspectionId , this.endPointControllerName + this.InspectionEndPoints.getPendingIGPItems)
            .pipe();
    }

}
