
import { Injectable } from '@angular/core';
import { FormGroup } from '@angular/forms';

@Injectable({
  providedIn: 'root'
})
export class IdentificacionValidadorService {

  constructor(

  ) {

  }

  EsIdentificacionValida(identificacion: string): boolean {
    let estado = false;
    let provincia = 0;

    if (identificacion.length >= 10) {
      identificacion = identificacion.trim();

      //char[] valced = identificacion.ToCharArray();
      let valced = identificacion.split(''); // .ToCharArray();

      //provincia = parseInt(((valced[0].toString() + valced[1].toString()));
      provincia = parseInt((valced[0].toString() + valced[1].toString()));

      if (provincia > 0 && provincia < 25) {
        if (parseInt(valced[2].toString()) < 6) {
          estado = this.EsCedulaValida(identificacion);
        }
        else if (parseInt(valced[2].toString()) == 6){
          estado = this.EsIdSectorPublico(identificacion);
        }
        else if (parseInt(valced[2].toString()) == 9) {
          estado = this.EsIdSectorPublico(identificacion);
        }
      }
    }

    return estado;
  }

  EsCedulaValida(identificacion: string): boolean {
    identificacion = identificacion.trim();
    //char[] valced = identificacion.ToCharArray();
    let valced = identificacion.split('');

    let aux = 0;
    let par = 0;
    let impar = 0;
      let  verifi;

    for (let i = 0; i < 9; i += 2) {
      aux = 2 * parseInt(valced[i].toString());
      if (aux > 9)
        aux -= 9;
      par += aux;
    }
    for (let i = 1; i < 9; i += 2) {
      impar += parseInt(valced[i].toString());
    }

    aux = par + impar;
    if (aux % 10 != 0) {
      verifi = 10 - (aux % 10);
    }
    else
      verifi = 0;
    if (verifi == parseInt(valced[9].toString()))
      return true;
    else
      return false;
  }

  EsRucValido(identificacion: string): boolean {
    let estado = false;
    let provincia = 0;

    if (identificacion.length >= 10) {
      identificacion = identificacion.trim();
      //char[] valced = identificacion.ToCharArray();
      let valced= identificacion.trim();
      provincia = parseInt((valced[0].toString() + valced[1].toString()));

      if (provincia > 0 && provincia < 25) {
        if (parseInt(valced[2].toString()) == 6) {
          estado = this.EsIdSectorPublico(identificacion);
        }
        else if (parseInt(valced[2].toString()) == 9) {
          estado = this.EsIdPersJuridicaValida(identificacion);
        }
      }
    }

    return estado;
  }

  EsIdPersJuridicaValida(identificacion: string): boolean {
    /*identificacion = identificacion.trim();
    let valruc = identificacion.ToCharArray();
*/
    let valruc = identificacion.split('');
    let aux = 0, prod;
    let veri = 0;
    veri = parseInt(valruc[10].toString()) + parseInt(valruc[11].toString()) + parseInt(valruc[12].toString());

    if (veri > 0) {
      /*int[] coeficiente = new int[9] { 4, 3, 2, 7, 6, 5, 4, 3, 2 };*/
      let coeficiente = [4, 3, 2, 7, 6, 5, 4, 3, 2];
      for (let i = 0; i < 9; i++) {
        prod = parseInt(valruc[i].toString()) * coeficiente[i];
        aux += prod;
      }
      if (aux % 11 == 0) {
        veri = 0;
      }
      else if (aux % 11 == 1) {
        return false;
      }
      else {
        aux = aux % 11;
        veri = 11 - aux;
      }

      if (veri == parseInt(valruc[9].toString())) {
        return true;
      }
      else {
        return false;
      }
    }
    else {
      return false;
    }
  }

  EsIdSectorPublico(identificacion: string): boolean {
    //let identificacion = identificacion1.split('');
    let valId = identificacion.split('');
    //let valId =Object.assign([], identificacion);

    let aux = 0, prod, veri;
    veri = parseInt(valId[9].toString()) + parseInt(valId[10].toString()) + parseInt(valId[11].toString()) + parseInt(valId[12].toString());
    if (veri > 0) {
      /*int[] coeficiente = new int[8] { 3, 2, 7, 6, 5, 4, 3, 2 };*/
      let coeficiente = [3, 2, 7, 6, 5, 4, 3, 2];
      for (let i = 0; i < 8; i++) {
        prod = parseInt(valId[i].toString()) * coeficiente[i];
        aux += prod;
      }

      if (aux % 11 == 0) {
        veri = 0;
      }
      else if (aux % 11 == 1) {
        return false;
      }
      else {
        aux = aux % 11;
        veri = 11 - aux;
      }

      if (veri == parseInt(valId[8].toString())) {
        return true;
      }
      else {
        return false;
      }
    }
    else {
      return false;
    }
  }
  
  
  CampoFormulario(nombreControl: string): any {
		return (formGroup: FormGroup) => {
			const arraySeleccionado = formGroup.get(nombreControl);
			
			if (arraySeleccionado!.value.length === 0) {
				arraySeleccionado?.setErrors({vacio: true});
			}
      if (arraySeleccionado!.value.length == 10) {
				arraySeleccionado?.setErrors({vacio: false});
			}
		
			// * elementos vacios
			if (arraySeleccionado!.value.length !== 0) {
				arraySeleccionado!.value.forEach((item: any) => {
					if (item === '') {
						arraySeleccionado?.setErrors({sinValores: true});
					}
				})
			}
		}
	}

}
