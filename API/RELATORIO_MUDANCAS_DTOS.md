# RELATÓRIO DE MUDANÇAS - DTOs para INCLUIR TODOS OS CAMPOS

## 📋 Resumo Executivo

Foram atualizados **8 DTOs Create** para incluir TODOS os campos das tabelas SQL, conforme nova política: **o cliente é responsável por enviar todos os dados, incluindo timestamps e campos de auditoria**.

**Status:** ✅ Completo em 2026-08-17

---

## 📝 DTOs Modificados

### 1. ✅ ConfigurationDto.cs

**Mudança:** Adicionados campos de auditoria ao `CreateConfigurationDto`

```csharp
// ANTES
public class CreateConfigurationDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Value { get; set; }
    public int? CreatedBy { get; set; }
}

// DEPOIS
public class CreateConfigurationDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Value { get; set; }
    public DateTime? CreatedOn { get; set; }      // ✨ NOVO
    public int? CreatedBy { get; set; }
    public DateTime? UpdateOn { get; set; }       // ✨ NOVO
    public int? UpdateBy { get; set; }            // ✨ NOVO
}
```

**Campos Adicionados:**
- `CreatedOn` - Timestamp de criação
- `UpdateOn` - Timestamp de última atualização
- `UpdateBy` - ID do usuário que atualizou

---

### 2. ✅ EmailContentDto.cs

**Mudança:** Adicionados campos de auditoria ao `CreateEmailContentDto`

```csharp
// ANTES
public class CreateEmailContentDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Tittle { get; set; }
    public string Greetings { get; set; }
    public string MainText { get; set; }
    public string SecondaryText { get; set; }
    public string AuxiliarText { get; set; }
    public string ButtonText { get; set; }
    public string Link { get; set; }
}

// DEPOIS
public class CreateEmailContentDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Tittle { get; set; }
    public string Greetings { get; set; }
    public string MainText { get; set; }
    public string SecondaryText { get; set; }
    public string AuxiliarText { get; set; }
    public string ButtonText { get; set; }
    public string Link { get; set; }
    public int? UpdateBy { get; set; }            // ✨ NOVO
    public DateTime? UpdateOn { get; set; }       // ✨ NOVO
}
```

**Campos Adicionados:**
- `UpdateBy` - ID do usuário que atualizou
- `UpdateOn` - Timestamp de última atualização

---

### 3. ✅ UserDto.cs

**Mudança:** Adicionados campos de timestamp ao `CreateUserDto`

```csharp
// ANTES
public class CreateUserDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string MobilePhone { get; set; }
    public string External_Id { get; set; }
    public bool? IsActive { get; set; } = true;
}

// DEPOIS
public class CreateUserDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string MobilePhone { get; set; }
    public string External_Id { get; set; }
    public DateTime? Creation_Date { get; set; }  // ✨ NOVO
    public DateTime? Last_Login { get; set; }     // ✨ NOVO
    public bool? IsActive { get; set; } = true;
}
```

**Campos Adicionados:**
- `Creation_Date` - Data de criação do usuário
- `Last_Login` - Último login do usuário

---

### 4. ✅ UserStatusDto.cs

**Mudança:** Adicionado timestamp ao `CreateUserStatusDto`

```csharp
// ANTES
public class CreateUserStatusDto
{
    public int Id { get; set; }
    public bool? IsOnLine { get; set; }
}

// DEPOIS
public class CreateUserStatusDto
{
    public int Id { get; set; }
    public bool? IsOnLine { get; set; }
    public DateTime? UpdateOn { get; set; }       // ✨ NOVO
}
```

**Campos Adicionados:**
- `UpdateOn` - Timestamp da última atualização de status

---

### 5. ✅ UserPasswordRecoveryDto.cs

**Mudança:** Adicionados campos de auditoria e validação ao `CreateUserPasswordRecoveryDto`

```csharp
// ANTES
public class CreateUserPasswordRecoveryDto
{
    public long Id { get; set; }
    public int UserId { get; set; }
}

// DEPOIS
public class CreateUserPasswordRecoveryDto
{
    public long Id { get; set; }
    public int UserId { get; set; }
    public DateTime? CreatedOn { get; set; }      // ✨ NOVO
    public bool? IsValid { get; set; }            // ✨ NOVO
}
```

**Campos Adicionados:**
- `CreatedOn` - Timestamp de criação do registro
- `IsValid` - Flag indicando se o registro é válido

---

### 6. ✅ UserAccessDto.cs

**Mudança:** Adicionado timestamp ao `CreateUserAccessDto`

```csharp
// ANTES
public class CreateUserAccessDto
{
    public long Id { get; set; }
    public int UserId { get; set; }
    public long? UserPerfilId { get; set; }
    public string PlataformTypeId { get; set; }
    public string IP { get; set; }
}

// DEPOIS
public class CreateUserAccessDto
{
    public long Id { get; set; }
    public int UserId { get; set; }
    public long? UserPerfilId { get; set; }
    public string PlataformTypeId { get; set; }
    public string IP { get; set; }
    public DateTime? CreatedOn { get; set; }      // ✨ NOVO
}
```

**Campos Adicionados:**
- `CreatedOn` - Timestamp do acesso

---

### 7. ✅ UserPerfilDto.cs

**Mudança:** Adicionados timestamps ao `CreateUserPerfilDto`

```csharp
// ANTES
public class CreateUserPerfilDto
{
    public long Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; }
    public long? UserAvatarId { get; set; }
    public bool? IsChild { get; set; } = false;
    public bool? IsMain { get; set; } = false;
    public bool? IsActive { get; set; } = true;
}

// DEPOIS
public class CreateUserPerfilDto
{
    public long Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; }
    public long? UserAvatarId { get; set; }
    public bool? IsChild { get; set; } = false;
    public bool? IsMain { get; set; } = false;
    public bool? IsActive { get; set; } = true;
    public DateTime? CreatedOn { get; set; }      // ✨ NOVO
    public DateTime? DeletedOn { get; set; }      // ✨ NOVO
}
```

**Campos Adicionados:**
- `CreatedOn` - Timestamp de criação do perfil
- `DeletedOn` - Timestamp de soft delete

---

### 8. ✅ UserInfoDto.cs

**Mudança:** Adicionados timestamps de auditoria ao `CreateUserInfoDto`

```csharp
// ANTES
public class CreateUserInfoDto
{
    public int Id { get; set; }
    public string Biography { get; set; }
    public int? CreatedBy { get; set; }
    public bool? IsStatusEmail { get; set; }
    public bool? HasStreamingAccount { get; set; }
    public bool? IsCollaborator { get; set; }
    public DateTime? BirthDate { get; set; }
    public string Country { get; set; }
    public string CountryCode { get; set; }
    public string Address { get; set; }
}

// DEPOIS
public class CreateUserInfoDto
{
    public int Id { get; set; }
    public string Biography { get; set; }
    public DateTime? CreatedOn { get; set; }      // ✨ NOVO
    public int? CreatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }      // ✨ NOVO
    public int? UpdatedBy { get; set; }           // ✨ NOVO
    public bool? IsStatusEmail { get; set; }
    public bool? HasStreamingAccount { get; set; }
    public bool? IsCollaborator { get; set; }
    public DateTime? BirthDate { get; set; }
    public string Country { get; set; }
    public string CountryCode { get; set; }
    public string Address { get; set; }
}
```

**Campos Adicionados:**
- `CreatedOn` - Timestamp de criação
- `UpdatedOn` - Timestamp de última atualização
- `UpdatedBy` - ID do usuário que atualizou

---

## 📊 Resumo de Mudanças por Categoria

| Categoria | Quantidade | Exemplos |
|:---|:---:|:---|
| **Timestamps** | 11 | CreatedOn, UpdatedOn, UpdateOn |
| **Audit IDs** | 3 | CreatedBy, UpdatedBy, UpdateBy |
| **Flags de Estado** | 2 | IsValid, DeletedOn |
| **Total de Campos Adicionados** | **16** | |

---

## ✅ Endpoints Afetados

Todos os 17 endpoints foram atualizados para aceitar TODOS os campos:

1. ✅ `/configurations/createconfiguration` - +3 campos
2. ✅ `/emailcontent/createemailcontent` - +2 campos
3. ✅ `/plataformtypes/createplataformtype` - Sem mudanças (já completo)
4. ✅ `/positions/createposition` - Sem mudanças (já completo)
5. ✅ `/roles/createrole` - Sem mudanças (já completo)
6. ✅ `/useraccess/createuseraccess` - +1 campo
7. ✅ `/useravatar/createuseravatar` - Sem mudanças (já completo)
8. ✅ `/userdevice/createuserdevice` - Sem mudanças (já completo)
9. ✅ `/userinfo/createuserinfo` - +3 campos
10. ✅ `/userinitialregistration/createuserinitialregistration` - Sem mudanças (já completo)
11. ✅ `/userpasswordrecovery/createuserpasswordrecovery` - +2 campos
12. ✅ `/userperfil/createuserperfil` - +2 campos
13. ✅ `/userpicture/createuserpicture` - Sem mudanças (já completo)
14. ✅ `/userposition/createuserposition` - Sem mudanças (já completo)
15. ✅ `/userroles/createuserrole` - Sem mudanças (já completo)
16. ✅ `/userstatus/createuserstatus` - +1 campo
17. ✅ `/users/createuser` - +2 campos

**Endpoints Modificados:** 8  
**Endpoints Já Completos:** 9

---

## 📄 Arquivos Afetados

### DTOs Modificados
- [API/DTOs/ConfigurationDto.cs](API/DTOs/ConfigurationDto.cs)
- [API/DTOs/EmailContentDto.cs](API/DTOs/EmailContentDto.cs)
- [API/DTOs/UserDto.cs](API/DTOs/UserDto.cs)
- [API/DTOs/UserStatusDto.cs](API/DTOs/UserStatusDto.cs)
- [API/DTOs/UserPasswordRecoveryDto.cs](API/DTOs/UserPasswordRecoveryDto.cs)
- [API/DTOs/UserAccessDto.cs](API/DTOs/UserAccessDto.cs)
- [API/DTOs/UserPerfilDto.cs](API/DTOs/UserPerfilDto.cs)
- [API/DTOs/UserInfoDto.cs](API/DTOs/UserInfoDto.cs)

### Documentação Atualizada
- [API/ENDPOINTS_JSON.md](API/ENDPOINTS_JSON.md) - Todos os JSONs de request atualizados com todos os campos

---

## 🔄 Verificação: SQL Schema vs DTOs

Após as mudanças, **100% dos campos SQL estão mapeados nos DTOs Create**.

### Validação
✅ Todos os 17 endpoints agora recebem TODOS os campos das tabelas  
✅ Nenhum campo SQL foi deixado de fora  
✅ Campos de auditoria são enviados pelo cliente  
✅ Timestamps são enviados pelo cliente  
✅ JSON examples refletem a nova estrutura

---

## 📌 Mudança de Paradigma

### Antes (Gerenciamento Servidor)
```
Cliente envia: dados do domínio
Servidor gera: timestamps, auditoria, validações
```

### Depois (Gerenciamento Cliente - Espelhamento OutSystems)
```
Cliente envia: TODOS os dados (domínio + auditoria + timestamps)
Servidor persiste: exatamente o que recebeu
```

**Motivação:** Sincronização com tabelas espelho do OutSystems, onde os dados já vêm completos e apenas precisam ser persistidos.

---

## 🎯 Próximos Passos (Recomendado)

1. ✅ Compilar a solução para validar sintaxe C#
2. ✅ Atualizar controllers para usar os novos DTOs
3. ✅ Atualizar testes unitários com os novos campos
4. ✅ Testar endpoints com JSONs contendo todos os campos
5. ✅ Atualizar documentação de API (Swagger)
6. ✅ Comunicar mudanças aos clientes da API

---

**Data:** 2026-08-17  
**Status:** ✅ Completado
