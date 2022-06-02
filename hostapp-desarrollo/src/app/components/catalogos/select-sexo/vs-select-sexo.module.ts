import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { VsSelectSexoComponent } from './vs-select-sexo.component';
import { FormsModule } from '@angular/forms';
import { NzMessageModule } from 'ng-zorro-antd/message';


@NgModule({
	declarations: [VsSelectSexoComponent],
	exports: [VsSelectSexoComponent],
	imports: [
		CommonModule,
		FormsModule,
		NzSelectModule,
		NzMessageModule,
	]
})
export class VsSelectSexoModule { }
