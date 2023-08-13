import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DialogComponent } from './components/dialog/dialog.component';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { UserRolesDirective } from './directives/user-roles.directive';
import { ShowTextComponent } from './components/show-text/show-text.component';

import { LoadingComponent } from './components/loading/loading.component';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { InputDataComponent } from './components/input-data/input-data.component';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';

@NgModule({
  declarations: [
    DialogComponent,
    UserRolesDirective,
    ShowTextComponent,
    LoadingComponent,
    InputDataComponent,
  ],
  imports: [
    CommonModule,
    MatIconModule,
    MatFormFieldModule,
    MatDialogModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    FormsModule,MatInputModule
  ],
  exports: [UserRolesDirective,LoadingComponent, MatIconModule],
})
export class SharedModule {}
