import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { AccountGroupEndPoints } from './accountgroup.endpoints';

@Injectable({
    providedIn: 'root'
})

export class AccountGroupService extends BaseService<any> {

    endPointControllerName = "AccountGroup";
    constructor(httpClient: HttpClient, private http: HttpClient, private accountgroupEndPoints: AccountGroupEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllAccountGroups(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.accountgroupEndPoints.getAllAccountGroups)
            .pipe(map((data: any) => data));
    }

    saveAccountGroup(saveAccountGroupCommand: any) {
        return this.post(saveAccountGroupCommand, this.endPointControllerName + this.accountgroupEndPoints.saveAccountGroup)
            .pipe(map((data: any) => data));
    }

    deleteAccountGroup(id: number) {
        return this.delete(id, this.endPointControllerName + this.accountgroupEndPoints.deleteAccountGroup)
            .pipe();
    }

    getAccountGroupById(id: number) {
        return this.get(id, this.endPointControllerName + this.accountgroupEndPoints.getAccountGroupById)
            .pipe(map((data: any) => data));
    }

    getAccountGroupByName(name: string, accountgroupFlow: string[]) {
        const body = {
            name: name,
            accountgroupFlow: accountgroupFlow
        };
    
        const headers = new HttpHeaders({
            'Content-Type': 'application/json'  // Ensure JSON content type
        });
    
        return this.post(body,this.endPointControllerName + this.accountgroupEndPoints.getAccountGroupByName)
            .pipe(map((data: any) => data));
    }

    getAccountGroupCode(AccountGroupTypeId:number,Id:number) {
        return this.get('?AccountGroupTypeId=' + AccountGroupTypeId + '&Id=' + Id , this.endPointControllerName + this.accountgroupEndPoints.getAccountGroupCode)
            .pipe(map((data: any) => data));
    }
}
