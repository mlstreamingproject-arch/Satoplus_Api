# Endpoints API - Request e Response

## 1. POST /configurations/createconfiguration

### Request
```json
{
  "id": 0,
  "name": "string",
  "description": "string",
  "value": "string",
  "createdOn": "2026-08-17T00:00:00Z",
  "createdBy": 0,
  "updateOn": "2026-08-17T00:00:00Z",
  "updateBy": 0
}
```

### Response (200 OK)
```json
{
  "id": 0,
  "name": "string",
  "description": "string",
  "value": "string",
  "createdOn": "2026-08-17T00:00:00Z",
  "createdBy": 0,
  "updateOn": "2026-08-17T00:00:00Z",
  "updateBy": 0
}
```

---

## 2. POST /emailcontent/createemailcontent

### Request
```json
{
  "id": 0,
  "name": "string",
  "tittle": "string",
  "greetings": "string",
  "mainText": "string",
  "secondaryText": "string",
  "auxiliarText": "string",
  "buttonText": "string",
  "link": "string",
  "updateBy": 0,
  "updateOn": "2026-08-17T00:00:00Z"
}
```

### Response (200 OK)
```json
{
  "id": 0,
  "name": "string",
  "tittle": "string",
  "greetings": "string",
  "mainText": "string",
  "secondaryText": "string",
  "auxiliarText": "string",
  "buttonText": "string",
  "link": "string",
  "updateBy": 0,
  "updateOn": "2026-08-17T00:00:00Z"
}
```

---

## 3. POST /plataformtypes/createplataformtype

### Request
```json
{
  "id": "string",
  "label": "string",
  "order": 0,
  "isActive": true
}
```

### Response (200 OK)
```json
{
  "id": "string",
  "label": "string",
  "order": 0,
  "isActive": true
}
```

---

## 4. POST /positions/createposition

### Request
```json
{
  "id": 0,
  "name": "string",
  "description": "string",
  "createdOn": "2026-08-17T00:00:00Z",
  "createdBy": 0,
  "updatedOn": "2026-08-17T00:00:00Z",
  "updatedBy": 0,
  "isActive": true
}
```

### Response (200 OK)
```json
{
  "id": 0,
  "name": "string",
  "description": "string",
  "createdOn": "2026-08-17T00:00:00Z",
  "createdBy": 0,
  "updatedOn": "2026-08-17T00:00:00Z",
  "updatedBy": 0,
  "isActive": true
}
```

---

## 5. POST /roles/createrole

### Request
```json
{
  "id": 0,
  "name": "string",
  "persistent": true,
  "ss_Key": "string",
  "espace_Id": 0,
  "is_Active": true,
  "description": "string"
}
```

### Response (200 OK)
```json
{
  "id": 0,
  "name": "string",
  "persistent": true,
  "ss_Key": "string",
  "espace_Id": 0,
  "is_Active": true,
  "description": "string"
}
```

---

## 6. POST /useraccess/createuseraccess

### Request
```json
{
  "id": 0,
  "userId": 0,
  "userPerfilId": 0,
  "plataformTypeId": "string",
  "ip": "string",
  "createdOn": "2026-08-17T00:00:00Z"
}
```

### Response (200 OK)
```json
{
  "id": 0,
  "userId": 0,
  "userPerfilId": 0,
  "plataformTypeId": "string",
  "ip": "string",
  "createdOn": "2026-08-17T00:00:00Z"
}
```

---

## 7. POST /useravatar/createuseravatar

### Request
```json
{
  "id": 0,
  "name": "string",
  "binaryData": "base64_encoded_bytes",
  "isActive": true,
  "description": "string",
  "createdOn": "2026-08-17T00:00:00Z",
  "createdBy": 0,
  "updatedOn": "2026-08-17T00:00:00Z",
  "updatedBy": 0
}
```

### Response (200 OK)
```json
{
  "id": 0,
  "name": "string",
  "binaryData": "base64_encoded_bytes",
  "isActive": true,
  "description": "string",
  "createdOn": "2026-08-17T00:00:00Z",
  "createdBy": 0,
  "updatedOn": "2026-08-17T00:00:00Z",
  "updatedBy": 0
}
```

---

## 8. POST /userdevice/createuserdevice

### Request
```json
{
  "id": 0,
  "version": "string",
  "uuid": "string",
  "serial": "string",
  "platform": "string",
  "model": "string",
  "manufacturer": "string",
  "isVirtual": true,
  "getCordova": "string",
  "deviceType": "string",
  "userId": 0,
  "userInitialRegistrationToken": "string"
}
```

### Response (200 OK)
```json
{
  "id": 0,
  "version": "string",
  "uuid": "string",
  "serial": "string",
  "platform": "string",
  "model": "string",
  "manufacturer": "string",
  "isVirtual": true,
  "getCordova": "string",
  "deviceType": "string",
  "userId": 0,
  "userInitialRegistrationToken": "string"
}
```

---

## 9. POST /userinfo/createuserinfo

### Request
```json
{
  "id": 0,
  "biography": "string",
  "createdOn": "2026-08-17T00:00:00Z",
  "createdBy": 0,
  "updatedOn": "2026-08-17T00:00:00Z",
  "updatedBy": 0,
  "isStatusEmail": true,
  "hasStreamingAccount": true,
  "isCollaborator": true,
  "birthDate": "2026-08-17T00:00:00Z",
  "country": "string",
  "countryCode": "string",
  "address": "string"
}
```

### Response (200 OK)
```json
{
  "id": 0,
  "biography": "string",
  "createdOn": "2026-08-17T00:00:00Z",
  "createdBy": 0,
  "updatedOn": "2026-08-17T00:00:00Z",
  "updatedBy": 0,
  "isStatusEmail": true,
  "hasStreamingAccount": true,
  "isCollaborator": true,
  "birthDate": "2026-08-17T00:00:00Z",
  "country": "string",
  "countryCode": "string",
  "address": "string"
}
```

---

## 10. POST /userinitialregistration/createuserinitialregistration

### Request
```json
{
  "id": 0,
  "status": true,
  "email": "string",
  "plataformTypeId": "string",
  "ip": "string",
  "token": "string",
  "createdOn": "2026-08-17T00:00:00Z",
  "updateOn": "2026-08-17T00:00:00Z",
  "regionName": "string",
  "city": "string",
  "country": "string",
  "v_OS": "string",
  "v_Browser": "string",
  "deeplink": "string",
  "password": "string"
}
```

### Response (200 OK)
```json
{
  "id": 0,
  "status": true,
  "email": "string",
  "plataformTypeId": "string",
  "ip": "string",
  "token": "string",
  "createdOn": "2026-08-17T00:00:00Z",
  "updateOn": "2026-08-17T00:00:00Z",
  "regionName": "string",
  "city": "string",
  "country": "string",
  "v_OS": "string",
  "v_Browser": "string",
  "deeplink": "string",
  "password": "string"
}
```

---

## 11. POST /userpasswordrecovery/createuserpasswordrecovery

### Request
```json
{
  "id": 0,
  "userId": 0,
  "createdOn": "2026-08-17T00:00:00Z",
  "isValid": true
}
```

### Response (200 OK)
```json
{
  "id": 0,
  "userId": 0,
  "createdOn": "2026-08-17T00:00:00Z",
  "isValid": true
}
```

---

## 12. POST /userperfil/createuserperfil

### Request
```json
{
  "id": 0,
  "userId": 0,
  "isActive": true,
  "name": "string",
  "userAvatarId": 0,
  "isChild": false,
  "isMain": false,
  "createdOn": "2026-08-17T00:00:00Z",
  "deletedOn": "2026-08-17T00:00:00Z"
}
```

### Response (200 OK)
```json
{
  "id": 0,
  "userId": 0,
  "isActive": true,
  "name": "string",
  "userAvatarId": 0,
  "isChild": false,
  "isMain": false,
  "createdOn": "2026-08-17T00:00:00Z",
  "deletedOn": "2026-08-17T00:00:00Z"
}
```

---

## 13. POST /userpicture/createuserpicture

### Request
```json
{
  "id": 0,
  "binaryData": "base64_encoded_bytes",
  "name": "string"
}
```

### Response (200 OK)
```json
{
  "id": 0,
  "binaryData": "base64_encoded_bytes",
  "name": "string"
}
```

---

## 14. POST /userposition/createuserposition

### Request
```json
{
  "id": 0,
  "userId": 0,
  "positionId": 0,
  "createdOn": "2026-08-17T00:00:00Z",
  "createdBy": 0,
  "updatedOn": "2026-08-17T00:00:00Z",
  "updatedBy": 0
}
```

### Response (200 OK)
```json
{
  "id": 0,
  "userId": 0,
  "positionId": 0,
  "createdOn": "2026-08-17T00:00:00Z",
  "createdBy": 0,
  "updatedOn": "2026-08-17T00:00:00Z",
  "updatedBy": 0
}
```

---

## 15. POST /userroles/createuserrole

### Request
```json
{
  "id": 0,
  "user_Id": 0,
  "role_Id": 0
}
```

### Response (200 OK)
```json
{
  "id": 0,
  "user_Id": 0,
  "role_Id": 0
}
```

---

## 16. POST /userstatus/createuserstatus

### Request
```json
{
  "id": 0,
  "isOnLine": true,
  "updateOn": "2026-08-17T00:00:00Z"
}
```

### Response (200 OK)
```json
{
  "id": 0,
  "isOnLine": true,
  "updateOn": "2026-08-17T00:00:00Z"
}
```

---

## 17. POST /users/createuser

### Request
```json
{
  "id": 0,
  "name": "string",
  "username": "string",
  "password": "string",
  "email": "string",
  "mobilePhone": "string",
  "external_Id": "string",
  "creation_Date": "2026-08-17T00:00:00Z",
  "last_Login": "2026-08-17T00:00:00Z",
  "is_Active": true
}
```

### Response (200 OK)
```json
{
  "id": 0,
  "name": "string",
  "username": "string",
  "password": "string",
  "email": "string",
  "mobilePhone": "string",
  "external_Id": "string",
  "creation_Date": "2026-08-17T00:00:00Z",
  "last_Login": "2026-08-17T00:00:00Z",
  "is_Active": true
}
```

---

## Notas Importantes

- **TABELA RÉPLICA OUTSYSTEM**: Estas tabelas são réplicas do OutSystems; por isso, o request deve enviar todos os campos da tabela, sem exceção, para manter a consistência da réplica
- **NENHUM CAMPO GERENCIADO PELO BANCO**: Campos de auditoria, timestamps, status e chaves devem ser enviados explicitamente pelo cliente
- **INSERT EXPLÍCITO**: O banco grava exatamente os valores que chegam no payload; não há preenchimento automático de colunas em massa para estes endpoints
- **BinaryData**: Quando usado em criar avatares ou pictures, deve ser enviado como string em base64
- **Datas**: Formato ISO 8601 (2026-08-17T00:00:00Z)
- **Booleanos**: true/false
- **IDs numéricos**: Podem ser auto-gerados (0) ou específicos
- **Campos nullable**: Podem ser enviados como null quando o valor não existir
- **Cliente Responsável**: O cliente é responsável por enviar todos os dados da tabela, incluindo informações de auditoria, timestamps, status e campos de controle
- **Validação de contrato**: Qualquer campo da tabela que não estiver no request pode gerar inconsistência na réplica OutSystems
