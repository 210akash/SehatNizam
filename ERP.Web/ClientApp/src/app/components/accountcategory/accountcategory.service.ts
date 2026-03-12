import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { AccountCategoryEndPoints } from './accountcategory.endpoints';

@Injectable({
    providedIn: 'root'
})

export class AccountCategoryService extends BaseService<any> {

    endPointControllerName = "AccountCategory";
    constructor(httpClient: HttpClient, private http: HttpClient, private accountcategoryEndPoints: AccountCategoryEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllAccountCategorys(accountcategorysFilterForm: any) {
        return this.post(accountcategorysFilterForm, this.endPointControllerName + this.accountcategoryEndPoints.getAllAccountCategorys)
            .pipe(map((data: any) => data));
    }

    saveAccountCategory(saveAccountCategoryCommand: any) {
        return this.post(saveAccountCategoryCommand, this.endPointControllerName + this.accountcategoryEndPoints.saveAccountCategory)
            .pipe(map((data: any) => data));
    }

    deleteAccountCategory(id: number) {
        return this.delete(id, this.endPointControllerName + this.accountcategoryEndPoints.deleteAccountCategory)
            .pipe();
    }

    getAccountCategoryCode() {
        return this.get(this.endPointControllerName + this.accountcategoryEndPoints.getAccountCategoryCode)
            .pipe(map((data: any) => data));
    }
}
