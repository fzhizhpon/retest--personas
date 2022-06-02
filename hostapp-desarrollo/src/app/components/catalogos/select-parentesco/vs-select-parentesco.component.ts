import { AfterViewInit, Component, forwardRef } from '@angular/core';
import { NG_VALUE_ACCESSOR } from '@angular/forms';
import { NzMessageService } from 'ng-zorro-antd/message';
import { finalize } from 'rxjs/operators';
import { Respuesta } from 'src/app/models/respuesta';
import { CatalogoService } from 'src/app/services/api/catalogos/catalogo.service';
import { Parentesco } from '../catalogo';
import { CatComun } from '../CatComun';

@Component({
	selector: 'vs-select-parentesco',
	templateUrl: './vs-select-parentesco.component.html',
	styleUrls: ['../base/catalogo-base.scss'],
	providers: [{
		provide: NG_VALUE_ACCESSOR,
		multi: true,
		useExisting: forwardRef(() => VsSelectParentescoComponent)
	}]
})
export class VsSelectParentescoComponent extends CatComun<number> implements AfterViewInit {

	indexSelected = -1;
	parentescos: Parentesco[] = [];

	constructor(
		private _catalogoService: CatalogoService,
		private _message: NzMessageService
	) {
		super();
	}

	ngAfterViewInit(): void {
		this.cargarDatos()
	}

	changeCallback(): void {
		if (this.value == null) {
			this.indexSelected = -1
		} else {
			this.parentescos.forEach((p, i) => {
				if (p.codigoParentesco === this.value) {
					this.indexSelected = i
				}
			})
		}
	}

	cargarDatos() {
		this.isLoading = true
		this.tieneError = false

		this._catalogoService.obtenerParentescos()
		.pipe(finalize(() => this.isLoading = false))
		.subscribe(resp => {
			this.parentescos = resp.resultado
		}, (error: Respuesta<null>) => {
			this.tieneError = true
			this.labelError = error.mensajeUsuario
			this._message.create('error', error.mensajeUsuario);
		})
	}

}
