import { Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions, URLSearchParams } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/do';
import 'rxjs/add/operator/map';

import { IDepartment } from './idepartment';

@Injectable()
export class DepartmentService {
    private _departmentsUrl = 'http://localhost:60360/api/Departments';

    constructor(private _http: Http) { }

    getDepartments(): Observable<IDepartment[]> {
        return this._http.get(this._departmentsUrl)
            .map((response: Response) => <IDepartment[]>response.json())
            .do(data => console.log('All: ' + JSON.stringify(data)))
            .catch(this.handleError);
    }

    getDepartment(id: number): Observable<IDepartment> {
        const url = `${this._departmentsUrl}/${id}`;
        return this._http.get(url)
            .map((response: Response) => <IDepartment>response.json())
            .do(data => console.log('All: ' + JSON.stringify(data)))
            .catch(this.handleError);
    }

    deleteDepartment(id: number): Observable<IDepartment> {
        const url = `${this._departmentsUrl}/${id}`;
        return this._http.delete(url)
            .map((response: Response) => response.json())
            .do(data => console.log('All: ' + JSON.stringify(data)))
            .catch(this.handleError);
    }

    updateDepartment(dept: IDepartment): Observable<IDepartment> {
        const url = `${this._departmentsUrl}/${dept.departmentID}`;
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });

        return this._http.put(url, JSON.stringify(dept), options)
            .map((response: Response) => response.json())
            .do(data => console.log('All: ' + JSON.stringify(data)))
            .catch(this.handleError);
    }

    insertDepartment(dept: IDepartment): Observable<IDepartment> {
        const url = this._departmentsUrl;
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });

        return this._http.post(url, JSON.stringify(dept), options)
            .map((response: Response) => response.json())
            .do(data => console.log('All: ' + JSON.stringify(data)))
            .catch(this.handleError);
    }

    private handleError(error: Response) {
        console.error(error);
        return Observable.throw(error.json().error || 'Server error');
    }

}
