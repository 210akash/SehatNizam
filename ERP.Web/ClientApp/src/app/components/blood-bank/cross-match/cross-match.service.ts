import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { BaseService } from '../../../Service/base.service';
import { environment } from '../../../../environments/environment';
import { CrossMatchEndPoints } from './cross-match.endpoints';

@Injectable({ providedIn: 'root' })
export class CrossMatchService extends BaseService<any> {
    endPointControllerName = 'BloodCrossMatch';

    constructor(httpClient: HttpClient, private endpoints: CrossMatchEndPoints) {
        super(httpClient, environment.dev_uri);
    }

    getAll(filterForm: any) {
        return this.post(filterForm, this.endPointControllerName + this.endpoints.getAll).pipe(map((data: any) => data));
    }

    getWorklist(filterForm: any) {
        return this.post(filterForm, this.endPointControllerName + this.endpoints.getWorklist).pipe(map((data: any) => data));
    }

    save(command: any) {
        return this.post(command, this.endPointControllerName + this.endpoints.save).pipe(map((data: any) => data));
    }

    deleteItem(id: number) {
        return this.delete(id, this.endPointControllerName + this.endpoints.delete).pipe();
    }
}
