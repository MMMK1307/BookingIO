# BookingIO

# EF Core
	## Apenas uma vez:
		dotnet tool install dotnet-ef --local --version 7.0

	dotnet ef migrations add Nome_Migration
	dotnet ef database update

Grupo
	nome
	desc
	nivel

User:
	nome
	email
	senha
	grupo

TipoEspaço
	nome
	desc

Espaço
	nome
	desc
	capacidade
	tipo
	status

Reserva
	espaço
	perido
		inicio
		fim
	user
	
	
