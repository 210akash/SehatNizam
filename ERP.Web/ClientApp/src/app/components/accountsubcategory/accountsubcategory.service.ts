import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { AccountSubcategoryEndPoints } from './accountsubcategory.endpoints';

@Injectable({
    providedIn: 'root'
})

export class AccountSubcategoryService extends BaseService<any> {

    endPointControllerName = "AccountSubCategory";
    constructor(httpClient: HttpClient, private http: HttpClient, private AccountSubcategoryEndPoints: AccountSubcategoryEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllAccountSubcategorys(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.AccountSubcategoryEndPoints.getAllAccountSubcategorys)
            .pipe(map((data: any) => data));
    }

    saveAccountSubcategory(saveAccountSubcategoryCommand: any) {
        return this.post(saveAccountSubcategoryCommand, this.endPointControllerName + this.AccountSubcategoryEndPoints.saveAccountSubcategory)
            .pipe(map((data: any) => data));
    }

    deleteAccountSubcategory(id: number) {
        return this.delete(id, this.endPointControllerName + this.AccountSubcategoryEndPoints.deleteAccountSubcategory)
            .pipe();
    }

    getAccountSubcategoryByCompany() {
        return this.get(this.endPointControllerName + this.AccountSubcategoryEndPoints.getAccountSubcategoryByCompany)
            .pipe(map((data: any) => data));
    }

    getAccountSubcategoryCode(AccountCategoryId:number,Id:number) {
        return this.get('?AccountCategoryId=' + AccountCategoryId + '&Id=' + Id , this.endPointControllerName + this.AccountSubcategoryEndPoints.getAccountSubcategoryCode)
            .pipe(map((data: any) => data));
    }

    getSubCategoryByCategory(AccountCategoryId:number) {
        return this.get('?AccountCategoryId=' + AccountCategoryId , this.endPointControllerName + this.AccountSubcategoryEndPoints.getSubCategoryByCategory)
            .pipe(map((data: any) => data));
    }

    

}
