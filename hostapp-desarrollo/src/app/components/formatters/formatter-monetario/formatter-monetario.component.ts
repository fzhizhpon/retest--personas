import { AfterViewInit, Component, forwardRef, Input } from '@angular/core';
import { NG_VALUE_ACCESSOR } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { DataBinder } from '../../common/DataBinder';

@Component({
  selector: 'vs-input-number',
  templateUrl: './formatter-monetario.component.html',
  styleUrls: ['./formatter-monetario.component.scss'],
  providers: [{
		provide: NG_VALUE_ACCESSOR,
		multi: true,
		useExisting: forwardRef(() => FormatterInputNumberComponent)
	}]
})
export class FormatterInputNumberComponent extends DataBinder<Number> {
    formatterDollar = (value: number): string => `$ ${value ? value : ''}`;
    parserDollar = (value: string): string => value.replace('$ ', '');
    constructor() {
		super();
	}
   
}
