import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { TransactionEndPoints } from './transaction.endpoints';

@Injectable({
    providedIn: 'root'
})

export class TransactionService extends BaseService<any> {

    endPointControllerName = "Transaction";
    constructor(httpClient: HttpClient, private http: HttpClient, private transactionEndPoints: TransactionEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    async getAllTransactions(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.transactionEndPoints.getAllTransactions)
            .pipe(map((data: any) => data));
    }

    saveTransaction(saveTransactionCommand: any) {
        return this.post(saveTransactionCommand, this.endPointControllerName + this.transactionEndPoints.saveTransaction)
            .pipe(map((data: any) => data));
    }

    deleteTransaction(id: number) {
        return this.delete(id, this.endPointControllerName + this.transactionEndPoints.deleteTransaction)
            .pipe();
    }

    getTransactionById(id: number) {
        return this.get(id, this.endPointControllerName + this.transactionEndPoints.getTransactionById)
            .pipe(map((data: any) => data));
    }

    getTransactionByName(name: string) {
        return this.get(name, this.endPointControllerName + this.transactionEndPoints.getTransactionByName)
            .pipe(map((data: any) => data));
    }

    getTransactionCode(VoucherTypeId:number) {
        return this.get('?voucherTypeId=' + VoucherTypeId ,this.endPointControllerName + this.transactionEndPoints.getTransactionCode)
            .pipe(map((data: any) => data));
    }
    
    processTransaction(id: number) {
        return this.get('?id='+id, this.endPointControllerName + this.transactionEndPoints.processTransaction)
            .pipe();
    }

    getTransactionCount(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.transactionEndPoints.getTransactionCount)
            .pipe(map((data: any) => data));
    }

    approveTransaction(id: number) {
        return this.get('?id='+id, this.endPointControllerName + this.transactionEndPoints.approveTransaction)
            .pipe();
    }


}
