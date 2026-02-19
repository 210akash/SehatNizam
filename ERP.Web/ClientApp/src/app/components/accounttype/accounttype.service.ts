import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { AccountTypeEndPoints } from './accounttype.endpoints';

@Injectable({
    providedIn: 'root'
})

export class AccountTypeService extends BaseService<any> {

    endPointControllerName = "AccountType";
    constructor(httpClient: HttpClient, private http: HttpClient, private accounttypeEndPoints: AccountTypeEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllAccounttypes(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.accounttypeEndPoints.getAllAccounttypes)
            .pipe(map((data: any) => data));
    }

    saveAccounttype(saveAccounttypeCommand: any) {
        return this.post(saveAccounttypeCommand, this.endPointControllerName + this.accounttypeEndPoints.saveAccounttype)
            .pipe(map((data: any) => data));
    }

    deleteAccounttype(id: number) {
        return this.delete(id, this.endPointControllerName + this.accounttypeEndPoints.deleteAccounttype)
            .pipe();
    }

    getAccounttypeById(id: number) {
        return this.get(id, this.endPointControllerName + this.accounttypeEndPoints.getAccounttypeById)
            .pipe(map((data: any) => data));
    }

    getAccounttypeByName(name: string) {
        return this.get(name, this.endPointControllerName + this.accounttypeEndPoints.getAccounttypeByName)
            .pipe(map((data: any) => data));
    }

    getAccounttypeCode(CategoryId:number,Id:number) {
        return this.get('?AccountSubCategoryId=' + CategoryId + '&Id=' + Id , this.endPointControllerName + this.accounttypeEndPoints.getAccounttypeCode)
            .pipe(map((data: any) => data));
    }

    getAccounttypeBySubCategory(SubCategoryId:number) {
        return this.get('?AccountSubCategoryId=' + SubCategoryId, this.endPointControllerName + this.accounttypeEndPoints.getAccounttypeBySubCategory)
            .pipe(map((data: any) => data));
    }
}
