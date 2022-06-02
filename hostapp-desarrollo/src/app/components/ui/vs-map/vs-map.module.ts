import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { VsMapComponent } from './vs-map.component';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { LanguageDirectiveModule } from '../directives/language/language.directive.module';

@NgModule({
  declarations: [VsMapComponent],
	exports: [VsMapComponent],
  imports: [
    CommonModule,
    NzModalModule,
    LanguageDirectiveModule,
  ]
})
export class VsMapModule { }
