import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { AccountEndPoints } from './account.endpoints';

@Injectable({
    providedIn: 'root'
})

export class AccountService extends BaseService<any> {

    endPointControllerName = "Account";
    constructor(httpClient: HttpClient, private http: HttpClient, private accountEndPoints: AccountEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllAccounts(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.accountEndPoints.getAllAccounts)
            .pipe(map((data: any) => data));
    }

    saveAccount(saveAccountCommand: any) {
        return this.post(saveAccountCommand, this.endPointControllerName + this.accountEndPoints.saveAccount)
            .pipe(map((data: any) => data));
    }

    deleteAccount(id: number) {
        return this.delete(id, this.endPointControllerName + this.accountEndPoints.deleteAccount)
            .pipe();
    }

    getAccountById(id: number) {
        return this.get(id, this.endPointControllerName + this.accountEndPoints.getAccountById)
            .pipe(map((data: any) => data));
    }

    getAccountByName(name: string, accountFlow: string[]) {
        const body = {
            name: name,
            accountFlow: accountFlow
        };
    
        const headers = new HttpHeaders({
            'Content-Type': 'application/json'  // Ensure JSON content type
        });
    
        return this.post(body,this.endPointControllerName + this.accountEndPoints.getAccountByName)
            .pipe(map((data: any) => data));
    }

    getAccountCode(AccountTypeId:number,Id:number) {
        return this.get('?AccountTypeId=' + AccountTypeId + '&Id=' + Id , this.endPointControllerName + this.accountEndPoints.getAccountCode)
            .pipe(map((data: any) => data));
    }
}
