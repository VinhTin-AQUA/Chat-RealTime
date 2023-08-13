import { Injectable } from '@angular/core';
import { DialogComponent } from './components/dialog/dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { ShowTextComponent } from './components/show-text/show-text.component';
import { LoadingComponent } from './components/loading/loading.component';
import { ReplaySubject } from 'rxjs';


@Injectable({
  providedIn: 'root',
})
export class SharedService {
  private isLoading: ReplaySubject<boolean | null> = new ReplaySubject(1);
  isLoading$ = this.isLoading.asObservable();


  constructor(public dialog: MatDialog, private router: Router) {}

  showDialog(status: boolean, title: string, messages: string) {
    const dialogRef = this.dialog.open(DialogComponent, {
      data: { status: status, title: title, messages: messages },
    });
  }

  showText(messages: string) {
    const dialogRef = this.dialog.open(ShowTextComponent, {
      data: { messages: messages },
    });
  }

  setLoading(isLoading: boolean) {
    this.isLoading.next(isLoading);
  }
  
}
