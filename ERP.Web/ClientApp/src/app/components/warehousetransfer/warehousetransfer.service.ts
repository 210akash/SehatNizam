import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { WarehouseTransferEndPoints } from './warehousetransfer.endpoints';

@Injectable({
    providedIn: 'root'
})

export class WarehouseTransferService extends BaseService<any> {

    endPointControllerName = "WarehouseTransfer";
    constructor(httpClient: HttpClient, private http: HttpClient, private warehousetransferEndPoints: WarehouseTransferEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getAllWarehouseTransfers(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.warehousetransferEndPoints.getAllWarehouseTransfers)
            .pipe(map((data: any) => data));
    }

    saveWarehouseTransfer(saveWarehouseTransferCommand: any) {
        return this.post(saveWarehouseTransferCommand, this.endPointControllerName + this.warehousetransferEndPoints.saveWarehouseTransfer)
            .pipe(map((data: any) => data));
    }

    deleteWarehouseTransfer(id: number) {
        return this.delete(id, this.endPointControllerName + this.warehousetransferEndPoints.deleteWarehouseTransfer)
            .pipe();
    }

    getWarehouseTransferById(id: number) {
        return this.get(id, this.endPointControllerName + this.warehousetransferEndPoints.getWarehouseTransferById)
            .pipe(map((data: any) => data));
    }

    getWarehouseTransferByName(name: string) {
        return this.get(name, this.endPointControllerName + this.warehousetransferEndPoints.getWarehouseTransferByName)
            .pipe(map((data: any) => data));
    }

    getWarehouseTransferCode() {
        return this.get(this.endPointControllerName + this.warehousetransferEndPoints.getWarehouseTransferCode)
            .pipe(map((data: any) => data));
    }
    
    processWarehouseTransfer(id: number) {
        return this.get('?id='+id, this.endPointControllerName + this.warehousetransferEndPoints.processWarehouseTransfer)
            .pipe();
    }

    getWarehouseTransferCount(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.warehousetransferEndPoints.getWarehouseTransferCount)
            .pipe(map((data: any) => data));
    }

    approveWarehouseTransfer(id: number) {
        return this.get('?id='+id, this.endPointControllerName + this.warehousetransferEndPoints.approveWarehouseTransfer)
            .pipe();
    }

    getWarehouseTransferByItem(itemId: number) {
        return this.get('?itemId='+itemId, this.endPointControllerName + this.warehousetransferEndPoints.getWarehouseTransferByItem)
            .pipe();
    }

      revokeWarehouseTransfer(id: number) {
        return this.get('?id='+id, this.endPointControllerName + this.warehousetransferEndPoints.revokeWarehouseTransfer)
            .pipe();
    }

      getPendingCostSheet(itemId: number, costSheetId: number) {
        return this.get('?itemId=' + itemId + '&costSheetId=' + costSheetId, this.endPointControllerName + this.warehousetransferEndPoints.getPendingCostSheet)
            .pipe();
    }

}
