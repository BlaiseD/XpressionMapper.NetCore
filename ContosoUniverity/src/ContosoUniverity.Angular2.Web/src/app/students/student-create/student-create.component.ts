import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

import { Subscription } from 'rxjs/Subscription';

import { IStudent } from '../istudent';
import { StudentService } from '../student.service';

@Component({
  selector: 'app-student-create',
  templateUrl: './student-create.component.html',
  styleUrls: ['./student-create.component.css']
})
export class StudentCreateComponent implements OnInit {
  public title: string = 'Student';

  public student: IStudent = {//must initialize the student for the form to be visible
    id: 0,
    lastName: null,
    firstName: null,
    fullName: null,
    enrollmentDate: null,
    enrollments: null
  };

  errorMessage: string;


  constructor(private _route: ActivatedRoute,
    private _router: Router,
    private _studentService: StudentService) { }

  ngOnInit() {
  }

  ngOnDestroy() {

  }

  onBack(): void {
    this._router.navigate(['/students']);
  }

  submitClick(): void {
    this._studentService.insertStudent(this.student)
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
