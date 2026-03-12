import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { SubcategoryEndPoints } from './subcategory.endpoints';

@Injectable({
    providedIn: 'root'
})

export class SubcategoryService extends BaseService<any> {

    endPointControllerName = "Subcategory";
    constructor(httpClient: HttpClient, private http: HttpClient, private SubcategoryEndPoints: SubcategoryEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllSubcategorys(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.SubcategoryEndPoints.getAllSubcategorys)
            .pipe(map((data: any) => data));
    }

    saveSubcategory(saveSubcategoryCommand: any) {
        return this.post(saveSubcategoryCommand, this.endPointControllerName + this.SubcategoryEndPoints.saveSubcategory)
            .pipe(map((data: any) => data));
    }

    deleteSubcategory(id: number) {
        return this.delete(id, this.endPointControllerName + this.SubcategoryEndPoints.deleteSubcategory)
            .pipe();
    }

    getSubcategoryById(id: number) {
        return this.get(id, this.endPointControllerName + this.SubcategoryEndPoints.getSubcategoryById)
            .pipe(map((data: any) => data));
    }

    getSubcategoryByName(name: string) {
        return this.get(name, this.endPointControllerName + this.SubcategoryEndPoints.getSubcategoryByName)
            .pipe(map((data: any) => data));
    }

    getSubcategoryByCompany() {
        return this.get(this.endPointControllerName + this.SubcategoryEndPoints.getSubcategoryByCompany)
            .pipe(map((data: any) => data));
    }

    getSubcategoryCode(CategoryId:number,Id:number) {
        return this.get('?CategoryId=' + CategoryId + '&Id=' + Id , this.endPointControllerName + this.SubcategoryEndPoints.getSubcategoryCode)
            .pipe(map((data: any) => data));
    }

    getSubCategoryByCategory(CategoryId:number) {
        return this.get('?CategoryId=' + CategoryId , this.endPointControllerName + this.SubcategoryEndPoints.getSubCategoryByCategory)
            .pipe(map((data: any) => data));
    }

    

}
