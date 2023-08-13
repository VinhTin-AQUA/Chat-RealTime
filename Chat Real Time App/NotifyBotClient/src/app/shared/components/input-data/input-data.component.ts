import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-input-data',
  templateUrl: './input-data.component.html',
  styleUrls: ['./input-data.component.scss']
})
export class InputDataComponent {
  email: string = ''
  constructor(
    public dialogRef: MatDialogRef<InputDataComponent>,
    @Inject(MAT_DIALOG_DATA)
    public data: {
      title: string,
      email: string | undefined
    }
  ) {}

  onNoClick(): void {
    this.dialogRef.close();
  }
}
