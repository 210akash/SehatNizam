import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { ComparativeStatementEndPoints } from './comparativestatement.endpoints';

@Injectable({
    providedIn: 'root'
})

export class ComparativeStatementService extends BaseService<any> {

    endPointControllerName = "ComparativeStatement";
    constructor(httpClient: HttpClient, private http: HttpClient, private comparativestatementEndPoints: ComparativeStatementEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getAllComparativeStatements(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.comparativestatementEndPoints.getAllComparativeStatements)
            .pipe(map((data: any) => data));
    }

    saveComparativeStatement(saveComparativeStatementCommand: any) {
        return this.post(saveComparativeStatementCommand, this.endPointControllerName + this.comparativestatementEndPoints.saveComparativeStatement)
            .pipe(map((data: any) => data));
    }

    deleteComparativeStatement(id: number) {
        return this.delete(id, this.endPointControllerName + this.comparativestatementEndPoints.deleteComparativeStatement)
            .pipe();
    }

    getComparativeStatementById(id: number) {
        return this.get(id, this.endPointControllerName + this.comparativestatementEndPoints.getComparativeStatementById)
            .pipe(map((data: any) => data));
    }

    getComparativeStatementByName(name: string) {
        return this.get(name, this.endPointControllerName + this.comparativestatementEndPoints.getComparativeStatementByName)
            .pipe(map((data: any) => data));
    }

    getComparativeStatementCode() {
        return this.get(this.endPointControllerName + this.comparativestatementEndPoints.getComparativeStatementCode)
            .pipe(map((data: any) => data));
    }
    
    processComparativeStatement(id: number) {
        return this.get('?id='+id, this.endPointControllerName + this.comparativestatementEndPoints.processComparativeStatement)
            .pipe();
    }

    getIndentRequestCount(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.comparativestatementEndPoints.getIndentRequestCount)
            .pipe(map((data: any) => data));
    }

    approveComparativeStatement(id: number) {
        return this.get('?id='+id, this.endPointControllerName + this.comparativestatementEndPoints.approveComparativeStatement)
            .pipe();
    }

    getPendingDemand(purchaseDemandId:number,comparativeStatementId:number) {
        return this.get('?purchaseDemandId=' + purchaseDemandId + '&comparativeStatementId=' + comparativeStatementId,this.endPointControllerName + this.comparativestatementEndPoints.getPendingDemand)
            .pipe();
    }

    async getPendingDemandItems(purchaseDemandId:number, ComparativeStatementId : number) {
        return this.get('?PurchaseDemandId=' + purchaseDemandId + '&ComparativeStatementId=' + ComparativeStatementId ,this.endPointControllerName + this.comparativestatementEndPoints.getPendingDemandItems)
            .pipe();
    }
}
