import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { VsSelectParentescoComponent } from './vs-select-parentesco.component';
import { FormsModule } from '@angular/forms';
import { NzMessageModule } from 'ng-zorro-antd/message';
import { NzInputModule } from 'ng-zorro-antd/input';


@NgModule({
	declarations: [VsSelectParentescoComponent],
	exports: [VsSelectParentescoComponent],
	imports: [
		CommonModule,
		FormsModule,
		NzSelectModule,
		NzMessageModule,
		NzInputModule,
	]
})
export class VsSelectParentescoModule { }
