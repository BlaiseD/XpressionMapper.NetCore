import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/do';
import 'rxjs/add/operator/map';

import { IInstructor } from './iinstructor';

@Injectable()
export class InstructorService {
    private _instructorsUrl = 'http://localhost:60360/api/Instructors';

    constructor(private _http: Http) { }

    getInstructors(): Observable<IInstructor[]> {
        return this._http.get(this._instructorsUrl)
            .map((response: Response) => <IInstructor[]>response.json())
            .do(data => console.log('All: ' + JSON.stringify(data)))
            .catch(this.handleError);
    }
//.map((products: IProduct[]) => products.find(p => p.productId === id));
    getInstructor(id: number): Observable<IInstructor> {
        const url = `${this._instructorsUrl}/${id}`;
        return this._http.get(url)
            .map((response: Response) => <IInstructor>response.json())
            .do(data => console.log('All: ' + JSON.stringify(data)))
            .catch(this.handleError);
    }

    private handleError(error: Response) {
        console.error(error);
        return Observable.throw(error.json().error || 'Server error');
    }

}
