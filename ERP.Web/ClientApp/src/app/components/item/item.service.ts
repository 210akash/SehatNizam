import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { ItemEndPoints } from './item.endpoints';

@Injectable({
    providedIn: 'root'
})

export class ItemService extends BaseService<any> {

    endPointControllerName = "Item";
    constructor(httpClient: HttpClient, private http: HttpClient, private itemEndPoints: ItemEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllItems(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.itemEndPoints.getAllItems)
            .pipe(map((data: any) => data));
    }

    saveItem(saveItemCommand: any) {
        return this.post(saveItemCommand, this.endPointControllerName + this.itemEndPoints.saveItem)
            .pipe(map((data: any) => data));
    }

    deleteItem(id: number) {
        return this.delete(id, this.endPointControllerName + this.itemEndPoints.deleteItem)
            .pipe();
    }

    getItemById(id: number) {
        return this.get(id, this.endPointControllerName + this.itemEndPoints.getItemById)
            .pipe(map((data: any) => data));
    }

    getItemByName(name: string) {
        return this.get('?Name=' + name, this.endPointControllerName + this.itemEndPoints.getItemByName)
            .pipe(map((data: any) => data));
    }

    getItemCode(ItemTypeId:number,Id:number) {
        return this.get('?ItemTypeId=' + ItemTypeId + '&Id=' + Id , this.endPointControllerName + this.itemEndPoints.getItemCode)
            .pipe(map((data: any) => data));
    }

}
