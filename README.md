## 🧐 Sobre

<p align="left"> 
O projeto Trybe Hotel se trata da implementação de uma Api de um site de reserva de uma rede de Hoteis, onde nessa quarta fase foram implementados os seguintes processos:

### Desenvolvimento de um sistema de "ping".
  - O endpoint GET / deve retornar um status de sucesso com o seguinte corpo de resposta
```json
{
    "message": "online"
}
```
### Foi desenvolvido o Dockerfile da aplicação para conteineriza-la visando um futuro deploy.

### Estão disponíveis as seguintes rotas na aplicação:

### Cadastro e consulta de cidades
  - Rota de consulta GET /city
    - O corpo da resposta deve seguir o formato abaixo:

```json
[
    {
	    "cityId": 1,
	    "name": "Rio Branco"
      "state": "AC"
    },

  /*...*/
]
```
  - Rota de cadastro de cidades POST /city
    - O corpo da requisição deve seguir o padrão abaixo

```json
{
	"Name": "Rio de Janeiro"
  "State": "RJ"
}
```
### Cadastro e consulta de hotéis
  - Rota de consulta GET /hotel
    - O corpo da resposta deve seguir o formato abaixo:

```json
[
    {
		  "hotelId": 1,
		  "name": "Trybe Hotel SP",
		  "address": "Avenida Paulista, 1400",
		  "cityId": 1,
		  "cityName": "São Paulo",
      "state": "SP"
	  },

  /*...*/
]
```
  - Rota de cadastro de Hoteis POST /hotel
    - O corpo da requisição deve seguir o padrão abaixo
    - Essa rota requer Token com nível de acesso de admin

```json
{
	"Name":"Trybe Hotel RJ",
	"Address":"Avenida Atlântica, 1400",
	"CityId": 2
}
```
### Cadastro, consulta e exclusão de quartos
  - Rota de consulta de quarto GET /room/:hotelId
    - O corpo da resposta deve seguir o formato abaixo:

```json
[
    {
		  "roomId": 1,
		  "name": "Suite básica",
		  "capacity": 2,
		  "image": "image suite",
		  "hotel": {
  			"hotelId": 1,
			  "name": "Trybe Hotel SP",
			  "address": "Avenida Paulista, 1400",
			  "cityId": 1,
			  "cityName": "São Paulo",
        "state": "SP"
		  }
	  },

  /*...*/
]
```
  - Rota de cadastro de Quartos POST /room
    - O corpo da requisição deve seguir o padrão abaixo
    - - Essa rota requer Token com nível de acesso de admin

```json
{
	"Name":"Suite básica",
	"Capacity":2,
	"Image":"image suite",
	"HotelId": 1
}
```
  - Rota de exclusão de Quartos DELETE /room/:roomId
  - Essa rota requer Token com nível de acesso de admin
### Cadastro de pessoas usuárias
  - Rota de cadastro POST /user
    - O corpo da requisição deve seguir o formato abaixo:

```json
{
	"Name":"Rebeca",
	"Email": "rebeca.santos@trybehotel.com",
	"Password": "123456"
}
```
### Login
  - Rota Login POST /login
    - O corpo da requisição deve seguir o formato abaixo:

```json
{
	"Email": "rebeca.santos@trybehotel.com",
	"Password": "123456"
}

```
### Cadastro e listagem de reservas
  - Rota de cadastro de reservas POST /booking
    - O corpo da requisição deve seguir o formato abaixo:
    - Essa rota requer Token com nível de acesso de user

```json
{
	"CheckIn":"2030-08-27",
	"CheckOut":"2030-08-28",
	"GuestQuant":"1",
	"RoomId":1
}
```
  - Rota de listagem de reserva GET /booking/:id
    - O corpo da requisição é vazio
    - Essa rota requer Token com nível de acesso de admin
   
## ⚒ Instalando <a name = "installing"></a>

```bash
# Clone o projeto
$ git clone git@github.com:wesleymktd/project-trybe-hotel-fase-a.git
# Acesse
$ cd ./project-trybe-hotel-fase-a/src
# Instale as dependencias
$ dotnet restore
# Acesse o diretório TrybeHotel
$ cd TrybeHotel
# Inicie o projeto
$ dotnet run

```

## ⚒ Testes automatizados <a name = "installing"></a>

```bash
# Clone o projeto
$ git clone git@github.com:wesleymktd/project-trybe-hotel-fase-b.git
# Acesse o diretório TrybeHotel.test

# Execute o comando dotnet test
# para filtrar por algum teste específico execute o comando
$ dotnet test --filter `nome_do_teste`

```

## Principais tecnologias utilizadas:
- C#;
- ASP.NET
- EntityFramework
- Xunit
- JWT
- SQL Server
