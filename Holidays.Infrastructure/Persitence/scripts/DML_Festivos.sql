USE Festivos;
GO

--Registros tabla TIPO
INSERT INTO Tipo(Tipo) VALUES('Fijo');
INSERT INTO Tipo(Tipo) VALUES('Ley Puente Festivo');
INSERT INTO Tipo(Tipo) VALUES('Basado en Pascua');
INSERT INTO Tipo(Tipo) VALUES('Basado en Pascua y Ley Puente Festivo');
INSERT INTO Tipo(Tipo) VALUES('Ley Puente Festivo Viernes');

/*INSERT INTO Tipo(Tipo) VALUES('Fijo', 'No se puede variar.');
INSERT INTO Tipo(Tipo) VALUES('Ley Puente Festivo', 'Se traslada la fecha al siguiente lunes.');
INSERT INTO Tipo(Tipo) VALUES('Basado en Pascua', 'La fecha se calcula obteniendo la fecha del domingo de pascua y sumándole los días que correspondan.');
INSERT INTO Tipo(Tipo) VALUES('Basado en Pascua y Ley Puente Festivo', 'La fecha se calcula obteniendo la fecha del domingo de pascua y sumándole los días que correspondan. La fecha calculada debe ser trasladada al siguiente lunes.');
INSERT INTO Tipo(Tipo) VALUES('Ley Puente Festivo Viernes', 'No definida.');*/

--Registros tabla PAIS
INSERT INTO Pais (Nombre) VALUES('COLOMBIA');
INSERT INTO Pais (Nombre) VALUES('ARGENTINA');
INSERT INTO Pais (Nombre) VALUES('BOLIVIA');
INSERT INTO Pais (Nombre) VALUES('BRASIL');
INSERT INTO Pais (Nombre) VALUES('CANADA');
INSERT INTO Pais (Nombre) VALUES('COSTA RICA');
INSERT INTO Pais (Nombre) VALUES('REPUBLICA DOMINICANA');
INSERT INTO Pais (Nombre) VALUES('CUBA');
INSERT INTO Pais (Nombre) VALUES('CHILE');
INSERT INTO Pais (Nombre) VALUES('ECUADOR');
INSERT INTO Pais (Nombre) VALUES('ESTADOS UNIDOS DE AMÉRICA');
INSERT INTO Pais (Nombre) VALUES('GUATEMALA');
INSERT INTO Pais (Nombre) VALUES('HONDURAS');
INSERT INTO Pais (Nombre) VALUES('MÉXICO');
INSERT INTO Pais (Nombre) VALUES('NICARAGUA');
INSERT INTO Pais (Nombre) VALUES('PANAMA');
INSERT INTO Pais (Nombre) VALUES('PARAGUAY');
INSERT INTO Pais (Nombre) VALUES('PERU');
INSERT INTO Pais (Nombre) VALUES('URUGUAY');
INSERT INTO Pais (Nombre) VALUES('VENEZUELA');
INSERT INTO Pais (Nombre) VALUES('ESPAÑA');

--Registros tabla FESTIVO
INSERT INTO Festivo (IdPais, Dia, Mes, Nombre, IdTipo, DiasPascua) VALUES(1, 1, 1, 'Año nuevo', 1, 0);
INSERT INTO Festivo (IdPais, Dia, Mes, Nombre, IdTipo, DiasPascua) VALUES(1, 6, 1, 'Santos Reyes', 2, 0);
INSERT INTO Festivo (IdPais, Dia, Mes, Nombre, IdTipo, DiasPascua) VALUES(1, 19, 3, 'San José', 2, 0);
INSERT INTO Festivo (IdPais, Dia, Mes, Nombre, IdTipo, DiasPascua) VALUES(1, 0, 0, 'Jueves Santo', 3, -3);
INSERT INTO Festivo (IdPais, Dia, Mes, Nombre, IdTipo, DiasPascua) VALUES(1, 0, 0, 'Viernes Santo', 3, -2);
INSERT INTO Festivo (IdPais, Dia, Mes, Nombre, IdTipo, DiasPascua) VALUES(1, 0, 0, 'Domingo de Pascua', 3, 0);
INSERT INTO Festivo (IdPais, Dia, Mes, Nombre, IdTipo, DiasPascua) VALUES(1, 1, 5, 'Día del Trabajo', 1, 0);
INSERT INTO Festivo (IdPais, Dia, Mes, Nombre, IdTipo, DiasPascua) VALUES(1, 0, 0, 'Ascensión del Señor', 4, 40);
INSERT INTO Festivo (IdPais, Dia, Mes, Nombre, IdTipo, DiasPascua) VALUES(1, 0, 0, 'Corpus Christi', 4, 61);
INSERT INTO Festivo (IdPais, Dia, Mes, Nombre, IdTipo, DiasPascua) VALUES(1, 0, 0, 'Sagrado Corazón de Jesús', 4, 68);
INSERT INTO Festivo (IdPais, Dia, Mes, Nombre, IdTipo, DiasPascua) VALUES(1, 29, 6, 'San Pedro y San Pablo', 2, 0);
INSERT INTO Festivo (IdPais, Dia, Mes, Nombre, IdTipo, DiasPascua) VALUES(1, 20, 7, 'Independencia Colombia', 1, 0);
INSERT INTO Festivo (IdPais, Dia, Mes, Nombre, IdTipo, DiasPascua) VALUES(1, 7, 8, 'Batalla de Boyacá', 1, 0);
INSERT INTO Festivo (IdPais, Dia, Mes, Nombre, IdTipo, DiasPascua) VALUES(1, 15, 8, 'Asunción de la Virgen', 2, 0);
INSERT INTO Festivo (IdPais, Dia, Mes, Nombre, IdTipo, DiasPascua) VALUES(1, 12, 10, 'Día de la Raza', 2, 0);
INSERT INTO Festivo (IdPais, Dia, Mes, Nombre, IdTipo, DiasPascua) VALUES(1, 1, 11, 'Todos los santos', 2, 0);
INSERT INTO Festivo (IdPais, Dia, Mes, Nombre, IdTipo, DiasPascua) VALUES(1, 11, 11, 'Independencia de Cartagena', 2, 0);
INSERT INTO Festivo (IdPais, Dia, Mes, Nombre, IdTipo, DiasPascua) VALUES(1, 8, 12, 'Inmaculada Concepción', 1, 0);
INSERT INTO Festivo (IdPais, Dia, Mes, Nombre, IdTipo, DiasPascua) VALUES(1, 25, 12, 'Navidad', 1, 0);

INSERT INTO Festivo (IdPais, Dia, Mes, Nombre, IdTipo, DiasPascua) VALUES(10, 1, 1, 'Año nuevo', 1, 0);
INSERT INTO Festivo (IdPais, Dia, Mes, Nombre, IdTipo, DiasPascua) VALUES(10, 0, 0, 'Carnaval 1', 3, -43);
INSERT INTO Festivo (IdPais, Dia, Mes, Nombre, IdTipo, DiasPascua) VALUES(10, 0, 0, 'Carnaval 2', 3, -42);
INSERT INTO Festivo (IdPais, Dia, Mes, Nombre, IdTipo, DiasPascua) VALUES(10, 0, 0, 'Viernes Santo', 3, -2);
INSERT INTO Festivo (IdPais, Dia, Mes, Nombre, IdTipo, DiasPascua) VALUES(10, 1, 5, 'Día del Trabajo', 5, 0);
INSERT INTO Festivo (IdPais, Dia, Mes, Nombre, IdTipo, DiasPascua) VALUES(10, 24, 5, 'Batalla de Pichincha', 1, 0);
INSERT INTO Festivo (IdPais, Dia, Mes, Nombre, IdTipo, DiasPascua) VALUES(10, 10, 8, 'Primer Grito de Independencia', 5, 0);
INSERT INTO Festivo (IdPais, Dia, Mes, Nombre, IdTipo, DiasPascua) VALUES(10, 9, 10, 'Independencia de Guayaquil', 5, 0);
INSERT INTO Festivo (IdPais, Dia, Mes, Nombre, IdTipo, DiasPascua) VALUES(10, 2, 11, 'Día de los Difuntos', 5, 0);
INSERT INTO Festivo (IdPais, Dia, Mes, Nombre, IdTipo, DiasPascua) VALUES(10, 3, 11, 'Independencia de Cuenca', 5, 0);
INSERT INTO Festivo (IdPais, Dia, Mes, Nombre, IdTipo, DiasPascua) VALUES(10, 25, 12, 'Navidad', 5, 0);