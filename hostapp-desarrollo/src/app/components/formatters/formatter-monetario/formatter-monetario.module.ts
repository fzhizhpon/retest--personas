import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormatterInputNumberComponent } from './formatter-monetario.component';
import { NzInputNumberModule } from 'ng-zorro-antd/input-number';
import { FormsModule, NgModel, ReactiveFormsModule } from '@angular/forms';


@NgModule({
  declarations: [FormatterInputNumberComponent],
  exports:[FormatterInputNumberComponent],
  imports: [
    CommonModule,
    NzInputNumberModule,
    FormsModule,
    ReactiveFormsModule
  ]
})
export class FormatterInputMonetarioModule { }
