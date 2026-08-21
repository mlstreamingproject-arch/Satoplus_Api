# Fluxo de autenticacao JWT

## Visao geral

A API usa tres credenciais diferentes:

1. **Token de login inicial**: codigo curto, alfanumerico e de uso unico. Fica armazenado na tabela `initialtokenlogin`.
2. **AccessToken**: JWT usado para acessar endpoints protegidos.
3. **RefreshToken**: JWT usado exclusivamente pelo endpoint `ensureValidToken` para renovar a sessao.

Fluxo principal:

```text
POST /generateLoginToken
        |
        | gera e grava o codigo no MySQL
        v
POST /validateLoginToken
        |
        | consome o codigo uma unica vez
        | gera AccessToken e RefreshToken
        v
Endpoints protegidos com Authorization: Bearer <AccessToken>
        |
        | quando necessario
        v
POST /ensureValidToken
        |
        | confirma o AccessToken ou gera um novo par
        v
Novo AccessToken + novo RefreshToken
```

## 1. Gerar o token de login

### Endpoint

```http
POST /generateLoginToken
Content-Type: application/json
```

### Corpo

```json
{
  "userId": 10,
  "model": "SM-A256E"
}
```

### Comportamento

- Gera um codigo de 6 caracteres.
- Usa apenas letras maiusculas e numeros.
- Nao usa caracteres especiais.
- Grava o codigo em `initialtokenlogin`.
- A chave unica do banco impede repeticao do token.
- O registro inicia com `Status = 1` e `IsValidado = 0`.

### Resposta

```json
{
  "Token": "HPPPBF",
  "UserId": 10,
  "Model": "SM-A256E",
  "ExpiresInMinutes": 5
}
```

O `Token` deve ser tratado como codigo de login inicial. Ele nao deve ser enviado nas chamadas normais da API.

## 2. Fazer login e obter os JWTs

### Endpoint

```http
POST /validateLoginToken
Content-Type: application/json
```

### Corpo sem validade personalizada

```json
{
  "token": "HPPPBF"
}
```

### Corpo com validade personalizada

```json
{
  "token": "HPPPBF",
  "accessTokenDays": 7
}
```

`accessTokenDays` e opcional. O limite maximo configurado atualmente e de 30 dias:

```xml
<add key="JwtMaxAccessTokenDays" value="30" />
```

### Comportamento

A API:

1. Normaliza o token para maiusculas.
2. Exige exatamente 6 caracteres alfanumericos.
3. Localiza o registro ativo no MySQL.
4. Aceita somente registros com `Status = 1` e `IsValidado = 0`.
5. Atualiza o registro de forma atomica:
   - `Status = 0`
   - `IsValidado = 1`
   - `DateValidate = CURRENT_DATE`
   - `UpdateDate` e atualizado pelo MySQL
6. Usa o `UserId` e o `Model` armazenados no banco.
7. Gera o `AccessToken` e o `RefreshToken`.

Uma segunda chamada com o mesmo token retorna `401 Unauthorized`.

### Resposta

```json
{
  "AccessToken": "eyJ...",
  "RefreshToken": "eyJ...",
  "UserId": 10,
  "Model": "SM-A256E",
  "ExpiresInSeconds": 604800,
  "AccessTokenExpiresAt": "2026-08-28T15:54:13.0000000Z",
  "RefreshTokenExpiresAt": "2026-08-28T15:54:13.0000000Z"
}
```

As datas sao UTC e usam formato ISO 8601. O campo `ExpiresInSeconds` permanece por compatibilidade.

## 3. Manter ou renovar a sessao

### Endpoint

```http
POST /ensureValidToken
Content-Type: application/json
```

### Corpo

```json
{
  "accessToken": "ACCESS_TOKEN_ATUAL",
  "refreshToken": "REFRESH_TOKEN_ATUAL"
}
```

### AccessToken ainda valido

A API devolve o mesmo access token e nao retransmite o refresh token:

```json
{
  "AccessToken": "ACCESS_TOKEN_ATUAL",
  "RefreshToken": null,
  "UserId": "10",
  "Model": "SM-A256E",
  "ExpiresInSeconds": 604799,
  "AccessTokenExpiresAt": "2026-08-28T15:54:13.0000000Z",
  "RefreshTokenExpiresAt": null,
  "Refreshed": false
}
```

### AccessToken expirado

Se o access token estiver expirado, a API valida o refresh token e, se ele ainda for valido, devolve um novo par:

```json
{
  "AccessToken": "NOVO_ACCESS_TOKEN",
  "RefreshToken": "NOVO_REFRESH_TOKEN",
  "UserId": "10",
  "Model": "SM-A256E",
  "ExpiresInSeconds": 604800,
  "AccessTokenExpiresAt": "2026-09-04T15:54:13.0000000Z",
  "RefreshTokenExpiresAt": "2026-09-04T15:54:13.0000000Z",
  "Refreshed": true
}
```

O app deve substituir os tokens armazenados pelos valores novos quando `Refreshed` for `true`.

### Ambos invalidos

Se o access token e o refresh token estiverem expirados ou invalidos:

```http
401 Unauthorized
```

Nesse caso, a aplicacao precisa iniciar um novo login usando um novo token de `initialtokenlogin`.

## 4. Usar o AccessToken

Os endpoints protegidos devem receber somente o access token no cabecalho:

```http
Authorization: Bearer eyJ...
```

Atualmente exigem `[JwtAuthorize]`:

```text
POST /configurations/createconfiguration
PUT  /configurations/updateconfiguration/{id}
POST /emailcontent/createemailcontent
PUT  /emailcontent/updateemailcontent/{id}
```

O atributo valida:

- esquema `Bearer`;
- assinatura HMAC-SHA256;
- emissor `MeuProxySsl`;
- audiencia `TokuPlusApp`;
- validade do JWT.

Se o access token estiver expirado, o endpoint protegido retorna `401`. O app deve chamar `ensureValidToken`, atualizar os tokens e repetir a operacao original uma unica vez.

## 5. Validades configuradas

```xml
<add key="JwtAccessTokenExpirationHours" value="6" />
<add key="JwtMaxAccessTokenDays" value="30" />
<add key="JwtRefreshTokenExpirationDays" value="7" />
```

Regras:

- Sem `accessTokenDays`, o access token usa a validade padrao de 6 horas.
- Sem `accessTokenDays`, o refresh token usa a validade padrao de 7 dias.
- Com `accessTokenDays`, o valor deve estar entre 1 e 30 dias.
- A implementacao atual grava `accessTokenDays` como claim para reaplicar essa validade durante o refresh.
- Ao informar `accessTokenDays`, a implementacao atual calcula access e refresh com o mesmo instante de expiracao. Nesse caso, o refresh nao conseguira renovar depois que o access token expirar. Para que o refresh seja util nesse modo, a validade do refresh deve ser alterada para ser maior que a do access token.

## 6. Tabela do token inicial

```sql
SELECT Id, Token, UserId, Model, CreateDate,
       IsValidado, Status, UpdateDate, DateValidate
FROM initialtokenlogin
ORDER BY Id DESC;
```

Estados esperados:

### Token recem-gerado

```text
Status = 1
IsValidado = 0
DateValidate = NULL
```

### Token consumido no login

```text
Status = 0
IsValidado = 1
DateValidate preenchido
UpdateDate preenchido
```

## 7. Boas praticas para o app

- Armazene `AccessToken`, `RefreshToken` e as datas de expiracao em armazenamento seguro.
- Nunca envie o `RefreshToken` para endpoints de negocio.
- Use `AccessTokenExpiresAt` para renovar preventivamente.
- Trate `401` chamando `ensureValidToken` e repita a requisicao original apenas uma vez.
- Evite varias chamadas simultaneas de refresh; uma chamada deve renovar e as demais devem aguardar o resultado.
- Gere um novo login quando o refresh token tambem estiver expirado.
- Nao registre tokens em logs.
- Use HTTPS em todos os ambientes expostos.
