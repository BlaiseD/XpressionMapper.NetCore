import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

import { Subscription } from 'rxjs/Subscription';

import { IStudent } from '../istudent';
import { StudentService } from '../student.service';


@Component({
  selector: 'app-student-edit',
  templateUrl: './student-edit.component.html',
  styleUrls: ['./student-edit.component.css']
})
export class StudentEditComponent implements OnInit {
  public title: string = 'Student';
  public student: IStudent;
  // public student: IStudent = {//must initialize the student for the form to be visible
  //   "studentID": 0,
  //   "name": "",
  //   "budget": 0,
  //   "startDate": new Date(),
  //   "instructorID": 0,
  //   "rowVersion": "",
  //   "administratorName": ""
  // };

  errorMessage: string;
  private sub: Subscription;

  constructor(private _route: ActivatedRoute,
    private _router: Router,
    private _studentService: StudentService) { }

  ngOnInit() {
    this.sub = this._route.params.subscribe(
      params => {
        let id = +params['id'];
        this.getStudent(id);
      });
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }

  getStudent(id: number) {
    this._studentService.getStudent(id).subscribe(
      stud => this.student = stud,
      error => this.errorMessage = <any>error);
  }

  onBack(): void {
    this._router.navigate(['/students']);
  }

  submitClick(): void {
    this._studentService.updateStudent(this.student)
      .subscribe(dept => {
        this._router.navigate(['/students']);
      },
      error => this.errorMessage = <any>error);

  }

  set convertToDate(e) {
    var parts = e.split('-');

    if (parts.length > 2 && !isNaN(parseInt(parts[0])) && !isNaN(parseInt(parts[1])) && !isNaN(parseInt(parts[2]))) {
      let year = parseInt(parts[0]);
      let month = parseInt(parts[1]);
      let day = parseInt(parts[2]);

      this.student.enrollmentDate = new Date(year, month - 1, day);
    }
  }

  get convertToDate() {
    if (this.student.enrollmentDate) {
      return this.student.enrollmentDate.toString().substring(0, 10);
    }
    else
      return null;
  }
}
