import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { AccountHeadEndPoints } from './accounthead.endpoints';

@Injectable({
    providedIn: 'root'
})

export class AccountHeadService extends BaseService<any> {

    endPointControllerName = "AccountHead";
    constructor(httpClient: HttpClient, private http: HttpClient, private accountheadEndPoints: AccountHeadEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllAccountHeads(accountheadsFilterForm: any) {
        return this.post(accountheadsFilterForm, this.endPointControllerName + this.accountheadEndPoints.getAllAccountHeads)
            .pipe(map((data: any) => data));
    }

    saveAccountHead(saveAccountHeadCommand: any) {
        return this.post(saveAccountHeadCommand, this.endPointControllerName + this.accountheadEndPoints.saveAccountHead)
            .pipe(map((data: any) => data));
    }

    deleteAccountHead(id: number) {
        return this.delete(id, this.endPointControllerName + this.accountheadEndPoints.deleteAccountHead)
            .pipe();
    }

    getAccountHeadCode() {
        return this.get(this.endPointControllerName + this.accountheadEndPoints.getAccountHeadCode)
            .pipe(map((data: any) => data));
    }
}
