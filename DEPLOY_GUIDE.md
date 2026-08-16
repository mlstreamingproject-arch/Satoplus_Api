# 📦 Guia de Deploy - SatoPlus API

## ✅ O que você tem pronto

A build de release foi gerada em:
```
c:\Users\Marcus Louzada\Desktop\proxy\SatoPlus\publish_release
```

Esta pasta contém tudo que precisa para publicar em produção.

---

## 📋 Arquivos/Pastas a SUBIR para o servidor

### ✅ Arquivos obrigatórios (TUDO isso):
```
MeuProxySsl.dll                              (arquivo principal da API)
MeuProxySsl.dll.config                       (configurações da aplicação)
MeuProxySsl.pdb                              (debug symbols)

BouncyCastle.Cryptography.dll
Google.Protobuf.dll
K4os.Compression.LZ4.dll
K4os.Compression.LZ4.Streams.dll
K4os.Hash.xxHash.dll
Microsoft.Bcl.AsyncInterfaces.dll
Microsoft.IdentityModel.JsonWebTokens.dll
Microsoft.IdentityModel.Logging.dll
Microsoft.IdentityModel.Tokens.dll
Microsoft.Owin.Cors.dll
Microsoft.Owin.dll
Microsoft.Owin.Host.HttpListener.dll
Microsoft.Owin.Host.SystemWeb.dll
Microsoft.Owin.Hosting.dll
Microsoft.Web.Infrastructure.dll
MySql.Data.dll
Newtonsoft.Json.dll
Owin.dll
Swashbuckle.Core.dll
System.Buffers.dll
System.Configuration.ConfigurationManager.dll
System.Diagnostics.DiagnosticSource.dll
System.IdentityModel.Tokens.Jwt.dll
System.IO.Pipelines.dll
System.Memory.dll
System.Net.Http.Formatting.dll
System.Numerics.Vectors.dll
System.Runtime.CompilerServices.Unsafe.dll
System.Threading.Tasks.Extensions.dll
System.Web.Cors.dll
System.Web.Http.dll
System.Web.Http.Owin.dll
System.Web.Http.WebHost.dll
WebActivatorEx.dll
ZstdSharp.dll
```

---

## 🔧 Configuração no servidor Windows

### Passo 1: IIS Manager
1. Abra **Internet Information Services (IIS) Manager**
2. Crie um novo **Application** ou **Virtual Directory**
3. Aponte para a pasta da release: `publish_release`

### Passo 2: Application Pool
Configurar assim:
- **.NET CLR Version**: v4.0.30319
- **Managed Pipeline Mode**: Integrated
- **Enable 32-Bit Applications**: True (se servidor for 32-bit)

### Passo 3: Site Binding
Configure o binding (protocolo e porta):
- **Type**: http ou https
- **Port**: 80 (http) ou 443 (https)
- **Host Name**: seu domínio (ex: api.satoplus.com)

---

## 🔐 Web.config - Configuração para TESTE temporário

Edite o arquivo `MeuProxySsl.dll.config` no servidor com:

### Para TESTE (Swagger aberto):
```xml
<add key="Swagger:Enabled" value="true" />
<add key="JwtSecretKey" value="KlkvtINGHP3RdAO6dUrcPrlSd8H6961GBiEUl4oLUzU=" />
<add key="JwtAccessTokenExpirationHours" value="6" />
<add key="JwtRefreshTokenExpirationDays" value="7" />
<add key="Upstream:BaseUrl" value="https://personal-b0zlpsmh.outsystemscloud.com/TokuPlus_API/rest/RESTAPITOKUPLUS/" />
<add key="Upstream:IgnoreInvalidCertificate" value="true" />
<add key="MySql:ConnectionString" value="Server=SEU_SERVIDOR_MYSQL;Port=3306;Database=SEU_BANCO;Uid=SEU_USER;Pwd=SUA_SENHA;SslMode=none;" />
```

### Para PRODUÇÃO (Swagger fechado):
```xml
<add key="Swagger:Enabled" value="false" />
<add key="JwtSecretKey" value="KlkvtINGHP3RdAO6dUrcPrlSd8H6961GBiEUl4oLUzU=" />
<add key="JwtAccessTokenExpirationHours" value="6" />
<add key="JwtRefreshTokenExpirationDays" value="7" />
<add key="Upstream:BaseUrl" value="https://personal-b0zlpsmh.outsystemscloud.com/TokuPlus_API/rest/RESTAPITOKUPLUS/" />
<add key="Upstream:IgnoreInvalidCertificate" value="true" />
<add key="MySql:ConnectionString" value="Server=SEU_SERVIDOR_MYSQL;Port=3306;Database=SEU_BANCO;Uid=SEU_USER;Pwd=SUA_SENHA;SslMode=none;" />
```

---

## ✅ URLs de acesso

### Endpoints da API:
- `https://api.satoplus.com/api/users`
- `https://api.satoplus.com/api/configurations`
- `https://api.satoplus.com/api/emailcontent`
- etc.

### Swagger (se ligado):
- `https://api.satoplus.com/swagger/ui/index`

---

## 🚨 Importante - Segurança

✅ **Todos os endpoints** (exceto alguns como `/validateTVBoxToken` e `/refreshToken`) **requerem Bearer Token JWT**

Exemplo de request:
```bash
curl -H "Authorization: Bearer SEU_TOKEN_JWT" \
  https://api.satoplus.com/api/users
```

---

## 📝 Checklist de Deploy

- [ ] Pasta `publish_release` copiada para servidor Windows
- [ ] IIS configurado com Application Pool .NET 4.0
- [ ] Binding (http/https + porta) configurado
- [ ] Web.config editado com credenciais do banco
- [ ] Swagger ligado para teste ou desligado para produção
- [ ] Testar `/api/configurations` com Bearer Token
- [ ] Validar resposta da API

---

## ❓ Em caso de erro

Se der erro 500 ou 502:
1. Verifique se **MySql está rodando** e **acessível** do servidor
2. Verifique se **JwtSecretKey está correto**
3. Verifique os **logs do IIS**: `%SystemDrive%\inetpub\logs\LogFiles`
4. Verifique **Application Pool Events**

---

## 📌 Resumo prático

1. **Copie a pasta inteira** `publish_release` para: `C:\inetpub\wwwroot\SatoPlus` (ou onde preferir)
2. **Crie um Application** no IIS apontando para essa pasta
3. **Configure o Application Pool** para .NET 4.0 Integrated
4. **Edite MeuProxySsl.dll.config** com sua conexão MySQL e chaves
5. **Acesse** `https://api.satoplus.com` (ou seu domínio)

Pronto! 🚀
