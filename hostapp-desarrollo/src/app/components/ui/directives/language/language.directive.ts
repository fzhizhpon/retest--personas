import { Directive, ElementRef, Input, Renderer2, SecurityContext } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { LanguagesService } from 'src/app/services/common/languages/languages.service';

@Directive({
	selector: '[lang]'
})
export class LanguageDirective {

	@Input() langLabel!: string;
	@Input('langTag') langTag: string | null = null;
	@Input('langTagIndex') langTagIndex = 0;

	constructor(
		private el: ElementRef,
		private _langService: LanguagesService,
		private renderer: Renderer2,
		private sanitizer: DomSanitizer,
	) { }

	ngOnChanges() {
		this.setLanguage()
	}

	setLanguage() {
		this._langService.get(this.langLabel).subscribe(api => {
			if (api.resultado == null || api.resultado == undefined) return

			let parent
			if (this.langTag !== null)
				parent = this.el.nativeElement.getElementsByTagName(this.langTag)[this.langTagIndex]
			else
				parent = this.el.nativeElement;

			let txt;
			parent.childNodes.forEach((e: ChildNode) => {
				if (e.nodeType === 3) {
					txt = e
					return
				}
			})

			if (txt === null) return

			this.renderer.setProperty(
				/*txt,
				'nodeValue',*/
				this.el.nativeElement,
				'innerHTML',
				this.sanitizer.sanitize(SecurityContext.HTML, api.resultado)
			);
		})
	}

}
