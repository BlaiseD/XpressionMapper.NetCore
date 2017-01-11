import { IEnrollment } from './ienrollment';

export interface IStudent {
    id: number;
    lastName: string;
    firstName: string;
    fullName: string;
    enrollmentDate: Date;
    enrollments: IEnrollment[];
}
