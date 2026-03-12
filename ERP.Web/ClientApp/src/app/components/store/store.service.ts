import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { StoreEndPoints } from './store.endpoints';

@Injectable({
    providedIn: 'root'
})

export class StoreService extends BaseService<any> {

    endPointControllerName = "Store";
    constructor(httpClient: HttpClient, private http: HttpClient, private storeEndPoints: StoreEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllStores(storesFilterForm: any) {
        return this.post(storesFilterForm, this.endPointControllerName + this.storeEndPoints.getAllStores)
            .pipe(map((data: any) => data));
    }

    saveStore(saveStoreCommand: any) {
        return this.post(saveStoreCommand, this.endPointControllerName + this.storeEndPoints.saveStore)
            .pipe(map((data: any) => data));
    }

    deleteStore(id: number) {
        return this.delete(id, this.endPointControllerName + this.storeEndPoints.deleteStore)
            .pipe();
    }

    getStoreById(id: number) {
        return this.get(id, this.endPointControllerName + this.storeEndPoints.getStoreById)
            .pipe(map((data: any) => data));
    }

    getStoreByName(name: string) {
        return this.get(name, this.endPointControllerName + this.storeEndPoints.getStoreByName)
            .pipe(map((data: any) => data));
    }

    getStoreByCompany(companyId: number,fixedAsset : Boolean) {
        return this.get('?CompanyId=' + companyId + '&FixedAsset=' + fixedAsset, this.endPointControllerName + this.storeEndPoints.getStoreByCompany)
            .pipe(map((data: any) => data));
    }
}
