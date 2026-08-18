# UPDATE Endpoints API - Request e Response Schemas

## 1. PUT /configurations/updateconfiguration/{id}

### Request
```json
{
  "name": "string",
  "description": "string",
  "value": "string",
  "updateOn": "2026-08-18T00:00:00Z",
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
  "createdOn": "2026-08-18T00:00:00Z",
  "createdBy": 0,
  "updateOn": "2026-08-18T00:00:00Z",
  "updateBy": 0
}
```

---

## 2. PUT /emailcontent/updateemailcontent/{id}

### Request
```json
{
  "name": "string",
  "tittle": "string",
  "greetings": "string",
  "mainText": "string",
  "secondaryText": "string",
  "auxiliarText": "string",
  "buttonText": "string",
  "link": "string",
  "updateOn": "2026-08-18T00:00:00Z",
  "updateBy": 0
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
  "updateOn": "2026-08-18T00:00:00Z"
}
```

---

## 3. PUT /plataformtypes/updateplataformtype/{id}

### Request
```json
{
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

## 4. PUT /positions/updateposition/{id}

### Request
```json
{
  "name": "string",
  "description": "string",
  "updatedOn": "2026-08-18T00:00:00Z",
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
  "createdOn": "2026-08-18T00:00:00Z",
  "createdBy": 0,
  "updatedOn": "2026-08-18T00:00:00Z",
  "updatedBy": 0,
  "isActive": true
}
```

---

## 5. PUT /roles/updaterole/{id}

### Request
```json
{
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

## 6. PUT /useraccess/updateuseraccess/{id}

### Request
```json
{
  "userId": 0,
  "userPerfilId": 0,
  "plataformTypeId": "string",
  "ip": "string"
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
  "createdOn": "2026-08-18T00:00:00Z"
}
```

---

## 7. PUT /useravatar/updateuseravatar/{id}

### Request
```json
{
  "name": "string",
  "binaryData": "base64_encoded_bytes",
  "isActive": true,
  "description": "string",
  "updatedOn": "2026-08-18T00:00:00Z",
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
  "createdOn": "2026-08-18T00:00:00Z",
  "createdBy": 0,
  "updatedOn": "2026-08-18T00:00:00Z",
  "updatedBy": 0
}
```

---

## 8. PUT /userdevice/updateuserdevice/{id}

### Request
```json
{
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

## 9. PUT /userinfo/updateuserinfo/{id}

### Request
```json
{
  "biography": "string",
  "updatedOn": "2026-08-18T00:00:00Z",
  "updatedBy": 0,
  "isStatusEmail": true,
  "hasStreamingAccount": true,
  "isCollaborator": true,
  "birthDate": "2026-08-18T00:00:00Z",
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
  "createdOn": "2026-08-18T00:00:00Z",
  "createdBy": 0,
  "updatedOn": "2026-08-18T00:00:00Z",
  "updatedBy": 0,
  "isStatusEmail": true,
  "hasStreamingAccount": true,
  "isCollaborator": true,
  "birthDate": "2026-08-18T00:00:00Z",
  "country": "string",
  "countryCode": "string",
  "address": "string"
}
```

---

## 10. PUT /userinitialregistration/updateuserinitialregistration/{id}

### Request
```json
{
  "status": true,
  "email": "string",
  "plataformTypeId": "string",
  "ip": "string",
  "token": "string",
  "updateOn": "2026-08-18T00:00:00Z",
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
  "createdOn": "2026-08-18T00:00:00Z",
  "updateOn": "2026-08-18T00:00:00Z",
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

## 11. PUT /userpasswordrecovery/updateuserpasswordrecovery/{id}

### Request
```json
{
  "userId": 0,
  "isValid": true
}
```

### Response (200 OK)
```json
{
  "id": 0,
  "userId": 0,
  "createdOn": "2026-08-18T00:00:00Z",
  "isValid": true
}
```

---

## 12. PUT /userperfil/updateuserperfil/{id}

### Request
```json
{
  "userId": 0,
  "isActive": true,
  "name": "string",
  "userAvatarId": 0,
  "isChild": false,
  "isMain": false,
  "deletedOn": "2026-08-18T00:00:00Z"
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
  "createdOn": "2026-08-18T00:00:00Z",
  "deletedOn": "2026-08-18T00:00:00Z"
}
```

---

## 13. PUT /userpicture/updateuserpicture/{id}

### Request
```json
{
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

## 14. PUT /userposition/updateuserposition/{id}

### Request
```json
{
  "userId": 0,
  "positionId": 0,
  "updatedOn": "2026-08-18T00:00:00Z",
  "updatedBy": 0
}
```

### Response (200 OK)
```json
{
  "id": 0,
  "userId": 0,
  "positionId": 0,
  "createdOn": "2026-08-18T00:00:00Z",
  "createdBy": 0,
  "updatedOn": "2026-08-18T00:00:00Z",
  "updatedBy": 0
}
```

---

## 15. PUT /userroles/updateuserrole/{id}

### Request
```json
{
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

## 16. PUT /userstatus/updateuserstatus/{id}

### Request
```json
{
  "isOnLine": true,
  "updateOn": "2026-08-18T00:00:00Z"
}
```

### Response (200 OK)
```json
{
  "id": 0,
  "isOnLine": true,
  "updateOn": "2026-08-18T00:00:00Z"
}
```

---

## 17. PUT /users/updateuser/{id}

### Request
```json
{
  "name": "string",
  "username": "string",
  "password": "string",
  "email": "string",
  "mobilePhone": "string",
  "external_Id": "string",
  "last_Login": "2026-08-18T00:00:00Z",
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
  "creation_Date": "2026-08-18T00:00:00Z",
  "last_Login": "2026-08-18T00:00:00Z",
  "is_Active": true
}
```

---

## 📋 Notas Importantes

- **TABELA RÉPLICA OUTSYSTEM**: Estas tabelas são réplicas do OutSystems
- **TIMESTAMPS**: Formato ISO 8601 (2026-08-18T00:00:00Z)
- **CAMPOS OPCIONAIS**: Indicados por `?` nos DTOs; podem ser enviados como `null`
- **DEFAULT VALUES**: Se timestamp não fornecido, sistema usa `DateTime.Now`
- **SOFT DELETE**: Campo `deletedOn` é usado para marcar registros como deletados
- **AUDITORIA**: `updatedBy`/`updateBy` e `updatedOn`/`updateOn` devem ser fornecidos pelo cliente para sincronização correta

---

## ⚠️ Prioridade de Correção

### P1 - Críticos (8 endpoints - adicionar field de timestamp)
- Configuration *(+ updateOn)*
- EmailContent *(+ updateOn)*
- Position *(+ updatedOn)*
- UserAvatar *(+ updatedOn)*
- UserInfo *(+ updatedOn)*
- UserInitialRegistration *(+ updateOn)*
- UserPosition *(+ updatedOn)*
- UserStatus *(+ updateOn)*

### P2 - Altos (2 endpoints - adicionar fields específicos)
- UserPerfil *(+ deletedOn)*
- User *(+ last_Login)*

### P3 - Baixos (7 endpoints - já completos)
- PlataformType
- Role
- UserAccess
- UserDevice
- UserPasswordRecovery
- UserPicture
- UserRole
