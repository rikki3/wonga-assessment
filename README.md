# Wonga
### React + .NET Core + Postgres + Docker
##
### Run 
1) Start or ensure Docker Engine is running.
2) Open a terminal and run the following command:
```bash
git clone https://github.com/rikki3/wonga-assessment.git && cd wonga-assessment && docker compose up --build
```
3) Access the React frontend and C# API backend at the URLs below.\
If the port numbers assigned to the Docker Containers differs, you will have to \
confirm the ports in Docker and adjust the URLs below accordingly.

### URLs
* React: \
http://localhost:3000
* API: \
http://localhost:8080/swagger
* Postgres: \
localhost:5432

### Unit Testing
1) Open a terminal to the "backend\Wonga.Api.Tests" directory and run the following command:
```bash
dotnet test
```
