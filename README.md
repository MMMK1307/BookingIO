# BookingIO

# EF Core
	## Apenas uma vez:
		dotnet tool install dotnet-ef --local --version 7.0

	dotnet ef migrations add Nome_Migration
	dotnet ef database update

# Tasks
	- Common
		- Home
	- Reservations
	    - Reserve + Search

    - Organizar as páginas já criadas

    - Finalizar o Cadastro: 
        - Space
        - SpaceType
        - User Groups (MK)

    - Modificações Models
        - Reserve.status. string => ReserveStatus
        - Space.Location (-75.0393, +10.2039)

    - Busca por Espaços
        - Finalizar busca
        - Trazer previsão do tempo para os próximos dias

    - Reservas
        - Página do Espaços com detalhes e reservas já feitas
        - Função para checkar a disponibilidade de certa reserva

