import { Directive, ElementRef, HostListener, Input, Renderer2 } from '@angular/core';
import { AbstractControl, NgControl, NgModel } from '@angular/forms';

@Directive({
	selector: '[vs-trim]',
	providers: [NgModel],
})
export class TrimDirective extends NgControl {

	viewToModelUpdate(newValue: any): void {
		throw new Error('Method not implemented.');
	}
	get control(): AbstractControl | null {
		throw new Error('Method not implemented.');
	}

	@Input('vsRemoveSpaces') removeSpaces = true;

	constructor(
		private renderer: Renderer2,
		private elementRef: ElementRef,
		private ngModel: NgModel,
		private ngControl: NgControl,
	) {
		super();
	}

	@HostListener('blur')
	onBlur() {
		console.log('blur');

		let value = this.ngModel.model || this.elementRef.nativeElement.value;

		if (value) {
			if (this.removeSpaces) value = value.replace(/\s+/g, ' ')
			value = value.trim();

			this.renderer.setProperty(
				this.elementRef.nativeElement, 'value', value);
			this.renderer.setAttribute(
				this.elementRef.nativeElement, 'value', value);

			this.ngModel.update.emit(value);
			this.ngControl?.control?.setValue(value);
		}
	}
}
