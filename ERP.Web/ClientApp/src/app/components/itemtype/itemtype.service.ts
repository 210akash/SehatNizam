import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { ItemtypeEndPoints } from './itemtype.endpoints';

@Injectable({
    providedIn: 'root'
})

export class ItemtypeService extends BaseService<any> {

    endPointControllerName = "Itemtype";
    constructor(httpClient: HttpClient, private http: HttpClient, private itemtypeEndPoints: ItemtypeEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllItemtypes(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.itemtypeEndPoints.getAllItemtypes)
            .pipe(map((data: any) => data));
    }

    saveItemtype(saveItemtypeCommand: any) {
        return this.post(saveItemtypeCommand, this.endPointControllerName + this.itemtypeEndPoints.saveItemtype)
            .pipe(map((data: any) => data));
    }

    deleteItemtype(id: number) {
        return this.delete(id, this.endPointControllerName + this.itemtypeEndPoints.deleteItemtype)
            .pipe();
    }

    getItemtypeById(id: number) {
        return this.get(id, this.endPointControllerName + this.itemtypeEndPoints.getItemtypeById)
            .pipe(map((data: any) => data));
    }

    getItemtypeByName(name: string) {
        return this.get(name, this.endPointControllerName + this.itemtypeEndPoints.getItemtypeByName)
            .pipe(map((data: any) => data));
    }

    getItemtypeCode(CategoryId:number,Id:number) {
        return this.get('?SubCategoryId=' + CategoryId + '&Id=' + Id , this.endPointControllerName + this.itemtypeEndPoints.getItemtypeCode)
            .pipe(map((data: any) => data));
    }

    getItemtypeBySubCategory(SubCategoryId:number) {
        return this.get('?SubCategoryId=' + SubCategoryId, this.endPointControllerName + this.itemtypeEndPoints.getItemtypeBySubCategory)
            .pipe(map((data: any) => data));
    }
}
