# TokuPlus API Proxy

Proxy reverso .NET Framework 4.8 que encaminha requisições para a API TokuPlus upstream (OutSystems Cloud).

## Arquitetura

- **Framework:** .NET Framework 4.8
- **Hosting:** OWIN self-host (console) ou IIS/IIS Express
- **Padrão:** ASP.NET Web API + CORS
- **Upstream:** `https://personal-b0zlpsmh.outsystemscloud.com/TokuPlus_API/rest/RESTAPITOKUPLUS/`

## Configuração

Edite `MeuProxySsl/Web.config`:

```xml
<appSettings>
  <add key="Upstream:BaseUrl" value="https://personal-b0zlpsmh.outsystemscloud.com/TokuPlus_API/rest/RESTAPITOKUPLUS/" />
  <add key="Upstream:IgnoreInvalidCertificate" value="true" />
</appSettings>
```

- **Upstream:BaseUrl:** URL base da API TokuPlus (deve terminar com `/`)
- **Upstream:IgnoreInvalidCertificate:** `true` ignora erros de certificado SSL (usar apenas em desenvolvimento)

## Execução

### Como Exe (OWIN Self-Host)

```bash
cd MeuProxySsl
dotnet build
.\bin\Debug\net48\MeuProxySsl.exe
```

Servidor roda em `http://localhost:5002`

### Em IIS/IIS Express

1. Abra a solução em Visual Studio 2022+
2. Clique em "Run IIS Express" (ou pressione F5)
3. Site estará disponível em `http://localhost:5002`

## Endpoints da API TokuPlus

Todos os endpoints abaixo são automaticamente encaminhados para o upstream. Acesse via `http://localhost:5002/{endpoint}`.

| # | Endpoint | Método | Parâmetros | Descrição |
|---|----------|--------|-----------|-----------|
| 1 | `createOrDeleteFavorites` | GET | `UserId`, `SerieId` | Adiciona/remove série dos favoritos |
| 2 | `createOrUpdateTokenTV` | GET | `UserId`, `Token` | Cria ou atualiza token de TVBox |
| 3 | `getContinuousEpisodesUser` | GET | `UserId`, `SerieId` | Obtém episódios contínuos do usuário |
| 4 | `getEpisodes` | GET | `Id`, `SerieId`, `Status`, `UserId` | Lista episódios com filtros |
| 5 | `getFavorites` | GET | `UserId` | Lista séries favoritas do usuário |
| 6 | `getGenre` | GET | *nenhum* | Lista todos os gêneros |
| 7 | `getInitial` | GET | `Filter`, `MaxRegisters`, `UserId`, `Id` *(opt)*, `Status` *(opt)* | Conteúdo inicial com filtros |
| 8 | `getReleases` | GET | `UserId`, `Filter` | Lista novos lançamentos |
| 9 | `getSerieCatalogo` | GET | `MaxRegisters`, `UserId`, `GenreId`, `Filter` | Catálogo de séries |
| 10 | `getSeriesHigh` | GET | `Number`, `UserId` | Top séries mais assistidas |
| 11 | `getTvChannels` | GET | *nenhum* | Lista canais de TV |
| 12 | `getUserEpisodes` | GET | `MaxRecord`, `UserId`, `Filter` | Episódios do usuário |
| 13 | `SearchCatalogo` | GET | `UserId`, `Text` | Busca séries por texto |
| 14 | `ValidarLinkTV` | GET | `InfoToken` | Valida link de TV |
| 15 | `ValidarToken` | GET | `InfoToken`, `InfoDevice` | Valida token de dispositivo |
| 16 | `validateTVBoxToken` | GET | `Token` | Valida token de TVBox |
| 17 | `putCreateUserAccess` | PUT | `UserId`, `Plataform`, `IP` | Cria/atualiza acesso do usuário |

### Exemplos de Requisição

#### 1. getSerieCatalogo
```bash
curl "http://localhost:5002/getSerieCatalogo?MaxRegisters=10&UserId=1&GenreId=0&Filter=0"
```

**Resposta (200 OK):**
```json
[
  {
    "Id": 1,
    "TypeMovieId": 1,
    "GenreId": 5,
    "NameGenre": "Henshin Hero",
    "Year": "1978",
    "Name": "Série X",
    "Description": "...",
    "LinkImageMenor": "https://tokuplus.com/...",
    "Status": 3
  }
]
```

#### 2. getFavorites
```bash
curl "http://localhost:5002/getFavorites?UserId=1"
```

**Resposta (200 OK):** Array com séries favoritas do usuário

#### 3. getTvChannels
```bash
curl "http://localhost:5002/getTvChannels"
```

**Resposta (200 OK):**
```json
[
  {
    "Id": 6,
    "Name": "Toku+ TV",
    "Link": "https://stmv1.srvif.com/tokuplus/tokuplus/playlist.m3u8",
    "Img": "https://tokuplus.com/Img/TVS/TOKUTV.png",
    "Order": 1,
    "IsDefault": true
  }
]
```

#### 4. getGenre
```bash
curl "http://localhost:5002/getGenre"
```

**Resposta (200 OK):** Array com todos os gêneros

#### 5. validateTVBoxToken
```bash
curl "http://localhost:5002/validateTVBoxToken?Token=abc123"
```

**Resposta (200 OK):**
```json
{
  "IsSuccess": true,
  "Message": "Token não existe"
}
```

#### 6. SearchCatalogo
```bash
curl "http://localhost:5002/SearchCatalogo?UserId=1&Text=Kamen"
```

**Resposta (200 OK):** Array com resultados da busca

#### 7. putCreateUserAccess (PUT)
```bash
curl -X PUT "http://localhost:5002/putCreateUserAccess?UserId=1&Plataform=iOS&IP=192.168.1.100"
```

**Resposta (200 OK):** Usuário criado/atualizado

### Validação de Parâmetros

Parâmetros **obrigatórios**são validados. Se faltar, retorna 400 Bad Request:

```bash
curl "http://localhost:5002/SearchCatalogo?UserId=1"
# Erro: Text is required
```

Parâmetros com **caracteres especiais** são automaticamente codificados (URL encoding):

```bash
curl "http://localhost:5002/SearchCatalogo?UserId=1&Text=Sentai%20Uchu"
```

## Comportamento do Proxy

- ✅ Encaminha todos os métodos HTTP (GET, POST, PUT, DELETE, PATCH, OPTIONS)
- ✅ Preserva cabeçalhos de requisição
- ✅ Valida certificados SSL da upstream (adaptável via config)
- ✅ Streaming de respostas (sem buffering completo)
- ✅ CORS habilitado (todas as origens permitidas em dev)

## Arquivos Principais

| Arquivo | Descrição |
|---------|-----------|
| `MeuProxySsl.csproj` | Definição de projeto, dependências OWIN/WebAPI |
| `Program.cs` | Entry point do self-host OWIN |
| `Startup.cs` | Configuração de rotas WebAPI e middleware |
| `Global.asax.cs` | Aplicação httpmodule (ASP.NET) |
| `Controllers/ProxyController.cs` | Rotas genéricas de proxy (catch-all) |
| `Web.config` | Configuração (BaseUrl upstream, SSL ignore) |
| `appsettings.json` | *(Legado - usar Web.config)* |

## Dependências

```xml
<PackageReference Include="Microsoft.AspNet.WebApi.Core" Version="5.2.9" />
<PackageReference Include="Microsoft.AspNet.WebApi.Client" Version="5.2.9" />
<PackageReference Include="Microsoft.AspNet.WebApi.Owin" Version="5.2.9" />
<PackageReference Include="Microsoft.Owin.Hosting" Version="4.2.0" />
<PackageReference Include="Microsoft.Owin.Host.SystemWeb" Version="4.2.0" />
<PackageReference Include="Microsoft.Owin.Cors" Version="4.2.0" />
```

## Deployment

### Para Exposição em https://api.tokuplus.com

1. Configure DNS apontando para o host do proxy
2. Instale certificado SSL válido (não use IgnoreInvalidCertificate em produção)
3. Configure IIS binding HTTPS na porta 443
4. Desabilite IgnoreInvalidCertificate no Web.config
5. Implemente autenticação/autorização conforme necessário

### Adicionar Autenticação

Para forçar API Key antes de encaminhar:

```csharp
// Em ProxyController.cs, antes de encaminhar:
var apiKey = http.Request.Headers.GetValues("X-API-Key").FirstOrDefault();
if (string.IsNullOrEmpty(apiKey) || apiKey != "seu-chave-secreta")
{
    http.Response.StatusCode = 401;
    await http.Response.WriteAsync("Unauthorized");
    return;
}
```

## Troubleshooting

**Erro: Self-host não inicia**
- Verifique porta 5002 não está em uso: `netstat -ano | findstr :5002`
- Resete IIS: `iisreset`

**Erro: Certificado SSL inválido**
- Confirme `Upstream:IgnoreInvalidCertificate = true` no Web.config
- Em produção, use certificado válido e desabilite esta flag

**Erro: Proxy forwards mas response está vazia**
- Verifique headers `Content-Type` da upstream
- Valide querystring (ex: Filter precisa ser inteiro numérico)

## Próximos Passos

- [ ] Implementar logging (Serilog/NLog)
- [ ] Adicionar circuit-breaker para timeout de upstream
- [ ] Implementar cache de respostas (Redis)
- [ ] Adicionar rate limiting
- [ ] Integrar autenticação (OAuth/JWT)
- [ ] Configurar HTTPS com wildcard cert

---

**Data:** 2026-02-07  
**Status:** Ready for testing / Pronto para testes
