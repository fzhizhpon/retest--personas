import {AfterViewInit, Component, forwardRef, Input} from '@angular/core';
import {NG_VALUE_ACCESSOR} from '@angular/forms';
import {NzMessageService} from 'ng-zorro-antd/message';
import {finalize} from 'rxjs/operators';
import {Respuesta} from 'src/app/models/respuesta';
import {CatalogoService} from 'src/app/services/api/catalogos/catalogo.service';
import {CatComun} from '../CatComun';

@Component({
	selector: 'vs-select-institucion-financiera',
	templateUrl: '../base/catalogo-base.html',
	styleUrls: ['../base/catalogo-base.scss'],
	providers: [{
		provide: NG_VALUE_ACCESSOR,
		multi: true,
		useExisting: forwardRef(() => VsSelectInstitucionFinancieraComponent)
	}]
})
export class VsSelectInstitucionFinancieraComponent extends CatComun<number> implements AfterViewInit {

	_codigoTipoFinanciero !: number;

	@Input('codigoTipoFinanciero')
	set codigoTipoFinanciero(value: number) {
		this.writeValue(null)
		this._codigoTipoFinanciero = value
		this.cargarDatos()
	}

	get codigoTipoFinanciero(): number {
		return this._codigoTipoFinanciero;
	}

	constructor(
		private _catalogoService: CatalogoService,
		private _message: NzMessageService
	) {
		super();
	}


	ngAfterViewInit(): void {
		this.cargarDatos()
	}

	cargarDatos() {
		if (this._codigoTipoFinanciero == null) return;

		this.isLoading = true
		this.tieneError = false
		this._catalogoService.obtenerPorGet<number>(`institucionesfinancieras`, {codigoTipoFinanciera: this.codigoTipoFinanciero})
			.pipe(finalize(() => this.isLoading = false))
			.subscribe(resp => {
				this.catalogos = resp.resultado
			}, (error: Respuesta<null>) => {
				this.tieneError = true
				this.labelError = error.mensajeUsuario
				this._message.create('error', error.mensajeUsuario);
			})
	}
}
