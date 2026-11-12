import { Routes } from '@angular/router';
import { CountryForm } from './features/countries/country-form/country-form';
import { CountryList } from './features/countries/country-list/country-list';
import { HolidayCheck } from './features/holiday-check/holiday-check';
import { HolidayTypeForm } from './features/holiday-types/holiday-type-form/holiday-type-form';
import { HolidayTypeList } from './features/holiday-types/holiday-type-list/holiday-type-list';
import { HolidayForm } from './features/holidays/holiday-form/holiday-form';
import { HolidayList } from './features/holidays/holiday-list/holiday-list';

// Order matters: 'paises/nuevo' must come before 'paises/:id'
export const routes: Routes = [
  { path: '', redirectTo: 'validacion', pathMatch: 'full' },
  { path: 'validacion', component: HolidayCheck, title: 'Validación de festivos' },

  { path: 'paises', component: CountryList, title: 'Países' },
  { path: 'paises/nuevo', component: CountryForm, title: 'Nuevo país' },
  { path: 'paises/:id', component: CountryForm, title: 'Editar país' },

  { path: 'tipos', component: HolidayTypeList, title: 'Tipos de festivo' },
  { path: 'tipos/nuevo', component: HolidayTypeForm, title: 'Nuevo tipo' },
  { path: 'tipos/:id', component: HolidayTypeForm, title: 'Editar tipo' },

  { path: 'festivos', component: HolidayList, title: 'Festivos' },
  { path: 'festivos/nuevo', component: HolidayForm, title: 'Nuevo festivo' },
  { path: 'festivos/:id', component: HolidayForm, title: 'Editar festivo' },

  { path: '**', redirectTo: 'validacion' },
];
