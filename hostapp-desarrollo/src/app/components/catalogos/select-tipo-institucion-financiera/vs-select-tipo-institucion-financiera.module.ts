import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {NzSelectModule} from 'ng-zorro-antd/select';
import {FormsModule} from '@angular/forms';
import {NzMessageModule} from 'ng-zorro-antd/message';
import {VsSelectTipoInstitucionFinancieraComponent} from './vs-select-tipo-institucion-financiera.component';

@NgModule({
	declarations: [VsSelectTipoInstitucionFinancieraComponent],
	exports: [VsSelectTipoInstitucionFinancieraComponent],
	imports: [
		CommonModule,
		NzSelectModule,
		FormsModule,
		NzMessageModule
	]
})
export class VsSelectTipoInstitucionFinancieraModule {
}
