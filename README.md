# ApiJwt: Autenticação com JWT em ASP.NET Core

API desenvolvida em C# (.NET 10) para a lista de exercícios sobre APIs, autenticação, autorização e proteção de rotas. O objetivo é demonstrar o funcionamento da autenticação com **JWT (JSON Web Token)**.

> **Aviso:** o usuário, a senha e a chave JWT deste projeto são **fixos e didáticos**. Não representam credenciais reais e não devem ser usados em produção.

## Tecnologias

- C# e ASP.NET Core Web API (.NET 10)
- Pacote `Microsoft.AspNetCore.Authentication.JwtBearer`
- Postman (testes)

## Estrutura do projeto

```
ApiJwt/
├── Controllers/
│   ├── AuthController.cs      # Rota de login e geração do token
│   └── UsuarioController.cs   # Rota protegida com [Authorize]
├── Models/
│   └── LoginRequest.cs        # Modelo do JSON de login
├── evidencias/                # Prints dos testes no Postman
├── appsettings.json           # Configuração do JWT (chave didática)
├── Program.cs                 # Configuração da autenticação
└── README.md
```

## Como executar

**Pré-requisito:** [.NET 10 SDK](https://dotnet.microsoft.com/download) instalado.

1. Clone o repositório e entre na pasta do projeto:
   ```bash
   git clone <url-do-repositorio>
   cd ApiJwt
   ```
2. Restaure os pacotes e execute:
   ```bash
   dotnet restore
   dotnet run
   ```
3. A API sobe em `http://localhost:5195`. Confira a porta na linha `Now listening on:` do terminal, pois ela pode variar.

## Credenciais de teste

| Campo   | Valor    |
|---------|----------|
| Usuário | `admin`  |
| Senha   | `123456` |

Os valores estão fixos no `AuthController`, conforme o enunciado do exercício (sem banco de dados).

## Rotas

| Método | Rota                  | Acesso        | Descrição                                                    |
|--------|-----------------------|---------------|--------------------------------------------------------------|
| POST   | `/api/auth/login`     | Pública       | Recebe usuário e senha em JSON e retorna um token JWT        |
| GET    | `/api/usuario/perfil` | `[Authorize]` | Retorna uma mensagem com o nome do usuário autenticado       |

### POST /api/auth/login

**Requisição:**
```json
{
  "usuario": "admin",
  "senha": "123456"
}
```

**Resposta (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs..."
}
```

**Resposta (401 Unauthorized):**
```json
{
  "mensagem": "Usuário ou senha inválidos."
}
```

### GET /api/usuario/perfil

**Cabeçalho obrigatório:**
```
Authorization: Bearer <token>
```

**Resposta (200 OK):**
```json
{
  "mensagem": "Olá, admin! Você está autenticado."
}
```

**Sem token, com token inválido ou expirado:** `401 Unauthorized`.

## Como testar no Postman

Com a API em execução (`dotnet run`), faça os testes abaixo.

### Teste 1: Login válido
- `POST http://localhost:5195/api/auth/login`
- Body → raw → **JSON**:
  ```json
  { "usuario": "admin", "senha": "123456" }
  ```
- **Esperado:** `200 OK` com o token JWT.
- Copie o token (somente o texto, sem aspas) para usar no Teste 4.

### Teste 2: Login inválido
- Mesma requisição do Teste 1, com senha incorreta:
  ```json
  { "usuario": "admin", "senha": "senhaerrada" }
  ```
- **Esperado:** `401 Unauthorized`.

### Teste 3: Acesso sem token
- `GET http://localhost:5195/api/usuario/perfil`
- Authorization → **No Auth**.
- **Esperado:** `401 Unauthorized`.

### Teste 4: Acesso com token válido
- `GET http://localhost:5195/api/usuario/perfil`
- Authorization → Type: **Bearer Token** → cole o token obtido no Teste 1.
- **Esperado:** `200 OK` com a mensagem `Olá, admin! Você está autenticado.`

> O token expira em 30 minutos. Se o Teste 4 retornar `401`, faça o login novamente e use o novo token.

## Evidências dos testes

Os prints da execução no Postman estão na pasta [`evidencias/`](evidencias/):

| Teste | Descrição                | Resultado       | Arquivo                                  |
|-------|--------------------------|-----------------|------------------------------------------|
| 1     | Login válido             | `200 OK`        | `evidencias/teste1-login-valido.png`     |
| 2     | Login inválido           | `401`           | `evidencias/teste2-login-invalido.png`   |
| 3     | Acesso com token válido  | `200 OK`        | `evidencias/teste4-com-token.png`        |

## Como funciona

1. O cliente envia usuário e senha para `POST /api/auth/login`.
2. A API confere as credenciais fixas. Se estiverem corretas, gera um JWT assinado com a chave configurada em `appsettings.json`, contendo o nome do usuário e a data de expiração.
3. O cliente envia o token no cabeçalho `Authorization: Bearer <token>` nas requisições seguintes.
4. Nas rotas com