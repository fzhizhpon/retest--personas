import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { VsSelectCodigoPaisComponent } from './vs-select-codigo-pais.component';

import { FormsModule } from '@angular/forms';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzMessageModule } from 'ng-zorro-antd/message';



@NgModule({
  declarations: [VsSelectCodigoPaisComponent],
  exports:[VsSelectCodigoPaisComponent],
  imports: [
    CommonModule,
    FormsModule,
		NzSelectModule,
		NzMessageModule,
  ]
})
export class VsSelectCodigoPaisModule { }
