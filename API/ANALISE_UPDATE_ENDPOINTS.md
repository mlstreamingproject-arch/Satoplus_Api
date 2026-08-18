# Análise de UPDATE Endpoints - Replicação OutSystems

## 📊 Resumo Executivo

Este documento analisa os 17 endpoints UPDATE para identificar inconsistências no contrato de API em relação ao padrão estabelecido nos CREATE endpoints.

### ⚠️ Problema Identificado

Os UPDATE DTOs aceitam **MENOS campos** que os CREATE DTOs, violando o princípio de replicação OutSystems onde o cliente deve controlar todos os campos da tabela, incluindo dados de auditoria.

**Exemplo:**
- `CreateEmailContentDto` → 11 campos (incluindo UpdateOn)
- `UpdateEmailContentDto` → 9 campos (falta UpdateOn)

---

## 📋 Análise Comparativa por Endpoint

### 1. PUT /configurations/updateconfiguration/{id}

**DTO Atual:**
```csharp
public class UpdateConfigurationDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Value { get; set; }
    public int? UpdateBy { get; set; }
}
```

**Campos Disponíveis:** 4
**Campos da Tabela:** 8 (Id, Name, Description, Value, CreatedOn, CreatedBy, UpdateOn, UpdateBy)
**Status:** ⚠️ **INCOMPLETO** - Falta `UpdateOn`

**Recomendação:** Adicionar `DateTime? UpdateOn` ao UpdateConfigurationDto

**Controller atual** (ConfigurationController.cs, linha ~107):
```csharp
configuration.UpdateOn = DateTime.Now; // ❌ HARDCODED
configuration.UpdateBy = dto.UpdateBy ?? configuration.UpdateBy;
```

---

### 2. PUT /emailcontent/updateemailcontent/{id}

**DTO Atual:**
```csharp
public class UpdateEmailContentDto
{
    public string Name { get; set; }
    public string Tittle { get; set; }
    public string Greetings { get; set; }
    public string MainText { get; set; }
    public string SecondaryText { get; set; }
    public string AuxiliarText { get; set; }
    public string ButtonText { get; set; }
    public string Link { get; set; }
    public int? UpdateBy { get; set; }
}
```

**Campos Disponíveis:** 9
**Campos da Tabela:** 11 (todas as acima + UpdateOn, Id)
**Status:** ⚠️ **INCOMPLETO** - Falta `UpdateOn`

**Recomendação:** Adicionar `DateTime? UpdateOn` ao UpdateEmailContentDto

---

### 3. PUT /plataformtypes/updateplataformtype/{id}

**DTO Atual:**
```csharp
public class UpdatePlataformTypeDto
{
    public string Label { get; set; }
    public int? Order { get; set; }
    public bool? IsActive { get; set; }
}
```

**Campos Disponíveis:** 3
**Campos da Tabela:** 4 (Id, Label, Order, IsActive)
**Status:** ✅ **COMPLETO** - Sem campos de auditoria

---

### 4. PUT /positions/updateposition/{id}

**DTO Atual:**
```csharp
public class UpdatePositionDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int? UpdatedBy { get; set; }
    public bool? IsActive { get; set; }
}
```

**Campos Disponíveis:** 4
**Campos da Tabela:** 8 (Id, Name, Description, CreatedOn, CreatedBy, UpdatedOn, UpdatedBy, IsActive)
**Status:** ⚠️ **INCOMPLETO** - Falta `UpdatedOn`

**Recomendação:** Adicionar `DateTime? UpdatedOn` ao UpdatePositionDto

**Controller atual** (PositionController.cs, linha ~76):
```csharp
position.UpdatedOn = DateTime.Now; // ❌ HARDCODED
position.UpdatedBy = dto.UpdatedBy ?? position.UpdatedBy;
```

---

### 5. PUT /roles/updaterole/{id}

**DTO Atual:**
```csharp
public class UpdateRoleDto
{
    public string Name { get; set; }
    public bool? Persistent { get; set; }
    public string SS_Key { get; set; }
    public int? Espace_Id { get; set; }
    public bool? IsActive { get; set; }
    public string Description { get; set; }
}
```

**Campos Disponíveis:** 6
**Campos da Tabela:** 7 (Id, Name, Persistent, SS_Key, Espace_Id, IsActive, Description)
**Status:** ✅ **COMPLETO** - Sem campos de auditoria

---

### 6. PUT /useraccess/updateuseraccess/{id}

**DTO Atual:**
```csharp
public class UpdateUserAccessDto
{
    public int? UserId { get; set; }
    public long? UserPerfilId { get; set; }
    public string PlataformTypeId { get; set; }
    public string IP { get; set; }
}
```

**Campos Disponíveis:** 4
**Campos da Tabela:** 6 (Id, UserId, UserPerfilId, PlataformTypeId, IP, CreatedOn)
**Status:** ⚠️ **INCOMPLETO** - CreatedOn é read-only, mas pode ser problema se precisa sincronizar

---

### 7. PUT /useravatar/updateuseravatar/{id}

**DTO Atual:**
```csharp
public class UpdateUserAvatarDto
{
    public string Name { get; set; }
    public byte[] BinaryData { get; set; }
    public bool? IsActive { get; set; }
    public string Description { get; set; }
    public int? UpdatedBy { get; set; }
}
```

**Campos Disponíveis:** 5
**Campos da Tabela:** 9 (Id, Name, BinaryData, IsActive, Description, CreatedOn, CreatedBy, UpdatedOn, UpdatedBy)
**Status:** ⚠️ **INCOMPLETO** - Falta `UpdatedOn`

**Recomendação:** Adicionar `DateTime? UpdatedOn` ao UpdateUserAvatarDto

**Controller atual** (UserAvatarController.cs, linha ~89):
```csharp
avatar.UpdatedOn = DateTime.Now; // ❌ HARDCODED
avatar.UpdatedBy = dto.UpdatedBy ?? avatar.UpdatedBy;
```

---

### 8. PUT /userdevice/updateuserdevice/{id}

**DTO Atual:**
```csharp
public class UpdateUserDeviceDto
{
    public string Version { get; set; }
    public string UUID { get; set; }
    public string Serial { get; set; }
    public string Platform { get; set; }
    public string Model { get; set; }
    public string Manufacturer { get; set; }
    public bool? IsVirtual { get; set; }
    public string GetCordova { get; set; }
    public string DeviceType { get; set; }
    public int? UserId { get; set; }
    public string UserInitialRegistrationToken { get; set; }
}
```

**Campos Disponíveis:** 11
**Campos da Tabela:** 12 (todos os acima + Id)
**Status:** ✅ **COMPLETO** - Sem campos de auditoria

---

### 9. PUT /userinfo/updateuserinfo/{id}

**DTO Atual:**
```csharp
public class UpdateUserInfoDto
{
    public string Biography { get; set; }
    public int? UpdatedBy { get; set; }
    public bool? IsStatusEmail { get; set; }
    public bool? HasStreamingAccount { get; set; }
    public bool? IsCollaborator { get; set; }
    public DateTime? BirthDate { get; set; }
    public string Country { get; set; }
    public string CountryCode { get; set; }
    public string Address { get; set; }
}
```

**Campos Disponíveis:** 9
**Campos da Tabela:** 13 (Id, Biography, CreatedOn, CreatedBy, UpdatedOn, UpdatedBy, IsStatusEmail, HasStreamingAccount, IsCollaborator, BirthDate, Country, CountryCode, Address)
**Status:** ⚠️ **INCOMPLETO** - Falta `UpdatedOn`

**Recomendação:** Adicionar `DateTime? UpdatedOn` ao UpdateUserInfoDto

**Controller atual** (UserInfoController.cs, linha ~99):
```csharp
info.UpdatedOn = DateTime.Now; // ❌ HARDCODED
info.UpdatedBy = dto.UpdatedBy ?? info.UpdatedBy;
```

---

### 10. PUT /userinitialregistration/updateuserinitialregistration/{id}

**DTO Atual:**
```csharp
public class UpdateUserInitialRegistrationDto
{
    public bool? Status { get; set; }
    public string Email { get; set; }
    public string PlataformTypeId { get; set; }
    public string IP { get; set; }
    public string Token { get; set; }
    public string RegionName { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string V_OS { get; set; }
    public string V_Browser { get; set; }
    public string Deeplink { get; set; }
    public string Password { get; set; }
}
```

**Campos Disponíveis:** 12
**Campos da Tabela:** 15 (todos os acima + Id, CreatedOn, UpdateOn)
**Status:** ⚠️ **INCOMPLETO** - Falta `UpdateOn`

**Recomendação:** Adicionar `DateTime? UpdateOn` ao UpdateUserInitialRegistrationDto

**Controller atual** (UserInitialRegistrationController.cs, linha ~101):
```csharp
registration.UpdateOn = DateTime.Now; // ❌ HARDCODED
```

---

### 11. PUT /userpasswordrecovery/updateuserpasswordrecovery/{id}

**DTO Atual:**
```csharp
public class UpdateUserPasswordRecoveryDto
{
    public int? UserId { get; set; }
    public bool? IsValid { get; set; }
}
```

**Campos Disponíveis:** 2
**Campos da Tabela:** 4 (Id, UserId, CreatedOn, IsValid)
**Status:** ✅ **COMPLETO** - Sem campos de auditoria mutáveis

---

### 12. PUT /userperfil/updateuserperfil/{id}

**DTO Atual:**
```csharp
public class UpdateUserPerfilDto
{
    public int? UserId { get; set; }
    public bool? IsActive { get; set; }
    public string Name { get; set; }
    public long? UserAvatarId { get; set; }
    public bool? IsChild { get; set; }
    public bool? IsMain { get; set; }
}
```

**Campos Disponíveis:** 6
**Campos da Tabela:** 9 (Id, UserId, IsActive, Name, UserAvatarId, IsChild, IsMain, CreatedOn, DeletedOn)
**Status:** ⚠️ **INCOMPLETO** - Falta `DeletedOn` (soft-delete)

**Recomendação:** Adicionar `DateTime? DeletedOn` ao UpdateUserPerfilDto

---

### 13. PUT /userpicture/updateuserpicture/{id}

**DTO Atual:**
```csharp
public class UpdateUserPictureDto
{
    public byte[] BinaryData { get; set; }
    public string Name { get; set; }
}
```

**Campos Disponíveis:** 2
**Campos da Tabela:** 3 (Id, BinaryData, Name)
**Status:** ✅ **COMPLETO** - Sem campos de auditoria

---

### 14. PUT /userposition/updateuserposition/{id}

**DTO Atual:**
```csharp
public class UpdateUserPositionDto
{
    public int? UserId { get; set; }
    public long? PositionId { get; set; }
    public int? UpdatedBy { get; set; }
}
```

**Campos Disponíveis:** 3
**Campos da Tabela:** 7 (Id, UserId, PositionId, CreatedOn, CreatedBy, UpdatedOn, UpdatedBy)
**Status:** ⚠️ **INCOMPLETO** - Falta `UpdatedOn`

**Recomendação:** Adicionar `DateTime? UpdatedOn` ao UpdateUserPositionDto

**Controller atual** (UserPositionController.cs, linha ~92):
```csharp
userPosition.UpdatedOn = DateTime.Now; // ❌ HARDCODED
userPosition.UpdatedBy = dto.UpdatedBy ?? userPosition.UpdatedBy;
```

---

### 15. PUT /userroles/updateuserrole/{id}

**DTO Atual:**
```csharp
public class UpdateUserRoleDto
{
    public int? User_Id { get; set; }
    public int? Role_Id { get; set; }
}
```

**Campos Disponíveis:** 2
**Campos da Tabela:** 3 (Id, User_Id, Role_Id)
**Status:** ✅ **COMPLETO** - Sem campos de auditoria

---

### 16. PUT /userstatus/updateuserstatus/{id}

**DTO Atual:**
```csharp
public class UpdateUserStatusDto
{
    public bool? IsOnLine { get; set; }
}
```

**Campos Disponíveis:** 1
**Campos da Tabela:** 3 (Id, IsOnLine, UpdateOn)
**Status:** ⚠️ **INCOMPLETO** - Falta `UpdateOn`

**Recomendação:** Adicionar `DateTime? UpdateOn` ao UpdateUserStatusDto

**Controller atual** (UserStatusController.cs):
```csharp
status.UpdateOn = DateTime.Now; // ❌ Provavelmente hardcoded
```

---

### 17. PUT /users/updateuser/{id}

**DTO Atual:**
```csharp
public class UpdateUserDto
{
    public string Name { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string MobilePhone { get; set; }
    public string External_Id { get; set; }
    public bool? IsActive { get; set; }
}
```

**Campos Disponíveis:** 7
**Campos da Tabela:** 10 (Id, Name, Username, Password, Email, MobilePhone, External_Id, Creation_Date, Last_Login, IsActive)
**Status:** ⚠️ **INCOMPLETO** - Falta `Last_Login` (pode ser atualizado pelo cliente)

**Recomendação:** Adicionar `DateTime? Last_Login` ao UpdateUserDto

---

## 📊 Resumo de Achados

| # | Endpoint | Campos | Status | Ação Necessária |
|---|----------|--------|--------|-----------------|
| 1 | Configuration | 4/8 | ⚠️ Incompleto | + UpdateOn |
| 2 | EmailContent | 9/11 | ⚠️ Incompleto | + UpdateOn |
| 3 | PlataformType | 3/4 | ✅ Completo | - |
| 4 | Position | 4/8 | ⚠️ Incompleto | + UpdatedOn |
| 5 | Role | 6/7 | ✅ Completo | - |
| 6 | UserAccess | 4/6 | ✅ Aceitável | - |
| 7 | UserAvatar | 5/9 | ⚠️ Incompleto | + UpdatedOn |
| 8 | UserDevice | 11/12 | ✅ Completo | - |
| 9 | UserInfo | 9/13 | ⚠️ Incompleto | + UpdatedOn |
| 10 | UserInitialRegistration | 12/15 | ⚠️ Incompleto | + UpdateOn |
| 11 | UserPasswordRecovery | 2/4 | ✅ Completo | - |
| 12 | UserPerfil | 6/9 | ⚠️ Incompleto | + DeletedOn |
| 13 | UserPicture | 2/3 | ✅ Completo | - |
| 14 | UserPosition | 3/7 | ⚠️ Incompleto | + UpdatedOn |
| 15 | UserRole | 2/3 | ✅ Completo | - |
| 16 | UserStatus | 1/3 | ⚠️ Incompleto | + UpdateOn |
| 17 | User | 7/10 | ⚠️ Incompleto | + Last_Login |

**Total:** 9 incompletos, 8 completos

---

## 🎯 Recomendações Estratégicas

### Prioridade P1 - Critical (8 endpoints)
- Configuration
- EmailContent
- Position
- UserAvatar
- UserInfo
- UserInitialRegistration
- UserPosition
- UserStatus

**Ação:** Adicionar campo `UpdatedOn`/`UpdateOn` aos UpdateXxxDto e respeitar valor do payload ao invés de hardcodar DateTime.Now

### Prioridade P2 - High (2 endpoints)
- UserPerfil (adicionar DeletedOn para soft-delete)
- User (adicionar Last_Login)

### Prioridade P3 - Low (7 endpoints)
- PlataformType, Role, UserAccess, UserDevice, UserPasswordRecovery, UserPicture, UserRole

**Status:** Já possuem todos os campos atualizáveis

---

## 🔧 Padrão de Correção Recomendado

### Exemplo: UpdateConfigurationDto

**ANTES:**
```csharp
public class UpdateConfigurationDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Value { get; set; }
    public int? UpdateBy { get; set; }
}
```

**DEPOIS:**
```csharp
public class UpdateConfigurationDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Value { get; set; }
    public DateTime? UpdateOn { get; set; }  // ✨ NOVO
    public int? UpdateBy { get; set; }
}
```

### Exemplo: Controller Update Method

**ANTES:**
```csharp
public IHttpActionResult Update(long id, [FromBody] UpdateConfigurationDto dto)
{
    // ...
    configuration.UpdateOn = DateTime.Now;  // ❌ HARDCODED
    configuration.UpdateBy = dto.UpdateBy ?? configuration.UpdateBy;
    // ...
}
```

**DEPOIS:**
```csharp
public IHttpActionResult Update(long id, [FromBody] UpdateConfigurationDto dto)
{
    // ...
    configuration.UpdateOn = dto.UpdateOn ?? DateTime.Now;  // ✅ Respeita payload
    configuration.UpdateBy = dto.UpdateBy ?? configuration.UpdateBy;
    // ...
}
```

---

## 📝 JSON Schemas - UPDATE Endpoints

Veja o arquivo `ENDPOINTS_UPDATE_JSON.md` para os schemas JSON de Request/Response de todos os 17 UPDATE endpoints.
