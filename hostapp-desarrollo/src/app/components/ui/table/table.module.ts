import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableComponent } from './table.component';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzEmptyModule } from 'ng-zorro-antd/empty';
import { LanguageDirectiveModule } from '../directives/language/language.directive.module';


@NgModule({
	declarations: [TableComponent],
	exports: [TableComponent],
	imports: [
		CommonModule,
		NzButtonModule,
		NzIconModule,
		NzSpinModule,
		NzEmptyModule,
		LanguageDirectiveModule
	]
})
export class VsTableModule { }
