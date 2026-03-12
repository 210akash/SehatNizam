import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../Service/base.service';
import { environment } from '../../../environments/environment';
import { CategoryEndPoints } from './category.endpoints';

@Injectable({
    providedIn: 'root'
})

export class CategoryService extends BaseService<any> {

    endPointControllerName = "Category";
    constructor(httpClient: HttpClient, private http: HttpClient, private categoryEndPoints: CategoryEndPoints) {
        super(
            httpClient,
            environment.dev_uri
        );
    }

    getAllCategorys(categorysFilterForm: any) {
        return this.post(categorysFilterForm, this.endPointControllerName + this.categoryEndPoints.getAllCategorys)
            .pipe(map((data: any) => data));
    }

    saveCategory(saveCategoryCommand: any) {
        return this.post(saveCategoryCommand, this.endPointControllerName + this.categoryEndPoints.saveCategory)
            .pipe(map((data: any) => data));
    }

    deleteCategory(id: number) {
        return this.delete(id, this.endPointControllerName + this.categoryEndPoints.deleteCategory)
            .pipe();
    }

    getCategoryById(id: number) {
        return this.get(id, this.endPointControllerName + this.categoryEndPoints.getCategoryById)
            .pipe(map((data: any) => data));
    }

    getCategoryByName(name: string) {
        return this.get(name, this.endPointControllerName + this.categoryEndPoints.getCategoryByName)
            .pipe(map((data: any) => data));
    }

    getCategoryCode() {
        return this.get(this.endPointControllerName + this.categoryEndPoints.getCategoryCode)
            .pipe(map((data: any) => data));
    }

}
