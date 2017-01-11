import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs/Rx';
import { FormsModule } from '@angular/forms';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/do';
import { Ng2PaginationModule } from 'ng2-pagination';

import { IStudent } from '../istudent'
import { StudentService } from '../student.service';
import { IPagedStudents } from '../ipagedstudents'
import { ISortCollection } from '../../isortcollection'

@Component({
  selector: 'app-student-list',
  templateUrl: './student-list.component.html',
  styleUrls: ['./student-list.component.css']
})
export class StudentListComponent implements OnInit {

  constructor(private _studentService: StudentService) { }

  public title: string = 'Students';
  public sortProperty: string = "FullName"
  public sortDirection: number = 0;

  errorMessage: string;
  students: IStudent[];

  public _page: number = 1;
  public _total: number;
  public _itemsPerPage: number = 3;
  listFilter: string = "";

  ngOnInit() {
    this.getPage(this._page);
  }

  fullList(e) {
    this.sortProperty = "FullName"
    this.sortDirection = 0;
    this.listFilter = "";
    this._page = 1;

    this.getPage(this._page);
  }

  sortBy(e) {

    var idAttr = event.srcElement.id;

    if (this.sortProperty == idAttr)
      this.sortDirection = this.sortDirection == 0 ? 1 : 0;
    else {
      this.sortDirection = 0;
      this.sortProperty = idAttr;
    }

    this.getPage(this._page);
  }

  searchByName(e) {
    this.sortProperty = "FullName"
    this.sortDirection = 0;
    this._page = 1;
    this.getPage(this._page);
  }

  getPage(page: number) {
    this._page = page;
    this._studentService.getOrderedStudents(this.listFilter, {
      sortDescriptions: [{ propertyName: this.sortProperty, sortDirection: this.sortDirection }],
      skip: this._itemsPerPage * (page - 1),
      take: this._itemsPerPage
    })
      .subscribe(pagedStudents => {
        this.students = pagedStudents.students;
        this._total = pagedStudents.studentCount;
      },
      error => this.errorMessage = <any>error);
  }

}