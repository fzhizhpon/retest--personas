import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { VsSelectBienInmuebleComponent } from './vs-select-bien-inmueble';
import { FormsModule } from '@angular/forms';
import { NzMessageModule } from 'ng-zorro-antd/message';
import { NzSelectModule } from 'ng-zorro-antd/select';



@NgModule({
  declarations: [VsSelectBienInmuebleComponent],
	exports: [VsSelectBienInmuebleComponent],
	imports: [
		CommonModule,
		FormsModule,
		NzSelectModule,
		NzMessageModule,
	]
})
export class VsSelectBienInmuebleModule { }
