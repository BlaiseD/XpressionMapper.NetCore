export interface IDepartment {
        departmentID: number;
        name: string;
        budget: number;
        startDate: Date;
        instructorID: number;
        rowVersion: string;
        administratorName: string;
}

export class Department implements IDepartment {
        departmentID: number;
        name: string;
        budget: number;
        startDate: Date;
        instructorID: number;
        rowVersion: string;
        administratorName: string;
}
