import { Injectable } from '@angular/core';
import { environment } from 'projects/autenticacion/src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { LoginCedulaDto } from '../../../models/dtos/login-cedula-dto';
import { Respuesta } from 'src/app/models/respuesta';
import { Observable } from 'rxjs';
import { Usuario } from '../../../models/usuario';
import * as Crypto from 'crypto-js';

@Injectable({
	providedIn: 'root'
})
export class SeguridadService {

	readonly url: string = environment.seguridad;

	constructor(private http: HttpClient) { }

	loginCedula(body: LoginCedulaDto): Observable<Respuesta<Usuario>> {
		return this.http.post<Respuesta<Usuario>>(`${this.url}/login-cedula`, body);
	}

	encriptar(data: string): string {
		const clave = 'C5waKyi2ll4L0X7IPUCqVkvJ99C/XNB62nkgZagfuzQ=';
		const base64Clave = Crypto.enc.Base64.parse(clave);

		const vector = "q1JnirDT1uMy09pFs+5dBA==";
		const base64Vector = Crypto.enc.Base64.parse(vector);

		const res = Crypto.AES.encrypt(data, base64Clave, {iv: base64Vector}).toString()

		return res;
	}
}
