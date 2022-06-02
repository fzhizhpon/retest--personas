import { Injectable } from '@angular/core';

@Injectable({
	providedIn: 'root'
})
export class EdadService {

	constructor() { }

	CalcularEdad(edad: any): string {
		if (edad) {
			let anio = edad?.split('-');
			let today_date = new Date();
			let today_year = today_date.getFullYear();
			let today_month = today_date.getMonth();
			let today_day = today_date.getDate();
			let age = today_year - anio[0];

			if (today_month < (anio[1] - 1)) {
				age--;
			}
			if (((anio[1] - 1) == today_month) && (today_day < anio[2])) {
				age--;
			}
			if (age < 1) {
				//console.log(today_month - anio[1]);
				/*if ((today_month - anio[1]) === 0) {
					console.log(today_day);
					console.log(anio[2]);
					return (today_day - anio[2]) + " días";
				}else*/
				return Math.abs(today_month - anio[1]) + " meses"
			}
			if (age > 1000) {
				return "definir fecha";
			}
			return Math.abs(age) + " años";

		}
		return '';

	}
}
