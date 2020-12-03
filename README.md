
<p align="center">
  <a href="#">
    <img src="https://encrypted-tbn0.gstatic.com/images?q=tbn%3AANd9GcS49HPDAbme5yIhOyFf-rOKCAzmXPCOdQyqMsbdE0tP61uE8ccP&usqp=CAU" alt="Logo" width="110" height="110">
  </a>
  <h1 align="center">Singular Roulette</h1>
  <p align="center">
    Application for a virtual roulette game
    <br />

 
  </p>
  <br>  
  
  ## Contents

- [About](#About)
- [Prerequisites](#Production-Prerequisites)
- [Notes](#Notes)
- [Authentication](#Authentication)
---

## About
Project is written on <b>.net 5.0</b>, based on <b>jwt token authentication</b>, with <b>sql server</b> database. see more below:

- Dapper
- NSwag
- Fluent Migration
- Fluent Validation
- Sentry (auto logging all errors)
- Xunit
---


## Production Prerequisites 
- Clone this repo to your local machine using `https://github.com/sandr01/Singular.Roulette.Api.git`

```shell
$ clone https://github.com/sandr01/Roulette.Api.git
```
- Find <B>docker-compose.production.yml </B> file is solution folder and run in cmd for production purposes (command below)
```shell
$ run docker-compose -f docker-compose.production.yml up
```
> Navigate :
 http://localhost:8000/api
>
---

## Debug Prerequisites

- Find <B>docker-compose.development.yml </B> file is solution folder and run in cmd for development purposes (command below)
```shell
$ run docker-compose -f docker-compose.development.yml up
```
---
## Notes
- <b>docker-compose.production.yml</b> installs .net 5, sql server and builds all projects in docker containers.
- <b>docker-compose.development.yml</b> installs sql server and you can build project with <b>IIS</b> and debug it.
- Connect from management studio with server name <b>"localhost, 1433"</b>, <b>username "sa", Password "Asdfqwer1234"</b>
- There must not be, persistant/important data in docker containers
- You don't need to build unit tests with docker
- You don't need SSL in project built from docker
---

<h4>Default user : "Admin"</h4>
<h4>Default password : "h*{V?Nw,7?y`A*x8"</h4>

---


## **Authentication**
![Recordit GIF](Auth.gif)
---
---


## Author
👤 **Aleksandre Sisauri**

- Linkedin: [@aleksandre-sisauri](https://www.linkedin.com/in/aleksandre-sisauri-a81442b8/)
---
