# Guia de Deploy no Plesk (Interface Web)

## Resumo
Este guia mostra como fazer upload e configurar a API MeuProxySsl apenas usando o painel web do Plesk, sem PowerShell/RDP.

---

## Pré-requisitos
- ZIP publicado: `MeuProxySsl-publish.zip` (gerar localmente: `dotnet build MeuProxySsl.csproj -c Release` → copiar `bin\Release\net48\` → criar ZIP)
- Domínio/subdomínio criado no Plesk: ex. `api.tokuplus.com`
- Plano Plesk Windows com suporte a ASP.NET/.NET Framework 4.8

---

## Pasos no Plesk

### 1. Acessar o Plesk
- Vá para seu Painel Plesk (ex.: https://seu.servidor:8443)
- Login com suas credenciais

### 2. Selecionar o domínio
- Painel Inicial → Domains
- Clique no domínio/subdomínio desejado (ex.: `api.tokuplus.com`)

### 3. Upload do ZIP
- Clique em **File Manager** (ou Files)
- Navegue para a pasta `httpdocs` (pasta raiz do site)
- Clique em **Upload** → selecione `MeuProxySsl-publish.zip`
- Aguarde o upload finalizar

### 4. Extrair o ZIP
- No File Manager, clique com botão direito no `MeuProxySsl-publish.zip`
- Escolha **Extract** (ou descompactar)
- Aguarde a extração (os arquivos aparecerão na mesma pasta: `MeuProxySsl.exe`, `Web.config`, `bin\`, etc.)
- **Opcional**: após extração, você pode deletar o arquivo `.zip` para economizar espaço

### 5. Configurar .NET Framework
- Volte para o Domains → clique no seu domínio
- Clique em **Hosting Settings** (ou ASP.NET Settings)
- Procure pela opção **.NET Framework version** ou **.NET Framework**
- Selecione **v4.x** (para .NET 4.8 use a maior versão 4.x disponível)
- Se houver opção de **Application Pool Mode**, escolha **Integrated**
- Salve as alterações

### 6. Ajustar configurações da aplicação (Web.config)
- No File Manager, navegue até `httpdocs`
- Localize o arquivo `Web.config`
- Clique em **Edit** (ou ícone de editor)
- Verificar/ajustar:
  - `Upstream:BaseUrl` — deve estar correto (URL do TokuPlus)
  - `Upstream:IgnoreInvalidCertificate` — em produção, altere para `false`
  ```xml
  <add key="Upstream:BaseUrl" value="https://personal-b0zlpsmh.outsystemscloud.com/TokuPlus_API/rest/RESTAPITOKUPLUS/" />
  <add key="Upstream:IgnoreInvalidCertificate" value="false" />
  ```
- Salve o arquivo

### 7. Configurar HTTPS/SSL
- Volte para o Domains → selecione seu domínio
- Clique em **SSL/TLS Certificates**
- Se não houver certificado:
  - Clique em **Add SSL Certificate**
  - Escolha **Let's Encrypt** (gratuito) ou faça upload de um certificado próprio
  - Selecione o domínio (ex.: `api.tokuplus.com`)
  - Clique em **Get Free Certificate** (Let's Encrypt) ou **Upload**
  - Aguarde a instalação
- Após certificado instalado:
  - Vá para **Hosting Settings** (ou **Domains**)
  - Procure por **HTTPS** / **Redirect to HTTPS** / **Secure domain**
  - Ative HTTPS e selecione o certificado instalado
  - Salve

### 8. Configurar Bindings (Hostname e Portas)
- Domains → selecione seu domínio
- Clique em **Hosting Settings**
- Verifique:
  - **Domain name**: deve ser `api.tokuplus.com`
  - **Physical path**: deve apontar para `httpdocs` (onde extraiu o ZIP)
- Se houver seção de **Bindings** ou **IP address**:
  - Certifique-se que http (porta 80) e https (porta 443) estão habilitadas
  - Salve

### 9. Testar a API
- Aguarde 1–2 minutos (aplicação IIS reinicia)
- Abra um navegador ou terminal e teste:
  ```bash
  https://api.tokuplus.com/getTvChannels
  https://api.tokuplus.com/validateTVBoxToken?token=6787678
  https://api.tokuplus.com/createOrUpdateUserTimeProgress?UserId=1&SerieId=2&EpisodeId=3&TimeProgress=120
  ```
- Você deve receber respostas JSON (ou erros gerados pela aplicação com dados úteis)

### 10. Troubleshooting
Se houver erro 500:

#### Via Plesk File Manager
- Vá para **Domains** → **Logs**
- Clique em **error.log** ou **access.log** para ver erros HTTP/IIS
- Procure por mensagens de erro da aplicação

#### Checklist
- ✓ Arquivo `Web.config` foi extraído (verifique via File Manager)
- ✓ `.NET Framework` está definido como v4.x nas Hosting Settings
- ✓ `Upstream:BaseUrl` em `Web.config` está correto (URL do TokuPlus)
- ✓ SSL está ativo e certificado é válido
- ✓ Firewall: portas 80/443 estão abertas no servidor (geralmente já estão no Plesk)

### 11. Configuração Avançada (Opcional)
Se precisar de logs detalhados:
- **File Manager** → crie uma pasta `logs` em `httpdocs`
- No `Web.config`, adicione uma seção de logging (Serilog ou similar) se desejar mais detalhes

---

## Checklist Final
- [ ] ZIP foi extraído em `httpdocs`
- [ ] `.NET Framework v4.x` está configurado
- [ ] `Web.config` foi verificado e `Upstream:BaseUrl` está correto
- [ ] SSL/HTTPS está ativo
- [ ] Teste básico (GET /getTvChannels) retorna dados
- [ ] Em produção: `Upstream:IgnoreInvalidCertificate` = `false`

---

## Próximos Passos
- Monitorar logs regularmente (Plesk → Logs)
- Configurar alertas/monitoramento se disponível no plano
- Implementar autenticação/rate limiting conforme necessário
