import { Component } from '@angular/core';
import { SeguridadService } from '../../shared/services/seguridad/seguridad.service';
import { ParametrosService } from '../../shared/services/parametros/parametros.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Sucursal } from '../../models/sucursal';
import { Agencia } from '../../models/agencia';
import { finalize } from 'rxjs/operators';
import { Periodo } from '../../models/periodo';
import { LoginCedulaDto } from '../../models/dtos/login-cedula-dto';
import { LoadingService } from 'src/app/services/common/loading/loading.service';
import { LocalStorageService } from 'src/app/services/common/local-storage/local-storage.service';
import { Router } from '@angular/router';
// import hotkeys from 'hotkeys-js';

@Component({
	selector: 'app-acceso',
	templateUrl: './acceso.component.html',
	styleUrls: ['./acceso.component.scss']
})
export class AccesoComponent {

	idiomas = [
		{
			prefix: 'es',
			label: 'Español',
			icon: 'https://raw.githubusercontent.com/hampusborgos/country-flags/main/svg/ec.svg',
		},
		{
			prefix: 'en',
			label: 'English',
			icon: 'https://raw.githubusercontent.com/hampusborgos/country-flags/main/svg/us.svg',
		},
	]

	langPrefix!: {
		prefix: string,
		label: string,
		icon: string,
	};

	formLogin: FormGroup = new FormGroup({});

	sucursales: Sucursal[] = [];
	agencias: Agencia[] = [];
	periodos: Periodo[] = [];

	nombreEmpresa = "";

	loadings = {
		agencias: false,
		sucursales: false,
		periodos: false,
		acceder: false,
	};

	loginError = "";
	mostrarError = false;

	constructor(
		private _seguridadService: SeguridadService,
		private _parametrosService: ParametrosService,
		private _loadingService: LoadingService,
		private _storage: LocalStorageService,
		private _router: Router,
	) {
		this.crearFormulario()

		this.obtenerDatosEmpresa()

		const langPrefix = this._storage.getStorage('lang_prefix') || 'es';
		this.idiomas.forEach(idioma => {
			if(idioma.prefix == langPrefix) this.langPrefix = idioma
		});

		this.changeImage()

		this.test();
	}

	cambiarIdioma(lang: {prefix: string}): void {
		this._storage.setStorage('lang_prefix', lang.prefix)
		window.location.reload()
	}

	crearFormulario(): void {
		this.formLogin = new FormGroup({
			cedula: new FormControl(null, [Validators.required]),
			contrasenia: new FormControl(null, [Validators.required]),
			codigoEmpresa: new FormControl({ value: null, disabled: true }, [Validators.required]),
			codigoSucursal: new FormControl({ value: null, disabled: true }, [Validators.required]),
			codigoAgencia: new FormControl({ value: null, disabled: true }, [Validators.required]),
			codigoPeriodo: new FormControl({ value: null, disabled: true }, [Validators.required]),
			forzarCierreSesion: new FormControl(false, [Validators.required]),
		})
	}

	cargarDatosFormulario(): void {
		if(this.formLogin.get('cedula')?.invalid) return

		const cedula = this.formLogin.get('cedula')?.value;
		this.obtenerSucursales(cedula)
	}

	restablecerFormulario(): void {
		this.formLogin.get('codigoSucursal')?.setValue(null)
		this.formLogin.get('codigoSucursal')?.disable()
		this.formLogin.get('codigoAgencia')?.setValue(null)
		this.formLogin.get('codigoAgencia')?.disable()
		this.formLogin.get('codigoPeriodo')?.setValue(null)
		this.formLogin.get('codigoPeriodo')?.disable()
	}

	obtenerDatosEmpresa(): void {
		this._loadingService.show()

		this._parametrosService.obtenerEmpresa()
		.pipe(finalize(() => this._loadingService.hide()))
		.subscribe(
			data => {
				this.formLogin.get('codigoEmpresa')?.setValue(data.resultado.codigoEmpresa)
				this.nombreEmpresa = data.resultado.nombre
			},
			error => {
				console.log('Error: ' + error);
			}
		);
	}

	obtenerSucursales(cedula: string): void {
		const sucursalDto = { cedula: cedula };
		this.loadings.sucursales = true

		this._parametrosService.obtenerSucursales(sucursalDto)
		.pipe(finalize(() => this.loadings.sucursales = false))
		.subscribe(
			data => {
				this.sucursales = data.resultado
				this.formLogin.get('codigoSucursal')?.enable()
				this.formLogin.get('codigoSucursal')?.setValue(data.resultado[0].codigoSucursal)

				this.obtenerAgencias()
				this.obtenerPeriodos()
			},
			error => {
				console.log('Error: ' + error);
			}
		);
	}

	obtenerAgencias(): void {
		const agenciaDto = {
			cedula: this.formLogin.get('cedula')?.value,
			codigoSucursal: this.formLogin.get('codigoSucursal')?.value
		};

		this.loadings.agencias = true

		this._parametrosService.obtenerAgencias(agenciaDto)
		.pipe(finalize(() => this.loadings.agencias = false))
		.subscribe(
			data => {
				this.agencias = data.resultado
				this.formLogin.get('codigoAgencia')?.enable()
				this.formLogin.get('codigoAgencia')?.setValue(data.resultado[0].codigoAgencia)
			},
			error => {
				console.log('Error: ' + error);
			}
		);
	}

	obtenerPeriodos(): void {
		this.loadings.periodos = true

		const periodosDto = {
			codigoSucursal: this.formLogin.get('codigoSucursal')?.value,
			codigoEmpresa: this.formLogin.get('codigoEmpresa')?.value
		};

		this._parametrosService.obtenerPeriodos(periodosDto)
		.pipe(finalize(() => this.loadings.periodos = false))
		.subscribe(
			data => {
				this.periodos = data.resultado
				this.formLogin.get('codigoPeriodo')?.enable()
				this.formLogin.get('codigoPeriodo')?.setValue(data.resultado[0].codigoPeriodo)
			},
			error => {
				console.log('Error: ' + error);
			}
		);
	}

	loginCedula(): void {
		this.mostrarError = false

		if(this.formLogin.invalid) {
			this.formLogin.markAllAsTouched()
			return;
		}

		this.loadings.acceder = true

		const loginDto = <LoginCedulaDto>(this.formLogin.getRawValue());
		loginDto.contrasenia = this._seguridadService.encriptar(loginDto.contrasenia)
		this._seguridadService.loginCedula(loginDto)
		.pipe(finalize(() => this.loadings.acceder = false))
		.subscribe(
			data => {
				if(data.codigo == 1) {
					this.colocarError(data.mensajeUsuario)
				} else {
					console.log(data);

					this._storage.setStorage('sesion', data.resultado)
					this._router.navigate(['inicio'])
				}
			},
			error => {
				this.loginError = error;
				this.mostrarError = true
			}
		);
	}

	colocarError(error: string): void {
		this.loginError = error
		this.mostrarError = true
	}

	test() {
		// hotkeys('ctrl+s, command+s', function () {
		// 	alert('stopped reload!');
		// 	return false;
		// });
	}

	actual = 0;
	imgs = [
		"./assets/login/PANTALLA-INICIAL-ARTE-1.png",
		"./assets/login/PANTALLA-INICIAL-ARTE-2.png",
	]

	changeImage() {
		setInterval(() => {
			if(this.actual + 1 >= this.imgs.length) {
				this.actual = 0
			} else {
				this.actual++
			}
		}, 3500)
	}

}
