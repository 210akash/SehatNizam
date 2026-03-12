import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { GRNEndPoints } from './grn.endpoints';

@Injectable({
    providedIn: 'root'
})

export class GRNService extends BaseService<any> {

    endPointControllerName = "GRN";
    constructor(httpClient: HttpClient, private http: HttpClient, private gRNEndPoints: GRNEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getAllGRNs(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.gRNEndPoints.getAllGRNs)
            .pipe(map((data: any) => data));
    }

    saveGRN(saveGRNCommand: any) {
        return this.post(saveGRNCommand, this.endPointControllerName + this.gRNEndPoints.saveGRN)
            .pipe(map((data: any) => data));
    }

    deleteGRN(id: number) {
        return this.delete(id, this.endPointControllerName + this.gRNEndPoints.deleteGRN)
            .pipe();
    }

    getGRNById(id: number) {
        return this.get(id, this.endPointControllerName + this.gRNEndPoints.getGRNById)
            .pipe(map((data: any) => data));
    }

    getGRNByName(name: string) {
        return this.get(name, this.endPointControllerName + this.gRNEndPoints.getGRNByName)
            .pipe(map((data: any) => data));
    }

    getGRNCode() {
        return this.get(this.endPointControllerName + this.gRNEndPoints.getGRNCode)
            .pipe(map((data: any) => data));
    }

    processGRN(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.gRNEndPoints.processGRN)
            .pipe();
    }

    getIndentRequestCount(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.gRNEndPoints.getIndentRequestCount)
            .pipe(map((data: any) => data));
    }

    approveGRN(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.gRNEndPoints.approveGRN)
            .pipe();
    }

    approvePurchaseInvoice(id: number) {
        return this.get('?id=' + id, this.endPointControllerName + this.gRNEndPoints.approvePurchaseInvoice)
            .pipe();
    }

    processPurchaseInvoice(id: number, comments: string) {
        return this.get('?id=' + id + '&comments=' + comments, this.endPointControllerName + this.gRNEndPoints.processPurchaseInvoice)
            .pipe();
    }

    rejectPurchaseInvoice(id: number, comments: string) {
        return this.get('?id=' + id + '&comments=' + comments, this.endPointControllerName + this.gRNEndPoints.rejectPurchaseInvoice)
            .pipe();
    }




    // getPendingDemand(purchaseDemandId: number, gRNId: number) {
    //     return this.get('?purchaseDemandId=' + purchaseDemandId + '&gRNId=' + gRNId, this.endPointControllerName + this.gRNEndPoints.getPendingDemand)
    //         .pipe();
    // }

    getPendingInspection(inspectionId: any) {
        return this.get('?inspectionId=' + inspectionId, this.endPointControllerName + this.gRNEndPoints.getPendingInspection)
            .pipe();
    }

    async getPendingInspectionItems(inspectionId: number, gRNId: number) {
        return this.get('?inspectionId=' + inspectionId + '&gRNId=' + gRNId, this.endPointControllerName + this.gRNEndPoints.getPendingInspectionItems)
            .pipe();
    }

    async getAllPurchaseInvoices(purchaseInvoicesForm: any) {
        return this.post(purchaseInvoicesForm, this.endPointControllerName + this.gRNEndPoints.getAllPurchaseInvoices)
            .pipe(map((data: any) => data));
    }

    getPurchaseInvoiceCount(purchaseInvoiceCountForm: any) {
        return this.post(purchaseInvoiceCountForm, this.endPointControllerName + this.gRNEndPoints.getPurchaseInvoiceCount)
            .pipe(map((data: any) => data));
    }

    updateWHTPercentage(saveGRNCommand: any) {
        return this.post(
            saveGRNCommand,
            this.endPointControllerName + this.gRNEndPoints.updateWHTPercentage
        ).pipe(map((data: any) => data));
    }

    getPendingCostSheet(itemId: number, costSheetId: number) {
        return this.get('?itemId=' + itemId + '&costSheetId=' + costSheetId, this.endPointControllerName + this.gRNEndPoints.getPendingCostSheet)
            .pipe();
    }


}