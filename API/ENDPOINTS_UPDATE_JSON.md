# Endpoints de Update - JSON Completo

As tabelas `*_backup` sao replicas do OutSystems. O cliente deve enviar todos os campos da tabela, exceto `Id`. A API usa o `Id` informado na rota apenas para localizar o registro.

Campos com valor `null` sao persistidos como `NULL`. Nenhum campo e preenchido automaticamente pela API. Datas usam formato ISO 8601.

## 1. Configuration

`PUT /configurations/updateconfiguration/{id}`

```json
{
  "name": "Nome da configuracao",
  "description": "Descricao da configuracao",
  "value": "Valor da configuracao",
  "createdOn": "2026-08-18T00:00:00Z",
  "createdBy": 1,
  "updateOn": "2026-08-19T00:00:00Z",
  "updateBy": 1
}
```

## 2. EmailContent

`PUT /emailcontent/updateemailcontent/{id}`

```json
{
  "name": "Email de boas-vindas",
  "tittle": "Titulo do email",
  "greetings": "Ola, usuario",
  "mainText": "Texto principal",
  "secondaryText": "Texto secundario",
  "auxiliarText": "Texto auxiliar",
  "buttonText": "Acessar",
  "link": "https://example.com",
  "updateBy": 1,
  "updateOn": "2026-08-19T00:00:00Z"
}
```

## 3. PlataformType

`PUT /plataformtypes/updateplataformtype/{id}`

```json
{
  "label": "Web",
  "order": 1,
  "isActive": true
}
```

## 4. Position

`PUT /positions/updateposition/{id}`

```json
{
  "name": "Gerente",
  "description": "Descricao da posicao",
  "createdOn": "2026-08-18T00:00:00Z",
  "createdBy": 1,
  "updatedOn": "2026-08-19T00:00:00Z",
  "updatedBy": 1,
  "isActive": true
}
```

## 5. Role

`PUT /roles/updaterole/{id}`

```json
{
  "name": "Administrador",
  "persistent": true,
  "ss_Key": "role-admin",
  "espace_Id": 1,
  "isActive": true,
  "description": "Permissoes administrativas"
}
```

## 6. UserRole

`PUT /userroles/updateuserrole/{id}`

```json
{
  "user_Id": 1,
  "role_Id": 1
}
```

## 7. UserAccess

`PUT /useraccess/updateuseraccess/{id}`

```json
{
  "userId": 1,
  "userPerfilId": 1,
  "plataformTypeId": "web",
  "ip": "192.168.0.10",
  "createdOn": "2026-08-18T00:00:00Z"
}
```

## 8. UserAvatar

`PUT /useravatar/updateuseravatar/{id}`

```json
{
  "name": "Avatar principal",
  "binaryData": "AAECAwQ=",
  "isActive": true,
  "description": "Imagem do avatar",
  "createdOn": "2026-08-18T00:00:00Z",
  "createdBy": 1,
  "updatedOn": "2026-08-19T00:00:00Z",
  "updatedBy": 1
}
```

`binaryData` deve ser enviado como uma string Base64. Para gravar `NULL`, envie `null`.

## 9. UserDevice

`PUT /userdevice/updateuserdevice/{id}`

```json
{
  "version": "1.0.0",
  "uuid": "device-uuid",
  "serial": "serial-number",
  "platform": "Android",
  "model": "Model X",
  "manufacturer": "Manufacturer",
  "isVirtual": false,
  "getCordova": "cordova-info",
  "deviceType": "mobile",
  "userId": 1,
  "userInitialRegistrationToken": "registration-token"
}
```

## 10. User

`PUT /users/updateuser/{id}`

```json
{
  "name": "Nome do usuario",
  "username": "usuario",
  "password": "senha-ou-hash",
  "email": "usuario@example.com",
  "mobilePhone": "+55 11 99999-9999",
  "external_Id": "external-001",
  "creation_Date": "2026-08-18T00:00:00Z",
  "last_Login": "2026-08-19T00:00:00Z",
  "isActive": true
}
```

## 11. UserInfo

`PUT /userinfo/updateuserinfo/{id}`

```json
{
  "biography": "Biografia do usuario",
  "createdOn": "2026-08-18T00:00:00Z",
  "createdBy": 1,
  "updatedOn": "2026-08-19T00:00:00Z",
  "updatedBy": 1,
  "isStatusEmail": true,
  "hasStreamingAccount": false,
  "isCollaborator": true,
  "birthDate": "1990-01-15",
  "country": "Brasil",
  "countryCode": "BR",
  "address": "Endereco do usuario"
}
```

## 12. UserInitialRegistration

`PUT /userinitialregistration/updateuserinitialregistration/{id}`

```json
{
  "status": true,
  "email": "usuario@example.com",
  "plataformTypeId": "web",
  "ip": "192.168.0.10",
  "token": "registration-token",
  "createdOn": "2026-08-18T00:00:00Z",
  "updateOn": "2026-08-19T00:00:00Z",
  "regionName": "Sao Paulo",
  "city": "Sao Paulo",
  "country": "Brasil",
  "v_OS": "Android 14",
  "v_Browser": "Chrome",
  "deeplink": "app://registration",
  "password": "senha-ou-hash"
}
```

## 13. UserPasswordRecovery

`PUT /userpasswordrecovery/updateuserpasswordrecovery/{id}`

```json
{
  "userId": 1,
  "createdOn": "2026-08-18T00:00:00Z",
  "isValid": true
}
```

## 14. UserPerfil

`PUT /userperfil/updateuserperfil/{id}`

```json
{
  "userId": 1,
  "isActive": true,
  "name": "Perfil principal",
  "userAvatarId": 1,
  "isChild": false,
  "isMain": true,
  "createdOn": "2026-08-18T00:00:00Z",
  "deletedOn": null
}
```

## 15. UserPicture

`PUT /userpicture/updateuserpicture/{id}`

```json
{
  "binaryData": "AAECAwQ=",
  "name": "foto-perfil.jpg"
}
```

`binaryData` deve ser enviado como uma string Base64. Para gravar `NULL`, envie `null`.

## 16. UserPosition

`PUT /userposition/updateuserposition/{id}`

```json
{
  "userId": 1,
  "positionId": 1,
  "createdOn": "2026-08-18T00:00:00Z",
  "createdBy": 1,
  "updatedOn": "2026-08-19T00:00:00Z",
  "updatedBy": 1
}
```

## 17. UserStatus

`PUT /userstatus/updateuserstatus/{id}`

```json
{
  "isOnLine": true,
  "updateOn": "2026-08-19T00:00:00Z"
}
```

## Regras de persistencia

- Todos os campos acima sao enviados no corpo do `PUT`; somente `Id` fica fora do JSON e vai na rota.
- A API atualiza todas as colunas da tabela, exceto `Id`.
- O valor `null` enviado pelo cliente vira `NULL` no MySQL.
- A API nao usa valores anteriores para completar o payload.
- A API nao gera timestamps, usuarios de auditoria, booleanos, zeros ou strings vazias.
- Os nomes JSON seguem as propriedades dos DTOs e sao aceitos sem diferenciar maiusculas e minusculas.
