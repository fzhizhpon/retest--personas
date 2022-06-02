import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { NzSelectModule } from 'ng-zorro-antd/select';
import { VsSelectBienIntangibleComponent } from './vs-select-bien-intangible'; 
import { FormsModule } from '@angular/forms';
import { NzMessageModule } from 'ng-zorro-antd/message';


@NgModule({
	declarations: [VsSelectBienIntangibleComponent],
	exports: [VsSelectBienIntangibleComponent],
	imports: [
		CommonModule,
		FormsModule,
		NzSelectModule,
		NzMessageModule,
	]
})
export class VsSelectBienIntangibleModule { }


