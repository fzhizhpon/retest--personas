import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {NzSelectModule} from 'ng-zorro-antd/select';
import {VsSelectInstitucionFinancieraComponent} from './vs-select-institucion-financiera.component';
import {FormsModule} from '@angular/forms';
import {NzMessageModule} from 'ng-zorro-antd/message';

@NgModule({
	declarations: [VsSelectInstitucionFinancieraComponent],
	exports: [VsSelectInstitucionFinancieraComponent],
	imports: [
		CommonModule,
		FormsModule,
		NzSelectModule,
		NzMessageModule,
	]
})
export class VsSelectInstitucionFinancieraModule {
}
