Aplikacja do symulacji zakładów bukmacherskich z systemem rekomendacji zakładów (pl)

Application for Simulating Betting with a Recommendation System for Bets (en)

There are 2 ways of running the application: 

1) Fully with Docker:
  - change database connection string in program.cs to "DatabaseDocker" (line 27, Program.cs)
  - run all containers using **docker compose up --build** terminal command in main directory

2) Database with Docker, Backend and Frontend locally (for better development*)
  - change database connection string in program.cs to "DatabaseLocal" (line 27, Program.cs)
  - run database container using **docker compose up --build database** terminal command in main directory
  - run backend locally using **dotnet run** or **dotnet watch** for hot reload in main directory
  - run frontend locally using **npm install** and then **npm start** in frontend directory (**cd frontend**)

* There is an option to run dotnet watch command in dockerfile to achieve hot reload using Docker,
but this solution has a lot of problems and bugs, which are not yet resolved. Therefore running
backend locally is more stable solution and enables faster checks of changes. 
