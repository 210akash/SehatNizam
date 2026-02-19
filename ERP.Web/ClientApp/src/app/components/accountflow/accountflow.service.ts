import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { AccountFlowEndPoints } from './accountflow.endpoints';

@Injectable({
    providedIn: 'root'
})

export class AccountFlowService extends BaseService<any> {

    endPointControllerName = "AccountFlow";
    constructor(httpClient: HttpClient, private http: HttpClient, private accountflowEndPoints: AccountFlowEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllAccountFlows(accountflowsFilterForm: any) {
        return this.post(accountflowsFilterForm, this.endPointControllerName + this.accountflowEndPoints.getAllAccountFlows)
            .pipe(map((data: any) => data));
    }

    saveAccountFlow(saveAccountFlowCommand: any) {
        return this.post(saveAccountFlowCommand, this.endPointControllerName + this.accountflowEndPoints.saveAccountFlow)
            .pipe(map((data: any) => data));
    }

    deleteAccountFlow(id: number) {
        return this.delete(id, this.endPointControllerName + this.accountflowEndPoints.deleteAccountFlow)
            .pipe();
    }

    getAccountFlowCode() {
        return this.get(this.endPointControllerName + this.accountflowEndPoints.getAccountFlowCode)
            .pipe(map((data: any) => data));
    }
}
