import { Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions, URLSearchParams } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/do';
import 'rxjs/add/operator/map';

import { IStudent } from './istudent';
import { ISortCollection } from '../isortcollection';
import { IPagedStudents} from './ipagedstudents';

@Injectable()
export class StudentService {
    private _studentsUrl = 'http://localhost:60360/api/Students';

    constructor(private _http: Http) { }

    getStudents(): Observable<IStudent[]> {
        return this._http.get(this._studentsUrl)
            .map((response: Response) => <IStudent[]>response.json())
            .do(data => console.log('All: ' + JSON.stringify(data)))
            .catch(this.handleError);
    }

    getOrderedStudents(searchString: string, sorts: ISortCollection): Observable<IPagedStudents> {
        const url = `${this._studentsUrl}/Ordered/${searchString}`;
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });

        return this._http.post(url, JSON.stringify(sorts), options)
            .map((response: Response) => <IPagedStudents>response.json())
            .do(data => console.log('All: ' + JSON.stringify(data)))
            .catch(this.handleError);
    }

    getStudent(id: number): Observable<IStudent> {
        const url = `${this._studentsUrl}/${id}`;
        return this._http.get(url)
            .map((response: Response) => <IStudent>response.json())
            .do(data => console.log('All: ' + JSON.stringify(data)))
            .catch(this.handleError);
    }

    deleteStudent(id: number): Observable<IStudent> {
        const url = `${this._studentsUrl}/${id}`;
        return this._http.delete(url)
            .map((response: Response) => response.json())
            .do(data => console.log('All: ' + JSON.stringify(data)))
            .catch(this.handleError);
    }

    updateStudent(stud: IStudent): Observable<IStudent> {
        const url = `${this._studentsUrl}/${stud.id}`;
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });

        return this._http.put(url, JSON.stringify(stud), options)
            .map((response: Response) => response.json())
            .do(data => console.log('All: ' + JSON.stringify(data)))
            .catch(this.handleError);
    }

    insertStudent(stud: IStudent): Observable<IStudent> {
        const url = this._studentsUrl;
        let headers = new Headers({ 'Content-Type': 'application/json' });
        let options = new RequestOptions({ headers: headers });

        return this._http.post(url, JSON.stringify(stud), options)
            .map((response: Response) => response.json())
            .do(data => console.log('All: ' + JSON.stringify(data)))
            .catch(this.handleError);
    }

    private handleError(error: Response) {
        console.error(error);
        return Observable.throw(error.json().error || 'Server error');
    }

}
